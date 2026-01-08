-- =============================================
-- MoviesDB Database Setup (Drop & Recreate)
-- Fully reproducible schema for development/testing
-- =============================================

-- ==========================
-- Drop and recreate database
-- ==========================
IF DB_ID('MoviesDB') IS NOT NULL
    BEGIN
        DROP DATABASE MoviesDB;
    END
GO

CREATE DATABASE MoviesDB;
GO

USE MoviesDB;
GO

-- ==========================
-- Create SQL Server logins
-- ==========================
-- CREATE LOGIN normal_user_login WITH PASSWORD = 'NormalUserStrongPassword!123';
--CREATE LOGIN moderator_user_login WITH PASSWORD = 'ModeratorStrongPassword!123';
--CREATE LOGIN library_manager_login WITH PASSWORD = 'LibraryManagerStrongPassword!123';
GO

-- ==========================
-- Create database users
-- ==========================
CREATE USER normal_user FOR LOGIN normal_user_login;
CREATE USER moderator_user FOR LOGIN moderator_user_login;
CREATE USER library_manager FOR LOGIN library_manager_login;
GO

-- ==========================
-- Create Tables
-- ==========================

-- Users
CREATE TABLE dbo.[user] (
                            id INT IDENTITY(1,1) PRIMARY KEY,
                            username NVARCHAR(255) NOT NULL UNIQUE,
                            password_hash NVARCHAR(255) NOT NULL,
                            role NVARCHAR(20) NOT NULL CHECK (role IN ('normal', 'moderator')),
                            created_at DATETIME NOT NULL DEFAULT GETDATE()
);

-- Genre
CREATE TABLE dbo.genre (
                           id INT IDENTITY(1,1) PRIMARY KEY,
                           name NVARCHAR(255) NOT NULL UNIQUE
);

-- Media
CREATE TABLE dbo.media (
                           id INT IDENTITY(1,1) PRIMARY KEY,
                           created_at DATETIME NOT NULL DEFAULT GETDATE(),
                           title NVARCHAR(255) NOT NULL,
                           description NVARCHAR(300) NULL,
                           type NVARCHAR(20) NOT NULL CHECK (type IN ('movie', 'series')),
                           rating_count INT NOT NULL DEFAULT 0 CHECK (rating_count >= 0),
                           rating_sum FLOAT NOT NULL DEFAULT 0 CHECK (rating_sum >= 0)
);

-- Movie (1-to-1 with media)
CREATE TABLE dbo.movie (
                           media_id INT PRIMARY KEY REFERENCES dbo.media(id) ON DELETE CASCADE,
                           genre_id INT NOT NULL REFERENCES dbo.genre(id),
                           duration_minutes INT NULL CHECK (duration_minutes > 0)
);

-- Series (1-to-1 with media)
CREATE TABLE dbo.series (
                            media_id INT PRIMARY KEY REFERENCES dbo.media(id) ON DELETE CASCADE,
                            genre_id INT NOT NULL REFERENCES dbo.genre(id)
);

-- Season
CREATE TABLE dbo.season (
                            id INT IDENTITY(1,1) PRIMARY KEY,
                            series_id INT NOT NULL REFERENCES dbo.series(media_id) ON DELETE CASCADE,
                            title NVARCHAR(255) NOT NULL,
                            number INT NOT NULL CHECK (number > 0),
                            CONSTRAINT uq_season_perseries UNIQUE (series_id, number)
);

-- Episode
CREATE TABLE dbo.episode (
                             id INT IDENTITY(1,1) PRIMARY KEY,
                             season_id INT NOT NULL REFERENCES dbo.season(id) ON DELETE CASCADE,
                             title NVARCHAR(255) NOT NULL,
                             episode_number INT NOT NULL CHECK (episode_number > 0),
                             created_at DATETIME NOT NULL DEFAULT GETDATE(),
                             CONSTRAINT uq_episode_perseason UNIQUE (season_id, episode_number)
);

-- Library Entry
CREATE TABLE dbo.library_entry (
                                   media_id INT NOT NULL REFERENCES dbo.media(id),
                                   user_id INT NOT NULL REFERENCES dbo.[user](id),
                                   watched BIT NOT NULL DEFAULT 0,
                                   created_at DATETIME NOT NULL DEFAULT GETDATE(),
                                   CONSTRAINT pk_library_entry PRIMARY KEY (media_id, user_id)
);

-- Review
CREATE TABLE dbo.review (
                            id INT IDENTITY(1,1) PRIMARY KEY,
                            media_id INT NOT NULL REFERENCES dbo.media(id),
                            user_id INT NOT NULL REFERENCES dbo.[user](id),
                            title NVARCHAR(255) NOT NULL,
                            content NVARCHAR(MAX) NOT NULL,
                            rating FLOAT NOT NULL CHECK (rating >= 0 AND rating <= 5),
                            created_at DATETIME NOT NULL DEFAULT GETDATE(),
                            CONSTRAINT uq_review_peruser UNIQUE (media_id, user_id)
);
GO

-- ==========================
-- Create Views
-- ==========================
CREATE VIEW dbo.vw_movie AS
SELECT
    m.id AS media_id,
    m.title,
    m.description,
    g.id AS genre_id,
    g.name AS genre_name,
    m.rating_count,
    m.rating_sum,
    CASE WHEN m.rating_count > 0 THEN m.rating_sum / m.rating_count ELSE 0 END AS rating,
    mv.duration_minutes,
    m.created_at
FROM dbo.media m
         INNER JOIN dbo.movie mv ON mv.media_id = m.id
         INNER JOIN dbo.genre g ON g.id = mv.genre_id
WHERE m.type = 'movie';

CREATE VIEW dbo.vw_series AS
SELECT
    s.media_id AS series_id,
    m.title AS title,
    m.description AS description,
    g.name AS genre_name,
    COUNT(se.id) AS season_count,
    m.rating_count AS rating_count,
    m.rating_sum AS rating_sum,
    CASE WHEN m.rating_count > 0 THEN m.rating_sum / m.rating_count ELSE 0 END AS rating,
    m.created_at AS created_at
FROM dbo.series s
         INNER JOIN dbo.media m ON s.media_id = m.id
         INNER JOIN dbo.genre g ON s.genre_id = g.id
         LEFT JOIN dbo.season se ON se.series_id = s.media_id
GROUP BY
    s.media_id,
    m.title,
    m.description,
    g.name,
    m.rating_count,
    m.rating_sum,
    m.created_at;
GO

-- ==========================
-- Grant Permissions
-- ==========================

-- Normal user
GRANT SELECT ON dbo.media TO normal_user;
GRANT SELECT ON dbo.genre TO normal_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.library_entry TO normal_user;
GRANT SELECT ON dbo.review TO normal_user;

-- Moderator (normal user + review management)
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.review TO moderator_user;

-- Library manager (full management)
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.media TO library_manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.movie TO library_manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.series TO library_manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.genre TO library_manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.season TO library_manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.episode TO library_manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.library_entry TO library_manager;

GRANT SELECT ON dbo.vw_movie TO library_manager;
GRANT SELECT ON dbo.vw_series TO library_manager;

-- ==========================
-- Trigger: After Insert on Review
-- Automatically updates media rating_count and rating_sum when a new review is added
-- ==========================
CREATE TRIGGER trg_review_after_insert
    ON dbo.review
    AFTER INSERT
    AS
BEGIN
    SET NOCOUNT ON;

    UPDATE media
    SET
        media.rating_count = media.rating_count + 1,
        media.rating_sum = media.rating_sum + inserted_review.rating
    FROM dbo.media AS media
             INNER JOIN inserted AS inserted_review ON media.id = inserted_review.media_id;
END
GO

-- ==========================
-- Trigger: After Update on Review
-- Automatically adjusts media rating_sum when an existing review's rating is updated
-- ==========================
CREATE TRIGGER trg_review_after_update
    ON dbo.review
    AFTER UPDATE
    AS
BEGIN
    SET NOCOUNT ON;

    UPDATE media
    SET media.rating_sum = media.rating_sum - deleted_review.rating + inserted_review.rating
    FROM dbo.media AS media
             INNER JOIN inserted AS inserted_review ON media.id = inserted_review.media_id
             INNER JOIN deleted AS deleted_review ON inserted_review.id = deleted_review.id
    WHERE inserted_review.rating <> deleted_review.rating;
END
GO

SELECT * FROM vw_movie
-- ==========================
-- Trigger: After Delete on Review
-- Automatically decrements media rating_count and subtracts from rating_sum when a review is deleted
-- ==========================
CREATE TRIGGER trg_review_after_delete
    ON dbo.review
    AFTER DELETE
    AS
BEGIN
    SET NOCOUNT ON;

    UPDATE media
    SET
        media.rating_count = media.rating_count - 1,
        media.rating_sum = media.rating_sum - deleted_review.rating
    FROM dbo.media AS media
             INNER JOIN deleted AS deleted_review ON media.id = deleted_review.media_id;
END
GO