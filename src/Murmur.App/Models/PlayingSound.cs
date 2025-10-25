namespace Murmur.App.Models;

public class PlayingSound
{
    public Sound Sound { get; set; } = null!;
    public float Volume { get; set; } = 1.0f;
    public bool IsPlaying { get; set; }
}
