namespace MoviDB.Presentation.CLI.Constraints;

/// <summary>
/// Example: Constraint that allows only specific characters.
/// </summary>
public class AllowedCharactersConstraint : IParameterConstraint
{
    private readonly string _allowedCharacters;

    public AllowedCharactersConstraint(string allowedCharacters)
    {
        _allowedCharacters = allowedCharacters;
    }

    public bool IsValid(string value)
    {
        foreach (var c in value)
        {
            if (!_allowedCharacters.Contains(c))
                return false;
        }
        return true;
    }

    public string Description => $"Value can only contain these characters: {_allowedCharacters}";
}
