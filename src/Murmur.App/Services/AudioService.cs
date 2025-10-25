using Plugin.Maui.Audio;
using Murmur.App.Models;

namespace Murmur.App.Services;

public interface IAudioService
{
    Task<bool> PlaySoundAsync(Sound sound, float volume = 1.0f);
    Task StopSoundAsync(int soundId);
    Task StopAllAsync();
    Task SetVolumeAsync(int soundId, float volume);
    Task FadeInAsync(int soundId, float targetVolume, int durationMs);
    Task FadeOutAsync(int soundId, int durationMs);
    bool IsSoundPlaying(int soundId);
    List<int> GetPlayingSoundIds();
}

public class AudioService : IAudioService
{
    private readonly IAudioManager _audioManager;
    private readonly Dictionary<int, IAudioPlayer> _players = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public AudioService(IAudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    public async Task<bool> PlaySoundAsync(Sound sound, float volume = 1.0f)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_players.ContainsKey(sound.Id))
            {
                return false; // Already playing
            }

            try
            {
                var audioFile = await FileSystem.OpenAppPackageFileAsync($"Sounds/{sound.FileName}");
                var player = _audioManager.CreatePlayer(audioFile);
                
                player.Volume = volume;
                player.Loop = true;
                
                _players[sound.Id] = player;
                player.Play();
                
                return true;
            }
            catch (Exception ex)
            {
                // Audio file not found - log the error for debugging
                System.Diagnostics.Debug.WriteLine($"Failed to load audio file: Sounds/{sound.FileName}");
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                throw new FileNotFoundException($"Audio file 'Sounds/{sound.FileName}' not found. Please add audio files to the Resources/Raw/Sounds directory.", ex);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task StopSoundAsync(int soundId)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_players.TryGetValue(soundId, out var player))
            {
                player.Stop();
                player.Dispose();
                _players.Remove(soundId);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task StopAllAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            foreach (var player in _players.Values)
            {
                player.Stop();
                player.Dispose();
            }
            _players.Clear();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task SetVolumeAsync(int soundId, float volume)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_players.TryGetValue(soundId, out var player))
            {
                player.Volume = Math.Clamp(volume, 0f, 1f);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task FadeInAsync(int soundId, float targetVolume, int durationMs)
    {
        if (!_players.ContainsKey(soundId))
            return;

        const int steps = 20;
        var stepDelay = durationMs / steps;
        var volumeStep = targetVolume / steps;

        for (int i = 0; i <= steps; i++)
        {
            await SetVolumeAsync(soundId, volumeStep * i);
            await Task.Delay(stepDelay);
        }
    }

    public async Task FadeOutAsync(int soundId, int durationMs)
    {
        if (!_players.TryGetValue(soundId, out var player))
            return;

        var currentVolume = player.Volume;
        const int steps = 20;
        var stepDelay = durationMs / steps;
        var volumeStep = currentVolume / steps;

        for (int i = steps; i >= 0; i--)
        {
            await SetVolumeAsync(soundId, (float)(volumeStep * i));
            await Task.Delay(stepDelay);
        }

        await StopSoundAsync(soundId);
    }

    public bool IsSoundPlaying(int soundId)
    {
        return _players.ContainsKey(soundId) && _players[soundId].IsPlaying;
    }

    public List<int> GetPlayingSoundIds()
    {
        return _players.Keys.ToList();
    }
}
