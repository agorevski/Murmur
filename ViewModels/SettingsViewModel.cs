using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Murmur.App.Models;
using Murmur.App.Services;

namespace Murmur.App.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly IDataService _dataService;
    private readonly IAdService _adService;

    [ObservableProperty]
    private UserPreferences preferences = new();

    [ObservableProperty]
    private string appVersion = "1.0.0";

    public SettingsViewModel(
        IDataService dataService,
        IAdService adService,
        IAnalyticsService analyticsService) : base(analyticsService)
    {
        _dataService = dataService;
        _adService = adService;
        Title = "Settings";
    }

    [RelayCommand]
    private async Task LoadPreferencesAsync()
    {
        Preferences = await _dataService.GetUserPreferencesAsync();
    }

    [RelayCommand]
    private async Task SavePreferencesAsync()
    {
        await _dataService.SaveUserPreferencesAsync(Preferences);
        _adService.AdsEnabled = Preferences.AdsEnabled;
        
        await Shell.Current.DisplayAlert("Saved", "Settings saved successfully!", "OK");
        
        AnalyticsService.TrackEvent("settings_saved");
    }

    [RelayCommand]
    private async Task ResetToDefaultsAsync()
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Reset Settings",
            "Are you sure you want to reset all settings to defaults?",
            "Reset",
            "Cancel");

        if (!confirm)
            return;

        Preferences = new UserPreferences
        {
            Id = 1,
            IsPremium = Preferences.IsPremium, // Keep premium status
            AdsEnabled = !Preferences.IsPremium,
            DefaultTimerMinutes = 30,
            DefaultVolume = 0.7f
        };

        await _dataService.SaveUserPreferencesAsync(Preferences);
        
        await Shell.Current.DisplayAlert("Reset", "Settings reset to defaults!", "OK");
        
        AnalyticsService.TrackEvent("settings_reset");
    }

    [RelayCommand]
    private async Task OpenPrivacyPolicyAsync()
    {
        await Browser.OpenAsync("https://example.com/privacy", BrowserLaunchMode.SystemPreferred);
        AnalyticsService.TrackEvent("privacy_policy_opened");
    }

    [RelayCommand]
    private async Task OpenTermsOfServiceAsync()
    {
        await Browser.OpenAsync("https://example.com/terms", BrowserLaunchMode.SystemPreferred);
        AnalyticsService.TrackEvent("terms_opened");
    }
}
