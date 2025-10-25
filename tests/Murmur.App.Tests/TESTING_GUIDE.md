## Murmur App - Comprehensive Testing Guide

## Overview

This testing guide provides complete information for running, understanding, and extending the Murmur app test suite.

## Test Suite Status

### ✅ Completed Tests (289 tests)

1. **ViewModels** (189 tests)
   - BaseViewModelTests.cs - 7 tests
   - HomeViewModelTests.cs - 38 tests  
   - FavoritesViewModelTests.cs - 35 tests
   - MixerViewModelTests.cs - 40 tests
   - SettingsViewModelTests.cs - 32 tests
   - PremiumViewModelTests.cs - 37 tests

2. **Converters** (54 tests)
   - ValueConvertersTests.cs - 54 tests (all 7 converters)

3. **Models** (46 tests)
   - SoundTests.cs - 5 tests
   - MixTests.cs - 10 tests
   - PlayingSoundTests.cs - 7 tests
   - UserPreferencesTests.cs - 24 tests

### ⏳ Remaining Tests (Services & UI)

The following test files still need to be created for complete coverage:

#### Service Tests (Estimated: ~120 tests)
- AudioServiceTests.cs
- DataServiceTests.cs
- SoundLibraryServiceTests.cs
- BillingServiceTests.cs
- AdServiceTests.cs
- AnalyticsServiceTests.cs

#### UI/UX Tests with Appium (Estimated: ~80 tests)
- AppiumTestBase.cs (Helper infrastructure)
- HomePageUITests.cs
- MixerPageUITests.cs
- FavoritesPageUITests.cs
- SettingsPageUITests.cs
- PremiumPageUITests.cs
- NavigationUITests.cs

**Current Progress: 289/~490 tests (59%)**

## Running Tests

### Prerequisites

- .NET 9.0 SDK
- Visual Studio Code or Visual Studio 2022
- Android SDK (for UI tests)
- Appium Server (for UI tests)

### Run All Tests

```bash
# From solution root
dotnet test

# From test project directory
cd Murmur.App.Tests
dotnet test
```

### Run Specific Test Class

```bash
dotnet test --filter "FullyQualifiedName~HomeViewModelTests"
dotnet test --filter "FullyQualifiedName~ValueConvertersTests"
dotnet test --filter "FullyQualifiedName~ModelTests"
```

### Run Tests by Category

```bash
# Run all ViewModel tests
dotnet test --filter "FullyQualifiedName~ViewModels"

# Run all Converter tests
dotnet test --filter "FullyQualifiedName~Converters"

# Run all Model tests
dotnet test --filter "FullyQualifiedName~Models"
```

### Run with Detailed Output

```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run with Code Coverage

```bash
# Generate coverage report
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./coverage/

# View coverage in Visual Studio Code
# Install "Coverage Gutters" extension and open coverage.opencover.xml
```

## Test Structure

### ViewModel Tests

All ViewModel tests follow this pattern:

```csharp
public class ExampleViewModelTests
{
    private readonly MockService _mockService;
    private readonly ExampleViewModel _viewModel;

    public ExampleViewModelTests()
    {
        _mockService = new MockService();
        _viewModel = new ExampleViewModel(_mockService.Mock.Object);
    }

    [Fact]
    public async Task Method_Condition_ExpectedBehavior()
    {
        // Arrange
        // Setup test data and mocks

        // Act
        // Execute the method being tested

        // Assert
        // Verify the expected behavior
    }
}
```

### Test Categories

1. **Functional Tests**: Core business logic
2. **Integration Tests**: Multi-component workflows
3. **Error Handling Tests**: Exception and edge cases
4. **UX/Behavior Tests**: User interaction scenarios
5. **Property Notification Tests**: INotifyPropertyChanged
6. **Analytics Tests**: Event tracking verification
7. **Command CanExecute Tests**: Command state validation

## Creating Service Tests

### Example Pattern for Service Tests

```csharp
using FluentAssertions;
using Moq;
using Murmur.App.Services;

namespace Murmur.App.Tests.Services;

public class AudioServiceTests
{
    [Fact]
    public async Task PlaySound_WithValidSound_ShouldReturnTrue()
    {
        // Arrange
        var audioService = new AudioService();
        var sound = new Sound { Id = 1, Name = "Rain", FileName = "rain.mp3" };

        // Act
        var result = await audioService.PlaySoundAsync(sound);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task PlaySound_WithNullSound_ShouldThrowException()
    {
        // Arrange
        var audioService = new AudioService();

        // Act
        var act = () => audioService.PlaySoundAsync(null);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
```

### Key Areas to Test in Services

1. **Initialization**: Service startup and configuration
2. **Success Paths**: Normal operation flows
3. **Error Paths**: Exception handling
4. **Edge Cases**: Boundary conditions, null values
5. **State Management**: Internal state changes
6. **Dependencies**: Interaction with other services
7. **Platform Integration**: Native API calls

## Creating UI Tests with Appium

### Setup Appium Test Base

```csharp
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;

namespace Murmur.App.Tests.UI;

public abstract class AppiumTestBase : IDisposable
{
    protected AndroidDriver<AndroidElement> Driver { get; private set; }

    protected void InitializeDriver()
    {
        var appiumOptions = new AppiumOptions();
        appiumOptions.AddAdditionalCapability("platformName", "Android");
        appiumOptions.AddAdditionalCapability("deviceName", "Android Emulator");
        appiumOptions.AddAdditionalCapability("app", "/path/to/Murmur.App.apk");
        appiumOptions.AddAdditionalCapability("automationName", "UiAutomator2");

        Driver = new AndroidDriver<AndroidElement>(
            new Uri("http://localhost:4723/wd/hub"),
            appiumOptions);
    }

    public void Dispose()
    {
        Driver?.Quit();
    }
}
```

### Example UI Test

```csharp
[Trait("Category", "UI")]
public class HomePageUITests : AppiumTestBase
{
    [Fact]
    public void TapRainSound_ShouldStartPlaying()
    {
        // Arrange
        InitializeDriver();
        var rainButton = Driver.FindElementByAccessibilityId("RainSoundButton");

        // Act
        rainButton.Click();
        Thread.Sleep(1000); // Wait for animation

        // Assert
        var stopAllButton = Driver.FindElementByAccessibilityId("StopAllButton");
        stopAllButton.Enabled.Should().BeTrue();
    }
}
```

### Running UI Tests

1. Start Android Emulator
2. Start Appium Server:
   ```bash
   appium
   ```
3. Run UI tests:
   ```bash
   dotnet test --filter "Category=UI"
   ```

## Code Coverage Goals

| Component | Target | Current | Status |
|-----------|--------|---------|--------|
| ViewModels | 95% | ~95% | ✅ Complete |
| Services | 90% | 0% | ⏳ Pending |
| Models | 85% | ~90% | ✅ Complete |
| Converters | 100% | 100% | ✅ Complete |
| UI Components | 70% | 0% | ⏳ Pending |

**Overall Target: 85%**
**Current Overall: ~59%**

## Best Practices

### 1. Test Naming Convention
```
MethodName_StateUnderTest_ExpectedBehavior
```

Example:
```csharp
LoadMixes_WhenNoFavorites_ShouldLoadEmptyCollection
```

### 2. Arrange-Act-Assert Pattern

Always structure tests in three clear sections:

```csharp
// Arrange - Set up test data
var sound = new Sound { Id = 1, Name = "Rain" };

// Act - Execute the method
await viewModel.PlaySoundCommand.ExecuteAsync(sound);

// Assert - Verify the results
mockAudioService.Verify(x => x.PlaySoundAsync(sound), Times.Once());
```

### 3. Use FluentAssertions

Prefer FluentAssertions for more readable assertions:

```csharp
// Good
result.Should().BeTrue();
collection.Should().HaveCount(3);
exception.Should().BeOfType<ArgumentNullException>();

// Avoid
Assert.True(result);
Assert.Equal(3, collection.Count);
Assert.IsType<ArgumentNullException>(exception);
```

### 4. Test One Thing

Each test should verify a single behavior:

```csharp
// Good - Tests one thing
[Fact]
public void PlaySound_ShouldCallAudioService()
{
    // Test only the interaction with audio service
}

// Bad - Tests multiple things
[Fact]
public void PlaySound_ShouldCallAudioServiceAndUpdateUIAndTrackAnalytics()
{
    // Too many responsibilities in one test
}
```

### 5. Mock External Dependencies

Always mock external dependencies:

```csharp
var mockAudioService = new Mock<IAudioService>();
var mockDataService = new Mock<IDataService>();
var viewModel = new HomeViewModel(mockAudioService.Object, mockDataService.Object);
```

### 6. Use Theory for Similar Tests

Use `[Theory]` with `[InlineData]` for testing multiple inputs:

```csharp
[Theory]
[InlineData(0, true)]
[InlineData(1, false)]
[InlineData(100, false)]
public void IsZeroConverter_VariousValues_ShouldConvertCorrectly(int value, bool expected)
{
    // Test implementation
}
```

## Debugging Tests

### In Visual Studio Code

1. Set breakpoints in test code
2. Open Test Explorer (View → Testing)
3. Right-click test → Debug Test

### In Visual Studio 2022

1. Open Test Explorer (Test → Test Explorer)
2. Right-click test → Debug
3. Or use Ctrl+R, Ctrl+T to run, Ctrl+R, D to debug

### Common Issues

#### Tests Not Discovered

```bash
# Clean and rebuild
dotnet clean
dotnet build
```

#### Async Tests Hanging

Ensure all async methods are properly awaited:
```csharp
// Good
await viewModel.LoadDataCommand.ExecuteAsync(null);

// Bad - Will hang
viewModel.LoadDataCommand.ExecuteAsync(null); // Missing await
```

#### Mock Setup Not Working

Verify mock setup matches method signature exactly:
```csharp
// Ensure parameter types match
mockService.Setup(x => x.Method(It.IsAny<int>())).ReturnsAsync(true);
```

## Continuous Integration

### GitHub Actions Example

```yaml
name: Run Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v2
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 9.0.x
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Test
      run: dotnet test --no-build --verbosity normal /p:CollectCoverage=true
    
    - name: Upload coverage
      uses: codecov/codecov-action@v2
      with:
        file: ./coverage.opencover.xml
```

## Test Data Management

### Using Builders for Test Data

```csharp
public class SoundBuilder
{
    private Sound _sound = new Sound();

    public SoundBuilder WithId(int id)
    {
        _sound.Id = id;
        return this;
    }

    public SoundBuilder WithName(string name)
    {
        _sound.Name = name;
        return this;
    }

    public Sound Build() => _sound;
}

// Usage in tests
var sound = new SoundBuilder()
    .WithId(1)
    .WithName("Rain")
    .Build();
```

## Performance Testing

### Benchmark Example

```csharp
[Fact]
public async Task LoadMixes_Performance_ShouldCompleteWithin500ms()
{
    // Arrange
    var stopwatch = Stopwatch.StartNew();

    // Act
    await viewModel.LoadMixesCommand.ExecuteAsync(null);
    stopwatch.Stop();

    // Assert
    stopwatch.ElapsedMilliseconds.Should().BeLessThan(500);
}
```

## Resources

- [xUnit Documentation](https://xunit.net/)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Appium Documentation](http://appium.io/docs/en/about-appium/intro/)
- [.NET MAUI Testing](https://docs.microsoft.com/en-us/dotnet/maui/deployment/testing)

## Contributing

When adding new tests:

1. Follow existing test patterns
2. Maintain AAA structure
3. Use descriptive test names
4. Add appropriate assertions
5. Update TEST_SUITE_SUMMARY.md
6. Ensure all tests pass before committing

## Support

For questions or issues:
- Review existing tests for patterns
- Check TEST_SUITE_SUMMARY.md for test organization
- Consult xUnit/Moq/FluentAssertions documentation

---

**Last Updated**: 2025-10-25
**Test Coverage**: 59% (289/~490 tests)
