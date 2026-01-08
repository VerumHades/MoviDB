-- Step 1: Create logins for each type of user with strong passwords
CREATE LOGIN normal_user_login WITH PASSWORD = 'NormalUserStrongPassword!123';
CREATE LOGIN moderator_user_login WITH PASSWORD = 'ModeratorStrongPassword!123';
CREATE LOGIN library_manager_login WITH PASSWORD = 'LibraryManagerStrongPassword!123';

-- Step 2: Create database users in MoviesDB for each login
USE MoviesDB;
CREATE USER normal_user FOR LOGIN normal_user_login;
CREATE USER moderator_user FOR LOGIN moderator_user_login;
CREATE USER library_manager FOR LOGIN library_manager_login;

-- Step 3: Grant permissions to each user

-- Normal user: read-only on most tables, full access to own library entries and reviews
GRANT SELECT ON dbo.media TO normal_user;
GRANT SELECT ON dbo.movie TO normal_user;
GRANT SELECT ON dbo.series TO normal_user;
GRANT SELECT ON dbo.vw_movie TO normal_user;
GRANT SELECT ON dbo.vw_series TO normal_user;
GRANT SELECT ON dbo.genre TO normal_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.library_entry TO normal_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.review TO normal_user;

-- Moderator: normal user permissions + review management
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.review TO moderator_user;

-- Library manager: full CRUD access to all relevant tables
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.media TO library_manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.movie TO library_manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.series TO library_manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.genre TO library_manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.season TO library_manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.episode TO library_manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.library_entry TO library_manager;

-- Views access (optional for library manager)
GRANT SELECT ON dbo.vw_movie TO library_manager;
GRANT SELECT ON dbo.vw_series TO library_manager;