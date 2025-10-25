using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Murmur.App.Models;
using Murmur.App.Services;
using System.Collections.ObjectModel;

namespace Murmur.App.ViewModels;

public partial class MixerViewModel : BaseViewModel
{
    private readonly IAudioService _audioService;
    private readonly ISoundLibraryService _soundLibraryService;
    private readonly IDataService _dataService;

    [ObservableProperty]
    private ObservableCollection<PlayingSound> activeSounds = new();

    [ObservableProperty]
    private string mixName = string.Empty;

    [ObservableProperty]
    private bool canSave;

    public MixerViewModel(
        IAudioService audioService,
        ISoundLibraryService soundLibraryService,
        IDataService dataService,
        IAnalyticsService analyticsService) : base(analyticsService)
    {
        _audioService = audioService;
        _soundLibraryService = soundLibraryService;
        _dataService = dataService;
        Title = "Mixer";
    }

    partial void OnActiveSoundsChanged(ObservableCollection<PlayingSound> value)
    {
        CanSave = value.Count > 0;
    }

    [RelayCommand]
    private async Task UpdateVolumeAsync(PlayingSound playingSound)
    {
        await _audioService.SetVolumeAsync(playingSound.Sound.Id, playingSound.Volume);
    }

    [RelayCommand]
    private async Task RemoveSoundAsync(PlayingSound playingSound)
    {
        await _audioService.FadeOutAsync(playingSound.Sound.Id, 500);
        ActiveSounds.Remove(playingSound);
    }

    [RelayCommand]
    private async Task SaveMixAsync()
    {
        if (string.IsNullOrWhiteSpace(MixName) || ActiveSounds.Count == 0)
            return;

        var mix = new Mix
        {
            Name = MixName,
            SoundIds = string.Join(",", ActiveSounds.Select(ps => ps.Sound.Id)),
            CreatedAt = DateTime.Now,
            LastUsed = DateTime.Now,
            IsFavorite = false
        };

        await _dataService.SaveMixAsync(mix);
        
        await Shell.Current.DisplayAlert("Saved", $"Mix '{MixName}' saved successfully!", "OK");
        MixName = string.Empty;
        
        AnalyticsService.TrackEvent("mix_saved", new Dictionary<string, string>
        {
            { "mix_name", mix.Name },
            { "sound_count", ActiveSounds.Count.ToString() }
        });
    }
}
