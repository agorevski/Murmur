using FluentAssertions;
using Moq;
using Murmur.App.Services;
using Murmur.App.ViewModels;

namespace Murmur.App.Tests.ViewModels;

public class BaseViewModelTests
{
    private class TestViewModel : BaseViewModel
    {
        public TestViewModel(IAnalyticsService analyticsService) : base(analyticsService)
        {
        }
    }

    [Fact]
    public void BaseViewModel_Constructor_ShouldInitializeAnalyticsService()
    {
        // Arrange
        var mockAnalytics = new Mock<IAnalyticsService>();

        // Act
        var viewModel = new TestViewModel(mockAnalytics.Object);

        // Assert
        viewModel.AnalyticsService.Should().NotBeNull();
        viewModel.AnalyticsService.Should().BeSameAs(mockAnalytics.Object);
    }

    [Fact]
    public void BaseViewModel_Title_DefaultValue_ShouldBeEmpty()
    {
        // Arrange
        var mockAnalytics = new Mock<IAnalyticsService>();

        // Act
        var viewModel = new TestViewModel(mockAnalytics.Object);

        // Assert
        viewModel.Title.Should().BeEmpty();
    }

    [Fact]
    public void BaseViewModel_Title_WhenSet_ShouldUpdateValue()
    {
        // Arrange
        var mockAnalytics = new Mock<IAnalyticsService>();
        var viewModel = new TestViewModel(mockAnalytics.Object);

        // Act
        viewModel.Title = "Test Page";

        // Assert
        viewModel.Title.Should().Be("Test Page");
    }

    [Fact]
    public void BaseViewModel_IsBusy_DefaultValue_ShouldBeFalse()
    {
        // Arrange
        var mockAnalytics = new Mock<IAnalyticsService>();

        // Act
        var viewModel = new TestViewModel(mockAnalytics.Object);

        // Assert
        viewModel.IsBusy.Should().BeFalse();
    }

    [Fact]
    public void BaseViewModel_IsBusy_WhenSet_ShouldUpdateValue()
    {
        // Arrange
        var mockAnalytics = new Mock<IAnalyticsService>();
        var viewModel = new TestViewModel(mockAnalytics.Object);

        // Act
        viewModel.IsBusy = true;

        // Assert
        viewModel.IsBusy.Should().BeTrue();
    }

    [Fact]
    public void BaseViewModel_PropertyChanged_ShouldFireForTitle()
    {
        // Arrange
        var mockAnalytics = new Mock<IAnalyticsService>();
        var viewModel = new TestViewModel(mockAnalytics.Object);
        var propertyChangedFired = false;
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.Title))
                propertyChangedFired = true;
        };

        // Act
        viewModel.Title = "New Title";

        // Assert
        propertyChangedFired.Should().BeTrue();
    }

    [Fact]
    public void BaseViewModel_PropertyChanged_ShouldFireForIsBusy()
    {
        // Arrange
        var mockAnalytics = new Mock<IAnalyticsService>();
        var viewModel = new TestViewModel(mockAnalytics.Object);
        var propertyChangedFired = false;
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.IsBusy))
                propertyChangedFired = true;
        };

        // Act
        viewModel.IsBusy = true;

        // Assert
        propertyChangedFired.Should().BeTrue();
    }
}
