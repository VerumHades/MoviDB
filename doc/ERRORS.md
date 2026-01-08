# MoviDB CLI Application - Error Documentation

This document outlines the possible errors in the `Program` class, including configuration loading, database connection, input validation, and runtime operation errors, along with guidance for resolving them.
Do note that some of these messages are purely illustrative and may be displayed another way.
---

## 1. Configuration Load Errors

**Location in code:**

```csharp
config = new JsonFileConfigLoader("DatabaseConfig.json").LoadConfiguration();
```

**Potential Errors:**

* FileNotFoundException if the configuration file is missing.
* Malformed JSON causing deserialization to fail.

**Causes:**

* `DatabaseConfig.json` does not exist in the same folder as the executable.
* JSON structure does not match `DatabaseConnectionConfig`.

**Solutions:**

1. Ensure `DatabaseConfig.json` is present **next to the executable**.
2. Verify that the JSON structure is correct:

```json
{
  "Server": "localhost",
  "Database": "MoviesDB",
  "UserId": "normal_user_login",
  "Password": "NormalUserStrongPassword!123"
}
```

3. Check file permissions to make sure the program can read it.

---

## 2. Database Connection Errors

**Location in code:**

```csharp
connectionFactory = new SqlConnectionFactory(config);
using var initialConnection = connectionFactory.CreateOpenConnection();
```

**Potential Errors:**

* `SqlException` or inability to open the connection.

**Common Causes:**

* Incorrect **server** or **database** names.
* Wrong **login credentials** (username/password).
* Missing database user or insufficient permissions.
* SQL Server not running or unreachable.

**Solutions:**

1. Confirm the values in `DatabaseConfig.json`.
2. Ensure SQL Server is running and accessible.
3. Verify that the login exists in SQL Server and is mapped to a user in the database.
4. Check that the database name is correct.
5. Verify network access for remote SQL Server connections.

--- 
## 3. Input Validation Errors

### 1. Command Not Found

**When it occurs:**

* The user types a command that does not exist in the CLI registry.

**Example Message:**

```
Command 'UnknownCommand' not found. Type 'Help' to see available commands.
```

**How to solve:**

* Check spelling and capitalization of the command.
* Type `Help` to see a list of all valid commands.

---

### 2. Invalid Parameter Format

**When it occurs:**

* The user provides parameters that are not in `key=value` format.

**Example Message:**

```
Skipping invalid parameter: invalidParam
```

**How to solve:**

* Always provide parameters in `key=value` format.
* Example:

```
RegisterMovie title="Inception" genre_name="Sci-Fi" release_year=2010
```

---

### 3. Missing Required Parameters

**When it occurs:**

* The user omits a parameter that is required by a command.

**Example Message:**

```
Parameter validation failed:
 - Missing required parameter 'title'.
```

**How to solve:**

* Provide all required parameters.
* Use `Help <CommandName>` to see which parameters are required.
* Example:

```
ImportSeriesCommand title="Breaking Bad" genre_name="Drama"
```

---

### 4. Invalid Parameter Type

**When it occurs:**

* The user provides a parameter value that cannot be converted to the expected type.

**Example Message:**

```
Parameter 'episode_number' must be of type Int32.
```

**How to solve:**

* Make sure parameter values match the expected type.
* Example:

```
episode_number=1  // integer, not text
```

---

### 5. Constraint Validation Failure

**When it occurs:**

* The user provides a parameter that violates a constraint (e.g., min/max length, allowed range).

**Example Message:**

```
Parameter 'season_number' invalid: must be greater than 0
```

**How to solve:**

* Follow the constraints indicated in the error.
* Check `Help` for rules on parameter values.
* Example:

```
season_number=1  // must be positive integer
```

---

### 6. General Input Errors

**When it occurs:**

* Any combination of missing, incorrectly formatted, or invalid parameters.

**Example Message:**

```
Parameter validation failed:
 - Missing required parameter 'title'.
 - Parameter 'episode_number' must be of type Int32.
```

**How to solve:**

* Review the error messages for each parameter.
* Correct the input to meet all requirements, types, and constraints.
* Always refer to `Help` for proper usage.

---

### Summary Table of CLI Input Errors

| Error Type                    | When it Occurs                                       | Example User Message                                      | Correct Input Example                                   |
| ----------------------------- | ---------------------------------------------------- | --------------------------------------------------------- | ------------------------------------------------------- |
| Command not found             | Command does not exist in the registry               | Command 'UnknownCommand' not found                        | Help / Use a valid command name                         |
| Invalid parameter format      | Parameter not in key=value form                      | Skipping invalid parameter: invalidParam                  | title="Inception" genre_name="Sci-Fi" release_year=2010 |
| Missing required parameters   | Required parameter is omitted                        | Missing required parameter 'title'                        | title="Breaking Bad" genre_name="Drama"                 |
| Invalid parameter type        | Parameter value cannot be converted to expected type | Parameter 'episode_number' must be of type Int32          | episode_number=1                                        |
| Constraint validation failure | Parameter violates rules (range, length, etc.)       | Parameter 'season_number' invalid: must be greater than 0 | season_number=1                                         |
| General input errors          | Multiple issues in input                             | Parameter validation failed: ...                          | Correct all highlighted issues, check Help for guidance |
