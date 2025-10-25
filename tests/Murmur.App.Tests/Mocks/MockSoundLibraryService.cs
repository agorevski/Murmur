using Moq;
using Murmur.App.Models;
using Murmur.App.Services;

namespace Murmur.App.Tests.Mocks;

public class MockSoundLibraryService
{
    public Mock<ISoundLibraryService> Mock { get; }

    public MockSoundLibraryService()
    {
        Mock = new Mock<ISoundLibraryService>();
        SetupDefaultBehavior();
    }

    private void SetupDefaultBehavior()
    {
        var allSounds = GetTestSounds();
        var freeSounds = allSounds.Where(s => !s.IsPremium).ToList();

        Mock.Setup(x => x.GetAllSoundsAsync())
            .ReturnsAsync(allSounds);

        Mock.Setup(x => x.GetFreeSoundsAsync())
            .ReturnsAsync(freeSounds);

        Mock.Setup(x => x.GetSoundByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => allSounds.FirstOrDefault(s => s.Id == id));
    }

    public List<Sound> GetTestSounds()
    {
        return new List<Sound>
        {
            new Sound
            {
                Id = 1,
                Name = "Rain",
                Description = "Gentle rain sounds",
                FileName = "rain.mp3",
                Category = "Nature",
                IsPremium = false,
                IconUrl = ""
            },
            new Sound
            {
                Id = 2,
                Name = "Ocean",
                Description = "Ocean waves",
                FileName = "ocean.mp3",
                Category = "Nature",
                IsPremium = false,
                IconUrl = ""
            },
            new Sound
            {
                Id = 3,
                Name = "Forest",
                Description = "Forest ambience",
                FileName = "forest.mp3",
                Category = "Nature",
                IsPremium = false,
                IconUrl = ""
            },
            new Sound
            {
                Id = 4,
                Name = "Thunder",
                Description = "Thunder storm",
                FileName = "thunder.mp3",
                Category = "Nature",
                IsPremium = true,
                IconUrl = ""
            }
        };
    }

    public Sound GetRainSound()
    {
        return GetTestSounds().First(s => s.Name == "Rain");
    }

    public Sound GetOceanSound()
    {
        return GetTestSounds().First(s => s.Name == "Ocean");
    }

    public Sound GetThunderSound()
    {
        return GetTestSounds().First(s => s.Name == "Thunder");
    }
}
