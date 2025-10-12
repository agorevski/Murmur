using Moq;
using Murmur.App.Models;
using Murmur.App.Services;

namespace Murmur.App.Tests.Mocks;

public class MockDataService
{
    public Mock<IDataService> Mock { get; }
    private UserPreferences _preferences;

    public MockDataService(bool isPremium = false)
    {
        Mock = new Mock<IDataService>();
        _preferences = new UserPreferences
        {
            IsPremium = isPremium,
            DefaultTimerMinutes = 30,
            DefaultVolume = 0.7f,
            AdsEnabled = true
        };
        SetupDefaultBehavior();
    }

    private void SetupDefaultBehavior()
    {
        Mock.Setup(x => x.GetUserPreferencesAsync())
            .ReturnsAsync(() => _preferences);

        Mock.Setup(x => x.SaveUserPreferencesAsync(It.IsAny<UserPreferences>()))
            .Returns((UserPreferences prefs) =>
            {
                _preferences = prefs;
                return Task.CompletedTask;
            });
    }

    public void SetPremiumStatus(bool isPremium)
    {
        _preferences.IsPremium = isPremium;
    }
}
