namespace Murmur.App.Models;

public class Sound
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsPremium { get; set; }
    public string IconUrl { get; set; } = string.Empty;
}
