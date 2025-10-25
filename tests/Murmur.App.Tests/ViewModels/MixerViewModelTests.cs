using FluentAssertions;
using Moq;
using Murmur.App.Models;
using Murmur.App.Services;
using Murmur.App.Tests.Mocks;
using Murmur.App.ViewModels;
using System.Collections.ObjectModel;

namespace Murmur.App.Tests.ViewModels;

public class MixerViewModelTests
{
    private readonly MockAudioService _mockAudioService;
    private readonly MockDataService _mockDataService;
    private readonly Mock<IAnalyticsService> _mockAnalyticsService;
    private readonly MixerViewModel _viewModel;

    public MixerViewModelTests()
    {
        _mockAudioService = new MockAudioService();
        _mockDataService = new MockDataService(isPremium: false);
        _mockAnalyticsService = new Mock<IAnalyticsService>();

        _viewModel = new MixerViewModel(
            _mockAudioService.Mock.Object,
            _mockDataService.Mock.Object,
            _mockAnalyticsService.Object
        );
    }

    #region ActiveSounds Management Tests

    [Fact]
    public void ActiveSounds_WhenInitialized_ShouldBeEmpty()
    {
        // Assert
        _viewModel.ActiveSounds.Should().NotBeNull();
        _viewModel.ActiveSounds.Should().BeEmpty();
    }

    [Fact]
    public void ActiveSounds_WhenChanged_ShouldNotifyPropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MixerViewModel.ActiveSounds))
                propertyChangedRaised = true;
        };

        // Act
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound>
        {
            new PlayingSound { Sound = new Sound { Id = 1, Name = "Rain" }, Volume = 1.0f }
        };

        // Assert
        propertyChangedRaised.Should().BeTrue();
    }

    [Fact]
    public void OnActiveSoundsChanged_ShouldUpdateHasActiveSounds()
    {
        // Arrange
        _viewModel.HasActiveSounds.Should().BeFalse();

        // Act
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound>
        {
            new PlayingSound { Sound = new Sound { Id = 1, Name = "Rain" }, Volume = 1.0f }
        };

        // Assert
        _viewModel.HasActiveSounds.Should().BeTrue();
    }

    [Fact]
    public void OnActiveSoundsChanged_WhenEmpty_HasActiveSoundsShouldBeFalse()
    {
        // Arrange
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound>
        {
            new PlayingSound { Sound = new Sound { Id = 1, Name = "Rain" }, Volume = 1.0f }
        };
        _viewModel.HasActiveSounds.Should().BeTrue();

        // Act
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound>();

        // Assert
        _viewModel.HasActiveSounds.Should().BeFalse();
    }

    #endregion

    #region UpdateVolume Tests

    [Fact]
    public async Task UpdateVolume_ShouldCallAudioServiceSetVolume()
    {
        // Arrange
        var playingSound = new PlayingSound
        {
            Sound = new Sound { Id = 1, Name = "Rain", FilePath = "rain.mp3" },
            Volume = 0.8f
        };

        // Act
        await _viewModel.UpdateVolumeCommand.ExecuteAsync(playingSound);

        // Assert
        _mockAudioService.Mock.Verify(
            x => x.SetVolumeAsync(playingSound.Sound.Id, 0.8f),
            Times.Once);
    }

    [Fact]
    public async Task UpdateVolume_WithNullPlayingSound_ShouldNotThrow()
    {
        // Act
        var act = () => _viewModel.UpdateVolumeCommand.ExecuteAsync(null);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task UpdateVolume_WithZeroVolume_ShouldSetCorrectly()
    {
        // Arrange
        var playingSound = new PlayingSound
        {
            Sound = new Sound { Id = 1, Name = "Rain", FilePath = "rain.mp3" },
            Volume = 0.0f
        };

        // Act
        await _viewModel.UpdateVolumeCommand.ExecuteAsync(playingSound);

        // Assert
        _mockAudioService.Mock.Verify(
            x => x.SetVolumeAsync(1, 0.0f),
            Times.Once);
    }

    [Fact]
    public async Task UpdateVolume_WithMaxVolume_ShouldSetCorrectly()
    {
        // Arrange
        var playingSound = new PlayingSound
        {
            Sound = new Sound { Id = 1, Name = "Rain", FilePath = "rain.mp3" },
            Volume = 1.0f
        };

        // Act
        await _viewModel.UpdateVolumeCommand.ExecuteAsync(playingSound);

        // Assert
        _mockAudioService.Mock.Verify(
            x => x.SetVolumeAsync(1, 1.0f),
            Times.Once);
    }

    [Fact]
    public async Task UpdateVolume_ShouldTrackAnalytics()
    {
        // Arrange
        var playingSound = new PlayingSound
        {
            Sound = new Sound { Id = 1, Name = "Rain", FilePath = "rain.mp3" },
            Volume = 0.5f
        };

        // Act
        await _viewModel.UpdateVolumeCommand.ExecuteAsync(playingSound);

        // Assert
        _mockAnalyticsService.Verify(
            x => x.TrackEvent("volume_adjusted", It.Is<Dictionary<string, string>>(
                d => d.ContainsKey("sound_name") && d.ContainsKey("volume"))),
            Times.Once);
    }

    [Fact]
    public async Task UpdateVolume_WhenAudioServiceFails_ShouldHandleGracefully()
    {
        // Arrange
        _mockAudioService.Mock
            .Setup(x => x.SetVolumeAsync(It.IsAny<int>(), It.IsAny<float>()))
            .ThrowsAsync(new Exception("Audio service error"));

        var playingSound = new PlayingSound
        {
            Sound = new Sound { Id = 1, Name = "Rain" },
            Volume = 0.5f
        };

        // Act
        var act = () => _viewModel.UpdateVolumeCommand.ExecuteAsync(playingSound);

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region RemoveSound Tests

    [Fact]
    public async Task RemoveSound_ShouldStopSoundInAudioService()
    {
        // Arrange
        var playingSound = new PlayingSound
        {
            Sound = new Sound { Id = 1, Name = "Rain", FilePath = "rain.mp3" },
            Volume = 1.0f
        };
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound> { playingSound };

        // Act
        await _viewModel.RemoveSoundCommand.ExecuteAsync(playingSound);

        // Assert
        _mockAudioService.Mock.Verify(
            x => x.FadeOutAsync(playingSound.Sound.Id, It.IsAny<int>()),
            Times.Once);
    }

    [Fact]
    public async Task RemoveSound_ShouldRemoveFromActiveSounds()
    {
        // Arrange
        var playingSound = new PlayingSound
        {
            Sound = new Sound { Id = 1, Name = "Rain", FilePath = "rain.mp3" },
            Volume = 1.0f
        };
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound> { playingSound };

        // Act
        await _viewModel.RemoveSoundCommand.ExecuteAsync(playingSound);

        // Assert
        _viewModel.ActiveSounds.Should().NotContain(playingSound);
    }

    [Fact]
    public async Task RemoveSound_WithMultipleSounds_ShouldRemoveOnlySpecifiedSound()
    {
        // Arrange
        var sound1 = new PlayingSound { Sound = new Sound { Id = 1, Name = "Rain" }, Volume = 1.0f };
        var sound2 = new PlayingSound { Sound = new Sound { Id = 2, Name = "Ocean" }, Volume = 0.8f };
        var sound3 = new PlayingSound { Sound = new Sound { Id = 3, Name = "Forest" }, Volume = 0.6f };
        
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound> { sound1, sound2, sound3 };

        // Act
        await _viewModel.RemoveSoundCommand.ExecuteAsync(sound2);

        // Assert
        _viewModel.ActiveSounds.Should().HaveCount(2);
        _viewModel.ActiveSounds.Should().Contain(sound1);
        _viewModel.ActiveSounds.Should().NotContain(sound2);
        _viewModel.ActiveSounds.Should().Contain(sound3);
    }

    [Fact]
    public async Task RemoveSound_WithNullSound_ShouldNotThrow()
    {
        // Act
        var act = () => _viewModel.RemoveSoundCommand.ExecuteAsync(null);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task RemoveSound_ShouldTrackAnalytics()
    {
        // Arrange
        var playingSound = new PlayingSound
        {
            Sound = new Sound { Id = 1, Name = "Rain" },
            Volume = 1.0f
        };
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound> { playingSound };

        // Act
        await _viewModel.RemoveSoundCommand.ExecuteAsync(playingSound);

        // Assert
        _mockAnalyticsService.Verify(
            x => x.TrackEvent("sound_removed_from_mixer", It.Is<Dictionary<string, string>>(
                d => d.ContainsKey("sound_name"))),
            Times.Once);
    }

    [Fact]
    public async Task RemoveSound_WhenLastSound_ShouldUpdateHasActiveSounds()
    {
        // Arrange
        var playingSound = new PlayingSound
        {
            Sound = new Sound { Id = 1, Name = "Rain" },
            Volume = 1.0f
        };
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound> { playingSound };
        _viewModel.HasActiveSounds.Should().BeTrue();

        // Act
        await _viewModel.RemoveSoundCommand.ExecuteAsync(playingSound);

        // Assert
        _viewModel.HasActiveSounds.Should().BeFalse();
    }

    #endregion

    #region SaveMix Tests

    [Fact]
    public async Task SaveMix_WithValidName_ShouldSaveMixToDatabase()
    {
        // Arrange
        _viewModel.MixName = "My Test Mix";
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound>
        {
            new PlayingSound { Sound = new Sound { Id = 1, Name = "Rain" }, Volume = 0.8f },
            new PlayingSound { Sound = new Sound { Id = 2, Name = "Ocean" }, Volume = 0.6f }
        };

        // Act
        await _viewModel.SaveMixCommand.ExecuteAsync(null);

        // Assert
        _mockDataService.Mock.Verify(
            x => x.SaveMixAsync(It.Is<Mix>(m => 
                m.Name == "My Test Mix" && 
                m.Sounds.Count == 2)),
            Times.Once);
    }

    [Fact]
    public async Task SaveMix_ShouldIncludeAllActiveSounds()
    {
        // Arrange
        _viewModel.MixName = "Test Mix";
        var sound1 = new PlayingSound { Sound = new Sound { Id = 1, Name = "Rain" }, Volume = 0.8f };
        var sound2 = new PlayingSound { Sound = new Sound { Id = 2, Name = "Ocean" }, Volume = 0.6f };
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound> { sound1, sound2 };

        // Act
        await _viewModel.SaveMixCommand.ExecuteAsync(null);

        // Assert
        _mockDataService.Mock.Verify(
            x => x.SaveMixAsync(It.Is<Mix>(m => 
                m.Sounds.Any(s => s.Sound.Name == "Rain" && s.Volume == 0.8f) &&
                m.Sounds.Any(s => s.Sound.Name == "Ocean" && s.Volume == 0.6f))),
            Times.Once);
    }

    [Fact]
    public async Task SaveMix_WithEmptyName_ShouldNotSave()
    {
        // Arrange
        _viewModel.MixName = "";
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound>
        {
            new PlayingSound { Sound = new Sound { Id = 1, Name = "Rain" }, Volume = 0.8f }
        };

        // Act
        await _viewModel.SaveMixCommand.ExecuteAsync(null);

        // Assert
        _mockDataService.Mock.Verify(
            x => x.SaveMixAsync(It.IsAny<Mix>()),
            Times.Never);
    }

    [Fact]
    public async Task SaveMix_WithWhitespaceName_ShouldNotSave()
    {
        // Arrange
        _viewModel.MixName = "   ";
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound>
        {
            new PlayingSound { Sound = new Sound { Id = 1, Name = "Rain" }, Volume = 0.8f }
        };

        // Act
        await _viewModel.SaveMixCommand.ExecuteAsync(null);

        // Assert
        _mockDataService.Mock.Verify(
            x => x.SaveMixAsync(It.IsAny<Mix>()),
            Times.Never);
    }

    [Fact]
    public async Task SaveMix_WithNoActiveSounds_ShouldNotSave()
    {
        // Arrange
        _viewModel.MixName = "Test Mix";
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound>();

        // Act
        await _viewModel.SaveMixCommand.ExecuteAsync(null);

        // Assert
        _mockDataService.Mock.Verify(
            x => x.SaveMixAsync(It.IsAny<Mix>()),
            Times.Never);
    }

    [Fact]
    public async Task SaveMix_ShouldTrackAnalytics()
    {
        // Arrange
        _viewModel.MixName = "Test Mix";
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound>
        {
            new PlayingSound { Sound = new Sound { Id = 1, Name = "Rain" }, Volume = 0.8f }
        };

        // Act
        await _viewModel.SaveMixCommand.ExecuteAsync(null);

        // Assert
        _mockAnalyticsService.Verify(
            x => x.TrackEvent("mix_saved", It.Is<Dictionary<string, string>>(
                d => d.ContainsKey("mix_name") && d.ContainsKey("sound_count"))),
            Times.Once);
    }

    [Fact]
    public async Task SaveMix_ShouldClearMixNameAfterSaving()
    {
        // Arrange
        _viewModel.MixName = "Test Mix";
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound>
        {
            new PlayingSound { Sound = new Sound { Id = 1, Name = "Rain" }, Volume = 0.8f }
        };

        // Act
        await _viewModel.SaveMixCommand.ExecuteAsync(null);

        // Assert
        _viewModel.MixName.Should().BeEmpty();
    }

    [Fact]
    public async Task SaveMix_WhenDatabaseFails_ShouldHandleGracefully()
    {
        // Arrange
        _mockDataService.Mock
            .Setup(x => x.SaveMixAsync(It.IsAny<Mix>()))
            .ThrowsAsync(new Exception("Database error"));

        _viewModel.MixName = "Test Mix";
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound>
        {
            new PlayingSound { Sound = new Sound { Id = 1, Name = "Rain" }, Volume = 0.8f }
        };

        // Act
        var act = () => _viewModel.SaveMixCommand.ExecuteAsync(null);

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region Property Notification Tests

    [Fact]
    public void MixName_ShouldNotifyPropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MixerViewModel.MixName))
                propertyChangedRaised = true;
        };

        // Act
        _viewModel.MixName = "New Mix Name";

        // Assert
        propertyChangedRaised.Should().BeTrue();
        _viewModel.MixName.Should().Be("New Mix Name");
    }

    [Fact]
    public void HasActiveSounds_ShouldNotifyPropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MixerViewModel.HasActiveSounds))
                propertyChangedRaised = true;
        };

        // Act
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound>
        {
            new PlayingSound { Sound = new Sound { Id = 1, Name = "Rain" }, Volume = 1.0f }
        };

        // Assert
        propertyChangedRaised.Should().BeTrue();
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task Integration_AddSounds_AdjustVolumes_SaveMix_Workflow()
    {
        // Arrange
        var sound1 = new PlayingSound { Sound = new Sound { Id = 1, Name = "Rain" }, Volume = 1.0f };
        var sound2 = new PlayingSound { Sound = new Sound { Id = 2, Name = "Ocean" }, Volume = 1.0f };

        // Act & Assert: Add sounds
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound> { sound1, sound2 };
        _viewModel.HasActiveSounds.Should().BeTrue();
        _viewModel.ActiveSounds.Should().HaveCount(2);

        // Act & Assert: Adjust volumes
        sound1.Volume = 0.7f;
        await _viewModel.UpdateVolumeCommand.ExecuteAsync(sound1);
        _mockAudioService.Mock.Verify(x => x.SetVolumeAsync(1, 0.7f), Times.Once);

        sound2.Volume = 0.5f;
        await _viewModel.UpdateVolumeCommand.ExecuteAsync(sound2);
        _mockAudioService.Mock.Verify(x => x.SetVolumeAsync(2, 0.5f), Times.Once);

        // Act & Assert: Save mix
        _viewModel.MixName = "My Perfect Mix";
        await _viewModel.SaveMixCommand.ExecuteAsync(null);
        _mockDataService.Mock.Verify(
            x => x.SaveMixAsync(It.Is<Mix>(m => 
                m.Name == "My Perfect Mix" && 
                m.Sounds.Count == 2)),
            Times.Once);
        _viewModel.MixName.Should().BeEmpty();
    }

    [Fact]
    public async Task Integration_AddMultipleSounds_RemoveOne_UpdateVolume_SaveMix()
    {
        // Arrange
        var sound1 = new PlayingSound { Sound = new Sound { Id = 1, Name = "Rain" }, Volume = 1.0f };
        var sound2 = new PlayingSound { Sound = new Sound { Id = 2, Name = "Ocean" }, Volume = 1.0f };
        var sound3 = new PlayingSound { Sound = new Sound { Id = 3, Name = "Forest" }, Volume = 1.0f };

        // Add 3 sounds
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound> { sound1, sound2, sound3 };
        _viewModel.ActiveSounds.Should().HaveCount(3);

        // Remove middle sound
        await _viewModel.RemoveSoundCommand.ExecuteAsync(sound2);
        _viewModel.ActiveSounds.Should().HaveCount(2);
        _viewModel.ActiveSounds.Should().Contain(sound1);
        _viewModel.ActiveSounds.Should().NotContain(sound2);
        _viewModel.ActiveSounds.Should().Contain(sound3);

        // Update remaining sounds' volumes
        sound1.Volume = 0.8f;
        await _viewModel.UpdateVolumeCommand.ExecuteAsync(sound1);
        sound3.Volume = 0.6f;
        await _viewModel.UpdateVolumeCommand.ExecuteAsync(sound3);

        // Save mix
        _viewModel.MixName = "Two Sound Mix";
        await _viewModel.SaveMixCommand.ExecuteAsync(null);
        _mockDataService.Mock.Verify(
            x => x.SaveMixAsync(It.Is<Mix>(m => m.Sounds.Count == 2)),
            Times.Once);
    }

    [Fact]
    public async Task Integration_RemoveAllSounds_HasActiveSoundsShouldBeFalse()
    {
        // Arrange
        var sound1 = new PlayingSound { Sound = new Sound { Id = 1, Name = "Rain" }, Volume = 1.0f };
        var sound2 = new PlayingSound { Sound = new Sound { Id = 2, Name = "Ocean" }, Volume = 1.0f };
        _viewModel.ActiveSounds = new ObservableCollection<PlayingSound> { sound1, sound2 };
        _viewModel.HasActiveSounds.Should().BeTrue();

        // Act
        await _viewModel.RemoveSoundCommand.ExecuteAsync(sound1);
        _viewModel.HasActiveSounds.Should().BeTrue(); // Still one left

        await _viewModel.RemoveSoundCommand.ExecuteAsync(sound2);

        // Assert
        _viewModel.HasActiveSounds.Should().BeFalse();
        _viewModel.ActiveSounds.Should().BeEmpty();
    }

    #endregion

    #region Command CanExecute Tests

    [Fact]
    public void UpdateVolumeCommand_ShouldBeExecutable()
    {
        // Arrange
        var playingSound = new PlayingSound { Sound = new Sound { Id = 1, Name = "Rain" }, Volume = 0.5f };

        // Act
        var canExecute = _viewModel.UpdateVolumeCommand.CanExecute(playingSound);

        // Assert
        canExecute.Should().BeTrue();
    }

    [Fact]
    public void RemoveSoundCommand_ShouldBeExecutable()
    {
        // Arrange
        var playingSound = new PlayingSound { Sound = new Sound { Id = 1, Name = "Rain" }, Volume = 0.5f };

        // Act
        var canExecute = _viewModel.RemoveSoundCommand.CanExecute(playingSound);

        // Assert
        canExecute.Should().BeTrue();
    }

    [Fact]
    public void SaveMixCommand_ShouldBeExecutable()
    {
        // Act
        var canExecute = _viewModel.SaveMixCommand.CanExecute(null);

        // Assert
        canExecute.Should().BeTrue();
    }

    #endregion
}
