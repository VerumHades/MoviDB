namespace MoviDB.Presentation.CLI.Constraints;

/// <summary>
/// Example: Constraint using a regex pattern.
/// </summary>
public class RegexConstraint : IParameterConstraint
{
    private readonly System.Text.RegularExpressions.Regex _regex;
    private readonly string _description;

    public RegexConstraint(string pattern, string description)
    {
        _regex = new System.Text.RegularExpressions.Regex(pattern);
        _description = description;
    }

    public bool IsValid(string value) => _regex.IsMatch(value);

    public string Description => _description;
}