using FluentAssertions;
using Murmur.App.Models;

namespace Murmur.App.Tests.Models;

public class SoundTests
{
    [Fact]
    public void Sound_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var sound = new Sound();

        // Assert
        sound.Id.Should().Be(0);
        sound.Name.Should().BeEmpty();
        sound.Description.Should().BeEmpty();
        sound.FileName.Should().BeEmpty();
        sound.Category.Should().BeEmpty();
        sound.IsPremium.Should().BeFalse();
        sound.IconUrl.Should().BeEmpty();
    }

    [Fact]
    public void Sound_SetProperties_ShouldStoreValues()
    {
        // Arrange
        var sound = new Sound();

        // Act
        sound.Id = 1;
        sound.Name = "Rain";
        sound.Description = "Gentle rain sounds";
        sound.FileName = "rain.mp3";
        sound.Category = "Nature";
        sound.IsPremium = true;
        sound.IconUrl = "rain_icon.png";

        // Assert
        sound.Id.Should().Be(1);
        sound.Name.Should().Be("Rain");
        sound.Description.Should().Be("Gentle rain sounds");
        sound.FileName.Should().Be("rain.mp3");
        sound.Category.Should().Be("Nature");
        sound.IsPremium.Should().BeTrue();
        sound.IconUrl.Should().Be("rain_icon.png");
    }

    [Fact]
    public void Sound_ObjectInitializer_ShouldSetProperties()
    {
        // Act
        var sound = new Sound
        {
            Id = 1,
            Name = "Ocean Waves",
            Description = "Calming ocean sounds",
            FileName = "ocean.mp3",
            Category = "Water",
            IsPremium = false,
            IconUrl = "ocean_icon.png"
        };

        // Assert
        sound.Name.Should().Be("Ocean Waves");
        sound.Category.Should().Be("Water");
        sound.IsPremium.Should().BeFalse();
    }

    [Theory]
    [InlineData("", true)]
    [InlineData("Test", false)]
    [InlineData("   ", false)]
    public void Sound_NameValidation_ShouldHandleVariousValues(string name, bool expectEmpty)
    {
        // Arrange
        var sound = new Sound { Name = name };

        // Assert
        if (expectEmpty)
            sound.Name.Should().BeEmpty();
        else
            sound.Name.Should().NotBeEmpty();
    }
}

public class MixTests
{
    [Fact]
    public void Mix_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var mix = new Mix();

        // Assert
        mix.Id.Should().Be(0);
        mix.Name.Should().BeEmpty();
        mix.SoundIds.Should().BeEmpty();
        mix.CreatedAt.Should().Be(default);
        mix.LastUsed.Should().Be(default);
        mix.IsFavorite.Should().BeFalse();
    }

    [Fact]
    public void Mix_SetProperties_ShouldStoreValues()
    {
        // Arrange
        var mix = new Mix();
        var createdDate = DateTime.UtcNow;
        var lastUsedDate = DateTime.UtcNow.AddDays(-1);

        // Act
        mix.Id = 1;
        mix.Name = "Relaxing Mix";
        mix.SoundIds = "1,2,3";
        mix.CreatedAt = createdDate;
        mix.LastUsed = lastUsedDate;
        mix.IsFavorite = true;

        // Assert
        mix.Id.Should().Be(1);
        mix.Name.Should().Be("Relaxing Mix");
        mix.SoundIds.Should().Be("1,2,3");
        mix.CreatedAt.Should().Be(createdDate);
        mix.LastUsed.Should().Be(lastUsedDate);
        mix.IsFavorite.Should().BeTrue();
    }

    [Fact]
    public void Mix_ObjectInitializer_ShouldSetProperties()
    {
        // Arrange
        var now = DateTime.UtcNow;

        // Act
        var mix = new Mix
        {
            Id = 5,
            Name = "Morning Sounds",
            SoundIds = "10,11,12",
            CreatedAt = now,
            LastUsed = now,
            IsFavorite = true
        };

        // Assert
        mix.Name.Should().Be("Morning Sounds");
        mix.SoundIds.Should().Be("10,11,12");
        mix.IsFavorite.Should().BeTrue();
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("1,2", 2)]
    [InlineData("1,2,3", 3)]
    [InlineData("1,2,3,4,5", 5)]
    [InlineData("", 0)]
    public void Mix_SoundIds_ShouldSupportCommaSeparatedValues(string soundIds, int expectedCount)
    {
        // Arrange
        var mix = new Mix { SoundIds = soundIds };

        // Act
        var ids = string.IsNullOrEmpty(soundIds) 
            ? Array.Empty<string>() 
            : soundIds.Split(',');

        // Assert
        ids.Length.Should().Be(expectedCount);
    }

    [Fact]
    public void Mix_Favorite_ShouldToggle()
    {
        // Arrange
        var mix = new Mix { IsFavorite = false };

        // Act & Assert
        mix.IsFavorite = true;
        mix.IsFavorite.Should().BeTrue();

        mix.IsFavorite = false;
        mix.IsFavorite.Should().BeFalse();
    }

    [Fact]
    public void Mix_DatabaseAttributes_ShouldBePresent()
    {
        // Act
        var type = typeof(Mix);
        var attributes = type.GetCustomAttributes(typeof(SQLite.TableAttribute), false);

        // Assert
        attributes.Should().NotBeEmpty("Mix should have Table attribute for SQLite");
    }

    [Fact]
    public void Mix_PrimaryKey_ShouldHaveAutoIncrementAttribute()
    {
        // Act
        var property = typeof(Mix).GetProperty("Id");
        var primaryKeyAttr = property?.GetCustomAttributes(typeof(SQLite.PrimaryKeyAttribute), false);
        var autoIncrementAttr = property?.GetCustomAttributes(typeof(SQLite.AutoIncrementAttribute), false);

        // Assert
        primaryKeyAttr.Should().NotBeEmpty("Id should have PrimaryKey attribute");
        autoIncrementAttr.Should().NotBeEmpty("Id should have AutoIncrement attribute");
    }
}

public class PlayingSoundTests
{
    [Fact]
    public void PlayingSound_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var playingSound = new PlayingSound();

        // Assert
        playingSound.Sound.Should().BeNull();
        playingSound.Volume.Should().Be(1.0f);
        playingSound.IsPlaying.Should().BeFalse();
    }

    [Fact]
    public void PlayingSound_SetProperties_ShouldStoreValues()
    {
        // Arrange
        var playingSound = new PlayingSound();
        var sound = new Sound { Id = 1, Name = "Rain" };

        // Act
        playingSound.Sound = sound;
        playingSound.Volume = 0.5f;
        playingSound.IsPlaying = true;

        // Assert
        playingSound.Sound.Should().BeSameAs(sound);
        playingSound.Volume.Should().Be(0.5f);
        playingSound.IsPlaying.Should().BeTrue();
    }

    [Fact]
    public void PlayingSound_ObjectInitializer_ShouldSetProperties()
    {
        // Arrange
        var sound = new Sound { Id = 2, Name = "Ocean" };

        // Act
        var playingSound = new PlayingSound
        {
            Sound = sound,
            Volume = 0.75f,
            IsPlaying = true
        };

        // Assert
        playingSound.Sound.Name.Should().Be("Ocean");
        playingSound.Volume.Should().Be(0.75f);
        playingSound.IsPlaying.Should().BeTrue();
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(0.25f)]
    [InlineData(0.5f)]
    [InlineData(0.75f)]
    [InlineData(1.0f)]
    public void PlayingSound_Volume_ShouldAcceptValidRange(float volume)
    {
        // Arrange
        var playingSound = new PlayingSound();

        // Act
        playingSound.Volume = volume;

        // Assert
        playingSound.Volume.Should().Be(volume);
        playingSound.Volume.Should().BeInRange(0f, 1f);
    }

    [Fact]
    public void PlayingSound_IsPlaying_ShouldToggle()
    {
        // Arrange
        var playingSound = new PlayingSound { IsPlaying = false };

        // Act & Assert
        playingSound.IsPlaying = true;
        playingSound.IsPlaying.Should().BeTrue();

        playingSound.IsPlaying = false;
        playingSound.IsPlaying.Should().BeFalse();
    }

    [Fact]
    public void PlayingSound_WithSound_ShouldMaintainReference()
    {
        // Arrange
        var sound = new Sound { Id = 3, Name = "Forest", Category = "Nature" };
        var playingSound = new PlayingSound { Sound = sound, Volume = 0.8f };

        // Act & Assert
        playingSound.Sound.Id.Should().Be(3);
        playingSound.Sound.Name.Should().Be("Forest");
        playingSound.Sound.Category.Should().Be("Nature");
    }
}

public class UserPreferencesTests
{
    [Fact]
    public void UserPreferences_DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var prefs = new UserPreferences();

        // Assert
        prefs.Id.Should().Be(1, "Default ID should be 1");
        prefs.IsPremium.Should().BeFalse();
        prefs.PremiumExpiryDate.Should().BeNull();
        prefs.AdsEnabled.Should().BeTrue();
        prefs.DefaultTimerMinutes.Should().Be(30);
        prefs.DefaultVolume.Should().Be(0.7f);
    }

    [Fact]
    public void UserPreferences_SetProperties_ShouldStoreValues()
    {
        // Arrange
        var prefs = new UserPreferences();
        var expiryDate = DateTime.UtcNow.AddMonths(1);

        // Act
        prefs.Id = 1;
        prefs.IsPremium = true;
        prefs.PremiumExpiryDate = expiryDate;
        prefs.AdsEnabled = false;
        prefs.DefaultTimerMinutes = 60;
        prefs.DefaultVolume = 0.9f;

        // Assert
        prefs.IsPremium.Should().BeTrue();
        prefs.PremiumExpiryDate.Should().Be(expiryDate);
        prefs.AdsEnabled.Should().BeFalse();
        prefs.DefaultTimerMinutes.Should().Be(60);
        prefs.DefaultVolume.Should().Be(0.9f);
    }

    [Fact]
    public void UserPreferences_ObjectInitializer_ShouldSetProperties()
    {
        // Arrange
        var expiry = DateTime.UtcNow.AddYears(1);

        // Act
        var prefs = new UserPreferences
        {
            IsPremium = true,
            PremiumExpiryDate = expiry,
            AdsEnabled = false,
            DefaultTimerMinutes = 45,
            DefaultVolume = 0.85f
        };

        // Assert
        prefs.IsPremium.Should().BeTrue();
        prefs.PremiumExpiryDate.Should().Be(expiry);
        prefs.AdsEnabled.Should().BeFalse();
        prefs.DefaultTimerMinutes.Should().Be(45);
        prefs.DefaultVolume.Should().Be(0.85f);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(15)]
    [InlineData(30)]
    [InlineData(60)]
    [InlineData(120)]
    public void UserPreferences_DefaultTimerMinutes_ShouldAcceptVariousValues(int minutes)
    {
        // Arrange
        var prefs = new UserPreferences();

        // Act
        prefs.DefaultTimerMinutes = minutes;

        // Assert
        prefs.DefaultTimerMinutes.Should().Be(minutes);
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(0.3f)]
    [InlineData(0.5f)]
    [InlineData(0.7f)]
    [InlineData(1.0f)]
    public void UserPreferences_DefaultVolume_ShouldAcceptValidRange(float volume)
    {
        // Arrange
        var prefs = new UserPreferences();

        // Act
        prefs.DefaultVolume = volume;

        // Assert
        prefs.DefaultVolume.Should().Be(volume);
        prefs.DefaultVolume.Should().BeInRange(0f, 1f);
    }

    [Fact]
    public void UserPreferences_PremiumExpiryDate_ShouldBeNullable()
    {
        // Arrange
        var prefs = new UserPreferences();

        // Act & Assert - Can be null
        prefs.PremiumExpiryDate = null;
        prefs.PremiumExpiryDate.Should().BeNull();

        // Can be set to a date
        var date = DateTime.UtcNow;
        prefs.PremiumExpiryDate = date;
        prefs.PremiumExpiryDate.Should().Be(date);
    }

    [Fact]
    public void UserPreferences_IsPremium_AdsEnabled_ShouldBeIndependent()
    {
        // Arrange
        var prefs = new UserPreferences();

        // Act - Set premium true, ads should not automatically change
        prefs.IsPremium = true;

        // Assert
        prefs.IsPremium.Should().BeTrue();
        prefs.AdsEnabled.Should().BeTrue("AdsEnabled should remain independent");

        // Act - Explicitly disable ads
        prefs.AdsEnabled = false;

        // Assert
        prefs.AdsEnabled.Should().BeFalse();
    }

    [Fact]
    public void UserPreferences_DatabaseAttributes_ShouldBePresent()
    {
        // Act
        var type = typeof(UserPreferences);
        var attributes = type.GetCustomAttributes(typeof(SQLite.TableAttribute), false);

        // Assert
        attributes.Should().NotBeEmpty("UserPreferences should have Table attribute for SQLite");
    }

    [Fact]
    public void UserPreferences_PrimaryKey_ShouldBeOnId()
    {
        // Act
        var property = typeof(UserPreferences).GetProperty("Id");
        var primaryKeyAttr = property?.GetCustomAttributes(typeof(SQLite.PrimaryKeyAttribute), false);

        // Assert
        primaryKeyAttr.Should().NotBeEmpty("Id should have PrimaryKey attribute");
    }

    [Fact]
    public void UserPreferences_FreeToPremiumTransition_ShouldUpdateCorrectly()
    {
        // Arrange - Free user
        var prefs = new UserPreferences
        {
            IsPremium = false,
            PremiumExpiryDate = null,
            AdsEnabled = true
        };

        // Act - Upgrade to premium
        prefs.IsPremium = true;
        prefs.PremiumExpiryDate = DateTime.UtcNow.AddYears(1);
        prefs.AdsEnabled = false;

        // Assert
        prefs.IsPremium.Should().BeTrue();
        prefs.PremiumExpiryDate.Should().NotBeNull();
        prefs.AdsEnabled.Should().BeFalse();
    }

    [Fact]
    public void UserPreferences_PremiumToFreeTransition_ShouldUpdateCorrectly()
    {
        // Arrange - Premium user
        var prefs = new UserPreferences
        {
            IsPremium = true,
            PremiumExpiryDate = DateTime.UtcNow.AddYears(1),
            AdsEnabled = false
        };

        // Act - Revert to free (expiry)
        prefs.IsPremium = false;
        prefs.PremiumExpiryDate = null;
        prefs.AdsEnabled = true;

        // Assert
        prefs.IsPremium.Should().BeFalse();
        prefs.PremiumExpiryDate.Should().BeNull();
        prefs.AdsEnabled.Should().BeTrue();
    }
}
