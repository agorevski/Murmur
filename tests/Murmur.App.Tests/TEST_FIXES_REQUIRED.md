# Test Fixes Required

Due to mismatches between the test assumptions and actual implementation, the following fixes are needed:

## Critical Issues Found

### 1. ViewModels require IAnalyticsService parameter
All ViewModels inherit from BaseViewModel which requires IAnalyticsService.

**Fix**: Add Mock<IAnalyticsService> to all ViewModel test constructors.

### 2. SettingsViewModel uses UserPreferences property
The actual SettingsViewModel has a `Preferences` property of type `UserPreferences`, not individual properties.

**Fix**: Access settings via `viewModel.Preferences.PropertyName` instead of `viewModel.PropertyName`.

### 3. Sound model uses FileName not FilePath
The Sound model has `FileName` property, not `FilePath`.

**Fix**: Replace all `Sound.FilePath` with `Sound.FileName`.

### 4. Mix model structure different
Mix model has `SoundIds` property storing IDs, not a `Sounds` collection.

**Fix**: Tests should work with Mix.SoundIds instead of Mix.Sounds.

### 5. MixerViewModel properties don't match
MixerViewModel doesn't have `HasActiveSounds` property in tests.

**Fix**: Check ActiveSounds.Count > 0 instead of HasActiveSounds.

### 6. MockDataService missing methods
MockDataService doesn't implement SetUserPreferences, SetFavoriteMixes.

**Fix**: Use proper mocking setup with Moq instead of custom methods.

### 7. FluentAssertions Or() chaining issue
Using `.Or.EndWith()` syntax incorrectly.

**Fix**: Use separate Should() statements or proper chaining.

### 8. UI Test BackgroundApp parameter
Driver.BackgroundApp expects TimeSpan, not int.

**Fix**: Change `Driver.BackgroundApp(5)` to `Driver.BackgroundApp(TimeSpan.FromSeconds(5))`.

## Recommended Approach

Given the scope of fixes needed (111 errors across multiple files), I recommend:

### Option A: Simplified Test Suite
Create a new, simpler test suite that:
1. Focuses on critical business logic only
2. Uses actual implementation signatures
3. Tests public interfaces without mocking internals

### Option B: Fix Tests Incrementally  
1. Fix BaseViewModel tests first (foundation)
2. Fix Model tests (no dependencies)
3. Fix Converter tests (no dependencies)
4. Fix one ViewModel at a time
5. Fix Service tests
6. Fix UI tests last

### Option C: Use Test Templates
Use the templates in REMAINING_TESTS_TEMPLATES.md and implement tests incrementally as features are added, ensuring they match actual implementation.

## Quick Win: Run Tests That Don't Depend on ViewModels

The following test files should compile with minimal fixes:
- Models/ModelTests.cs (needs Sound.FilePath → Sound.FileName fix)
- Converters/ValueConvertersTests.cs (should work as-is)
- Services/SoundLibraryServiceTests.cs (needs FluentAssertions fix)

## Immediate Action Items

1. **Add IAnalyticsService to all ViewModel test constructors**
2. **Fix Model property references** (FilePath → FileName, etc.)
3. **Update SettingsViewModel tests** to use Preferences property
4. **Fix MockDataService** to use proper Moq Setup/Returns
5. **Update MixerViewModel tests** to check ActiveSounds.Count
6. **Fix FluentAssertions syntax** errors

## Estimated Effort

- Quick fixes (Models, Converters): 30 minutes
- ViewModel fixes: 2-3 hours
- Service test fixes: 1 hour
- UI test fixes: 30 minutes

**Total**: ~4-5 hours to fix all 111 errors

## Alternative: Minimal Test Suite

If time is constrained, create a minimal but passing test suite:
- 20 Model tests (core business logic)
- 20 Converter tests (UI logic)
- 30 ViewModel tests (critical paths only)
- 10 Service tests (key operations)
- 10 UI tests (smoke tests)

**Total**: ~90 passing tests in ~2 hours

This would provide good coverage while ensuring all tests pass.
