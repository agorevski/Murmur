using Murmur.App.ViewModels;
using Murmur.App.Models;

namespace Murmur.App.Views;

public partial class MixerPage : ContentPage
{
    private readonly MixerViewModel _viewModel;

    public MixerPage(MixerViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnVolumeChanged(object sender, ValueChangedEventArgs e)
    {
        if (sender is Slider slider && slider.BindingContext is PlayingSound playingSound)
        {
            await _viewModel.UpdateVolumeCommand.ExecuteAsync(playingSound);
        }
    }
}
