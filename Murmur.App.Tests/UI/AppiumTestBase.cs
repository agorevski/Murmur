using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;

namespace Murmur.App.Tests.UI;

/// <summary>
/// Base class for Appium UI tests providing common setup and helper methods
/// </summary>
public abstract class AppiumTestBase : IDisposable
{
    protected AndroidDriver Driver { get; private set; } = null!;
    protected bool IsDriverInitialized { get; private set; }

    /// <summary>
    /// Initialize the Appium driver with default settings
    /// </summary>
    protected void InitializeDriver()
    {
        InitializeDriver(new AppiumTestConfiguration());
    }

    /// <summary>
    /// Initialize the Appium driver with custom configuration
    /// </summary>
    protected void InitializeDriver(AppiumTestConfiguration config)
    {
        var appiumOptions = new AppiumOptions();
        
        // Platform capabilities
        appiumOptions.AddAdditionalAppiumOption(MobileCapabilityType.PlatformName, "Android");
        appiumOptions.AddAdditionalAppiumOption(MobileCapabilityType.DeviceName, config.DeviceName);
        appiumOptions.AddAdditionalAppiumOption(MobileCapabilityType.PlatformVersion, config.PlatformVersion);
        appiumOptions.AddAdditionalAppiumOption(MobileCapabilityType.AutomationName, "UiAutomator2");
        
        // App capabilities
        appiumOptions.AddAdditionalAppiumOption(MobileCapabilityType.App, config.AppPath);
        appiumOptions.AddAdditionalAppiumOption("appPackage", "com.driftly.murmur");
        appiumOptions.AddAdditionalAppiumOption("appActivity", "crc64e1fb321c08285b90.MainActivity");
        
        // Additional settings
        appiumOptions.AddAdditionalAppiumOption("noReset", config.NoReset);
        appiumOptions.AddAdditionalAppiumOption("fullReset", config.FullReset);
        appiumOptions.AddAdditionalAppiumOption("newCommandTimeout", config.CommandTimeout);
        appiumOptions.AddAdditionalAppiumOption("autoGrantPermissions", true);

        Driver = new AndroidDriver(
            new Uri(config.AppiumServerUrl),
            appiumOptions,
            TimeSpan.FromSeconds(config.InitializationTimeout));

        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(config.ImplicitWaitTimeout);
        IsDriverInitialized = true;
    }

    #region Helper Methods

    /// <summary>
    /// Find element by AutomationId (ContentDescription in Android)
    /// </summary>
    protected AppiumElement? FindByAutomationId(string automationId)
    {
        try
        {
            return Driver.FindElement(MobileBy.AccessibilityId(automationId));
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Find element by text
    /// </summary>
    protected AppiumElement? FindByText(string text)
    {
        try
        {
            return Driver.FindElement(MobileBy.AndroidUIAutomator(
                $"new UiSelector().text(\"{text}\")"));
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Find element by partial text
    /// </summary>
    protected AppiumElement? FindByPartialText(string partialText)
    {
        try
        {
            return Driver.FindElement(MobileBy.AndroidUIAutomator(
                $"new UiSelector().textContains(\"{partialText}\")"));
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Wait for element to be visible
    /// </summary>
    protected bool WaitForElement(string automationId, int timeoutSeconds = 10)
    {
        var endTime = DateTime.Now.AddSeconds(timeoutSeconds);
        while (DateTime.Now < endTime)
        {
            var element = FindByAutomationId(automationId);
            if (element != null && element.Displayed)
                return true;
            
            Thread.Sleep(500);
        }
        return false;
    }

    /// <summary>
    /// Tap element by AutomationId
    /// </summary>
    protected void TapElement(string automationId)
    {
        var element = FindByAutomationId(automationId);
        element?.Click();
    }

    /// <summary>
    /// Get text from element
    /// </summary>
    protected string? GetElementText(string automationId)
    {
        var element = FindByAutomationId(automationId);
        return element?.Text;
    }

    /// <summary>
    /// Check if element is enabled
    /// </summary>
    protected bool IsElementEnabled(string automationId)
    {
        var element = FindByAutomationId(automationId);
        return element?.Enabled ?? false;
    }

    /// <summary>
    /// Check if element is displayed
    /// </summary>
    protected bool IsElementDisplayed(string automationId)
    {
        var element = FindByAutomationId(automationId);
        return element?.Displayed ?? false;
    }

    /// <summary>
    /// Swipe from one coordinate to another
    /// </summary>
    protected void Swipe(int startX, int startY, int endX, int endY, int duration = 1000)
    {
        // Note: Swipe implementation depends on Appium version
        // This is a placeholder for the swipe gesture
    }

    /// <summary>
    /// Navigate back (Android back button)
    /// </summary>
    protected void NavigateBack()
    {
        Driver.Navigate().Back();
    }

    /// <summary>
    /// Take screenshot
    /// </summary>
    protected void TakeScreenshot(string fileName)
    {
        var screenshot = Driver.GetScreenshot();
        var fullPath = Path.Combine("Screenshots", $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
        Directory.CreateDirectory("Screenshots");
        screenshot.SaveAsFile(fullPath);
    }

    /// <summary>
    /// Wait for a specified time
    /// </summary>
    protected void Wait(int milliseconds)
    {
        Thread.Sleep(milliseconds);
    }

    #endregion

    public virtual void Dispose()
    {
        if (IsDriverInitialized)
        {
            try
            {
                Driver?.Quit();
            }
            catch
            {
                // Ignore disposal errors
            }
        }
    }
}

/// <summary>
/// Configuration for Appium tests
/// </summary>
public class AppiumTestConfiguration
{
    public string AppiumServerUrl { get; set; } = "http://localhost:4723/wd/hub";
    public string DeviceName { get; set; } = "Android Emulator";
    public string PlatformVersion { get; set; } = "11";
    public string AppPath { get; set; } = @"C:\path\to\Murmur.App.apk";
    public bool NoReset { get; set; } = true;
    public bool FullReset { get; set; } = false;
    public int CommandTimeout { get; set; } = 300;
    public int InitializationTimeout { get; set; } = 60;
    public int ImplicitWaitTimeout { get; set; } = 10;
}
