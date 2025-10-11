using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using Murmur.App.Services;
using Murmur.App.ViewModels;
using Murmur.App.Views;

namespace Murmur.App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Register services
		builder.Services.AddSingleton(AudioManager.Current);
		builder.Services.AddSingleton<IAudioService, AudioService>();
		builder.Services.AddSingleton<IDataService, DataService>();
		builder.Services.AddSingleton<ISoundLibraryService, SoundLibraryService>();
		builder.Services.AddSingleton<IAdService, AdService>();
		builder.Services.AddSingleton<IBillingService, BillingService>();
		builder.Services.AddSingleton<IAnalyticsService, AnalyticsService>();

		// Register ViewModels
		builder.Services.AddTransient<HomeViewModel>();
		builder.Services.AddTransient<MixerViewModel>();
		builder.Services.AddTransient<FavoritesViewModel>();
		builder.Services.AddTransient<PremiumViewModel>();
		builder.Services.AddTransient<SettingsViewModel>();

		// Register Views
		builder.Services.AddTransient<HomePage>();
		builder.Services.AddTransient<MixerPage>();
		builder.Services.AddTransient<FavoritesPage>();
		builder.Services.AddTransient<PremiumPage>();
		builder.Services.AddTransient<SettingsPage>();

		return builder.Build();
	}
}
