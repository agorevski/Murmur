using SQLite;

namespace Murmur.App.Models;

[Table("UserPreferences")]
public class UserPreferences
{
    [PrimaryKey]
    public int Id { get; set; } = 1;
    
    public bool IsPremium { get; set; }
    public DateTime? PremiumExpiryDate { get; set; }
    public bool AdsEnabled { get; set; } = true;
    public int DefaultTimerMinutes { get; set; } = 30;
    public float DefaultVolume { get; set; } = 0.7f;
}
