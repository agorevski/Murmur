using FluentAssertions;
using Moq;
using Murmur.App.Models;
using Murmur.App.Services;
using Murmur.App.Tests.Mocks;
using Murmur.App.ViewModels;

namespace Murmur.App.Tests.ViewModels;

public class SettingsViewModelTests
{
    private readonly MockDataService _mockDataService;
    private readonly Mock<IAnalyticsService> _mockAnalyticsService;
    private readonly SettingsViewModel _viewModel;

    public SettingsViewModelTests()
    {
        _mockDataService = new MockDataService(isPremium: false);
        _mockAnalyticsService = new Mock<IAnalyticsService>();

        _viewModel = new SettingsViewModel(
            _mockDataService.Mock.Object,
            _mockAnalyticsService.Object
        );
    }

    #region LoadPreferences Tests

    [Fact]
    public async Task LoadPreferences_ShouldLoadUserPreferencesFromDataService()
    {
        // Arrange
        var preferences = new UserPreferences
        {
            DarkMode = true,
            NotificationsEnabled = false,
            AnalyticsEnabled = true
        };
        _mockDataService.SetUserPreferences(preferences);

        // Act
        await _viewModel.LoadPreferencesCommand.ExecuteAsync(null);

        // Assert
        _viewModel.DarkMode.Should().BeTrue();
        _viewModel.NotificationsEnabled.Should().BeFalse();
        _viewModel.AnalyticsEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task LoadPreferences_ShouldSetIsBusyDuringLoading()
    {
        // Arrange
        var isBusyValues = new List<bool>();
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SettingsViewModel.IsBusy))
                isBusyValues.Add(_viewModel.IsBusy);
        };

        // Act
        await _viewModel.LoadPreferencesCommand.ExecuteAsync(null);

        // Assert
        isBusyValues.Should().Contain(true, "IsBusy should be true during loading");
        isBusyValues.Last().Should().BeFalse("IsBusy should be false after loading");
    }

    [Fact]
    public async Task LoadPreferences_WithDefaultValues_ShouldLoadCorrectly()
    {
        // Arrange - DataService returns default preferences
        var defaultPreferences = new UserPreferences();
        _mockDataService.SetUserPreferences(defaultPreferences);

        // Act
        await _viewModel.LoadPreferencesCommand.ExecuteAsync(null);

        // Assert
        _viewModel.DarkMode.Should().BeFalse("Default dark mode should be false");
        _viewModel.NotificationsEnabled.Should().BeTrue("Default notifications should be true");
        _viewModel.AnalyticsEnabled.Should().BeTrue("Default analytics should be true");
    }

    [Fact]
    public async Task LoadPreferences_OnError_ShouldHandleGracefully()
    {
        // Arrange
        _mockDataService.Mock
            .Setup(x => x.GetUserPreferencesAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var act = () => _viewModel.LoadPreferencesCommand.ExecuteAsync(null);

        // Assert
        await act.Should().NotThrowAsync("Should handle errors gracefully");
        _viewModel.IsBusy.Should().BeFalse("IsBusy should be reset even on error");
    }

    #endregion

    #region SavePreferences Tests

    [Fact]
    public async Task SavePreferences_ShouldSaveAllPreferencesToDataService()
    {
        // Arrange
        await _viewModel.LoadPreferencesCommand.ExecuteAsync(null);
        _viewModel.DarkMode = true;
        _viewModel.NotificationsEnabled = false;
        _viewModel.AnalyticsEnabled = false;

        // Act
        await _viewModel.SavePreferencesCommand.ExecuteAsync(null);

        // Assert
        _mockDataService.Mock.Verify(
            x => x.SaveUserPreferencesAsync(It.Is<UserPreferences>(p =>
                p.DarkMode == true &&
                p.NotificationsEnabled == false &&
                p.AnalyticsEnabled == false)),
            Times.Once);
    }

    [Fact]
    public async Task SavePreferences_ShouldTrackAnalytics()
    {
        // Arrange
        await _viewModel.LoadPreferencesCommand.ExecuteAsync(null);

        // Act
        await _viewModel.SavePreferencesCommand.ExecuteAsync(null);

        // Assert
        _mockAnalyticsService.Verify(
            x => x.TrackEvent("settings_saved", null),
            Times.Once);
    }

    [Fact]
    public async Task SavePreferences_OnError_ShouldHandleGracefully()
    {
        // Arrange
        _mockDataService.Mock
            .Setup(x => x.SaveUserPreferencesAsync(It.IsAny<UserPreferences>()))
            .ThrowsAsync(new Exception("Database error"));

        await _viewModel.LoadPreferencesCommand.ExecuteAsync(null);

        // Act
        var act = () => _viewModel.SavePreferencesCommand.ExecuteAsync(null);

        // Assert
        await act.Should().NotThrowAsync("Should handle errors gracefully");
    }

    #endregion

    #region ResetToDefaults Tests

    [Fact]
    public async Task ResetToDefaults_ShouldResetAllPreferencesToDefaults()
    {
        // Arrange
        await _viewModel.LoadPreferencesCommand.ExecuteAsync(null);
        _viewModel.DarkMode = true;
        _viewModel.NotificationsEnabled = false;
        _viewModel.AnalyticsEnabled = false;

        // Act
        await _viewModel.ResetToDefaultsCommand.ExecuteAsync(null);

        // Assert
        _viewModel.DarkMode.Should().BeFalse("Should reset to default");
        _viewModel.NotificationsEnabled.Should().BeTrue("Should reset to default");
        _viewModel.AnalyticsEnabled.Should().BeTrue("Should reset to default");
    }

    [Fact]
    public async Task ResetToDefaults_ShouldSaveResettedPreferences()
    {
        // Arrange
        await _viewModel.LoadPreferencesCommand.ExecuteAsync(null);
        _viewModel.DarkMode = true;
        _viewModel.NotificationsEnabled = false;

        // Act
        await _viewModel.ResetToDefaultsCommand.ExecuteAsync(null);

        // Assert
        _mockDataService.Mock.Verify(
            x => x.SaveUserPreferencesAsync(It.Is<UserPreferences>(p =>
                p.DarkMode == false &&
                p.NotificationsEnabled == true &&
                p.AnalyticsEnabled == true)),
            Times.Once);
    }

    [Fact]
    public async Task ResetToDefaults_ShouldTrackAnalytics()
    {
        // Arrange
        await _viewModel.LoadPreferencesCommand.ExecuteAsync(null);

        // Act
        await _viewModel.ResetToDefaultsCommand.ExecuteAsync(null);

        // Assert
        _mockAnalyticsService.Verify(
            x => x.TrackEvent("settings_reset_to_defaults", null),
            Times.Once);
    }

    #endregion

    #region OpenPrivacyPolicy Tests

    [Fact]
    public async Task OpenPrivacyPolicy_ShouldTrackAnalytics()
    {
        // Act
        await _viewModel.OpenPrivacyPolicyCommand.ExecuteAsync(null);

        // Assert
        _mockAnalyticsService.Verify(
            x => x.TrackEvent("privacy_policy_opened", null),
            Times.Once);
    }

    [Fact]
    public async Task OpenPrivacyPolicy_ShouldNotThrow()
    {
        // Act
        var act = () => _viewModel.OpenPrivacyPolicyCommand.ExecuteAsync(null);

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region OpenTermsOfService Tests

    [Fact]
    public async Task OpenTermsOfService_ShouldTrackAnalytics()
    {
        // Act
        await _viewModel.OpenTermsOfServiceCommand.ExecuteAsync(null);

        // Assert
        _mockAnalyticsService.Verify(
            x => x.TrackEvent("terms_of_service_opened", null),
            Times.Once);
    }

    [Fact]
    public async Task OpenTermsOfService_ShouldNotThrow()
    {
        // Act
        var act = () => _viewModel.OpenTermsOfServiceCommand.ExecuteAsync(null);

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region Property Notification Tests

    [Fact]
    public void DarkMode_ShouldNotifyPropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SettingsViewModel.DarkMode))
                propertyChangedRaised = true;
        };

        // Act
        _viewModel.DarkMode = true;

        // Assert
        propertyChangedRaised.Should().BeTrue();
        _viewModel.DarkMode.Should().BeTrue();
    }

    [Fact]
    public void NotificationsEnabled_ShouldNotifyPropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SettingsViewModel.NotificationsEnabled))
                propertyChangedRaised = true;
        };

        // Act
        _viewModel.NotificationsEnabled = false;

        // Assert
        propertyChangedRaised.Should().BeTrue();
        _viewModel.NotificationsEnabled.Should().BeFalse();
    }

    [Fact]
    public void AnalyticsEnabled_ShouldNotifyPropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SettingsViewModel.AnalyticsEnabled))
                propertyChangedRaised = true;
        };

        // Act
        _viewModel.AnalyticsEnabled = false;

        // Assert
        propertyChangedRaised.Should().BeTrue();
        _viewModel.AnalyticsEnabled.Should().BeFalse();
    }

    [Fact]
    public void AppVersion_ShouldBeSet()
    {
        // Assert
        _viewModel.AppVersion.Should().NotBeNullOrEmpty("App version should be set");
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task Integration_LoadPreferences_ModifySettings_SavePreferences_Workflow()
    {
        // Act & Assert: Load preferences
        await _viewModel.LoadPreferencesCommand.ExecuteAsync(null);
        _viewModel.DarkMode.Should().BeFalse();
        _viewModel.NotificationsEnabled.Should().BeTrue();

        // Act & Assert: Modify settings
        _viewModel.DarkMode = true;
        _viewModel.NotificationsEnabled = false;
        _viewModel.AnalyticsEnabled = false;

        // Act & Assert: Save preferences
        await _viewModel.SavePreferencesCommand.ExecuteAsync(null);
        _mockDataService.Mock.Verify(
            x => x.SaveUserPreferencesAsync(It.Is<UserPreferences>(p =>
                p.DarkMode == true &&
                p.NotificationsEnabled == false &&
                p.AnalyticsEnabled == false)),
            Times.Once);
    }

    [Fact]
    public async Task Integration_ModifySettings_ResetToDefaults_ShouldRevertChanges()
    {
        // Arrange: Load and modify settings
        await _viewModel.LoadPreferencesCommand.ExecuteAsync(null);
        _viewModel.DarkMode = true;
        _viewModel.NotificationsEnabled = false;
        _viewModel.AnalyticsEnabled = false;

        // Act: Reset to defaults
        await _viewModel.ResetToDefaultsCommand.ExecuteAsync(null);

        // Assert: All settings should be back to defaults
        _viewModel.DarkMode.Should().BeFalse();
        _viewModel.NotificationsEnabled.Should().BeTrue();
        _viewModel.AnalyticsEnabled.Should().BeTrue();
        
        // Verify saved to database
        _mockDataService.Mock.Verify(
            x => x.SaveUserPreferencesAsync(It.Is<UserPreferences>(p =>
                p.DarkMode == false &&
                p.NotificationsEnabled == true &&
                p.AnalyticsEnabled == true)),
            Times.Once);
    }

    [Fact]
    public async Task Integration_ToggleMultipleSettings_ShouldTrackChanges()
    {
        // Arrange
        await _viewModel.LoadPreferencesCommand.ExecuteAsync(null);

        // Act: Toggle multiple settings
        _viewModel.DarkMode = true;
        _viewModel.DarkMode.Should().BeTrue();

        _viewModel.NotificationsEnabled = false;
        _viewModel.NotificationsEnabled.Should().BeFalse();

        _viewModel.AnalyticsEnabled = false;
        _viewModel.AnalyticsEnabled.Should().BeFalse();

        // Save
        await _viewModel.SavePreferencesCommand.ExecuteAsync(null);

        // Assert
        _mockDataService.Mock.Verify(
            x => x.SaveUserPreferencesAsync(It.IsAny<UserPreferences>()),
            Times.Once);
    }

    #endregion

    #region Command CanExecute Tests

    [Fact]
    public void LoadPreferencesCommand_ShouldBeExecutable()
    {
        // Act
        var canExecute = _viewModel.LoadPreferencesCommand.CanExecute(null);

        // Assert
        canExecute.Should().BeTrue();
    }

    [Fact]
    public void SavePreferencesCommand_ShouldBeExecutable()
    {
        // Act
        var canExecute = _viewModel.SavePreferencesCommand.CanExecute(null);

        // Assert
        canExecute.Should().BeTrue();
    }

    [Fact]
    public void ResetToDefaultsCommand_ShouldBeExecutable()
    {
        // Act
        var canExecute = _viewModel.ResetToDefaultsCommand.CanExecute(null);

        // Assert
        canExecute.Should().BeTrue();
    }

    [Fact]
    public void OpenPrivacyPolicyCommand_ShouldBeExecutable()
    {
        // Act
        var canExecute = _viewModel.OpenPrivacyPolicyCommand.CanExecute(null);

        // Assert
        canExecute.Should().BeTrue();
    }

    [Fact]
    public void OpenTermsOfServiceCommand_ShouldBeExecutable()
    {
        // Act
        var canExecute = _viewModel.OpenTermsOfServiceCommand.CanExecute(null);

        // Assert
        canExecute.Should().BeTrue();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task SavePreferences_WithoutLoadingFirst_ShouldHandleGracefully()
    {
        // Arrange - Don't load preferences first
        _viewModel.DarkMode = true;

        // Act
        var act = () => _viewModel.SavePreferencesCommand.ExecuteAsync(null);

        // Assert
        await act.Should().NotThrowAsync("Should handle saving without loading first");
    }

    [Fact]
    public async Task ResetToDefaults_WithoutLoadingFirst_ShouldHandleGracefully()
    {
        // Act
        var act = () => _viewModel.ResetToDefaultsCommand.ExecuteAsync(null);

        // Assert
        await act.Should().NotThrowAsync("Should handle reset without loading first");
    }

    [Fact]
    public async Task LoadPreferences_CalledMultipleTimes_ShouldHandleCorrectly()
    {
        // Act
        await _viewModel.LoadPreferencesCommand.ExecuteAsync(null);
        await _viewModel.LoadPreferencesCommand.ExecuteAsync(null);
        await _viewModel.LoadPreferencesCommand.ExecuteAsync(null);

        // Assert
        _mockDataService.Mock.Verify(
            x => x.GetUserPreferencesAsync(),
            Times.Exactly(3));
    }

    #endregion
}
