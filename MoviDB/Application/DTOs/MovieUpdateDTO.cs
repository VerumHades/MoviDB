namespace MoviDB.Application.DTOs;

public class MovieUpdateDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? GenreName { get; set; }
    public int? DurationMinutes { get; set; }
}