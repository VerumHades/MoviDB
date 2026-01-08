# MoviDB Project Documentation

**Author:** Filip Heger  
**Contact:** fila.heger@gmail.com  
**School:** SPŠE Ječná  
**Date:** 8.1.2026  
**Project Type:** School project

---

## 1. Project Overview

**Project Name:** MoviDB  
**Description:** MoviDB is a console-based application for managing movie data. The project is structured using a **Domain-Driven Design (DDD) inspired architecture**, with clear separation between Domain, Application, Infrastructure, and Presentation layers.

This is an early version and if does not yet support full operations for all actors.

---

## 2. User Requirements and Use Cases

### Use Case 1: Import Series

**Actor:** User (Admin or Moderator)  
**Description:** Imports a series along with its seasons and episodes from a JSON file.  
**Preconditions:** JSON file exists and is correctly formatted.  
**Postconditions:** Series, seasons, and episodes are added to the database.  
**Parameters:**  

* `filePath` (String) [Required]: Path to the JSON file containing series data

**Main Flow:**  

1. User invokes `ImportSeries` command.
2. System validates the file path and JSON format.
3. System adds series, seasons, and episodes to the database.
4. System confirms successful import.

### Use Case 2: Help

**Actor:** User  
**Description:** Lists all available commands with descriptions, parameters, and constraints.  
**Preconditions:** None  
**Postconditions:** User receives a detailed command reference.  
**Parameters:** None  
**Main Flow:**  

1. User invokes `Help` command.
2. System displays all available commands, their parameters, and constraints.

### Use Case 3: Register Movie

**Actor:** User (Admin or Moderator)  
**Description:** Registers a new movie in the system.  
**Preconditions:** Genre exists in the database.  
**Postconditions:** Movie is created in the database and associated with the specified genre.  
**Parameters:**  

* `title` (String, 1-255) [Required]
* `description` (String, 1-300) [Required]
* `genre` (String, 1-255) [Required]
* `durationMinutes` (Int32) [Required]

**Main Flow:**

1. User provides movie details and invokes `RegisterMovie`.
2. System validates parameter lengths and genre existence.
3. System creates movie and associates it with the genre.
4. System confirms successful registration.

### Use Case 4: List Movies

**Actor:** User  
**Description:** Displays a paginated list of movies with table-like formatting.  
**Preconditions:** Movies exist in the database.  
**Postconditions:** User sees a list of movies, optionally paginated.  
**Parameters:**  

* `batchSize` (Int32) [Optional]: Number of movies per page

**Main Flow:**

1. User invokes `ListMovies`.
2. System retrieves movies from the database.
3. System displays movies in a table format, handling paging if `batchSize` is provided.

### Use Case 5: Create Genre

**Actor:** User (Admin or Moderator)  
**Description:** Adds a new genre to the system.  
**Preconditions:** Genre name must be unique.  
**Postconditions:** Genre is added to the database.  
**Parameters:**  

* `name` (String) [Required]: Name of the genre

**Main Flow:**

1. User invokes `CreateGenre` with a genre name.
2. System checks for uniqueness.
3. System inserts new genre into the database.
4. System confirms creation.

### Use Case 6: List Genres

**Actor:** User  
**Description:** Displays all existing genres.  
**Preconditions:** Genres exist in the database.  
**Postconditions:** User sees a complete list of genres.  
**Parameters:** None  
**Main Flow:**  

1. User invokes `ListGenres`.
2. System retrieves all genres from the database.
3. System displays genres in a list.

### Use Case 7: Update Movie

**Actor:** User (Admin or Moderator)  
**Description:** Updates details of an existing movie. Only fields provided are updated.  
**Preconditions:** Movie with the specified `title` exists.  
**Postconditions:** Movie fields are updated in the database.  
**Parameters:**  

* `title` (String, 1-255) [Required]: Current title
* `newTitle` (String, 1-255) [Optional]
* `description` (String, 1-300) [Optional]
* `genre` (String, 1-255) [Optional]
* `durationMinutes` (Int32) [Optional]

**Main Flow:**

1. User provides the movie title and any fields to update.
2. System validates input constraints.
3. System updates only the provided fields in the database.
4. System confirms successful update.

### Use Case 8: Delete Movie

**Actor:** User (Admin or Moderator)  
**Description:** Deletes an existing movie from the system.  
**Preconditions:** Movie with the specified `title` exists.  
**Postconditions:** Movie is removed from the database along with related media entries.  
**Parameters:**  

* `title` (String, 1-255) [Required]: Title of the movie  

**Main Flow:**  

1. User invokes `DeleteMovie` with the movie title.
2. System validates the movie exists.
3. System deletes the movie and cascades delete to related tables.
4. System confirms successful deletion.

## 3. Architecture and Design

### 3.1 Layered Architecture

- **Domain Layer:** Contains domain models, and repository interfaces.
- **Application Layer:** Contains services that orchestrate domain operations and manage transactions through a **Unit of Work**. 
- **Infrastructure Layer:** Implements repository interfaces, manages SQL database connections, and handles data persistence. Each SQL operation creates a new `SqlConnection` (pooled for performance).
- **Presentation Layer:** Simple command-line interface using the **Command Pattern**. Commands are tailored to the domain operations but currently do not leverage async operations.

### 3.2 Design Patterns Used

- **Domain-Driven Design (DDD)** - lightly
- **Repository Pattern**
- **Unit of Work**
- **Command Pattern (in Presentation Layer)**
---
## 4. Database Design

### 4.1 Overview

The **MoviesDB** database is designed to support both movies and series, with relational integrity, rating aggregation, and user-specific library entries. All foreign keys enforce cascading deletes where appropriate to maintain data consistency.

The schema is fully reproducible and can be dropped and recreated for development or testing purposes.

### 4.2 Tables

**Users Table**

| Column | Type | Description |
|--------|------|-------------|
| id | INT IDENTITY(1,1) PRIMARY KEY | Unique user identifier |
| username | NVARCHAR(255) NOT NULL UNIQUE | User login name |
| password_hash | NVARCHAR(255) NOT NULL | Hashed password |
| role | NVARCHAR(20) NOT NULL | Role, either 'normal' or 'moderator' |
| created_at | DATETIME NOT NULL DEFAULT GETDATE() | Account creation timestamp |

**Genre Table**

| Column | Type | Description |
|--------|------|-------------|
| id | INT IDENTITY(1,1) PRIMARY KEY | Genre identifier |
| name | NVARCHAR(255) NOT NULL UNIQUE | Genre name |

**Media Table (Base for movies & series)**

| Column | Type | Description |
|--------|------|-------------|
| id | INT IDENTITY(1,1) PRIMARY KEY | Media identifier |
| created_at | DATETIME NOT NULL DEFAULT GETDATE() | Creation timestamp |
| title | NVARCHAR(255) NOT NULL | Media title |
| description | NVARCHAR(300) | Optional description |
| type | NVARCHAR(20) NOT NULL | 'movie' or 'series' |
| rating_count | INT NOT NULL DEFAULT 0 | Number of ratings |
| rating_sum | FLOAT NOT NULL DEFAULT 0 | Sum of ratings (for aggregation) |

**Movie Table**

| Column | Type | Description |
|--------|------|-------------|
| media_id | INT PRIMARY KEY REFERENCES media(id) ON DELETE CASCADE | Link to media |
| genre_id | INT NOT NULL REFERENCES genre(id) | Movie genre |
| duration_minutes | INT CHECK (> 0) | Duration in minutes |

**Series Table**

| Column | Type | Description |
|--------|------|-------------|
| media_id | INT PRIMARY KEY REFERENCES media(id) ON DELETE CASCADE | Link to media |
| genre_id | INT NOT NULL REFERENCES genre(id) | Series genre |

**Season Table**

| Column | Type | Description |
|--------|------|-------------|
| id | INT IDENTITY(1,1) PRIMARY KEY | Season identifier |
| series_id | INT REFERENCES series(media_id) ON DELETE CASCADE | Parent series |
| title | NVARCHAR(255) NOT NULL | Season title |
| number | INT CHECK (>0) | Sequential number of season |
| Constraint | uq_season_perseries UNIQUE(series_id, number) | Prevent duplicate season numbers per series |

**Episode Table**

| Column | Type | Description |
|--------|------|-------------|
| id | INT IDENTITY(1,1) PRIMARY KEY | Episode identifier |
| season_id | INT REFERENCES season(id) ON DELETE CASCADE | Parent season |
| title | NVARCHAR(255) NOT NULL | Episode title |
| episode_number | INT CHECK (>0) | Sequential episode number |
| created_at | DATETIME DEFAULT GETDATE() | Creation timestamp |
| Constraint | uq_episode_perseason UNIQUE(season_id, episode_number) | Unique episode number per season |

**Library Entry Table**

| Column | Type | Description |
|--------|------|-------------|
| media_id | INT REFERENCES media(id) | Media reference |
| user_id | INT REFERENCES user(id) | User reference |
| watched | BIT DEFAULT 0 | Watched flag |
| created_at | DATETIME DEFAULT GETDATE() | Entry timestamp |
| Constraint | PRIMARY KEY(media_id, user_id) | One library entry per user-media pair |

**Review Table**

| Column | Type | Description |
|--------|------|-------------|
| id | INT IDENTITY(1,1) PRIMARY KEY | Review identifier |
| media_id | INT REFERENCES media(id) | Media reference |
| user_id | INT REFERENCES user(id) | Reviewer |
| title | NVARCHAR(255) NOT NULL | Review title |
| content | NVARCHAR(MAX) NOT NULL | Review content |
| rating | FLOAT CHECK(0 ≤ rating ≤ 5) | Rating value |
| created_at | DATETIME DEFAULT GETDATE() | Review timestamp |
| Constraint | uq_review_peruser UNIQUE(media_id, user_id) | One review per user-media |

### 4.3 Views

**Movies View (`vw_movie`)**

- Joins `media`, `movie`, and `genre`.  
- Calculates average rating dynamically (`rating_sum / rating_count`).

**Series View (`vw_series`)**

- Joins `series`, `media`, `genre`, and counts seasons.  
- Calculates average rating dynamically.

### 4.4 Triggers

**Review Rating Triggers**

- **After Insert:** Updates `media.rating_count` and `media.rating_sum` when a new review is added.
- **After Update:** Adjusts `rating_sum` when a review rating is changed.
- **After Delete:** Decrements `rating_count` and subtracts deleted rating from `rating_sum`.

These triggers ensure that the media average rating remains accurate at all times without recalculating from scratch.

### 4.5 Notes on Database Operations

- All SQL operations are executed through repository classes in C# using `SqlConnection`.  
- Connections are created per operation but leverage connection pooling for efficiency.  
- Async operations are supported at the repository and application layer but not yet exposed in the console commands.

---

## 5. Application Configuration

- **Configuration File:** `appsettings.json` or environment variables  
- **Supported Options:**
  - Database connection string  
  - Logging level  
  - Async behavior toggle (currently for internal logic only)

---

## 6. Installation and Running

Refer to README.md in the root of the project

## 7. Error Handling

| Error | Code | Description | Resolution |
|-------|------|-------------|------------|
| SqlConnection failed | DB100 | Database unreachable | Check connection string and database server |
| Invalid command | CMD101 | User input not recognized | Show available commands help |
| Entity not found | ENT404 | Requested entity does not exist | Verify input ID or data existence |

## 8. Database Configuration Overview

The `DatabaseConnectionConfig` class provides the necessary configuration for establishing a connection to a SQL Server database using `SqlConnectionFactory`. This configuration tells the application **where the database is**, **which credentials to use**, and **which database to connect to**.

### Configuration Fields

- **Server**  
  The hostname or IP address of the SQL Server instance.  
  Example: `localhost` or `192.168.1.100`.

- **Database**  
  The name of the specific database you want to connect to on the SQL Server.  
  Example: `MoviesDB`.

- **UserId**  
  The username used for authentication with the SQL Server.  
  Example: `normal_user_login`.

- **Password**  
  The password corresponding to the UserId for authentication.  
  Example: `NormalUserStrongPassword!123`.

The `DatabaseConnectionConfig` allows `SqlConnectionFactory` to dynamically build a valid connection string. This string is then used to create and open SQL Server connections, both synchronously and asynchronously.  
An example `DatabaseConfig.json` is in the project root
---

## 9. Bulk Series Import Structure

The `SeriesCreationData` hierarchy is used to represent an entire series in memory, including its seasons and episodes, before inserting it into the database. This allows you to **import a complete series in bulk** in a structured way.

### Structure Overview

The bulk import uses three DTO (Data Transfer Object) records:

#### `EpisodeCreationData`
Represents a single episode within a season.

| Field | Description |
|-------|-------------|
| `title` | The title of the episode. |[ERRORS.md](ERRORS.md)
| `episode_number` | The numeric position of the episode within the season. |

#### `SeasonCreationData`
Represents a season, which contains multiple episodes.

| Field | Description |
|-------|-------------|
| `title` | The title of the season. |
| `number` | The season number (e.g., 1, 2, 3). |
| `episodes` | A list of `EpisodeCreationData` objects representing all episodes in this season. |

#### `SeriesCreationData`
Represents the top-level series object, containing seasons and their episodes.

| Field | Description |
|-------|-------------|
| `title` | The title of the series. |
| `description` | A short description or synopsis of the series. |
| `genre_name` | The name of the genre this series belongs to. |
| `seasons` | A list of `SeasonCreationData` objects representing all seasons of the series. |

By using this hierarchical structure, the application can maintain strong type safety and ensure that all nested elements (seasons and episodes) are included when creating or importing series.
The loading structure sample for json files in this format can be found in `samples/`

## 12. Errors and Solutions

You can find error documentation in ERRORS.md

## 11. Summary / Conclusion

MoviDB demonstrates my best attempt at clean, layered architecture with **DDD principles** applied. 
Separation of concerns ensures that domain logic remains isolated from infrastructure and presentation.


## 12. MoviDB Archive Builder Script

**Location:** `scripts/release_builder.py`

**Purpose:**

* This Python script packages key components of the MoviDB project into a single ZIP archive for distribution or backup.

**What it includes:**

* `doc` directory
* `MoviDB/bin/Release/net10.0/win-x64` renamed as `bin`
* `sql` directory
* `samples` directory
* `README.md copy` renamed as `README.md` in the archive

**How it works:**

1. Checks if `MoviDBPackage.zip` exists in the current directory and removes it if present.
2. Iterates over the specified directories and files, copying them into the archive.
3. For directories, preserves their internal folder structure.
4. Creates `MoviDBPackage.zip` containing all included items.

**Usage:**

```bash
python scripts/create_movidb_archive.py
```

**Notes:**

* Missing directories or files will be skipped with a warning.
* The resulting archive is ready for sharing, deployment, or backup.
