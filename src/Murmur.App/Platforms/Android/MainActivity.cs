using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

namespace Murmur.App;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("=== MainActivity.OnCreate starting ===");
            
            // Set up Android-specific exception handler
            AndroidEnvironment.UnhandledExceptionRaiser += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"ANDROID UNHANDLED EXCEPTION: {e.Exception.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {e.Exception.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"Inner exception: {e.Exception.InnerException?.Message}");
                e.Handled = false; // Let the system handle it but we've logged it
            };

            base.OnCreate(savedInstanceState);
            
            System.Diagnostics.Debug.WriteLine("=== MainActivity.OnCreate completed ===");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"EXCEPTION in MainActivity.OnCreate: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }
}
