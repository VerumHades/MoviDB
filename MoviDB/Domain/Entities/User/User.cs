using MoviDB.Domain.Common;

namespace MoviDB.Domain.Entities.User;


public enum UserRole
{
    Normal,
    Moderator
}

public sealed class User: Entity
{
    public string Username { get; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; }
    
    private const int MaxUsernameLength = 255;
    private const int MaxPasswordHashLength = 300;


    public User(string username, string passwordHash, UserRole role)
    {

        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));
        if (username.Length > MaxUsernameLength)
            throw new ArgumentException($"Username cannot exceed {MaxUsernameLength} characters.", nameof(username));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Description cannot be empty.", nameof(passwordHash));
        if (passwordHash.Length > MaxPasswordHashLength)
            throw new ArgumentException($"Description cannot exceed {MaxPasswordHashLength} characters.", nameof(passwordHash));
        
        Username = username;
        PasswordHash = passwordHash;
        Role = role;
    }

    public void ChangePassword(string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword)) throw new ArgumentException("Password cannot be empty");
        PasswordHash = newPassword;
    }

    public static User Hydrate(int id, string username, string passwordHash, UserRole role)
    {
        return new User(username, passwordHash, role) { Id = id };
    }
}