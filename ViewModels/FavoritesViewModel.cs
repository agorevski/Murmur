using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Murmur.App.Models;
using Murmur.App.Services;
using System.Collections.ObjectModel;

namespace Murmur.App.ViewModels;

public partial class FavoritesViewModel : BaseViewModel
{
    private readonly IDataService _dataService;
    private readonly IAudioService _audioService;
    private readonly ISoundLibraryService _soundLibraryService;

    [ObservableProperty]
    private ObservableCollection<Mix> mixes = new();

    [ObservableProperty]
    private ObservableCollection<Mix> favoriteMixes = new();

    public FavoritesViewModel(
        IDataService dataService,
        IAudioService audioService,
        ISoundLibraryService soundLibraryService,
        IAnalyticsService analyticsService) : base(analyticsService)
    {
        _dataService = dataService;
        _audioService = audioService;
        _soundLibraryService = soundLibraryService;
        Title = "Favorites";
    }

    [RelayCommand]
    private async Task LoadMixesAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        try
        {
            var allMixes = await _dataService.GetMixesAsync();
            Mixes = new ObservableCollection<Mix>(allMixes);

            var favorites = await _dataService.GetFavoriteMixesAsync();
            FavoriteMixes = new ObservableCollection<Mix>(favorites);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task PlayMixAsync(Mix mix)
    {
        // Stop all current sounds
        await _audioService.StopAllAsync();

        // Parse sound IDs
        var soundIds = mix.SoundIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(id => int.Parse(id.Trim()))
            .ToList();

        // Start each sound
        foreach (var soundId in soundIds)
        {
            var sound = await _soundLibraryService.GetSoundByIdAsync(soundId);
            if (sound != null)
            {
                await _audioService.PlaySoundAsync(sound, 0.7f);
            }
        }

        // Update last used timestamp
        mix.LastUsed = DateTime.Now;
        await _dataService.SaveMixAsync(mix);

        AnalyticsService.TrackEvent("mix_played", new Dictionary<string, string>
        {
            { "mix_name", mix.Name }
        });
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync(Mix mix)
    {
        mix.IsFavorite = !mix.IsFavorite;
        await _dataService.SaveMixAsync(mix);
        await LoadMixesAsync();

        AnalyticsService.TrackEvent("mix_favorite_toggled", new Dictionary<string, string>
        {
            { "mix_name", mix.Name },
            { "is_favorite", mix.IsFavorite.ToString() }
        });
    }

    [RelayCommand]
    private async Task DeleteMixAsync(Mix mix)
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Delete Mix",
            $"Are you sure you want to delete '{mix.Name}'?",
            "Delete",
            "Cancel");

        if (!confirm)
            return;

        await _dataService.DeleteMixAsync(mix);
        await LoadMixesAsync();

        AnalyticsService.TrackEvent("mix_deleted", new Dictionary<string, string>
        {
            { "mix_name", mix.Name }
        });
    }
}
