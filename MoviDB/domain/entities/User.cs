namespace MoviDB.domain.entities;


public enum UserRole
{
    Normal,
    Moderator
}

public sealed class User
{
    public int Id { get; }
    public string Username { get; }
    public string Password { get; private set; }
    public UserRole Role { get; }
    public DateTime CreatedAt { get; }

    public User(int id, string username, string password, UserRole role, DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username cannot be empty");
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password cannot be empty");

        Id = id;
        Username = username;
        Password = password;
        Role = role;
        CreatedAt = createdAt;
    }

    public void ChangePassword(string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword)) throw new ArgumentException("Password cannot be empty");
        Password = newPassword;
    }
}