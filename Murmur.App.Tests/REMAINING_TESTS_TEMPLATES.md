# Remaining Test Templates

This document contains templates and patterns for the remaining Service and UI tests.

## Service Tests Remaining (5 files)

### 1. AudioServiceTests.cs Template

```csharp
using FluentAssertions;
using Moq;
using Murmur.App.Models;
using Murmur.App.Services;

namespace Murmur.App.Tests.Services;

public class AudioServiceTests
{
    [Fact]
    public async Task PlaySound_WithValidSound_ShouldReturnTrue()
    {
        // Test: Play a valid sound
        // Verify: Audio playback starts successfully
    }

    [Fact]
    public async Task StopSound_WhenPlaying_ShouldStopPlayback()
    {
        // Test: Stop a currently playing sound
        // Verify: Sound playback stops
    }

    [Fact]
    public async Task SetVolume_WithValidValue_ShouldAdjustVolume()
    {
        // Test: Set volume to 0.5f
        // Verify: Volume is set correctly
    }

    [Fact]
    public async Task FadeIn_ShouldGraduallyIncreaseVolume()
    {
        // Test: Fade in a sound over 2 seconds
        // Verify: Volume increases from 0 to target
    }

    [Fact]
    public async Task FadeOut_ShouldGraduallyDecreaseVolume()
    {
        // Test: Fade out a sound over 2 seconds
        // Verify: Volume decreases to 0
    }

    [Fact]
    public async Task PlayMultipleSounds_ShouldMixAudio()
    {
        // Test: Play 3 different sounds simultaneously
        // Verify: All sounds play together
    }

    [Fact]
    public async Task PlaySound_WithInvalidPath_ShouldHandleGracefully()
    {
        // Test: Attempt to play non-existent file
        // Verify: Returns false or throws handled exception
    }
}
```

### 2. DataServiceTests.cs Template

```csharp
using FluentAssertions;
using Murmur.App.Models;
using Murmur.App.Services;

namespace Murmur.App.Tests.Services;

public class DataServiceTests
{
    [Fact]
    public async Task InitializeDatabase_ShouldCreateTables()
    {
        // Test: Initialize database
        // Verify: Tables are created
    }

    [Fact]
    public async Task SaveMix_ShouldPersistToDatabase()
    {
        // Test: Save a mix with sounds
        // Verify: Mix is retrievable
    }

    [Fact]
    public async Task GetAllMixes_ShouldReturnAllSavedMixes()
    {
        // Test: Save 3 mixes, then retrieve all
        // Verify: All 3 mixes returned
    }

    [Fact]
    public async Task DeleteMix_ShouldRemoveFromDatabase()
    {
        // Test: Delete an existing mix
        // Verify: Mix no longer retrievable
    }

    [Fact]
    public async Task GetFavoriteMixes_ShouldReturnOnlyFavorites()
    {
        // Test: Get mixes where IsFavorite = true
        // Verify: Only favorites returned
    }

    [Fact]
    public async Task SaveUserPreferences_ShouldPersist()
    {
        // Test: Save preferences with specific values
        // Verify: Values retrieved correctly
    }

    [Fact]
    public async Task UpdateMix_ShouldModifyExisting()
    {
        // Test: Update mix name
        // Verify: Changes persisted
    }
}
```

### 3. BillingServiceTests.cs Template

```csharp
using FluentAssertions;
using Moq;
using Murmur.App.Services;

namespace Murmur.App.Tests.Services;

public class BillingServiceTests
{
    [Fact]
    public async Task Initialize_ShouldConnectToStore()
    {
        // Test: Initialize billing service
        // Verify: Connection successful
    }

    [Fact]
    public async Task CheckPremiumStatus_ForFreeUser_ShouldReturnFalse()
    {
        // Test: Check premium status for free user
        // Verify: Returns false
    }

    [Fact]
    public async Task PurchasePremium_WhenSuccessful_ShouldReturnTrue()
    {
        // Test: Complete premium purchase
        // Verify: Purchase successful
    }

    [Fact]
    public async Task PurchasePremium_WhenCancelled_ShouldReturnFalse()
    {
        // Test: User cancels purchase
        // Verify: Returns false
    }

    [Fact]
    public async Task RestorePurchases_WithPreviousPurchase_ShouldReturnTrue()
    {
        // Test: Restore previous purchases
        // Verify: Premium status restored
    }
}
```

### 4. AdServiceTests.cs Template

```csharp
using FluentAssertions;
using Murmur.App.Services;

namespace Murmur.App.Tests.Services;

public class AdServiceTests
{
    [Fact]
    public async Task Initialize_ShouldSetupAdProvider()
    {
        // Test: Initialize ad service
        // Verify: Ad provider ready
    }

    [Fact]
    public async Task ShowInterstitial_ForFreeUser_ShouldDisplayAd()
    {
        // Test: Show interstitial ad
        // Verify: Ad displayed
    }

    [Fact]
    public async Task ShowInterstitial_ForPremiumUser_ShouldNotDisplay()
    {
        // Test: Attempt to show ad for premium user
        // Verify: No ad shown
    }

    [Fact]
    public async Task ShowRewardedAd_WhenCompleted_ShouldProvideReward()
    {
        // Test: Complete rewarded ad
        // Verify: Reward granted
    }
}
```

### 5. AnalyticsServiceTests.cs Template

```csharp
using FluentAssertions;
using Murmur.App.Services;

namespace Murmur.App.Tests.Services;

public class AnalyticsServiceTests
{
    [Fact]
    public async Task Initialize_ShouldSetupTracking()
    {
        // Test: Initialize analytics
        // Verify: Tracking ready
    }

    [Fact]
    public void TrackEvent_WithValidName_ShouldLog()
    {
        // Test: Track event with name
        // Verify: Event logged
    }

    [Fact]
    public void TrackEvent_WithParameters_ShouldIncludeData()
    {
        // Test: Track event with parameters
        // Verify: Parameters included
    }

    [Fact]
    public void TrackScreen_ShouldLogScreenView()
    {
        // Test: Track screen view
        // Verify: Screen view logged
    }
}
```

## UI Tests Remaining (5 files)

### 1. MixerPageUITests.cs Template

```csharp
using FluentAssertions;

namespace Murmur.App.Tests.UI;

[Trait("Category", "UI")]
public class MixerPageUITests : AppiumTestBase
{
    [Fact(Skip = "Requires Appium")]
    public void MixerPage_WithActiveSounds_ShouldDisplayVolumeSliders()
    {
        // Navigate to mixer with active sounds
        // Verify sliders visible
    }

    [Fact(Skip = "Requires Appium")]
    public void VolumeSlider_WhenAdjusted_ShouldChangeVolume()
    {
        // Adjust volume slider
        // Verify volume changes
    }

    [Fact(Skip = "Requires Appium")]
    public void RemoveSound_ShouldRemoveFromMixer()
    {
        // Tap remove button
        // Verify sound removed
    }

    [Fact(Skip = "Requires Appium")]
    public void SaveMix_WithName_ShouldSaveToFavorites()
    {
        // Enter mix name
        // Tap save
        // Verify saved
    }
}
```

### 2. FavoritesPageUITests.cs Template

```csharp
using FluentAssertions;

namespace Murmur.App.Tests.UI;

[Trait("Category", "UI")]
public class FavoritesPageUITests : AppiumTestBase
{
    [Fact(Skip = "Requires Appium")]
    public void FavoritesPage_ShouldDisplaySavedMixes()
    {
        // Navigate to favorites
        // Verify mixes displayed
    }

    [Fact(Skip = "Requires Appium")]
    public void TapMix_ShouldPlayAllSounds()
    {
        // Tap mix
        // Verify sounds play
    }

    [Fact(Skip = "Requires Appium")]
    public void DeleteMix_ShouldRemoveFromList()
    {
        // Swipe to delete
        // Verify removed
    }

    [Fact(Skip = "Requires Appium")]
    public void ToggleFavorite_ShouldUpdateStar()
    {
        // Tap star icon
        // Verify visual change
    }
}
```

### 3. SettingsPageUITests.cs Template

```csharp
using FluentAssertions;

namespace Murmur.App.Tests.UI;

[Trait("Category", "UI")]
public class SettingsPageUITests : AppiumTestBase
{
    [Fact(Skip = "Requires Appium")]
    public void ToggleDarkMode_ShouldChangeTheme()
    {
        // Toggle dark mode switch
        // Verify theme changes
    }

    [Fact(Skip = "Requires Appium")]
    public void TapPrivacyPolicy_ShouldOpenLink()
    {
        // Tap privacy policy
        // Verify browser opens
    }

    [Fact(Skip = "Requires Appium")]
    public void ResetToDefaults_ShouldRestoreSettings()
    {
        // Tap reset button
        // Verify defaults restored
    }
}
```

### 4. PremiumPageUITests.cs Template

```csharp
using FluentAssertions;

namespace Murmur.App.Tests.UI;

[Trait("Category", "UI")]
public class PremiumPageUITests : AppiumTestBase
{
    [Fact(Skip = "Requires Appium")]
    public void PremiumPage_ShouldShowFeaturesList()
    {
        // Navigate to premium
        // Verify features displayed
    }

    [Fact(Skip = "Requires Appium")]
    public void TapPurchase_ShouldShowPurchaseDialog()
    {
        // Tap purchase button
        // Verify dialog shown
    }

    [Fact(Skip = "Requires Appium")]
    public void TapRestore_ShouldRestorePurchases()
    {
        // Tap restore button
        // Verify restoration attempted
    }
}
```

### 5. NavigationUITests.cs Template

```csharp
using FluentAssertions;

namespace Murmur.App.Tests.UI;

[Trait("Category", "UI")]
public class NavigationUITests : AppiumTestBase
{
    [Fact(Skip = "Requires Appium")]
    public void TapHomeTab_ShouldNavigateToHome()
    {
        // Tap home tab
        // Verify home page shown
    }

    [Fact(Skip = "Requires Appium")]
    public void TapMixerTab_ShouldNavigateToMixer()
    {
        // Tap mixer tab
        // Verify mixer page shown
    }

    [Fact(Skip = "Requires Appium")]
    public void TapFavoritesTab_ShouldNavigateToFavorites()
    {
        // Tap favorites tab
        // Verify favorites page shown
    }

    [Fact(Skip = "Requires Appium")]
    public void NavigateBetweenPages_ShouldMaintainState()
    {
        // Navigate between pages
        // Verify state maintained
    }
}
```

## Implementation Instructions

1. **Copy template to new file**
2. **Implement test logic**:
   - Replace comments with actual test code
   - Use appropriate assertions
   - Follow AAA pattern
3. **Add appropriate mocks** for Service tests
4. **Update AutomationIds** in UI tests to match actual XAML
5. **Run and verify tests pass**

## Notes

- All UI tests are marked with `[Trait("Category", "UI")]` 
- UI tests are skipped by default (require Appium setup)
- Service tests should use real implementations where possible
- Use mocks for external dependencies (file system, network)
- Follow existing test patterns from completed tests
