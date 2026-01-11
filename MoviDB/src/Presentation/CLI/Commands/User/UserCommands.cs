using MoviDB.Application.Services;

namespace MoviDB.Presentation.CLI.Commands.User;

public class UserCommands
{
    public static ICommand AddMovieToLibraryCommand(UserService userService)
    {
        return new SimpleCommand<string>(
            name: "AddMovieToLibrary",
            description: "Adds a movie to a user's library.",
            parameters: new List<CommandParameter>
            {
                new("username", "The username of the user", typeof(string), IsOptional: false),
                new("movieTitle", "The title of the movie to add", typeof(string), IsOptional: false)
            },
            action: async (paramValues, output) =>
            {
                if (!paramValues.TryGetValue("username", out var usernameObj) || usernameObj is not string username)
                    throw new ArgumentException("Parameter 'username' is required and must be a string.");

                if (!paramValues.TryGetValue("movieTitle", out var movieTitleObj) || movieTitleObj is not string movieTitle)
                    throw new ArgumentException("Parameter 'movieTitle' is required and must be a string.");

                await userService.AddMovieToLibrary(username, movieTitle);

                output.WriteLine($"Movie '{movieTitle}' added to user '{username}' library.");
                return $"Movie '{movieTitle}' added.";
            }
        );
    }
    public static ICommand AddSeriesToLibraryCommand(UserService userService)
    {
        return new SimpleCommand<string>(
            name: "AddSeriesToLibrary",
            description: "Adds a series to a user's library.",
            parameters: new List<CommandParameter>
            {
                new("username", "The username of the user", typeof(string), IsOptional: false),
                new("seriesTitle", "The title of the series to add", typeof(string), IsOptional: false)
            },
            action: async (paramValues, output) =>
            {
                if (!paramValues.TryGetValue("username", out var usernameObj) || usernameObj is not string username)
                    throw new ArgumentException("Parameter 'username' is required and must be a string.");

                if (!paramValues.TryGetValue("seriesTitle", out var seriesTitleObj) || seriesTitleObj is not string seriesTitle)
                    throw new ArgumentException("Parameter 'seriesTitle' is required and must be a string.");

                await userService.AddSeriesToLibrary(username, seriesTitle);

                output.WriteLine($"Series '{seriesTitle}' added to user '{username}' library.");
                return $"Series '{seriesTitle}' added.";
            }
        );
    }
    
     public static ICommand MarkMovieWatchedCommand(UserService service) =>
        new SimpleCommand<string>(
            name: "MarkMovieWatched",
            description: "Marks a movie as watched or unwatched for a user.",
            parameters: new List<CommandParameter>
            {
                new("username", "The username of the user", typeof(string), false),
                new("movieTitle", "Title of the movie", typeof(string), false),
                new("isWatched", "Set to true if watched, false if unwatched", typeof(bool), false)
            },
            action: async (parameters, output) =>
            {
                string username = Convert.ToString(parameters["username"])!;
                string movieTitle = Convert.ToString(parameters["movieTitle"])!;
                bool isWatched = Convert.ToBoolean(parameters["isWatched"]);

                await service.MarkMovieWatchedAsync(username, movieTitle, isWatched);
                output.WriteLine($"Movie '{movieTitle}' marked as {(isWatched ? "watched" : "unwatched")} for user '{username}'.");
                return "Operation completed.";
            }
        );

    public static ICommand MarkSeriesWatchedCommand(UserService service) =>
        new SimpleCommand<string>(
            name: "MarkSeriesWatched",
            description: "Marks a series as watched or unwatched for a user.",
            parameters: new List<CommandParameter>
            {
                new("username", "The username of the user", typeof(string), false),
                new("seriesTitle", "Title of the series", typeof(string), false),
                new("isWatched", "Set to true if watched, false if unwatched", typeof(bool), false)
            },
            action: async (parameters, output) =>
            {
                string username = Convert.ToString(parameters["username"])!;
                string seriesTitle = Convert.ToString(parameters["seriesTitle"])!;
                bool isWatched = Convert.ToBoolean(parameters["isWatched"]);

                await service.MarkSeriesWatchedAsync(username, seriesTitle, isWatched);
                output.WriteLine($"Series '{seriesTitle}' marked as {(isWatched ? "watched" : "unwatched")} for user '{username}'.");
                return "Operation completed.";
            }
        );
}