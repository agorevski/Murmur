using Moq;
using Murmur.App.Models;
using Murmur.App.Services;

namespace Murmur.App.Tests.Mocks;

public class MockAudioService
{
    public Mock<IAudioService> Mock { get; }
    private readonly Dictionary<int, bool> _playingSounds = new();

    public MockAudioService()
    {
        Mock = new Mock<IAudioService>();
        SetupDefaultBehavior();
    }

    private void SetupDefaultBehavior()
    {
        // PlaySoundAsync - simulates successful sound playback
        Mock.Setup(x => x.PlaySoundAsync(It.IsAny<Sound>(), It.IsAny<float>()))
            .ReturnsAsync((Sound sound, float volume) =>
            {
                _playingSounds[sound.Id] = true;
                return true;
            });

        // StopSoundAsync - simulates stopping a sound
        Mock.Setup(x => x.StopSoundAsync(It.IsAny<int>()))
            .Returns((int soundId) =>
            {
                _playingSounds.Remove(soundId);
                return Task.CompletedTask;
            });

        // StopAllAsync - simulates stopping all sounds
        Mock.Setup(x => x.StopAllAsync())
            .Returns(() =>
            {
                _playingSounds.Clear();
                return Task.CompletedTask;
            });

        // SetVolumeAsync
        Mock.Setup(x => x.SetVolumeAsync(It.IsAny<int>(), It.IsAny<float>()))
            .Returns(Task.CompletedTask);

        // FadeInAsync
        Mock.Setup(x => x.FadeInAsync(It.IsAny<int>(), It.IsAny<float>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        // FadeOutAsync
        Mock.Setup(x => x.FadeOutAsync(It.IsAny<int>(), It.IsAny<int>()))
            .Returns((int soundId, int duration) =>
            {
                _playingSounds.Remove(soundId);
                return Task.CompletedTask;
            });

        // IsSoundPlaying
        Mock.Setup(x => x.IsSoundPlaying(It.IsAny<int>()))
            .Returns((int soundId) => _playingSounds.ContainsKey(soundId) && _playingSounds[soundId]);

        // GetPlayingSoundIds
        Mock.Setup(x => x.GetPlayingSoundIds())
            .Returns(() => _playingSounds.Keys.ToList());
    }

    public void Reset()
    {
        _playingSounds.Clear();
        Mock.Reset();
        SetupDefaultBehavior();
    }

    public void VerifyPlaySoundCalled(Sound sound, Times times)
    {
        Mock.Verify(x => x.PlaySoundAsync(
            It.Is<Sound>(s => s.Id == sound.Id),
            It.IsAny<float>()), times);
    }

    public void VerifyStopAllCalled(Times times)
    {
        Mock.Verify(x => x.StopAllAsync(), times);
    }

    public void VerifyFadeInCalled(int soundId, Times times)
    {
        Mock.Verify(x => x.FadeInAsync(soundId, It.IsAny<float>(), It.IsAny<int>()), times);
    }

    public void VerifyFadeOutCalled(int soundId, Times times)
    {
        Mock.Verify(x => x.FadeOutAsync(soundId, It.IsAny<int>()), times);
    }
}
