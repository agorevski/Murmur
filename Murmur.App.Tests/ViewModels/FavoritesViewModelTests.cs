using FluentAssertions;
using Moq;
using Murmur.App.Models;
using Murmur.App.Services;
using Murmur.App.Tests.Mocks;
using Murmur.App.ViewModels;

namespace Murmur.App.Tests.ViewModels;

public class FavoritesViewModelTests
{
    private readonly MockAudioService _mockAudioService;
    private readonly MockDataService _mockDataService;
    private readonly Mock<IAnalyticsService> _mockAnalyticsService;
    private readonly FavoritesViewModel _viewModel;

    public FavoritesViewModelTests()
    {
        _mockAudioService = new MockAudioService();
        _mockDataService = new MockDataService(isPremium: false);
        _mockAnalyticsService = new Mock<IAnalyticsService>();

        _viewModel = new FavoritesViewModel(
            _mockAudioService.Mock.Object,
            _mockDataService.Mock.Object,
            _mockAnalyticsService.Object
        );
    }

    #region LoadMixes Tests

    [Fact]
    public async Task LoadMixes_ShouldLoadFavoriteMixesFromDataService()
    {
        // Arrange
        var favoriteMixes = new List<Mix>
        {
            new Mix { Id = 1, Name = "Mix 1", IsFavorite = true },
            new Mix { Id = 2, Name = "Mix 2", IsFavorite = true }
        };
        _mockDataService.SetFavoriteMixes(favoriteMixes);

        // Act
        await _viewModel.LoadMixesCommand.ExecuteAsync(null);

        // Assert
        _viewModel.FavoriteMixes.Should().HaveCount(2);
        _viewModel.FavoriteMixes.Should().Contain(m => m.Name == "Mix 1");
        _viewModel.FavoriteMixes.Should().Contain(m => m.Name == "Mix 2");
    }

    [Fact]
    public async Task LoadMixes_WhenNoFavorites_ShouldLoadEmptyCollection()
    {
        // Arrange
        _mockDataService.SetFavoriteMixes(new List<Mix>());

        // Act
        await _viewModel.LoadMixesCommand.ExecuteAsync(null);

        // Assert
        _viewModel.FavoriteMixes.Should().BeEmpty();
    }

    [Fact]
    public async Task LoadMixes_ShouldSetIsBusyDuringLoading()
    {
        // Arrange
        var isBusyValues = new List<bool>();
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(FavoritesViewModel.IsBusy))
                isBusyValues.Add(_viewModel.IsBusy);
        };

        // Act
        await _viewModel.LoadMixesCommand.ExecuteAsync(null);

        // Assert
        isBusyValues.Should().Contain(true, "IsBusy should be true during loading");
        isBusyValues.Last().Should().BeFalse("IsBusy should be false after loading");
    }

    [Fact]
    public async Task LoadMixes_OnError_ShouldHandleGracefully()
    {
        // Arrange
        _mockDataService.Mock
            .Setup(x => x.GetFavoriteMixesAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var act = () => _viewModel.LoadMixesCommand.ExecuteAsync(null);

        // Assert
        await act.Should().NotThrowAsync("Should handle errors gracefully");
        _viewModel.IsBusy.Should().BeFalse("IsBusy should be reset even on error");
    }

    #endregion

    #region PlayMix Tests

    [Fact]
    public async Task PlayMix_ShouldPlayAllSoundsInMix()
    {
        // Arrange
        var mix = new Mix
        {
            Id = 1,
            Name = "Test Mix",
            Sounds = new List<PlayingSound>
            {
                new PlayingSound { Sound = new Sound { Id = 1, Name = "Rain", FilePath = "rain.mp3" }, Volume = 0.8f },
                new PlayingSound { Sound = new Sound { Id = 2, Name = "Ocean", FilePath = "ocean.mp3" }, Volume = 0.6f }
            }
        };

        // Act
        await _viewModel.PlayMixCommand.ExecuteAsync(mix);

        // Assert
        _mockAudioService.Mock.Verify(
            x => x.PlaySoundAsync(It.Is<Sound>(s => s.Name == "Rain"), 0.8f),
            Times.Once,
            "Rain sound should be played with correct volume");
        
        _mockAudioService.Mock.Verify(
            x => x.PlaySoundAsync(It.Is<Sound>(s => s.Name == "Ocean"), 0.6f),
            Times.Once,
            "Ocean sound should be played with correct volume");
    }

    [Fact]
    public async Task PlayMix_WithNullMix_ShouldNotThrow()
    {
        // Act
        var act = () => _viewModel.PlayMixCommand.ExecuteAsync(null);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task PlayMix_WithEmptySounds_ShouldNotThrow()
    {
        // Arrange
        var mix = new Mix
        {
            Id = 1,
            Name = "Empty Mix",
            Sounds = new List<PlayingSound>()
        };

        // Act
        var act = () => _viewModel.PlayMixCommand.ExecuteAsync(mix);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task PlayMix_ShouldTrackAnalytics()
    {
        // Arrange
        var mix = new Mix
        {
            Id = 1,
            Name = "Test Mix",
            Sounds = new List<PlayingSound>()
        };

        // Act
        await _viewModel.PlayMixCommand.ExecuteAsync(mix);

        // Assert
        _mockAnalyticsService.Verify(
            x => x.TrackEvent("mix_played", It.Is<Dictionary<string, string>>(
                d => d.ContainsKey("mix_name"))),
            Times.Once);
    }

    [Fact]
    public async Task PlayMix_WhenAudioServiceFails_ShouldHandleGracefully()
    {
        // Arrange
        _mockAudioService.Mock
            .Setup(x => x.PlaySoundAsync(It.IsAny<Sound>(), It.IsAny<float>()))
            .ThrowsAsync(new Exception("Audio playback error"));

        var mix = new Mix
        {
            Id = 1,
            Name = "Test Mix",
            Sounds = new List<PlayingSound>
            {
                new PlayingSound { Sound = new Sound { Id = 1, Name = "Rain", FilePath = "rain.mp3" } }
            }
        };

        // Act
        var act = () => _viewModel.PlayMixCommand.ExecuteAsync(mix);

        // Assert
        await act.Should().NotThrowAsync("Should handle audio errors gracefully");
    }

    #endregion

    #region ToggleFavorite Tests

    [Fact]
    public async Task ToggleFavorite_ShouldUnfavoriteMix()
    {
        // Arrange
        var mix = new Mix { Id = 1, Name = "Mix 1", IsFavorite = true };
        _mockDataService.SetFavoriteMixes(new List<Mix> { mix });
        await _viewModel.LoadMixesCommand.ExecuteAsync(null);

        // Act
        await _viewModel.ToggleFavoriteCommand.ExecuteAsync(mix);

        // Assert
        mix.IsFavorite.Should().BeFalse("Mix should be unfavorited");
        _mockDataService.Mock.Verify(x => x.SaveMixAsync(mix), Times.Once);
    }

    [Fact]
    public async Task ToggleFavorite_ShouldReloadMixes()
    {
        // Arrange
        var mix = new Mix { Id = 1, Name = "Mix 1", IsFavorite = true };
        _mockDataService.SetFavoriteMixes(new List<Mix> { mix });
        await _viewModel.LoadMixesCommand.ExecuteAsync(null);
        var initialCount = _viewModel.FavoriteMixes.Count;

        // Setup: After unfavoriting, it won't be in favorites anymore
        _mockDataService.SetFavoriteMixes(new List<Mix>());

        // Act
        await _viewModel.ToggleFavoriteCommand.ExecuteAsync(mix);

        // Assert
        _viewModel.FavoriteMixes.Should().BeEmpty("Unfavorited mix should be removed from list");
    }

    [Fact]
    public async Task ToggleFavorite_WithNullMix_ShouldNotThrow()
    {
        // Act
        var act = () => _viewModel.ToggleFavoriteCommand.ExecuteAsync(null);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ToggleFavorite_ShouldTrackAnalytics()
    {
        // Arrange
        var mix = new Mix { Id = 1, Name = "Mix 1", IsFavorite = true };

        // Act
        await _viewModel.ToggleFavoriteCommand.ExecuteAsync(mix);

        // Assert
        _mockAnalyticsService.Verify(
            x => x.TrackEvent("mix_unfavorited", It.Is<Dictionary<string, string>>(
                d => d.ContainsKey("mix_name"))),
            Times.Once);
    }

    #endregion

    #region DeleteMix Tests

    [Fact]
    public async Task DeleteMix_ShouldRemoveMixFromDatabase()
    {
        // Arrange
        var mix = new Mix { Id = 1, Name = "Mix 1", IsFavorite = true };
        _mockDataService.SetFavoriteMixes(new List<Mix> { mix });
        await _viewModel.LoadMixesCommand.ExecuteAsync(null);

        // Act
        await _viewModel.DeleteMixCommand.ExecuteAsync(mix);

        // Assert
        _mockDataService.Mock.Verify(x => x.DeleteMixAsync(mix), Times.Once);
    }

    [Fact]
    public async Task DeleteMix_ShouldReloadMixes()
    {
        // Arrange
        var mix = new Mix { Id = 1, Name = "Mix 1", IsFavorite = true };
        _mockDataService.SetFavoriteMixes(new List<Mix> { mix });
        await _viewModel.LoadMixesCommand.ExecuteAsync(null);

        // Setup: After deletion, list should be empty
        _mockDataService.SetFavoriteMixes(new List<Mix>());

        // Act
        await _viewModel.DeleteMixCommand.ExecuteAsync(mix);

        // Assert
        _viewModel.FavoriteMixes.Should().BeEmpty("Deleted mix should be removed from list");
    }

    [Fact]
    public async Task DeleteMix_WithNullMix_ShouldNotThrow()
    {
        // Act
        var act = () => _viewModel.DeleteMixCommand.ExecuteAsync(null);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteMix_ShouldTrackAnalytics()
    {
        // Arrange
        var mix = new Mix { Id = 1, Name = "Mix 1" };

        // Act
        await _viewModel.DeleteMixCommand.ExecuteAsync(mix);

        // Assert
        _mockAnalyticsService.Verify(
            x => x.TrackEvent("mix_deleted", It.Is<Dictionary<string, string>>(
                d => d.ContainsKey("mix_name"))),
            Times.Once);
    }

    [Fact]
    public async Task DeleteMix_WhenDatabaseFails_ShouldHandleGracefully()
    {
        // Arrange
        _mockDataService.Mock
            .Setup(x => x.DeleteMixAsync(It.IsAny<Mix>()))
            .ThrowsAsync(new Exception("Database error"));

        var mix = new Mix { Id = 1, Name = "Mix 1" };

        // Act
        var act = () => _viewModel.DeleteMixCommand.ExecuteAsync(mix);

        // Assert
        await act.Should().NotThrowAsync("Should handle database errors gracefully");
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task Integration_LoadMixes_PlayMix_DeleteMix_Workflow()
    {
        // Arrange
        var mix = new Mix
        {
            Id = 1,
            Name = "Test Mix",
            IsFavorite = true,
            Sounds = new List<PlayingSound>
            {
                new PlayingSound { Sound = new Sound { Id = 1, Name = "Rain", FilePath = "rain.mp3" } }
            }
        };
        _mockDataService.SetFavoriteMixes(new List<Mix> { mix });

        // Act & Assert: Load
        await _viewModel.LoadMixesCommand.ExecuteAsync(null);
        _viewModel.FavoriteMixes.Should().ContainSingle();

        // Act & Assert: Play
        await _viewModel.PlayMixCommand.ExecuteAsync(mix);
        _mockAudioService.Mock.Verify(x => x.PlaySoundAsync(It.IsAny<Sound>(), It.IsAny<float>()), Times.Once);

        // Act & Assert: Delete
        _mockDataService.SetFavoriteMixes(new List<Mix>());
        await _viewModel.DeleteMixCommand.ExecuteAsync(mix);
        _viewModel.FavoriteMixes.Should().BeEmpty();
    }

    [Fact]
    public async Task Integration_MultipleFavoriteMixes_ShouldHandleCorrectly()
    {
        // Arrange
        var mixes = new List<Mix>
        {
            new Mix { Id = 1, Name = "Morning Mix", IsFavorite = true },
            new Mix { Id = 2, Name = "Evening Mix", IsFavorite = true },
            new Mix { Id = 3, Name = "Focus Mix", IsFavorite = true }
        };
        _mockDataService.SetFavoriteMixes(mixes);

        // Act
        await _viewModel.LoadMixesCommand.ExecuteAsync(null);

        // Assert
        _viewModel.FavoriteMixes.Should().HaveCount(3);
        _viewModel.FavoriteMixes.Should().Contain(m => m.Name == "Morning Mix");
        _viewModel.FavoriteMixes.Should().Contain(m => m.Name == "Evening Mix");
        _viewModel.FavoriteMixes.Should().Contain(m => m.Name == "Focus Mix");
    }

    #endregion

    #region Command CanExecute Tests

    [Fact]
    public void PlayMixCommand_ShouldBeExecutable()
    {
        // Arrange
        var mix = new Mix { Id = 1, Name = "Test Mix" };

        // Act
        var canExecute = _viewModel.PlayMixCommand.CanExecute(mix);

        // Assert
        canExecute.Should().BeTrue();
    }

    [Fact]
    public void ToggleFavoriteCommand_ShouldBeExecutable()
    {
        // Arrange
        var mix = new Mix { Id = 1, Name = "Test Mix" };

        // Act
        var canExecute = _viewModel.ToggleFavoriteCommand.CanExecute(mix);

        // Assert
        canExecute.Should().BeTrue();
    }

    [Fact]
    public void DeleteMixCommand_ShouldBeExecutable()
    {
        // Arrange
        var mix = new Mix { Id = 1, Name = "Test Mix" };

        // Act
        var canExecute = _viewModel.DeleteMixCommand.CanExecute(mix);

        // Assert
        canExecute.Should().BeTrue();
    }

    #endregion
}
