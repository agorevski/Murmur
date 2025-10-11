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
		InitializeComponent();
		_dataService = dataService;
		_analyticsService = analyticsService;
		_adService = adService;
		_billingService = billingService;
		
		InitializeApp();
	}

	private async void InitializeApp()
	{
		// Initialize database
		await _dataService.InitializeAsync();
		
		// Initialize analytics
		await _analyticsService.InitializeAsync();
		
		// Initialize ads
		await _adService.InitializeAsync();
		
		// Initialize billing
		await _billingService.InitializeAsync();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}
