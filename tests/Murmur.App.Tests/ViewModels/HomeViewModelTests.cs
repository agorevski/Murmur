using FluentAssertions;
using Moq;
using Murmur.App.Models;
using Murmur.App.Services;
using Murmur.App.Tests.Mocks;
using Murmur.App.ViewModels;

namespace Murmur.App.Tests.ViewModels;

public class HomeViewModelTests
{
    private readonly MockAudioService _mockAudioService;
    private readonly MockSoundLibraryService _mockSoundLibraryService;
    private readonly MockDataService _mockDataService;
    private readonly Mock<IAdService> _mockAdService;
    private readonly Mock<IAnalyticsService> _mockAnalyticsService;
    private readonly HomeViewModel _viewModel;

    public HomeViewModelTests()
    {
        _mockAudioService = new MockAudioService();
        _mockSoundLibraryService = new MockSoundLibraryService();
        _mockDataService = new MockDataService(isPremium: false);
        _mockAdService = new Mock<IAdService>();
        _mockAnalyticsService = new Mock<IAnalyticsService>();

        _viewModel = new HomeViewModel(
            _mockAudioService.Mock.Object,
            _mockSoundLibraryService.Mock.Object,
            _mockDataService.Mock.Object,
            _mockAdService.Object,
            _mockAnalyticsService.Object
        );
    }

    #region UX User Test Scenarios

    [Fact]
    public async Task UX_Test_OpenApp_ClickRainButton_SoundPlays_StopAllEnabled()
    {
        // Arrange: Simulate opening the app
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
        var rainSound = _mockSoundLibraryService.GetRainSound();

        // Verify initial state - Stop-All should be disabled (IsPlaying = false)
        _viewModel.IsPlaying.Should().BeFalse("Stop-All should be disabled initially");

        // Act: Click the Rain button
        await _viewModel.ToggleSoundCommand.ExecuteAsync(rainSound);

        // Assert: Sound should play
        _mockAudioService.VerifyPlaySoundCalled(rainSound, Times.Once());
        _mockAudioService.VerifyFadeInCalled(rainSound.Id, Times.Once());

        // Assert: Stop-All button should be enabled (IsPlaying = true)
        _viewModel.IsPlaying.Should().BeTrue("Stop-All should be enabled when sound is playing");
        _viewModel.PlayingSounds.Should().ContainSingle()
            .Which.Sound.Name.Should().Be("Rain");
    }

    [Fact]
    public async Task UX_Test_ClickStopAll_ButtonGreysOut_AndStopsRainSounds()
    {
        // Arrange: Setup - Rain is already playing
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
        var rainSound = _mockSoundLibraryService.GetRainSound();
        await _viewModel.ToggleSoundCommand.ExecuteAsync(rainSound);

        // Verify Rain is playing and Stop-All is enabled
        _viewModel.IsPlaying.Should().BeTrue("Stop-All should be enabled");
        _viewModel.PlayingSounds.Should().ContainSingle();

        // Act: Click Stop-All button
        await _viewModel.StopAllCommand.ExecuteAsync(null);

        // Assert: Stop-All button should be greyed out (IsPlaying = false)
        _viewModel.IsPlaying.Should().BeFalse("Stop-All should be disabled/greyed out");

        // Assert: Rain sounds should be stopped (LoadData calls StopAll once, StopAllCommand calls it again)
        _mockAudioService.VerifyStopAllCalled(Times.AtLeast(1));
        _viewModel.PlayingSounds.Should().BeEmpty("All sounds should be stopped");
    }

    [Fact]
    public async Task UX_Test_FullWorkflow_OpenApp_PlayRain_StopAll()
    {
        // This test covers the complete user workflow you described

        // Step 1: Open the app
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
        _viewModel.IsPlaying.Should().BeFalse("Initially no sounds playing");

        // Step 2: Click 'Rain' button
        var rainSound = _mockSoundLibraryService.GetRainSound();
        await _viewModel.ToggleSoundCommand.ExecuteAsync(rainSound);

        // Step 3: Verify sound plays and Stop-All is enabled
        _mockAudioService.VerifyPlaySoundCalled(rainSound, Times.Once());
        _viewModel.IsPlaying.Should().BeTrue("Stop-All enabled after playing sound");
        _viewModel.PlayingSounds.Should().ContainSingle();

        // Step 4: Click Stop-All button
        await _viewModel.StopAllCommand.ExecuteAsync(null);

        // Step 5: Verify button is greyed out and sounds stopped
        _viewModel.IsPlaying.Should().BeFalse("Stop-All button greyed out");
        _viewModel.PlayingSounds.Should().BeEmpty("All sounds stopped");
        _mockAudioService.VerifyStopAllCalled(Times.AtLeast(1));
    }

    #endregion

    #region Sound Toggle Tests

    [Fact]
    public async Task ToggleSound_WhenSoundNotPlaying_ShouldStartPlayingSound()
    {
        // Arrange
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
        var rainSound = _mockSoundLibraryService.GetRainSound();

        // Act
        await _viewModel.ToggleSoundCommand.ExecuteAsync(rainSound);

        // Assert
        _mockAudioService.VerifyPlaySoundCalled(rainSound, Times.Once());
        _viewModel.PlayingSounds.Should().ContainSingle()
            .Which.Sound.Id.Should().Be(rainSound.Id);
        _viewModel.IsPlaying.Should().BeTrue();
    }

    [Fact]
    public async Task ToggleSound_WhenSoundAlreadyPlaying_ShouldStopSound()
    {
        // Arrange
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
        var rainSound = _mockSoundLibraryService.GetRainSound();
        await _viewModel.ToggleSoundCommand.ExecuteAsync(rainSound); // Start playing

        // Act
        await _viewModel.ToggleSoundCommand.ExecuteAsync(rainSound); // Stop playing

        // Assert
        _mockAudioService.VerifyFadeOutCalled(rainSound.Id, Times.Once());
        _viewModel.PlayingSounds.Should().BeEmpty();
        _viewModel.IsPlaying.Should().BeFalse();
    }

    [Fact]
    public async Task ToggleSound_MultipleSounds_ShouldPlayAll()
    {
        // Arrange
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
        var rainSound = _mockSoundLibraryService.GetRainSound();
        var oceanSound = _mockSoundLibraryService.GetOceanSound();

        // Act
        await _viewModel.ToggleSoundCommand.ExecuteAsync(rainSound);
        await _viewModel.ToggleSoundCommand.ExecuteAsync(oceanSound);

        // Assert
        _mockAudioService.VerifyPlaySoundCalled(rainSound, Times.Once());
        _mockAudioService.VerifyPlaySoundCalled(oceanSound, Times.Once());
        _viewModel.PlayingSounds.Should().HaveCount(2);
        _viewModel.IsPlaying.Should().BeTrue();
    }

    [Fact]
    public async Task ToggleSound_FreeUser_CanPlay3Sounds()
    {
        // Arrange - Free user
        _mockDataService.SetPremiumStatus(false);
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
        
        var sounds = new[]
        {
            _mockSoundLibraryService.GetRainSound(),
            _mockSoundLibraryService.GetOceanSound(),
            _mockSoundLibraryService.GetTestSounds().First(s => s.Name == "Forest")
        };

        // Act - Play 3 sounds (max for free users)
        await _viewModel.ToggleSoundCommand.ExecuteAsync(sounds[0]);
        await _viewModel.ToggleSoundCommand.ExecuteAsync(sounds[1]);
        await _viewModel.ToggleSoundCommand.ExecuteAsync(sounds[2]);

        // Assert - 3 sounds should be playing successfully
        _viewModel.PlayingSounds.Should().HaveCount(3, "Free users can play up to 3 sounds");
        _viewModel.IsPlaying.Should().BeTrue();
    }

    #endregion

    #region Stop-All Button State Tests

    [Fact]
    public async Task IsPlaying_WhenNoSoundsPlaying_ShouldBeFalse()
    {
        // Arrange & Act
        await _viewModel.LoadDataCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsPlaying.Should().BeFalse("Stop-All should be disabled when no sounds playing");
        _viewModel.PlayingSounds.Should().BeEmpty();
    }

    [Fact]
    public async Task IsPlaying_WhenSoundsPlaying_ShouldBeTrue()
    {
        // Arrange
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
        var rainSound = _mockSoundLibraryService.GetRainSound();

        // Act
        await _viewModel.ToggleSoundCommand.ExecuteAsync(rainSound);

        // Assert
        _viewModel.IsPlaying.Should().BeTrue("Stop-All should be enabled when sounds are playing");
    }

    [Fact]
    public async Task IsPlaying_AfterStoppingLastSound_ShouldBeFalse()
    {
        // Arrange
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
        var rainSound = _mockSoundLibraryService.GetRainSound();
        await _viewModel.ToggleSoundCommand.ExecuteAsync(rainSound);

        // Act
        await _viewModel.ToggleSoundCommand.ExecuteAsync(rainSound); // Stop the sound

        // Assert
        _viewModel.IsPlaying.Should().BeFalse("Stop-All should be disabled when all sounds stopped");
    }

    #endregion

    #region Stop-All Functionality Tests

    [Fact]
    public async Task StopAll_WhenMultipleSoundsPlaying_ShouldStopAllSounds()
    {
        // Arrange
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
        var rainSound = _mockSoundLibraryService.GetRainSound();
        var oceanSound = _mockSoundLibraryService.GetOceanSound();
        
        await _viewModel.ToggleSoundCommand.ExecuteAsync(rainSound);
        await _viewModel.ToggleSoundCommand.ExecuteAsync(oceanSound);
        
        _viewModel.PlayingSounds.Should().HaveCount(2);

        // Act
        await _viewModel.StopAllCommand.ExecuteAsync(null);

        // Assert
        _mockAudioService.VerifyStopAllCalled(Times.AtLeast(1));
        _viewModel.PlayingSounds.Should().BeEmpty();
        _viewModel.IsPlaying.Should().BeFalse();
    }

    [Fact]
    public async Task StopAll_ShouldClearPlayingSoundsCollection()
    {
        // Arrange
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
        var rainSound = _mockSoundLibraryService.GetRainSound();
        await _viewModel.ToggleSoundCommand.ExecuteAsync(rainSound);

        // Act
        await _viewModel.StopAllCommand.ExecuteAsync(null);

        // Assert
        _viewModel.PlayingSounds.Should().BeEmpty("PlayingSounds collection should be cleared");
    }

    [Fact]
    public async Task StopAll_ShouldDisableStopAllButton()
    {
        // Arrange
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
        var rainSound = _mockSoundLibraryService.GetRainSound();
        await _viewModel.ToggleSoundCommand.ExecuteAsync(rainSound);
        _viewModel.IsPlaying.Should().BeTrue();

        // Act
        await _viewModel.StopAllCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsPlaying.Should().BeFalse("IsPlaying should be false, disabling Stop-All button");
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task Integration_PlayMultipleSounds_ThenStopAll_ThenPlayAgain()
    {
        // Arrange
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
        var rainSound = _mockSoundLibraryService.GetRainSound();
        var oceanSound = _mockSoundLibraryService.GetOceanSound();

        // Act - Play multiple sounds
        await _viewModel.ToggleSoundCommand.ExecuteAsync(rainSound);
        await _viewModel.ToggleSoundCommand.ExecuteAsync(oceanSound);
        _viewModel.PlayingSounds.Should().HaveCount(2);
        _viewModel.IsPlaying.Should().BeTrue();

        // Act - Stop all
        await _viewModel.StopAllCommand.ExecuteAsync(null);
        _viewModel.PlayingSounds.Should().BeEmpty();
        _viewModel.IsPlaying.Should().BeFalse();

        // Act - Play again
        await _viewModel.ToggleSoundCommand.ExecuteAsync(rainSound);
        _viewModel.PlayingSounds.Should().HaveCount(1);
        _viewModel.IsPlaying.Should().BeTrue();
    }

    [Fact]
    public async Task Integration_LoadData_ShouldStopAnyExistingSounds()
    {
        // Arrange
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
        var rainSound = _mockSoundLibraryService.GetRainSound();
        await _viewModel.ToggleSoundCommand.ExecuteAsync(rainSound);

        // Act - Reload data (simulating app restart or navigation back)
        await _viewModel.LoadDataCommand.ExecuteAsync(null);

        // Assert
        _mockAudioService.VerifyStopAllCalled(Times.AtLeastOnce());
        _viewModel.PlayingSounds.Should().BeEmpty();
        _viewModel.IsPlaying.Should().BeFalse();
    }

    #endregion

    #region Analytics Tests

    [Fact]
    public async Task ToggleSound_ShouldTrackAnalyticsEvent()
    {
        // Arrange
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
        var rainSound = _mockSoundLibraryService.GetRainSound();

        // Act
        await _viewModel.ToggleSoundCommand.ExecuteAsync(rainSound);

        // Assert
        _mockAnalyticsService.Verify(x => x.TrackEvent(
            "sound_toggled",
            It.Is<Dictionary<string, string>>(d => 
                d.ContainsKey("sound_name") && 
                d.ContainsKey("action"))),
            Times.Once);
    }

    [Fact]
    public async Task StopAll_ShouldTrackAnalyticsEvent()
    {
        // Arrange
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
        var rainSound = _mockSoundLibraryService.GetRainSound();
        await _viewModel.ToggleSoundCommand.ExecuteAsync(rainSound);

        // Act
        await _viewModel.StopAllCommand.ExecuteAsync(null);

        // Assert
        _mockAnalyticsService.Verify(x => x.TrackEvent("stop_all", null), Times.Once());
    }

    #endregion
}
