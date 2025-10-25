using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Murmur.App.Models;
using Murmur.App.Services;
using System.Collections.ObjectModel;

namespace Murmur.App.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
    private readonly IAudioService _audioService;
    private readonly ISoundLibraryService _soundLibraryService;
    private readonly IDataService _dataService;
    private readonly IAdService _adService;

    [ObservableProperty]
    private ObservableCollection<PlayingSound> playingSounds = new();

    [ObservableProperty]
    private ObservableCollection<Sound> availableSounds = new();

    [ObservableProperty]
    private bool isPremium;

    [ObservableProperty]
    private bool isPlaying;

    [ObservableProperty]
    private int timerMinutes;

    [ObservableProperty]
    private bool timerActive;

    private CancellationTokenSource? _timerCts;

    public HomeViewModel(
        IAudioService audioService,
        ISoundLibraryService soundLibraryService,
        IDataService dataService,
        IAdService adService,
        IAnalyticsService analyticsService) : base(analyticsService)
    {
        _audioService = audioService;
        _soundLibraryService = soundLibraryService;
        _dataService = dataService;
        _adService = adService;
        Title = "Home";
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        try
        {
            // Stop any sounds that might be playing from a previous session
            await _audioService.StopAllAsync();
            PlayingSounds.Clear();
            IsPlaying = false;

            var prefs = await _dataService.GetUserPreferencesAsync();
            IsPremium = prefs.IsPremium;
            TimerMinutes = prefs.DefaultTimerMinutes;

            var sounds = IsPremium
                ? await _soundLibraryService.GetAllSoundsAsync()
                : await _soundLibraryService.GetFreeSoundsAsync();

            AvailableSounds = new ObservableCollection<Sound>(sounds);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ToggleSoundAsync(Sound sound)
    {
        var playingSound = PlayingSounds.FirstOrDefault(ps => ps.Sound.Id == sound.Id);

        if (playingSound != null)
        {
            // Stop the sound
            await _audioService.FadeOutAsync(sound.Id, 1000);
            PlayingSounds.Remove(playingSound);
        }
        else
        {
            // Check free user limit (3 sounds max)
            if (!IsPremium && PlayingSounds.Count >= 3)
            {
                await Shell.Current.DisplayAlert("Free Limit", "Free users can mix up to 3 sounds. Upgrade to Premium for unlimited mixing!", "OK");
                return;
            }

            // Start the sound
            try
            {
                var success = await _audioService.PlaySoundAsync(sound, 0f);
                if (success)
                {
                    var ps = new PlayingSound { Sound = sound, Volume = 0.7f, IsPlaying = true };
                    PlayingSounds.Add(ps);
                    await _audioService.FadeInAsync(sound.Id, 0.7f, 1000);
                }
            }
            catch (FileNotFoundException ex)
            {
                await Shell.Current.DisplayAlert(
                    "Audio Files Missing", 
                    $"The audio files haven't been added to the project yet.\n\n" +
                    $"Missing: {sound.FileName}\n\n" +
                    $"To fix this:\n" +
                    $"1. Create a 'Sounds' folder in Resources/Raw/\n" +
                    $"2. Add MP3 files: rain.mp3, ocean.mp3, forest.mp3, etc.\n" +
                    $"3. Set 'Build Action' to 'MauiAsset' for each file\n\n" +
                    $"See IMPLEMENTATION_SUMMARY.md for details.", 
                    "OK");
                return;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to play sound: {ex.Message}", "OK");
                return;
            }
        }

        IsPlaying = PlayingSounds.Count > 0;
        AnalyticsService.TrackEvent("sound_toggled", new Dictionary<string, string>
        {
            { "sound_name", sound.Name },
            { "action", playingSound != null ? "stop" : "start" }
        });
    }

    [RelayCommand]
    private async Task StopAllAsync()
    {
        await _audioService.StopAllAsync();
        PlayingSounds.Clear();
        IsPlaying = false;
        AnalyticsService.TrackEvent("stop_all");
    }

    [RelayCommand]
    private void StartTimer()
    {
        if (TimerActive || TimerMinutes <= 0)
            return;

        TimerActive = true;
        _timerCts = new CancellationTokenSource();

        Task.Run(async () =>
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(TimerMinutes), _timerCts.Token);
                await StopAllAsync();
                TimerActive = false;
            }
            catch (TaskCanceledException)
            {
                // Timer was cancelled
            }
        });

        AnalyticsService.TrackEvent("timer_started", new Dictionary<string, string>
        {
            { "duration_minutes", TimerMinutes.ToString() }
        });
    }

    [RelayCommand]
    private void StopTimer()
    {
        _timerCts?.Cancel();
        TimerActive = false;
        AnalyticsService.TrackEvent("timer_stopped");
    }
}
