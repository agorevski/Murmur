using Murmur.App.ViewModels;

namespace Murmur.App.Views;

public partial class PremiumPage : ContentPage
{
    private readonly PremiumViewModel _viewModel;

    public PremiumPage(PremiumViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CheckPremiumStatusCommand.ExecuteAsync(null);
    }
}
