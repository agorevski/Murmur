using FluentAssertions;
using Moq;
using Murmur.App.Models;
using Murmur.App.Services;

namespace Murmur.App.Tests.Services;

public class SoundLibraryServiceTests
{
    private readonly SoundLibraryService _service;

    public SoundLibraryServiceTests()
    {
        _service = new SoundLibraryService();
    }

    #region GetAllSounds Tests

    [Fact]
    public async Task GetAllSounds_ShouldReturnNonEmptyList()
    {
        // Act
        var sounds = await _service.GetAllSoundsAsync();

        // Assert
        sounds.Should().NotBeNull();
        sounds.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAllSounds_ShouldIncludeFreeAndPremiumSounds()
    {
        // Act
        var sounds = await _service.GetAllSoundsAsync();

        // Assert
        sounds.Should().Contain(s => s.IsPremium == false, "Should have free sounds");
        sounds.Should().Contain(s => s.IsPremium == true, "Should have premium sounds");
    }

    [Fact]
    public async Task GetAllSounds_ShouldReturnSoundsWithUniqueIds()
    {
        // Act
        var sounds = await _service.GetAllSoundsAsync();

        // Assert
        var ids = sounds.Select(s => s.Id).ToList();
        ids.Should().OnlyHaveUniqueItems("Each sound should have a unique ID");
    }

    [Fact]
    public async Task GetAllSounds_ShouldReturnSoundsWithNames()
    {
        // Act
        var sounds = await _service.GetAllSoundsAsync();

        // Assert
        sounds.Should().AllSatisfy(s => s.Name.Should().NotBeNullOrEmpty());
    }

    [Fact]
    public async Task GetAllSounds_ShouldReturnSoundsWithFileNames()
    {
        // Act
        var sounds = await _service.GetAllSoundsAsync();

        // Assert
        sounds.Should().AllSatisfy(s => s.FileName.Should().NotBeNullOrEmpty());
    }

    [Fact]
    public async Task GetAllSounds_ShouldReturnSoundsWithCategories()
    {
        // Act
        var sounds = await _service.GetAllSoundsAsync();

        // Assert
        sounds.Should().AllSatisfy(s => s.Category.Should().NotBeNullOrEmpty());
    }

    [Fact]
    public async Task GetAllSounds_CalledMultipleTimes_ShouldReturnConsistentResults()
    {
        // Act
        var sounds1 = await _service.GetAllSoundsAsync();
        var sounds2 = await _service.GetAllSoundsAsync();

        // Assert
        sounds1.Count.Should().Be(sounds2.Count);
    }

    #endregion

    #region GetFreeSounds Tests

    [Fact]
    public async Task GetFreeSounds_ShouldReturnOnlyFreeSounds()
    {
        // Act
        var sounds = await _service.GetFreeSoundsAsync();

        // Assert
        sounds.Should().NotBeEmpty();
        sounds.Should().AllSatisfy(s => s.IsPremium.Should().BeFalse());
    }

    [Fact]
    public async Task GetFreeSounds_ShouldBeSubsetOfAllSounds()
    {
        // Act
        var allSounds = await _service.GetAllSoundsAsync();
        var freeSounds = await _service.GetFreeSoundsAsync();

        // Assert
        freeSounds.Count.Should().BeLessThanOrEqualTo(allSounds.Count);
    }

    [Fact]
    public async Task GetFreeSounds_ShouldHaveAtLeastThreeSounds()
    {
        // Act
        var sounds = await _service.GetFreeSoundsAsync();

        // Assert
        sounds.Count.Should().BeGreaterThanOrEqualTo(3, "Should have at least 3 free sounds for basic functionality");
    }

    [Fact]
    public async Task GetFreeSounds_ShouldNotIncludePremiumSounds()
    {
        // Act
        var freeSounds = await _service.GetFreeSoundsAsync();

        // Assert
        freeSounds.Should().NotContain(s => s.IsPremium == true);
    }

    #endregion

    #region GetSoundById Tests

    [Fact]
    public async Task GetSoundById_WithValidId_ShouldReturnSound()
    {
        // Arrange
        var allSounds = await _service.GetAllSoundsAsync();
        var existingId = allSounds.First().Id;

        // Act
        var sound = await _service.GetSoundByIdAsync(existingId);

        // Assert
        sound.Should().NotBeNull();
        sound!.Id.Should().Be(existingId);
    }

    [Fact]
    public async Task GetSoundById_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var sound = await _service.GetSoundByIdAsync(99999);

        // Assert
        sound.Should().BeNull();
    }

    [Fact]
    public async Task GetSoundById_WithNegativeId_ShouldReturnNull()
    {
        // Act
        var sound = await _service.GetSoundByIdAsync(-1);

        // Assert
        sound.Should().BeNull();
    }

    [Fact]
    public async Task GetSoundById_WithZeroId_ShouldReturnNull()
    {
        // Act
        var sound = await _service.GetSoundByIdAsync(0);

        // Assert
        sound.Should().BeNull();
    }

    [Fact]
    public async Task GetSoundById_ShouldReturnSameObjectAsFromGetAllSounds()
    {
        // Arrange
        var allSounds = await _service.GetAllSoundsAsync();
        var expectedSound = allSounds.First();

        // Act
        var sound = await _service.GetSoundByIdAsync(expectedSound.Id);

        // Assert
        sound.Should().NotBeNull();
        sound!.Name.Should().Be(expectedSound.Name);
        sound.FileName.Should().Be(expectedSound.FileName);
        sound.Category.Should().Be(expectedSound.Category);
    }

    #endregion

    #region Sound Properties Validation Tests

    [Fact]
    public async Task AllSounds_ShouldHaveValidFileExtensions()
    {
        // Act
        var sounds = await _service.GetAllSoundsAsync();

        // Assert
        sounds.Should().AllSatisfy(s =>
        {
            var validExtensions = new[] { ".mp3", ".wav", ".ogg" };
            validExtensions.Should().Contain(ext => s.FileName.EndsWith(ext), 
                "file should have a valid audio extension");
        });
    }

    [Fact]
    public async Task AllSounds_ShouldHaveDescriptions()
    {
        // Act
        var sounds = await _service.GetAllSoundsAsync();

        // Assert
        sounds.Should().AllSatisfy(s => s.Description.Should().NotBeNull());
    }

    [Theory]
    [InlineData("Nature")]
    [InlineData("Water")]
    [InlineData("Urban")]
    [InlineData("White Noise")]
    public async Task AllSounds_ShouldHaveValidCategories(string expectedCategory)
    {
        // Act
        var sounds = await _service.GetAllSoundsAsync();

        // Assert - At least one sound should be in each major category
        sounds.Where(s => s.Category == expectedCategory).Should().NotBeEmpty($"Should have sounds in {expectedCategory} category");
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task Integration_GetAllSounds_ThenGetById_ShouldWorkCorrectly()
    {
        // Arrange
        var allSounds = await _service.GetAllSoundsAsync();
        var randomSound = allSounds[allSounds.Count / 2];

        // Act
        var retrievedSound = await _service.GetSoundByIdAsync(randomSound.Id);

        // Assert
        retrievedSound.Should().NotBeNull();
        retrievedSound!.Name.Should().Be(randomSound.Name);
    }

    [Fact]
    public async Task Integration_FreeSounds_ShouldAllBeAccessibleById()
    {
        // Arrange
        var freeSounds = await _service.GetFreeSoundsAsync();

        // Act & Assert
        foreach (var sound in freeSounds)
        {
            var retrieved = await _service.GetSoundByIdAsync(sound.Id);
            retrieved.Should().NotBeNull($"Free sound {sound.Name} should be retrievable by ID");
        }
    }

    [Fact]
    public async Task Integration_PremiumAndFreeSounds_ShouldHaveNoOverlap()
    {
        // Arrange
        var allSounds = await _service.GetAllSoundsAsync();
        var freeSounds = await _service.GetFreeSoundsAsync();

        // Act
        var premiumSounds = allSounds.Where(s => s.IsPremium).ToList();
        var freeIds = freeSounds.Select(s => s.Id).ToHashSet();
        var premiumIds = premiumSounds.Select(s => s.Id).ToHashSet();

        // Assert
        freeIds.Should().NotIntersectWith(premiumIds, "A sound cannot be both free and premium");
    }

    #endregion

    #region Performance Tests

    [Fact]
    public async Task GetAllSounds_ShouldCompleteQuickly()
    {
        // Arrange
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        await _service.GetAllSoundsAsync();
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000, "Sound loading should be fast");
    }

    [Fact]
    public async Task GetSoundById_ShouldCompleteQuickly()
    {
        // Arrange
        var allSounds = await _service.GetAllSoundsAsync();
        var testId = allSounds.First().Id;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        await _service.GetSoundByIdAsync(testId);
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100, "ID lookup should be very fast");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task GetAllSounds_CalledConcurrently_ShouldHandleCorrectly()
    {
        // Act
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => _service.GetAllSoundsAsync())
            .ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().AllSatisfy(sounds => sounds.Should().NotBeEmpty());
        var firstCount = results[0].Count;
        results.Should().AllSatisfy(sounds => sounds.Count.Should().Be(firstCount));
    }

    [Fact]
    public async Task GetFreeSounds_CalledMultipleTimes_ShouldReturnSameCount()
    {
        // Act
        var sounds1 = await _service.GetFreeSoundsAsync();
        var sounds2 = await _service.GetFreeSoundsAsync();
        var sounds3 = await _service.GetFreeSoundsAsync();

        // Assert
        sounds2.Count.Should().Be(sounds1.Count);
        sounds3.Count.Should().Be(sounds1.Count);
    }

    #endregion
}
