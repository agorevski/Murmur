using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Murmur.App.Services;

namespace Murmur.App.ViewModels;

public partial class PremiumViewModel : BaseViewModel
{
    private readonly IBillingService _billingService;
    private readonly IDataService _dataService;
    private readonly IAdService _adService;

    [ObservableProperty]
    private bool isPremium;

    [ObservableProperty]
    private List<string> premiumFeatures = new()
    {
        "Remove all ads",
        "Unlimited sound mixing",
        "Access to all premium sounds",
        "Offline playback",
        "Advanced timer settings",
        "Export custom mixes"
    };

    public PremiumViewModel(
        IBillingService billingService,
        IDataService dataService,
        IAdService adService,
        IAnalyticsService analyticsService) : base(analyticsService)
    {
        _billingService = billingService;
        _dataService = dataService;
        _adService = adService;
        Title = "Premium";
    }

    [RelayCommand]
    private async Task CheckPremiumStatusAsync()
    {
        var prefs = await _dataService.GetUserPreferencesAsync();
        IsPremium = prefs.IsPremium;
    }

    [RelayCommand]
    private async Task PurchasePremiumAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        try
        {
            var success = await _billingService.PurchasePremiumAsync();
            
            if (success)
            {
                var prefs = await _dataService.GetUserPreferencesAsync();
                prefs.IsPremium = true;
                prefs.AdsEnabled = false;
                await _dataService.SaveUserPreferencesAsync(prefs);

                _adService.AdsEnabled = false;
                IsPremium = true;

                await Shell.Current.DisplayAlert("Success", "Welcome to Murmur Premium!", "OK");
                
                AnalyticsService.TrackEvent("premium_purchased");
                AnalyticsService.SetUserProperty("is_premium", "true");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RestorePurchasesAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        try
        {
            var restored = await _billingService.RestorePurchasesAsync();
            
            if (restored)
            {
                var prefs = await _dataService.GetUserPreferencesAsync();
                prefs.IsPremium = true;
                prefs.AdsEnabled = false;
                await _dataService.SaveUserPreferencesAsync(prefs);

                _adService.AdsEnabled = false;
                IsPremium = true;

                await Shell.Current.DisplayAlert("Restored", "Your premium subscription has been restored!", "OK");
                
                AnalyticsService.TrackEvent("premium_restored");
            }
            else
            {
                await Shell.Current.DisplayAlert("Not Found", "No premium subscription found.", "OK");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}
