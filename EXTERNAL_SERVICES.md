# External Services Setup Guide

This document explains how to configure external services for the Driftly app.

## Firebase Setup

### Prerequisites
1. Create a Firebase project at [Firebase Console](https://console.firebase.google.com/)
2. Register your Android app with package name: `com.driftly.murmur`

### Configuration Steps
1. Download `google-services.json` from Firebase Console
2. Place it in `Platforms/Android/` directory
3. Add Firebase NuGet packages:
   ```bash
   dotnet add package Xamarin.Google.Android.Play.Services.Base
   dotnet add package Xamarin.Firebase.Analytics
   ```
4. Update `AnalyticsService.cs` to use actual Firebase SDK

### Firebase Features
- **Analytics**: Track user events and screen views
- **Crashlytics**: Monitor app crashes and errors
- **Remote Config**: Manage feature flags and dynamic content

## AdMob Integration

### Prerequisites
1. Create AdMob account at [AdMob Console](https://admob.google.com/)
2. Create ad units for:
   - Banner ads (home screen)
   - Interstitial ads (between sessions)
   - Rewarded ads (premium features trial)

### Configuration Steps
1. Get your AdMob App ID from AdMob Console
2. Update `AndroidManifest.xml` with your App ID:
   ```xml
   <meta-data
       android:name="com.google.android.gms.ads.APPLICATION_ID"
       android:value="ca-app-pub-XXXXXXXXXXXXXXXX~YYYYYYYYYY"/>
   ```
3. Add Google Mobile Ads NuGet package:
   ```bash
   dotnet add package Xamarin.Google.Android.Play.Services.Ads
   ```
4. Update `AdService.cs` with actual AdMob implementation
5. Get ad unit IDs for each ad type and add to configuration

### Ad Types
- **Banner**: Bottom of home screen (free users only)
- **Interstitial**: After 3 sound mix sessions
- **Rewarded**: Optional ad for temporary premium features

## Google Play Billing

### Prerequisites
1. Set up Google Play Console account
2. Create in-app products:
   - Premium subscription (monthly): `premium_monthly`
   - Premium subscription (yearly): `premium_yearly` (optional)

### Configuration Steps
1. Add Google Play Billing NuGet package:
   ```bash
   dotnet add package Xamarin.Google.Android.Play.Billing
   ```
2. Configure in-app products in Play Console
3. Update `BillingService.cs` with Play Billing Library v5+
4. Implement purchase flow and subscription management
5. Set up server-side receipt validation (recommended)

### Subscription Details
- **Product ID**: `premium_monthly`
- **Price**: $4.99/month (adjust per region)
- **Benefits**: 
  - No ads
  - All sounds unlocked
  - Unlimited mixing
  - Offline mode

## Testing

### Firebase Testing
- Use Firebase Console to verify events
- Test in Debug mode with Firebase DebugView

### AdMob Testing
- Use test ad unit IDs during development:
  ```
  Banner: ca-app-pub-3940256099942544/6300978111
  Interstitial: ca-app-pub-3940256099942544/1033173712
  Rewarded: ca-app-pub-3940256099942544/5224354917
  ```
- Add test device IDs to prevent policy violations

### Billing Testing
- Use Google Play Console test tracks (Internal/Alpha/Beta)
- Add test accounts for license testing
- Test subscription lifecycle (purchase, cancel, renew)

## Security Best Practices

1. **Never commit sensitive keys** to source control
2. Use Android keystore for release builds
3. Implement server-side receipt validation for billing
4. Enable ProGuard/R8 for code obfuscation
5. Use Firebase App Check to prevent API abuse
6. Implement certificate pinning for API calls

## Deployment Checklist

- [ ] Add `google-services.json` to Android project
- [ ] Configure AdMob App ID in AndroidManifest.xml
- [ ] Replace test ad units with production units
- [ ] Configure Play Billing products
- [ ] Test all integrations thoroughly
- [ ] Enable ProGuard/R8 in Release configuration
- [ ] Sign APK/AAB with release keystore
- [ ] Test on multiple Android devices/versions
- [ ] Submit to Google Play Console

## Support

For implementation help:
- Firebase: https://firebase.google.com/docs
- AdMob: https://developers.google.com/admob
- Play Billing: https://developer.android.com/google/play/billing

## Notes

The current implementation includes placeholder services that need to be replaced with actual SDK integrations. All external service implementations require:
1. NuGet packages installed
2. Configuration files added
3. Service classes updated with real implementations
4. Proper error handling and edge cases
5. User privacy compliance (GDPR, CCPA, etc.)
