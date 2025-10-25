using SQLite;

namespace Murmur.App.Models;

[Table("Mixes")]
public class Mix
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    public string SoundIds { get; set; } = string.Empty; // Comma-separated sound IDs
    public DateTime CreatedAt { get; set; }
    public DateTime LastUsed { get; set; }
    public bool IsFavorite { get; set; }
}
