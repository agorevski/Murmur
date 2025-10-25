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
// Add global exception handlers
AppDomain.CurrentDomain.UnhandledException += (s, e) =>
{
var ex = e.ExceptionObject as Exception;
System.Diagnostics.Debug.WriteLine($"UNHANDLED EXCEPTION: {ex?.Message}");
System.Diagnostics.Debug.WriteLine($"Stack trace: {ex?.StackTrace}");
System.Diagnostics.Debug.WriteLine($"Inner exception: {ex?.InnerException?.Message}");
};

TaskScheduler.UnobservedTaskException += (s, e) =>
{
System.Diagnostics.Debug.WriteLine($"UNOBSERVED TASK EXCEPTION: {e.Exception.Message}");
System.Diagnostics.Debug.WriteLine($"Stack trace: {e.Exception.StackTrace}");
e.SetObserved();
};

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
builder.Logging.SetMinimumLevel(LogLevel.Debug);
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
