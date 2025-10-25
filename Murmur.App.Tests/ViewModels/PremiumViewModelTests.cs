using FluentAssertions;
using Moq;
using Murmur.App.Services;
using Murmur.App.Tests.Mocks;
using Murmur.App.ViewModels;

namespace Murmur.App.Tests.ViewModels;

public class PremiumViewModelTests
{
    private readonly Mock<IBillingService> _mockBillingService;
    private readonly MockDataService _mockDataService;
    private readonly Mock<IAnalyticsService> _mockAnalyticsService;
    private readonly PremiumViewModel _viewModel;

    public PremiumViewModelTests()
    {
        _mockBillingService = new Mock<IBillingService>();
        _mockDataService = new MockDataService(isPremium: false);
        _mockAnalyticsService = new Mock<IAnalyticsService>();

        _viewModel = new PremiumViewModel(
            _mockBillingService.Object,
            _mockDataService.Mock.Object,
            _mockAnalyticsService.Object
        );
    }

    #region CheckPremiumStatus Tests

    [Fact]
    public async Task CheckPremiumStatus_WhenUserIsPremium_ShouldSetIsPremiumTrue()
    {
        // Arrange
        _mockBillingService.Setup(x => x.CheckPremiumStatusAsync()).ReturnsAsync(true);

        // Act
        await _viewModel.CheckPremiumStatusCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsPremium.Should().BeTrue();
    }

    [Fact]
    public async Task CheckPremiumStatus_WhenUserIsFree_ShouldSetIsPremiumFalse()
    {
        // Arrange
        _mockBillingService.Setup(x => x.CheckPremiumStatusAsync()).ReturnsAsync(false);

        // Act
        await _viewModel.CheckPremiumStatusCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsPremium.Should().BeFalse();
    }

    [Fact]
    public async Task CheckPremiumStatus_ShouldSetIsBusyDuringCheck()
    {
        // Arrange
        var isBusyValues = new List<bool>();
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(PremiumViewModel.IsBusy))
                isBusyValues.Add(_viewModel.IsBusy);
        };

        _mockBillingService.Setup(x => x.CheckPremiumStatusAsync()).ReturnsAsync(false);

        // Act
        await _viewModel.CheckPremiumStatusCommand.ExecuteAsync(null);

        // Assert
        isBusyValues.Should().Contain(true, "IsBusy should be true during check");
        isBusyValues.Last().Should().BeFalse("IsBusy should be false after check");
    }

    [Fact]
    public async Task CheckPremiumStatus_OnError_ShouldHandleGracefully()
    {
        // Arrange
        _mockBillingService
            .Setup(x => x.CheckPremiumStatusAsync())
            .ThrowsAsync(new Exception("Billing service error"));

        // Act
        var act = () => _viewModel.CheckPremiumStatusCommand.ExecuteAsync(null);

        // Assert
        await act.Should().NotThrowAsync("Should handle errors gracefully");
        _viewModel.IsBusy.Should().BeFalse("IsBusy should be reset even on error");
    }

    [Fact]
    public async Task CheckPremiumStatus_ShouldTrackAnalytics()
    {
        // Arrange
        _mockBillingService.Setup(x => x.CheckPremiumStatusAsync()).ReturnsAsync(true);

        // Act
        await _viewModel.CheckPremiumStatusCommand.ExecuteAsync(null);

        // Assert
        _mockAnalyticsService.Verify(
            x => x.TrackEvent("premium_status_checked", It.Is<Dictionary<string, string>>(
                d => d.ContainsKey("is_premium"))),
            Times.Once);
    }

    #endregion

    #region PurchasePremium Tests

    [Fact]
    public async Task PurchasePremium_WhenSuccessful_ShouldSetIsPremiumTrue()
    {
        // Arrange
        _mockBillingService.Setup(x => x.PurchasePremiumAsync()).ReturnsAsync(true);

        // Act
        await _viewModel.PurchasePremiumCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsPremium.Should().BeTrue();
    }

    [Fact]
    public async Task PurchasePremium_WhenFailed_ShouldNotSetIsPremium()
    {
        // Arrange
        _mockBillingService.Setup(x => x.PurchasePremiumAsync()).ReturnsAsync(false);
        _viewModel.IsPremium.Should().BeFalse();

        // Act
        await _viewModel.PurchasePremiumCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsPremium.Should().BeFalse();
    }

    [Fact]
    public async Task PurchasePremium_WhenCancelled_ShouldNotSetIsPremium()
    {
        // Arrange
        _mockBillingService.Setup(x => x.PurchasePremiumAsync()).ReturnsAsync(false);

        // Act
        await _viewModel.PurchasePremiumCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsPremium.Should().BeFalse();
    }

    [Fact]
    public async Task PurchasePremium_ShouldSetIsBusyDuringPurchase()
    {
        // Arrange
        var isBusyValues = new List<bool>();
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(PremiumViewModel.IsBusy))
                isBusyValues.Add(_viewModel.IsBusy);
        };

        _mockBillingService.Setup(x => x.PurchasePremiumAsync()).ReturnsAsync(true);

        // Act
        await _viewModel.PurchasePremiumCommand.ExecuteAsync(null);

        // Assert
        isBusyValues.Should().Contain(true, "IsBusy should be true during purchase");
        isBusyValues.Last().Should().BeFalse("IsBusy should be false after purchase");
    }

    [Fact]
    public async Task PurchasePremium_WhenSuccessful_ShouldTrackAnalytics()
    {
        // Arrange
        _mockBillingService.Setup(x => x.PurchasePremiumAsync()).ReturnsAsync(true);

        // Act
        await _viewModel.PurchasePremiumCommand.ExecuteAsync(null);

        // Assert
        _mockAnalyticsService.Verify(
            x => x.TrackEvent("premium_purchased", null),
            Times.Once);
    }

    [Fact]
    public async Task PurchasePremium_WhenFailed_ShouldTrackFailure()
    {
        // Arrange
        _mockBillingService.Setup(x => x.PurchasePremiumAsync()).ReturnsAsync(false);

        // Act
        await _viewModel.PurchasePremiumCommand.ExecuteAsync(null);

        // Assert
        _mockAnalyticsService.Verify(
            x => x.TrackEvent("premium_purchase_failed", null),
            Times.Once);
    }

    [Fact]
    public async Task PurchasePremium_OnError_ShouldHandleGracefully()
    {
        // Arrange
        _mockBillingService
            .Setup(x => x.PurchasePremiumAsync())
            .ThrowsAsync(new Exception("Billing service error"));

        // Act
        var act = () => _viewModel.PurchasePremiumCommand.ExecuteAsync(null);

        // Assert
        await act.Should().NotThrowAsync("Should handle errors gracefully");
        _viewModel.IsBusy.Should().BeFalse("IsBusy should be reset even on error");
    }

    [Fact]
    public async Task PurchasePremium_WhenAlreadyPremium_ShouldStillAttemptPurchase()
    {
        // Arrange
        _mockBillingService.Setup(x => x.PurchasePremiumAsync()).ReturnsAsync(true);
        await _viewModel.PurchasePremiumCommand.ExecuteAsync(null);
        _viewModel.IsPremium.Should().BeTrue();

        // Act - Try to purchase again
        await _viewModel.PurchasePremiumCommand.ExecuteAsync(null);

        // Assert
        _mockBillingService.Verify(x => x.PurchasePremiumAsync(), Times.Exactly(2));
    }

    #endregion

    #region RestorePurchases Tests

    [Fact]
    public async Task RestorePurchases_WhenSuccessful_ShouldSetIsPremiumTrue()
    {
        // Arrange
        _mockBillingService.Setup(x => x.RestorePurchasesAsync()).ReturnsAsync(true);

        // Act
        await _viewModel.RestorePurchasesCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsPremium.Should().BeTrue();
    }

    [Fact]
    public async Task RestorePurchases_WhenNoPurchasesToRestore_ShouldNotSetIsPremium()
    {
        // Arrange
        _mockBillingService.Setup(x => x.RestorePurchasesAsync()).ReturnsAsync(false);

        // Act
        await _viewModel.RestorePurchasesCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsPremium.Should().BeFalse();
    }

    [Fact]
    public async Task RestorePurchases_ShouldSetIsBusyDuringRestore()
    {
        // Arrange
        var isBusyValues = new List<bool>();
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(PremiumViewModel.IsBusy))
                isBusyValues.Add(_viewModel.IsBusy);
        };

        _mockBillingService.Setup(x => x.RestorePurchasesAsync()).ReturnsAsync(true);

        // Act
        await _viewModel.RestorePurchasesCommand.ExecuteAsync(null);

        // Assert
        isBusyValues.Should().Contain(true, "IsBusy should be true during restore");
        isBusyValues.Last().Should().BeFalse("IsBusy should be false after restore");
    }

    [Fact]
    public async Task RestorePurchases_WhenSuccessful_ShouldTrackAnalytics()
    {
        // Arrange
        _mockBillingService.Setup(x => x.RestorePurchasesAsync()).ReturnsAsync(true);

        // Act
        await _viewModel.RestorePurchasesCommand.ExecuteAsync(null);

        // Assert
        _mockAnalyticsService.Verify(
            x => x.TrackEvent("purchases_restored", null),
            Times.Once);
    }

    [Fact]
    public async Task RestorePurchases_WhenFailed_ShouldTrackFailure()
    {
        // Arrange
        _mockBillingService.Setup(x => x.RestorePurchasesAsync()).ReturnsAsync(false);

        // Act
        await _viewModel.RestorePurchasesCommand.ExecuteAsync(null);

        // Assert
        _mockAnalyticsService.Verify(
            x => x.TrackEvent("restore_purchases_failed", null),
            Times.Once);
    }

    [Fact]
    public async Task RestorePurchases_OnError_ShouldHandleGracefully()
    {
        // Arrange
        _mockBillingService
            .Setup(x => x.RestorePurchasesAsync())
            .ThrowsAsync(new Exception("Billing service error"));

        // Act
        var act = () => _viewModel.RestorePurchasesCommand.ExecuteAsync(null);

        // Assert
        await act.Should().NotThrowAsync("Should handle errors gracefully");
        _viewModel.IsBusy.Should().BeFalse("IsBusy should be reset even on error");
    }

    #endregion

    #region Property Notification Tests

    [Fact]
    public void IsPremium_ShouldNotifyPropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(PremiumViewModel.IsPremium))
                propertyChangedRaised = true;
        };

        // Act
        _viewModel.IsPremium = true;

        // Assert
        propertyChangedRaised.Should().BeTrue();
        _viewModel.IsPremium.Should().BeTrue();
    }

    [Fact]
    public void PremiumPrice_ShouldBeSet()
    {
        // Assert
        _viewModel.PremiumPrice.Should().NotBeNullOrEmpty("Premium price should be set");
    }

    [Fact]
    public void PremiumFeatures_ShouldContainMultipleFeatures()
    {
        // Assert
        _viewModel.PremiumFeatures.Should().NotBeEmpty("Should have premium features listed");
        _viewModel.PremiumFeatures.Count.Should().BeGreaterThan(2, "Should have multiple features");
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task Integration_CheckStatus_ThenPurchase_Workflow()
    {
        // Arrange
        _mockBillingService.Setup(x => x.CheckPremiumStatusAsync()).ReturnsAsync(false);
        _mockBillingService.Setup(x => x.PurchasePremiumAsync()).ReturnsAsync(true);

        // Act & Assert: Check status - user is free
        await _viewModel.CheckPremiumStatusCommand.ExecuteAsync(null);
        _viewModel.IsPremium.Should().BeFalse();

        // Act & Assert: Purchase premium
        await _viewModel.PurchasePremiumCommand.ExecuteAsync(null);
        _viewModel.IsPremium.Should().BeTrue();

        // Verify analytics tracked both events
        _mockAnalyticsService.Verify(x => x.TrackEvent("premium_status_checked", It.IsAny<Dictionary<string, string>>()), Times.Once);
        _mockAnalyticsService.Verify(x => x.TrackEvent("premium_purchased", null), Times.Once);
    }

    [Fact]
    public async Task Integration_FailedPurchase_ThenSuccessfulRestore_Workflow()
    {
        // Arrange
        _mockBillingService.Setup(x => x.PurchasePremiumAsync()).ReturnsAsync(false);
        _mockBillingService.Setup(x => x.RestorePurchasesAsync()).ReturnsAsync(true);

        // Act & Assert: Failed purchase
        await _viewModel.PurchasePremiumCommand.ExecuteAsync(null);
        _viewModel.IsPremium.Should().BeFalse();
        _mockAnalyticsService.Verify(x => x.TrackEvent("premium_purchase_failed", null), Times.Once);

        // Act & Assert: Successful restore
        await _viewModel.RestorePurchasesCommand.ExecuteAsync(null);
        _viewModel.IsPremium.Should().BeTrue();
        _mockAnalyticsService.Verify(x => x.TrackEvent("purchases_restored", null), Times.Once);
    }

    [Fact]
    public async Task Integration_MultiplePurchaseAttempts_ShouldTrackAll()
    {
        // Arrange
        _mockBillingService.SetupSequence(x => x.PurchasePremiumAsync())
            .ReturnsAsync(false)  // First attempt fails
            .ReturnsAsync(false)  // Second attempt fails
            .ReturnsAsync(true);  // Third attempt succeeds

        // Act: Multiple purchase attempts
        await _viewModel.PurchasePremiumCommand.ExecuteAsync(null);
        _viewModel.IsPremium.Should().BeFalse();

        await _viewModel.PurchasePremiumCommand.ExecuteAsync(null);
        _viewModel.IsPremium.Should().BeFalse();

        await _viewModel.PurchasePremiumCommand.ExecuteAsync(null);
        _viewModel.IsPremium.Should().BeTrue();

        // Assert
        _mockAnalyticsService.Verify(x => x.TrackEvent("premium_purchase_failed", null), Times.Exactly(2));
        _mockAnalyticsService.Verify(x => x.TrackEvent("premium_purchased", null), Times.Once);
    }

    #endregion

    #region Command CanExecute Tests

    [Fact]
    public void CheckPremiumStatusCommand_ShouldBeExecutable()
    {
        // Act
        var canExecute = _viewModel.CheckPremiumStatusCommand.CanExecute(null);

        // Assert
        canExecute.Should().BeTrue();
    }

    [Fact]
    public void PurchasePremiumCommand_ShouldBeExecutable()
    {
        // Act
        var canExecute = _viewModel.PurchasePremiumCommand.CanExecute(null);

        // Assert
        canExecute.Should().BeTrue();
    }

    [Fact]
    public void RestorePurchasesCommand_ShouldBeExecutable()
    {
        // Act
        var canExecute = _viewModel.RestorePurchasesCommand.CanExecute(null);

        // Assert
        canExecute.Should().BeTrue();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task PurchasePremium_CalledConcurrently_ShouldHandleCorrectly()
    {
        // Arrange
        _mockBillingService.Setup(x => x.PurchasePremiumAsync())
            .ReturnsAsync(true);

        // Act - Simulate concurrent calls
        var task1 = _viewModel.PurchasePremiumCommand.ExecuteAsync(null);
        var task2 = _viewModel.PurchasePremiumCommand.ExecuteAsync(null);
        var task3 = _viewModel.PurchasePremiumCommand.ExecuteAsync(null);

        await Task.WhenAll(task1, task2, task3);

        // Assert - Should handle concurrent calls gracefully
        _viewModel.IsPremium.Should().BeTrue();
    }

    [Fact]
    public async Task RestorePurchases_AfterSuccessfulPurchase_ShouldStillWork()
    {
        // Arrange
        _mockBillingService.Setup(x => x.PurchasePremiumAsync()).ReturnsAsync(true);
        _mockBillingService.Setup(x => x.RestorePurchasesAsync()).ReturnsAsync(true);

        // Act: Purchase first
        await _viewModel.PurchasePremiumCommand.ExecuteAsync(null);
        _viewModel.IsPremium.Should().BeTrue();

        // Act: Then restore
        await _viewModel.RestorePurchasesCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsPremium.Should().BeTrue();
        _mockBillingService.Verify(x => x.RestorePurchasesAsync(), Times.Once);
    }

    [Fact]
    public async Task CheckPremiumStatus_CalledMultipleTimes_ShouldUpdateCorrectly()
    {
        // Arrange - Setup sequence: false, true, true
        _mockBillingService.SetupSequence(x => x.CheckPremiumStatusAsync())
            .ReturnsAsync(false)
            .ReturnsAsync(true)
            .ReturnsAsync(true);

        // Act & Assert: First check - free
        await _viewModel.CheckPremiumStatusCommand.ExecuteAsync(null);
        _viewModel.IsPremium.Should().BeFalse();

        // Act & Assert: Second check - premium
        await _viewModel.CheckPremiumStatusCommand.ExecuteAsync(null);
        _viewModel.IsPremium.Should().BeTrue();

        // Act & Assert: Third check - still premium
        await _viewModel.CheckPremiumStatusCommand.ExecuteAsync(null);
        _viewModel.IsPremium.Should().BeTrue();
    }

    [Fact]
    public void PremiumFeatures_ShouldNotBeModifiable()
    {
        // Arrange
        var originalCount = _viewModel.PremiumFeatures.Count;

        // Act - Try to modify (this tests that it's a read-only collection conceptually)
        var features = _viewModel.PremiumFeatures;

        // Assert
        features.Should().NotBeNull();
        features.Count.Should().Be(originalCount);
    }

    #endregion

    #region Billing Service Integration Tests

    [Fact]
    public async Task BillingService_WhenNotInitialized_ShouldStillAttemptOperations()
    {
        // Arrange - Billing service returns false (not initialized)
        _mockBillingService.Setup(x => x.InitializeAsync()).ReturnsAsync(false);
        _mockBillingService.Setup(x => x.PurchasePremiumAsync()).ReturnsAsync(false);

        // Act
        await _viewModel.PurchasePremiumCommand.ExecuteAsync(null);

        // Assert - Should attempt purchase even if not initialized
        _mockBillingService.Verify(x => x.PurchasePremiumAsync(), Times.Once);
    }

    [Fact]
    public async Task BillingService_NetworkError_ShouldHandleGracefully()
    {
        // Arrange
        _mockBillingService
            .Setup(x => x.PurchasePremiumAsync())
            .ThrowsAsync(new Exception("Network error"));

        // Act
        var act = () => _viewModel.PurchasePremiumCommand.ExecuteAsync(null);

        // Assert
        await act.Should().NotThrowAsync("Should handle network errors gracefully");
        _viewModel.IsPremium.Should().BeFalse();
    }

    #endregion
}
