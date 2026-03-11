using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using PolyhydraGames.BlazorComponents.QueryStrings;
using Xunit;

namespace BlazorComponents.Tests.QueryStrings;

public class QueryStringParameterExtensionsTests
{
    #region Test Components

    private class TestComponent : ComponentBase
    {
        [QueryStringParameter("intParam")]
        public int IntParam { get; set; }

        [QueryStringParameter("nullableIntParam")]
        public int? NullableIntParam { get; set; }

        [QueryStringParameter("boolParam")]
        public bool BoolParam { get; set; }

        [QueryStringParameter("nullableBoolParam")]
        public bool? NullableBoolParam { get; set; }

        [QueryStringParameter("enumParam")]
        public TestEnum EnumParam { get; set; }

        [QueryStringParameter("nullableEnumParam")]
        public TestEnum? NullableEnumParam { get; set; }

        [QueryStringParameter("guidParam")]
        public Guid GuidParam { get; set; }

        [QueryStringParameter("nullableGuidParam")]
        public Guid? NullableGuidParam { get; set; }
    }

    private enum TestEnum
    {
        First,
        Second,
        Third
    }

    #endregion

    [Fact]
    public void ConvertValue_WithInt_ReturnsCorrectValue()
    {
        // Arrange
        var value = new StringValues("42");

        // Act - use reflection to call private method
        var result = CallPrivateConvertValue(value, typeof(int));

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public void ConvertValue_WithNullableInt_ReturnsCorrectValue()
    {
        // Arrange
        var value = new StringValues("123");

        // Act
        var result = CallPrivateConvertValue(value, typeof(int?));

        // Assert
        Assert.Equal(123, result);
    }

    [Fact]
    public void ConvertValue_WithNullableIntNullValue_ReturnsNull()
    {
        // Arrange
        var value = StringValues.Empty;

        // Act
        var result = CallPrivateConvertValue(value, typeof(int?));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ConvertValue_WithBool_ReturnsCorrectValue()
    {
        // Arrange
        var value = new StringValues("true");

        // Act
        var result = CallPrivateConvertValue(value, typeof(bool));

        // Assert
        Assert.Equal(true, result);
    }

    [Fact]
    public void ConvertValue_WithNullableBool_ReturnsCorrectValue()
    {
        // Arrange
        var value = new StringValues("false");

        // Act
        var result = CallPrivateConvertValue(value, typeof(bool?));

        // Assert
        Assert.Equal(false, result);
    }

    [Fact]
    public void ConvertValue_WithEnum_ReturnsCorrectValue()
    {
        // Arrange
        var value = new StringValues("Second");

        // Act
        var result = CallPrivateConvertValue(value, typeof(TestEnum));

        // Assert
        Assert.Equal(TestEnum.Second, result);
    }

    [Fact]
    public void ConvertValue_WithEnumCaseInsensitive_ReturnsCorrectValue()
    {
        // Arrange
        var value = new StringValues("second");

        // Act
        var result = CallPrivateConvertValue(value, typeof(TestEnum));

        // Assert
        Assert.Equal(TestEnum.Second, result);
    }

    [Fact]
    public void ConvertValue_WithNullableEnum_ReturnsCorrectValue()
    {
        // Arrange
        var value = new StringValues("Third");

        // Act
        var result = CallPrivateConvertValue(value, typeof(TestEnum?));

        // Assert
        Assert.Equal(TestEnum.Third, result);
    }

    [Fact]
    public void ConvertValue_WithNullableEnumNullValue_ReturnsNull()
    {
        // Arrange
        var value = StringValues.Empty;

        // Act
        var result = CallPrivateConvertValue(value, typeof(TestEnum?));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ConvertValue_WithGuid_ReturnsCorrectValue()
    {
        // Arrange
        var guidString = "550e8400-e29b-41d4-a716-446655440000";
        var value = new StringValues(guidString);
        var expectedGuid = new Guid(guidString);

        // Act
        var result = CallPrivateConvertValue(value, typeof(Guid));

        // Assert
        Assert.Equal(expectedGuid, result);
    }

    [Fact]
    public void ConvertValue_WithNullableGuid_ReturnsCorrectValue()
    {
        // Arrange
        var guidString = "6ba7b810-9dad-11d1-80b4-00c04fd430c8";
        var value = new StringValues(guidString);
        var expectedGuid = new Guid(guidString);

        // Act
        var result = CallPrivateConvertValue(value, typeof(Guid?));

        // Assert
        Assert.Equal(expectedGuid, result);
    }

    [Fact]
    public void ConvertValue_WithNullableGuidNullValue_ReturnsNull()
    {
        // Arrange
        var value = StringValues.Empty;

        // Act
        var result = CallPrivateConvertValue(value, typeof(Guid?));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ConvertValue_WithEmptyStringForNullableInt_ReturnsNull()
    {
        // Arrange
        var value = StringValues.Empty;

        // Act
        var result = CallPrivateConvertValue(value, typeof(int?));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ConvertValue_WithEmptyStringForNonNullableInt_ReturnsDefault()
    {
        // Arrange
        var value = StringValues.Empty;

        // Act
        var result = CallPrivateConvertValue(value, typeof(int));

        // Assert
        Assert.Equal(0, result);
    }

    /// <summary>
    /// Helper method to call the private ConvertValue method via reflection
    /// </summary>
    private static object? CallPrivateConvertValue(StringValues value, Type type)
    {
        var methodInfo = typeof(QueryStringParameterExtensions)
            .GetMethod("ConvertValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        Assert.NotNull(methodInfo);

        return methodInfo.Invoke(null, new object[] { value, type });
    }
}
