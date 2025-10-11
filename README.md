# Murmur - Driftly Relaxation App

A calming sound mixer app built with .NET MAUI for Android.

## Features

### Screens
- **Home**: Browse and play ambient sounds with live mixing
- **Mixer**: Fine-tune volume levels for each active sound
- **Favorites**: Save and manage your favorite sound combinations
- **Premium**: Unlock premium features with subscription
- **Settings**: Customize app preferences and view info

### Core Functionality
- Multi-sound playback with smooth fade in/out effects
- Gapless looping for continuous ambient experience
- Sleep timer with customizable duration
- Save custom mixes to favorites
- SQLite database for persistent storage

### Free vs Premium
**Free Tier:**
- 3 free ambient sounds (Rain, Ocean Waves, Forest)
- Mix up to 3 sounds simultaneously
- Ad-supported

**Premium Tier:**
- Remove all advertisements
- Access to all 10+ premium sounds
- Unlimited sound mixing
- Offline playback support
- Advanced timer settings

## Architecture

### MVVM Pattern
- **Models**: Sound, Mix, UserPreferences, PlayingSound
- **ViewModels**: Home, Mixer, Favorites, Premium, Settings
- **Views**: XAML pages with data binding

### Services
- **AudioService**: Multi-track audio playback with volume control and fade effects
- **DataService**: SQLite database operations
- **SoundLibraryService**: JSON-based sound library management
- **AdService**: AdMob integration (placeholder for actual implementation)
- **BillingService**: Google Play Billing integration (placeholder)
- **AnalyticsService**: Firebase Analytics integration (placeholder)

### Technology Stack
- **.NET 9.0** with C# 12
- **.NET MAUI** for cross-platform UI
- **CommunityToolkit.Mvvm** for MVVM helpers
- **Plugin.Maui.Audio** for audio playback
- **SQLite** for local data storage
- **Newtonsoft.Json** for JSON parsing

## Sound Library
The app includes a curated collection of ambient sounds:
- Nature sounds (Rain, Ocean, Forest, Wind, Stream, etc.)
- Indoor ambience (Fireplace)
- Focus sounds (White Noise)
- Wildlife (Birds, Crickets)

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- .NET MAUI workload for Android

### Build
```bash
dotnet build
```

### Run on Android
```bash
dotnet build -t:Run -f net9.0-android
```

## Project Structure
```
Murmur.App/
├── Models/          # Data models
├── ViewModels/      # MVVM view models
├── Views/           # XAML pages
├── Services/        # Business logic services
├── Converters/      # Value converters for XAML
├── Resources/       # Images, fonts, sounds, styles
└── Data/            # SQLite database
```

## Configuration

### External Services (To Be Implemented)
- **Firebase**: Analytics and crash reporting
- **AdMob**: Banner and interstitial ads
- **Google Play Billing**: In-app purchases and subscriptions

These require API keys and configuration files:
- `google-services.json` for Firebase
- AdMob App ID in AndroidManifest.xml
- Play Billing Library setup

## License
Proprietary - All rights reserved

## Version
1.0.0
