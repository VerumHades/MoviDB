using MoviDB.Application.Services;
using MoviDB.Domain.DTOs;

namespace MoviDB.Presentation.CLI.Commands.User;

public class UserManagmentCommands
{
    public static ICommand CreateListUsersCommand(UserManagmentService userService)
    {
        return new ListBatchCommand<UserProjection, UserCursor>(
            name: "ListUsers",
            description: "Lists users interactively with cursor-based paging.",
            getBatchFunc: (batchSize, cursor) =>
                userService.GetNextBatchOfAllAsync(batchSize, cursor).GetAwaiter().GetResult()
        );
    }
    
    public static ICommand CreateUserCommand(UserManagmentService userManagementService)
    {
        return new SimpleCommand<string>(
            name: "CreateUser",
            description: "Creates a new user with a username and password.",
            parameters: new List<CommandParameter>
            {
                new("username", "The username for the new user", typeof(string), IsOptional: false),
                new("password", "The password for the new user", typeof(string), IsOptional: false)
            },
            action: async (paramValues, output) =>
            {
                if (!paramValues.TryGetValue("username", out var usernameObj) || usernameObj is not string username)
                    throw new ArgumentException("Parameter 'username' is required and must be a string.");

                if (!paramValues.TryGetValue("password", out var passwordObj) || passwordObj is not string password)
                    throw new ArgumentException("Parameter 'password' is required and must be a string.");

                await userManagementService.CreateUser(username, password);

                output.WriteLine($"Successfully created user '{username}'");
                return $"User '{username}' created.";
            }
        );
    }
    
    public static ICommand DeleteUserCommand(UserManagmentService userManagementService)
    {
        return new SimpleCommand<string>(
            name: "DeleteUser",
            description: "Deletes an existing user by username.",
            parameters: new List<CommandParameter>
            {
                new("username", "The username of the user to delete", typeof(string), IsOptional: false)
            },
            action: async (paramValues, output) =>
            {
                if (!paramValues.TryGetValue("username", out var usernameObj) || usernameObj is not string username)
                    throw new ArgumentException("Parameter 'username' is required and must be a string.");

                await userManagementService.DeleteUser(username);

                output.WriteLine($"Successfully deleted user '{username}'");
                return $"User '{username}' deleted.";
            }
        );
    }
}