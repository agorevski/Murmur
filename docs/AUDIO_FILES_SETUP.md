# Adding Audio Files to Murmur/Driftly App

## Problem
The app is trying to play audio files that don't exist yet in the project. When you tap sound buttons like "Rain", "Ocean Waves", or "Forest", you'll see an error message indicating the audio files are missing.

## Solution: Add Audio Files

### Step 1: Create the Sounds Directory
1. Navigate to `Resources/Raw/` in your project
2. Create a new folder named `Sounds`
3. Final path should be: `Resources/Raw/Sounds/`

### Step 2: Obtain Audio Files
You need to obtain or create the following MP3 files:

**Free Sounds (Required for basic functionality):**
- `rain.mp3` - Gentle rain sounds
- `ocean.mp3` - Calming ocean waves
- `forest.mp3` - Forest ambience with birds

**Premium Sounds (Optional):**
- `thunder.mp3` - Distant thunder with rain
- `fire.mp3` - Crackling fireplace
- `whitenoise.mp3` - Pure white noise
- `birds.mp3` - Morning birds chirping
- `wind.mp3` - Gentle wind through trees
- `stream.mp3` - Flowing stream water
- `crickets.mp3` - Peaceful cricket sounds

### Step 3: Audio File Requirements
- **Format**: MP3
- **Sample Rate**: 44.1kHz (recommended)
- **Bit Rate**: 128-192 kbps (balance quality/size)
- **Length**: 2-5 minutes minimum (will loop seamlessly)
- **Normalization**: Audio should be normalized to -3dB to -6dB
- **Gapless**: Ensure the end connects smoothly to the beginning for looping

### Step 4: Add Files to Project

#### Using Visual Studio:
1. Right-click on `Resources/Raw/Sounds/` folder
2. Select "Add" → "Existing Item"
3. Select your MP3 files
4. For each file, right-click → "Properties"
5. Set "Build Action" to "MauiAsset"

#### Using VS Code:
1. Copy your MP3 files to `Resources/Raw/Sounds/`
2. Edit `Murmur.App.csproj` and add:
```xml
<ItemGroup>
    <MauiAsset Include="Resources\Raw\Sounds\*.mp3" />
</ItemGroup>
```

### Step 5: Verify Setup
1. Rebuild the project: `dotnet build`
2. Deploy to Android device/emulator
3. Tap a sound button (e.g., "Rain")
4. You should hear the audio playing

## Where to Find/Create Audio Files

### Free Sources:
- **Freesound.org** - Community uploaded sounds (check licenses)
- **Zapsplat.com** - Free sound effects (attribution required)
- **BBC Sound Effects** - Free for personal/education use
- **YouTube Audio Library** - Free royalty-free sounds

### Paid/Premium Sources:
- **AudioJungle** - High-quality ambient sounds
- **Pond5** - Professional sound library
- **Epidemic Sound** - Subscription-based

### DIY Recording:
- Record your own ambient sounds with a smartphone
- Use audio editing software (Audacity, free) to:
  - Trim and loop
  - Normalize volume
  - Remove background noise
  - Export as MP3

## Audio Optimization Tips

### Using Audacity (Free):
1. Import your audio file
2. **Normalize**: Effect → Normalize (-3.0 dB)
3. **Fade**: Add 1-2 second fade in/out for smooth looping
4. **Export**: File → Export → Export as MP3
   - Quality: 160-192 kbps
   - Joint Stereo

### File Size Considerations:
- 3-minute MP3 at 128 kbps ≈ 3 MB
- 3-minute MP3 at 192 kbps ≈ 4.5 MB
- Total for 10 sounds ≈ 30-45 MB
- Keep individual files under 5 MB for mobile

## Licensing Considerations

**Important**: Ensure you have the right to use the audio files:
- Commercial use license (if selling the app)
- Attribution requirements (credit the creator)
- Distribution rights (can you include in app?)
- Modification rights (can you edit the sounds?)

Add licensing information to your app's credits/about section.

## Testing After Setup

After adding files, test:
1. ✅ Sounds load without errors
2. ✅ Audio plays when button tapped
3. ✅ Volume controls work
4. ✅ Fade in/out is smooth
5. ✅ Looping is seamless (no gap/click)
6. ✅ Multiple sounds can play simultaneously
7. ✅ "Stop All" button works

## Troubleshooting

### "Audio file not found" error persists:
- Verify files are in `Resources/Raw/Sounds/` folder
- Check file names match exactly (case-sensitive on some systems)
- Confirm Build Action is set to "MauiAsset"
- Clean and rebuild: `dotnet clean && dotnet build`

### No sound plays:
- Check device volume is up
- Test with headphones
- Verify audio files aren't corrupted (play in media player)
- Check Android permissions in manifest

### Poor loop quality:
- Audio file doesn't fade properly at end
- Add fade out in last second of file
- Add fade in to first second of file
- Ensure no silence at beginning or end

## Quick Start for Testing

If you want to test quickly without full audio production:

1. Download 3 free ambient sound samples
2. Convert to MP3 if needed
3. Rename to `rain.mp3`, `ocean.mp3`, `forest.mp3`
4. Copy to `Resources/Raw/Sounds/`
5. Rebuild and test

This will let you test all app functionality with just the free tier sounds.
