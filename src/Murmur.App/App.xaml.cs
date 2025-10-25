using Murmur.App.Services;

namespace Murmur.App;

public partial class App : Application
{
private readonly IDataService _dataService;
private readonly IAnalyticsService _analyticsService;
private readonly IAdService _adService;
private readonly IBillingService _billingService;

public App(IDataService dataService, IAnalyticsService analyticsService, IAdService adService, IBillingService billingService)
{
try
{
System.Diagnostics.Debug.WriteLine("=== App constructor starting ===");
InitializeComponent();
System.Diagnostics.Debug.WriteLine("InitializeComponent completed");
_dataService = dataService;
_analyticsService = analyticsService;
_adService = adService;
_billingService = billingService;
System.Diagnostics.Debug.WriteLine("Services assigned");

System.Diagnostics.Debug.WriteLine("Creating AppShell...");
MainPage = new AppShell();
System.Diagnostics.Debug.WriteLine("AppShell created and assigned to MainPage");

// Initialize services after MainPage is set
System.Diagnostics.Debug.WriteLine("Starting background service initialization");
Task.Run(async () => await InitializeServicesAsync());
System.Diagnostics.Debug.WriteLine("=== App constructor completed ===");
}
catch (Exception ex)
{
System.Diagnostics.Debug.WriteLine($"FATAL ERROR in App constructor: {ex.Message}");
System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException?.Message}");
throw;
}
}

private async Task InitializeServicesAsync()
{
try
{
System.Diagnostics.Debug.WriteLine("=== Starting service initialization ===");

// Initialize database
try
{
System.Diagnostics.Debug.WriteLine("Initializing DataService...");
await _dataService.InitializeAsync();
System.Diagnostics.Debug.WriteLine("DataService initialized successfully");
}
catch (Exception ex)
{
System.Diagnostics.Debug.WriteLine($"ERROR: DataService initialization failed: {ex.Message}");
System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
// Don't crash - continue with other services
}

// Initialize analytics
try
{
System.Diagnostics.Debug.WriteLine("Initializing AnalyticsService...");
await _analyticsService.InitializeAsync();
System.Diagnostics.Debug.WriteLine("AnalyticsService initialized successfully");
}
catch (Exception ex)
{
System.Diagnostics.Debug.WriteLine($"ERROR: AnalyticsService initialization failed: {ex.Message}");
System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
// Don't crash - continue with other services
}

// Initialize ads
try
{
System.Diagnostics.Debug.WriteLine("Initializing AdService...");
await _adService.InitializeAsync();
System.Diagnostics.Debug.WriteLine("AdService initialized successfully");
}
catch (Exception ex)
{
System.Diagnostics.Debug.WriteLine($"ERROR: AdService initialization failed: {ex.Message}");
System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
// Don't crash - continue with other services
}

// Initialize billing
try
{
System.Diagnostics.Debug.WriteLine("Initializing BillingService...");
await _billingService.InitializeAsync();
System.Diagnostics.Debug.WriteLine("BillingService initialized successfully");
}
catch (Exception ex)
{
System.Diagnostics.Debug.WriteLine($"ERROR: BillingService initialization failed: {ex.Message}");
System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
// Don't crash - continue
}

System.Diagnostics.Debug.WriteLine("=== Service initialization complete ===");
}
catch (Exception ex)
{
System.Diagnostics.Debug.WriteLine($"FATAL ERROR during service initialization: {ex.Message}");
System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
}
}

protected override Window CreateWindow(IActivationState? activationState)
{
return new Window(MainPage);
}
}
