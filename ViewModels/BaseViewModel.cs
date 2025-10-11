using CommunityToolkit.Mvvm.ComponentModel;
using Murmur.App.Services;

namespace Murmur.App.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string title = string.Empty;

    protected readonly IAnalyticsService AnalyticsService;

    public BaseViewModel(IAnalyticsService analyticsService)
    {
        AnalyticsService = analyticsService;
    }

    protected virtual void OnAppearing()
    {
        AnalyticsService.TrackScreen(GetType().Name.Replace("ViewModel", ""));
    }
}
