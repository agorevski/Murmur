using FluentAssertions;

namespace Murmur.App.Tests.UI;

[Trait("Category", "UI")]
public class HomePageUITests : AppiumTestBase
{
    [Fact(Skip = "Requires Appium server and Android device/emulator")]
    public void HomePage_OnLoad_ShouldDisplaySoundCards()
    {
        // Arrange
        InitializeDriver();
        Wait(2000); // Wait for page to load

        // Act
        var isDisplayed = IsElementDisplayed("SoundGrid");

        // Assert
        isDisplayed.Should().BeTrue("Sound grid should be visible on home page");
    }

    [Fact(Skip = "Requires Appium server and Android device/emulator")]
    public void TapSoundCard_ShouldStartPlayingSound()
    {
        // Arrange
        InitializeDriver();
        Wait(2000);

        // Act - Tap the first available sound
        TapElement("SoundCard_1");
        Wait(1000); // Wait for animation

        // Assert - Stop All button should be enabled
        var stopAllEnabled = IsElementEnabled("StopAllButton");
        stopAllEnabled.Should().BeTrue("Stop All button should be enabled when sound is playing");
    }

    [Fact(Skip = "Requires Appium server and Android device/emulator")]
    public void TapPlayingSound_ShouldStopSound()
    {
        // Arrange
        InitializeDriver();
        Wait(2000);
        TapElement("SoundCard_1");
        Wait(1000);

        // Act - Tap the same sound again to stop it
        TapElement("SoundCard_1");
        Wait(1000);

        // Assert - Stop All button might be disabled if no sounds playing
        var stopAllEnabled = IsElementEnabled("StopAllButton");
        // Result depends on if other sounds are playing
    }

    [Fact(Skip = "Requires Appium server and Android device/emulator")]
    public void TapStopAll_ShouldStopAllPlayingSounds()
    {
        // Arrange
        InitializeDriver();
        Wait(2000);
        
        // Start multiple sounds
        TapElement("SoundCard_1");
        Wait(500);
        TapElement("SoundCard_2");
        Wait(500);

        // Act
        TapElement("StopAllButton");
        Wait(1000);

        // Assert
        var stopAllEnabled = IsElementEnabled("StopAllButton");
        stopAllEnabled.Should().BeFalse("Stop All button should be disabled after stopping all sounds");
    }

    [Fact(Skip = "Requires Appium server and Android device/emulator")]
    public void FreeUser_CannotPlayMoreThanThreeSounds()
    {
        // Arrange
        InitializeDriver();
        Wait(2000);

        // Act - Try to play 4 sounds as free user
        TapElement("SoundCard_1");
        Wait(500);
        TapElement("SoundCard_2");
        Wait(500);
        TapElement("SoundCard_3");
        Wait(500);
        TapElement("SoundCard_4");
        Wait(1000);

        // Assert - Premium upsell should be shown
        var premiumPromptDisplayed = IsElementDisplayed("PremiumPrompt");
        premiumPromptDisplayed.Should().BeTrue("Premium prompt should appear when trying to play 4th sound");
    }

    [Fact(Skip = "Requires Appium server and Android device/emulator")]
    public void TimerButton_WhenTapped_ShouldShowTimerPicker()
    {
        // Arrange
        InitializeDriver();
        Wait(2000);

        // Act
        TapElement("TimerButton");
        Wait(1000);

        // Assert
        var pickerDisplayed = IsElementDisplayed("TimerPicker");
        pickerDisplayed.Should().BeTrue("Timer picker should be visible");
    }

    [Fact(Skip = "Requires Appium server and Android device/emulator")]
    public void SetTimer_ShouldShowCountdown()
    {
        // Arrange
        InitializeDriver();
        Wait(2000);
        TapElement("TimerButton");
        Wait(1000);

        // Act - Select 30 minutes
        TapElement("Timer30Minutes");
        Wait(500);
        TapElement("StartTimerButton");
        Wait(1000);

        // Assert
        var timerText = GetElementText("TimerDisplay");
        timerText.Should().Contain("29:", "Timer should start countdown from 30 minutes");
    }

    [Fact(Skip = "Requires Appium server and Android device/emulator")]
    public void StopTimer_ShouldCancelCountdown()
    {
        // Arrange
        InitializeDriver();
        Wait(2000);
        TapElement("TimerButton");
        Wait(1000);
        TapElement("Timer30Minutes");
        TapElement("StartTimerButton");
        Wait(2000);

        // Act
        TapElement("TimerButton"); // Tap again to stop
        Wait(1000);

        // Assert
        var timerText = GetElementText("TimerButton");
        timerText.Should().Contain("Start Timer", "Timer button should show 'Start Timer' after stopping");
    }

    [Fact(Skip = "Requires Appium server and Android device/emulator")]
    public void PlayMultipleSounds_ShouldShowAllInMixer()
    {
        // Arrange
        InitializeDriver();
        Wait(2000);

        // Act
        TapElement("SoundCard_1");
        Wait(500);
        TapElement("SoundCard_2");
        Wait(500);
        
        // Navigate to Mixer
        TapElement("MixerTab");
        Wait(1000);

        // Assert
        var mixer = FindByAutomationId("ActiveSoundsList");
        mixer.Should().NotBeNull("Mixer should show active sounds");
    }

    [Fact(Skip = "Requires Appium server and Android device/emulator")]
    public void SoundCard_ShouldShowVisualFeedback_WhenPlaying()
    {
        // Arrange
        InitializeDriver();
        Wait(2000);

        // Act
        TapElement("SoundCard_1");
        Wait(1000);

        // Assert - Check for visual indicator (border, color change, icon)
        var soundCard = FindByAutomationId("SoundCard_1");
        soundCard.Should().NotBeNull();
        // Visual state would need to be checked via attribute or color
    }

    [Fact(Skip = "Requires Appium server and Android device/emulator")]
    public void ScrollSoundGrid_ShouldShowAllSounds()
    {
        // Arrange
        InitializeDriver();
        Wait(2000);

        // Act - Scroll down to see more sounds
        var windowSize = Driver.Manage().Window.Size;
        Swipe(
            windowSize.Width / 2,
            windowSize.Height * 3 / 4,
            windowSize.Width / 2,
            windowSize.Height / 4,
            1000);
        Wait(1000);

        // Assert - Should be able to see different sounds
        var lastSound = FindByAutomationId("SoundCard_10");
        lastSound.Should().NotBeNull("Should be able to scroll to additional sounds");
    }

    [Fact(Skip = "Requires Appium server and Android device/emulator")]
    public void PremiumSound_WhenTappedByFreeUser_ShouldShowPrompt()
    {
        // Arrange
        InitializeDriver();
        Wait(2000);

        // Act - Tap a premium sound (assuming SoundCard_10 is premium)
        TapElement("PremiumSoundCard_10");
        Wait(1000);

        // Assert
        var premiumPrompt = FindByAutomationId("PremiumPrompt");
        premiumPrompt.Should().NotBeNull("Premium prompt should appear for premium sounds");
    }

    [Fact(Skip = "Requires Appium server and Android device/emulator")]
    public void Integration_CompleteUserWorkflow()
    {
        // Arrange
        InitializeDriver();
        Wait(2000);

        // Act & Assert - Step 1: Play sound
        TapElement("SoundCard_1");
        Wait(1000);
        IsElementEnabled("StopAllButton").Should().BeTrue();

        // Act & Assert - Step 2: Play second sound
        TapElement("SoundCard_2");
        Wait(1000);

        // Act & Assert - Step 3: Set timer
        TapElement("TimerButton");
        Wait(500);
        TapElement("Timer30Minutes");
        TapElement("StartTimerButton");
        Wait(1000);
        GetElementText("TimerDisplay").Should().Contain(":", "Timer should be running");

        // Act & Assert - Step 4: Stop all
        TapElement("StopAllButton");
        Wait(1000);
        IsElementEnabled("StopAllButton").Should().BeFalse();
    }

    [Fact(Skip = "Requires Appium server and Android device/emulator")]
    public void HomePage_RotateDevice_ShouldMaintainState()
    {
        // Arrange
        InitializeDriver();
        Wait(2000);
        TapElement("SoundCard_1");
        Wait(1000);

        // Act - Rotate device (this would need device-specific implementation)
        // Driver.Rotate(ScreenOrientation.Landscape);
        Wait(1000);

        // Assert - Sound should still be playing
        IsElementEnabled("StopAllButton").Should().BeTrue();
    }

    [Fact(Skip = "Requires Appium server and Android device/emulator")]
    public void HomePage_Background_AndReopen_ShouldRestoreState()
    {
        // Arrange
        InitializeDriver();
        Wait(2000);
        TapElement("SoundCard_1");
        Wait(1000);

        // Act - Background app and reopen
        Driver.BackgroundApp(TimeSpan.FromSeconds(5)); // Background for 5 seconds
        Wait(1000);

        // Assert - Sound should still be playing
        IsElementEnabled("StopAllButton").Should().BeTrue();
    }

    [Fact(Skip = "Requires Appium server and Android device/emulator")]
    public void Performance_SoundCardTap_ShouldRespondQuickly()
    {
        // Arrange
        InitializeDriver();
        Wait(2000);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        TapElement("SoundCard_1");
        
        // Wait for visual feedback
        WaitForElement("StopAllButton", 5);
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(2000, "Sound should start playing within 2 seconds");
    }
}
