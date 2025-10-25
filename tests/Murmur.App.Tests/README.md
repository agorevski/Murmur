# Murmur App - Unit Tests

This test project contains comprehensive unit tests for the Murmur sound mixing application, focusing on UX user scenarios and ViewModel logic.

## Test Coverage

### UX User Test Scenarios ✅

The following tests verify the exact user experience flows you requested:

1. **UX_Test_OpenApp_ClickRainButton_SoundPlays_StopAllEnabled**
   - Opens the app
   - Verifies Stop-All button is initially disabled
   - Clicks the 'Rain' button
   - Verifies sound plays via AudioService
   - Verifies Stop-All button becomes enabled

2. **UX_Test_ClickStopAll_ButtonGreysOut_AndStopsRainSounds**
   - Plays Rain sound
   - Verifies Stop-All is enabled
   - Clicks Stop-All button
   - Verifies button becomes greyed out (disabled)
   - Verifies all sounds stop

3. **UX_Test_FullWorkflow_OpenApp_PlayRain_StopAll**
   - Complete end-to-end workflow test
   - Covers: App open → Play Rain → Stop-All → Verify states

### HomeViewModel Tests ✅

**Sound Toggle Tests (4 tests)**
- Playing and stopping individual sounds
- Playing multiple sounds simultaneously
- Free user sound limit (3 sounds max)

**Stop-All Button State Tests (3 tests)**
- Button disabled when no sounds playing
- Button enabled when sounds are playing
- Button disabled after stopping last sound

**Stop-All Functionality Tests (3 tests)**
- Stopping multiple sounds
- Clearing PlayingSounds collection
- Disabling Stop-All button after use

**Integration Tests (2 tests)**
- Full workflow: Play → Stop → Play again
- LoadData stops any existing sounds

**Analytics Tests (2 tests)**
- Tracking sound toggle events
- Tracking stop-all events

## Test Statistics

- **Total Tests**: 17
- **Passing**: 17 ✅
- **Coverage**: HomeViewModel sound playback and Stop-All functionality

## Running the Tests

### From Command Line

```bash
# Run all tests
dotnet test Murmur.App.Tests/Murmur.App.Tests.csproj

# Run with detailed output
dotnet test Murmur.App.Tests/Murmur.App.Tests.csproj --verbosity normal

# Run specific test
dotnet test Murmur.App.Tests/Murmur.App.Tests.csproj --filter "FullyQualifiedName~UX_Test"
```

### From Visual Studio

1. Open Test Explorer (Test → Test Explorer)
2. Click "Run All" to execute all tests
3. Individual tests can be run by right-clicking and selecting "Run"

### From Visual Studio Code

1. Install the .NET Core Test Explorer extension
2. Tests will appear in the Test Explorer sidebar
3. Click the play button to run tests

## Test Architecture

### Mock Services

The tests use mock implementations of services to isolate ViewModel logic:

- **MockAudioService**: Simulates audio playback without actual audio
- **MockSoundLibraryService**: Provides test sound data
- **MockDataService**: Simulates user preferences and premium status

### Testing Framework

- **xUnit**: Primary testing framework
- **Moq**: Mocking framework for service dependencies
- **FluentAssertions**: Readable assertion syntax

## Key Test Scenarios Covered

### 1. Button State Management
✅ Stop-All button properly enables/disables based on IsPlaying property
✅ Button state reflects whether sounds are currently playing

### 2. Sound Playback
✅ AudioService methods are called correctly
✅ PlayingSounds collection updates properly
✅ Fade in/out effects are applied

### 3. User Experience
✅ Complete user workflows verified
✅ State transitions work correctly
✅ Analytics events tracked properly

## Notes

- Tests use mocked services and don't require actual audio files
- Tests run quickly without UI or device dependencies
- Shell.Current-dependent scenarios (like alert dialogs) are tested up to the ViewModel boundary
- All tests are isolated and can run in any order

## Continuous Integration

These tests are designed to run in CI/CD pipelines:

```yaml
# Example CI configuration
- name: Run Tests
  run: dotnet test Murmur.App.Tests/Murmur.App.Tests.csproj --configuration Release
```

## Contributing

When adding new features to HomeViewModel:

1. Add corresponding unit tests
2. Follow the existing test naming convention
3. Use the mock services provided
4. Ensure tests are isolated and don't depend on execution order
5. Run all tests before committing

## Test Results

Last test run: ✅ **17/17 tests passed**

```
Test summary: total: 17, failed: 0, succeeded: 17, skipped: 0
