# MoviDB

**MoviDB** is a console-based movie and TV series management system written in **C#**.  
It provides an interactive **CLI (command-line interface)** for managing movies, genres, and TV series data stored in a **SQL Server** database.

The application is designed for educational and testing purposes and supports structured data import, paging, and validation-driven commands.

Author: **Filip Heger**

---

## Features

- Register, update, list, and delete movies
- Create and list genres
- Import TV series (including seasons and episodes) from JSON files
- Interactive CLI with command discovery
- Cursor-based paging for large result sets
- SQL Server–backed persistent storage

---

## Technology Stack

- **Language:** C#
- **Runtime:** .NET
- **Database:** Microsoft SQL Server (SQL Express supported)
- **Interface:** Console-based CLI

---

## Folder Structure

```
MoviDB/
│
├── src/MoviDB/                    # Application source code
│   ├── Domain/             # Domain entities (Movie, Genre, Series, etc.)
│   ├── Infrastructure/     # Database access and repositories
│   ├── Application/        # Business logic and orchestration
│   ├── Presentation/       # CLI Implementation
│   └── Program.cs          # Application entry point
│
├── sql/                    # Database creation scripts
│   └── database.sql        # SQL script used to create the database schema
│
├── samples/
│   └── series_import.json         # Example JSON file for ImportSeries command
│
├── DatabaseConfig.json # Database connection configuration
│
└── README.md               # Project documentation
```

---

## Database Setup (Required)

The database **must be created using the provided SQL script**.  
Manual schema creation is **not supported**.

### Prerequisites

- SQL Server or SQL Server Express installed
- SQL Server Management Studio (SSMS) or equivalent tool

### Steps

1. Open **SQL Server Management Studio**
2. Connect to your SQL Server instance
3. Open the script located at:

   ```
   sql/setup.sql
   ```

5. Setup user accounts with permission
6. Open and execute the script located at:

   ```
   sql/credentials.sql
   ```
7. Execute the necessary commands in this order:
   1. Create logins (If they don't already exist)
   2. Users (If they don't already exist)
   3. Grant user permissions

> Note: when configuring use the name of the database from the setup scripts which is `MoviesDB`

---

## Configuration

The application reads its database connection settings from a JSON configuration file.

### Configuration File

```
DatabaseConfig.json
```

### Example Configuration

```json
{
  "Server": "DESKTOP-EMT3CHH\\SQLEXPRESS",
  "Database": "moviesd",
  "UserId": "library_manager_login",
  "Password": "LibraryManagerStrongPassword!123"
}
```

---

## Running the Application

### From Release
You can find the [latest release here](https://github.com/VerumHades/MoviDB/releases/latest)
1. Download the latest release `Release.zip`
2. Extract it
3. Navigate until you find DatabaseConfig.json
4. Then from there run in the command line with:
```bash
bin/MoviDB.exe
```

### With Source Code
#### Prerequisites

- .NET SDK installed
- Database created and configured
- Configuration file correctly filled

#### Run Command

From the project root:

```bash
dotnet run
```

Once started, the application will present an interactive CLI prompt.

---

## Using the Application (Tester Instructions)

### General Usage

- All interaction happens through typed commands
- Commands are **case-sensitive**
- Parameters must follow the defined constraints
- Invalid input will result in validation errors

To see all available commands at any time:

```
> Help
```

---

## Available Commands

### ImportSeries

Imports a TV series with seasons and episodes from a JSON file.

**Command**
```
> ImportSeries filePath="samples/series.json"
```

---

### RegisterMovie

Registers a new movie.

```
> RegisterMovie title="Inception" description="Dream heist" genre="SciFi" durationMinutes=148
```

---

### ListMovies

Lists movies using a paged, table-like view.

```
> ListMovies
```

Optional:
```
> ListMovies batchSize=5
```

#### Paging Controls

- `n` → Next page
- `p` → Previous page
- `e` → Exit listing

---

### CreateGenre

Creates a new genre.

```
> CreateGenre name="Drama"
```

---

### ListGenres

Lists all genres.

```
> ListGenres
```

---

### UpdateMovie

Updates an existing movie. Only provide fields that should change.

```
> UpdateMovie title="Inception" newTitle="Inception (2010)"
```

---

### DeleteMovie

Deletes a movie by title.

```
> DeleteMovie title="Inception"
```

---
