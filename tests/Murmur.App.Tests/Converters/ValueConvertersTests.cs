using FluentAssertions;
using Murmur.App.Converters;
using System.Globalization;

namespace Murmur.App.Tests.Converters;

public class ValueConvertersTests
{
    private readonly CultureInfo _culture = CultureInfo.InvariantCulture;

    #region InverseBoolConverter Tests

    [Fact]
    public void InverseBoolConverter_Convert_TrueValue_ShouldReturnFalse()
    {
        // Arrange
        var converter = new InverseBoolConverter();

        // Act
        var result = converter.Convert(true, typeof(bool), null, _culture);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public void InverseBoolConverter_Convert_FalseValue_ShouldReturnTrue()
    {
        // Arrange
        var converter = new InverseBoolConverter();

        // Act
        var result = converter.Convert(false, typeof(bool), null, _culture);

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public void InverseBoolConverter_Convert_NullValue_ShouldReturnFalse()
    {
        // Arrange
        var converter = new InverseBoolConverter();

        // Act
        var result = converter.Convert(null, typeof(bool), null, _culture);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public void InverseBoolConverter_Convert_NonBoolValue_ShouldReturnFalse()
    {
        // Arrange
        var converter = new InverseBoolConverter();

        // Act
        var result = converter.Convert("not a bool", typeof(bool), null, _culture);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public void InverseBoolConverter_ConvertBack_TrueValue_ShouldReturnFalse()
    {
        // Arrange
        var converter = new InverseBoolConverter();

        // Act
        var result = converter.ConvertBack(true, typeof(bool), null, _culture);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public void InverseBoolConverter_ConvertBack_FalseValue_ShouldReturnTrue()
    {
        // Arrange
        var converter = new InverseBoolConverter();

        // Act
        var result = converter.ConvertBack(false, typeof(bool), null, _culture);

        // Assert
        result.Should().Be(true);
    }

    #endregion

    #region IsZeroConverter Tests

    [Fact]
    public void IsZeroConverter_Convert_ZeroValue_ShouldReturnTrue()
    {
        // Arrange
        var converter = new IsZeroConverter();

        // Act
        var result = converter.Convert(0, typeof(bool), null, _culture);

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public void IsZeroConverter_Convert_NonZeroValue_ShouldReturnFalse()
    {
        // Arrange
        var converter = new IsZeroConverter();

        // Act
        var result = converter.Convert(5, typeof(bool), null, _culture);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public void IsZeroConverter_Convert_NegativeValue_ShouldReturnFalse()
    {
        // Arrange
        var converter = new IsZeroConverter();

        // Act
        var result = converter.Convert(-5, typeof(bool), null, _culture);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public void IsZeroConverter_Convert_NullValue_ShouldReturnTrue()
    {
        // Arrange
        var converter = new IsZeroConverter();

        // Act
        var result = converter.Convert(null, typeof(bool), null, _culture);

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public void IsZeroConverter_Convert_NonIntValue_ShouldReturnTrue()
    {
        // Arrange
        var converter = new IsZeroConverter();

        // Act
        var result = converter.Convert("not an int", typeof(bool), null, _culture);

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public void IsZeroConverter_ConvertBack_ShouldThrowNotImplementedException()
    {
        // Arrange
        var converter = new IsZeroConverter();

        // Act
        var act = () => converter.ConvertBack(true, typeof(int), null, _culture);

        // Assert
        act.Should().Throw<NotImplementedException>();
    }

    #endregion

    #region FavoriteIconConverter Tests

    [Fact]
    public void FavoriteIconConverter_Convert_TrueValue_ShouldReturnFilledStar()
    {
        // Arrange
        var converter = new FavoriteIconConverter();

        // Act
        var result = converter.Convert(true, typeof(string), null, _culture);

        // Assert
        result.Should().Be("⭐");
    }

    [Fact]
    public void FavoriteIconConverter_Convert_FalseValue_ShouldReturnEmptyStar()
    {
        // Arrange
        var converter = new FavoriteIconConverter();

        // Act
        var result = converter.Convert(false, typeof(string), null, _culture);

        // Assert
        result.Should().Be("☆");
    }

    [Fact]
    public void FavoriteIconConverter_Convert_NullValue_ShouldReturnEmptyStar()
    {
        // Arrange
        var converter = new FavoriteIconConverter();

        // Act
        var result = converter.Convert(null, typeof(string), null, _culture);

        // Assert
        result.Should().Be("☆");
    }

    [Fact]
    public void FavoriteIconConverter_Convert_NonBoolValue_ShouldReturnEmptyStar()
    {
        // Arrange
        var converter = new FavoriteIconConverter();

        // Act
        var result = converter.Convert("not a bool", typeof(string), null, _culture);

        // Assert
        result.Should().Be("☆");
    }

    [Fact]
    public void FavoriteIconConverter_ConvertBack_ShouldThrowNotImplementedException()
    {
        // Arrange
        var converter = new FavoriteIconConverter();

        // Act
        var act = () => converter.ConvertBack("⭐", typeof(bool), null, _culture);

        // Assert
        act.Should().Throw<NotImplementedException>();
    }

    #endregion

    #region PremiumStatusConverter Tests

    [Fact]
    public void PremiumStatusConverter_Convert_TrueValue_ShouldReturnPremium()
    {
        // Arrange
        var converter = new PremiumStatusConverter();

        // Act
        var result = converter.Convert(true, typeof(string), null, _culture);

        // Assert
        result.Should().Be("✨ Premium");
    }

    [Fact]
    public void PremiumStatusConverter_Convert_FalseValue_ShouldReturnFree()
    {
        // Arrange
        var converter = new PremiumStatusConverter();

        // Act
        var result = converter.Convert(false, typeof(string), null, _culture);

        // Assert
        result.Should().Be("Free");
    }

    [Fact]
    public void PremiumStatusConverter_Convert_NullValue_ShouldReturnFree()
    {
        // Arrange
        var converter = new PremiumStatusConverter();

        // Act
        var result = converter.Convert(null, typeof(string), null, _culture);

        // Assert
        result.Should().Be("Free");
    }

    [Fact]
    public void PremiumStatusConverter_ConvertBack_ShouldThrowNotImplementedException()
    {
        // Arrange
        var converter = new PremiumStatusConverter();

        // Act
        var act = () => converter.ConvertBack("✨ Premium", typeof(bool), null, _culture);

        // Assert
        act.Should().Throw<NotImplementedException>();
    }

    #endregion

    #region PremiumColorConverter Tests

    [Fact]
    public void PremiumColorConverter_Convert_TrueValue_ShouldReturnGold()
    {
        // Arrange
        var converter = new PremiumColorConverter();

        // Act
        var result = converter.Convert(true, typeof(Color), null, _culture);

        // Assert
        result.Should().Be(Colors.Gold);
    }

    [Fact]
    public void PremiumColorConverter_Convert_FalseValue_ShouldReturnGray()
    {
        // Arrange
        var converter = new PremiumColorConverter();

        // Act
        var result = converter.Convert(false, typeof(Color), null, _culture);

        // Assert
        result.Should().Be(Colors.Gray);
    }

    [Fact]
    public void PremiumColorConverter_Convert_NullValue_ShouldReturnGray()
    {
        // Arrange
        var converter = new PremiumColorConverter();

        // Act
        var result = converter.Convert(null, typeof(Color), null, _culture);

        // Assert
        result.Should().Be(Colors.Gray);
    }

    [Fact]
    public void PremiumColorConverter_ConvertBack_ShouldThrowNotImplementedException()
    {
        // Arrange
        var converter = new PremiumColorConverter();

        // Act
        var act = () => converter.ConvertBack(Colors.Gold, typeof(bool), null, _culture);

        // Assert
        act.Should().Throw<NotImplementedException>();
    }

    #endregion

    #region AdsStatusConverter Tests

    [Fact]
    public void AdsStatusConverter_Convert_TrueValue_ShouldReturnEnabled()
    {
        // Arrange
        var converter = new AdsStatusConverter();

        // Act
        var result = converter.Convert(true, typeof(string), null, _culture);

        // Assert
        result.Should().Be("Enabled");
    }

    [Fact]
    public void AdsStatusConverter_Convert_FalseValue_ShouldReturnDisabled()
    {
        // Arrange
        var converter = new AdsStatusConverter();

        // Act
        var result = converter.Convert(false, typeof(string), null, _culture);

        // Assert
        result.Should().Be("Disabled");
    }

    [Fact]
    public void AdsStatusConverter_Convert_NullValue_ShouldReturnEnabled()
    {
        // Arrange
        var converter = new AdsStatusConverter();

        // Act
        var result = converter.Convert(null, typeof(string), null, _culture);

        // Assert
        result.Should().Be("Enabled");
    }

    [Fact]
    public void AdsStatusConverter_ConvertBack_ShouldThrowNotImplementedException()
    {
        // Arrange
        var converter = new AdsStatusConverter();

        // Act
        var act = () => converter.ConvertBack("Enabled", typeof(bool), null, _culture);

        // Assert
        act.Should().Throw<NotImplementedException>();
    }

    #endregion

    #region TimerButtonTextConverter Tests

    [Fact]
    public void TimerButtonTextConverter_Convert_TrueValue_ShouldReturnStopTimer()
    {
        // Arrange
        var converter = new TimerButtonTextConverter();

        // Act
        var result = converter.Convert(true, typeof(string), null, _culture);

        // Assert
        result.Should().Be("Stop Timer");
    }

    [Fact]
    public void TimerButtonTextConverter_Convert_FalseValue_ShouldReturnStartTimer()
    {
        // Arrange
        var converter = new TimerButtonTextConverter();

        // Act
        var result = converter.Convert(false, typeof(string), null, _culture);

        // Assert
        result.Should().Be("Start Timer");
    }

    [Fact]
    public void TimerButtonTextConverter_Convert_NullValue_ShouldReturnStartTimer()
    {
        // Arrange
        var converter = new TimerButtonTextConverter();

        // Act
        var result = converter.Convert(null, typeof(string), null, _culture);

        // Assert
        result.Should().Be("Start Timer");
    }

    [Fact]
    public void TimerButtonTextConverter_ConvertBack_ShouldThrowNotImplementedException()
    {
        // Arrange
        var converter = new TimerButtonTextConverter();

        // Act
        var act = () => converter.ConvertBack("Stop Timer", typeof(bool), null, _culture);

        // Assert
        act.Should().Throw<NotImplementedException>();
    }

    #endregion

    #region Integration Tests

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void InverseBoolConverter_RoundTrip_ShouldReturnOriginalValue(bool originalValue, bool expectedResult)
    {
        // Arrange
        var converter = new InverseBoolConverter();

        // Act
        var converted = converter.Convert(originalValue, typeof(bool), null, _culture);
        var convertedBack = converter.ConvertBack(converted, typeof(bool), null, _culture);

        // Assert
        converted.Should().Be(expectedResult);
        convertedBack.Should().Be(originalValue);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(1, false)]
    [InlineData(100, false)]
    [InlineData(-1, false)]
    public void IsZeroConverter_VariousIntegers_ShouldConvertCorrectly(int value, bool expected)
    {
        // Arrange
        var converter = new IsZeroConverter();

        // Act
        var result = converter.Convert(value, typeof(bool), null, _culture);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(true, "⭐")]
    [InlineData(false, "☆")]
    public void FavoriteIconConverter_BooleanValues_ShouldConvertToCorrectIcon(bool isFavorite, string expectedIcon)
    {
        // Arrange
        var converter = new FavoriteIconConverter();

        // Act
        var result = converter.Convert(isFavorite, typeof(string), null, _culture);

        // Assert
        result.Should().Be(expectedIcon);
    }

    [Theory]
    [InlineData(true, "✨ Premium")]
    [InlineData(false, "Free")]
    public void PremiumStatusConverter_BooleanValues_ShouldConvertToCorrectStatus(bool isPremium, string expectedStatus)
    {
        // Arrange
        var converter = new PremiumStatusConverter();

        // Act
        var result = converter.Convert(isPremium, typeof(string), null, _culture);

        // Assert
        result.Should().Be(expectedStatus);
    }

    #endregion
}
