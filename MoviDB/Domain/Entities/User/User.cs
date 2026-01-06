using MoviDB.Domain.Common;

namespace MoviDB.Domain.Entities.User;


public enum UserRole
{
    Normal,
    Moderator
}

public sealed class User: TimestampedEntity
{
    public string Username { get; }
    public string Password { get; private set; }
    public UserRole Role { get; }

    public User(string username, string password, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username cannot be empty");
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password cannot be empty");


        Username = username;
        Password = password;
        Role = role;
    }

    public void ChangePassword(string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword)) throw new ArgumentException("Password cannot be empty");
        Password = newPassword;
    }
}