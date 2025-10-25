using Newtonsoft.Json;
using Murmur.App.Models;

namespace Murmur.App.Services;

public interface ISoundLibraryService
{
    Task<List<Sound>> GetAllSoundsAsync();
    Task<List<Sound>> GetFreeSoundsAsync();
    Task<Sound?> GetSoundByIdAsync(int id);
}

public class SoundLibraryService : ISoundLibraryService
{
    private List<Sound>? _sounds;

    public async Task<List<Sound>> GetAllSoundsAsync()
    {
        if (_sounds != null)
            return _sounds;

        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("sounds.json");
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();
            _sounds = JsonConvert.DeserializeObject<List<Sound>>(json) ?? new List<Sound>();
        }
        catch
        {
            _sounds = GetDefaultSounds();
        }

        return _sounds;
    }

    public async Task<List<Sound>> GetFreeSoundsAsync()
    {
        var allSounds = await GetAllSoundsAsync();
        return allSounds.Where(s => !s.IsPremium).ToList();
    }

    public async Task<Sound?> GetSoundByIdAsync(int id)
    {
        var sounds = await GetAllSoundsAsync();
        return sounds.FirstOrDefault(s => s.Id == id);
    }

    private List<Sound> GetDefaultSounds()
    {
        return new List<Sound>
        {
            new Sound { Id = 1, Name = "Rain", Description = "Gentle rain sounds", FileName = "rain.mp3", Category = "Nature", IsPremium = false, IconUrl = "rain_icon.png" },
            new Sound { Id = 2, Name = "Ocean Waves", Description = "Calming ocean waves", FileName = "ocean.mp3", Category = "Nature", IsPremium = false, IconUrl = "ocean_icon.png" },
            new Sound { Id = 3, Name = "Forest", Description = "Forest ambience", FileName = "forest.mp3", Category = "Nature", IsPremium = false, IconUrl = "forest_icon.png" },
            new Sound { Id = 4, Name = "Thunderstorm", Description = "Distant thunder", FileName = "thunder.mp3", Category = "Nature", IsPremium = true, IconUrl = "thunder_icon.png" },
            new Sound { Id = 5, Name = "Fireplace", Description = "Crackling fire", FileName = "fire.mp3", Category = "Indoor", IsPremium = true, IconUrl = "fire_icon.png" },
            new Sound { Id = 6, Name = "White Noise", Description = "Pure white noise", FileName = "whitenoise.mp3", Category = "Noise", IsPremium = true, IconUrl = "whitenoise_icon.png" },
            new Sound { Id = 7, Name = "Birds", Description = "Morning birds", FileName = "birds.mp3", Category = "Nature", IsPremium = true, IconUrl = "birds_icon.png" },
            new Sound { Id = 8, Name = "Wind", Description = "Gentle wind", FileName = "wind.mp3", Category = "Nature", IsPremium = true, IconUrl = "wind_icon.png" }
        };
    }
}
