-- =============================================
-- MoviesDB Database Setup (Drop & Recreate)
-- Fully reproducible schema for development/testing
-- =============================================
-- ==========================
-- Create Tables
-- ==========================

CREATE TABLE dbo.[user] (
                            id INT IDENTITY(1,1) PRIMARY KEY,
                            username NVARCHAR(255) NOT NULL UNIQUE,
                            password_hash NVARCHAR(255) NOT NULL,
                            role NVARCHAR(20) NOT NULL CHECK (role IN ('normal', 'moderator')),
                            created_at DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE dbo.genre (
                           id INT IDENTITY(1,1) PRIMARY KEY,
                           name NVARCHAR(255) NOT NULL UNIQUE
);

CREATE TABLE dbo.media (
                           id INT IDENTITY(1,1) PRIMARY KEY,
                           created_at DATETIME NOT NULL DEFAULT GETDATE(),
                           title NVARCHAR(255) NOT NULL,
                           description NVARCHAR(300) NULL,
                           type NVARCHAR(20) NOT NULL CHECK (type IN ('movie', 'series')),
                           rating_count INT NOT NULL DEFAULT 0 CHECK (rating_count >= 0),
                           rating_sum FLOAT NOT NULL DEFAULT 0 CHECK (rating_sum >= 0)
);

CREATE TABLE dbo.movie (
                           media_id INT PRIMARY KEY REFERENCES dbo.media(id) ON DELETE CASCADE,
                           genre_id INT NOT NULL REFERENCES dbo.genre(id),
                           duration_minutes INT NULL CHECK (duration_minutes > 0)
);

CREATE TABLE dbo.series (
                            media_id INT PRIMARY KEY REFERENCES dbo.media(id) ON DELETE CASCADE,
                            genre_id INT NOT NULL REFERENCES dbo.genre(id)
);

CREATE TABLE dbo.season (
                            id INT IDENTITY(1,1) PRIMARY KEY,
                            series_id INT NOT NULL REFERENCES dbo.series(media_id) ON DELETE CASCADE,
                            title NVARCHAR(255) NOT NULL,
                            number INT NOT NULL CHECK (number > 0),
                            CONSTRAINT uq_season_perseries UNIQUE (series_id, number)
);

CREATE TABLE dbo.episode (
                             id INT IDENTITY(1,1) PRIMARY KEY,
                             season_id INT NOT NULL REFERENCES dbo.season(id) ON DELETE CASCADE,
                             title NVARCHAR(255) NOT NULL,
                             episode_number INT NOT NULL CHECK (episode_number > 0),
                             created_at DATETIME NOT NULL DEFAULT GETDATE(),
                             CONSTRAINT uq_episode_perseason UNIQUE (season_id, episode_number)
);

CREATE TABLE dbo.library_entry (
                                   media_id INT NOT NULL REFERENCES dbo.media(id),
                                   user_id INT NOT NULL REFERENCES dbo.[user](id),
                                   watched BIT NOT NULL DEFAULT 0,
                                   created_at DATETIME NOT NULL DEFAULT GETDATE(),
                                   CONSTRAINT pk_library_entry PRIMARY KEY (media_id, user_id)
);

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
-- Create Triggers
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
END;
GO

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
END;
GO

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
END;
GO
