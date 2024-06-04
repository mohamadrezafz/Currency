namespace Currency.Test;
using System;
using System.Collections.Generic;
using Moq;
using Xunit;

public class CurrencyConverterTests
{
    [Fact]
    public void TestCurrencyConverer_WhenSameCurrency_ShouldReturnSameAmount()
    {
        // Arrange
        var currencyConverter = CurrencyConverter.Instance;

        // Act
        double convertedAmount = currencyConverter.Convert("USD", "USD", 100);

        // Assert
        Assert.Equal(100, convertedAmount);
    }

    [Fact]
    public void TestCurrencyConverer_WhenWrongCurrency_ShouldReturnException()
    {
        // Arrange
        var currencyConverter = CurrencyConverter.Instance;
        currencyConverter.ClearConfiguration();
        currencyConverter.UpdateConfiguration(new List<Tuple<string, string, double>>
        {
            Tuple.Create("USD", "CAD", 1.34),
            Tuple.Create("CAD", "GBP", 0.58),
            Tuple.Create("USD", "EUR", 0.86)

        });

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>currencyConverter.Convert("USD", "JPY", 100)); 
    }


    [Fact]
    public void TestCurrencyConverer_WhenForwardCurrencyConversions_ShouldReturnCorrectAmount()
    {
        // Arrange
        var currencyConverter = CurrencyConverter.Instance;
        currencyConverter.ClearConfiguration();
        currencyConverter.UpdateConfiguration(new List<Tuple<string, string, double>>
        {
            Tuple.Create("USD", "CAD", 1.34),
            Tuple.Create("CAD", "GBP", 0.58),
            Tuple.Create("USD", "EUR", 0.86)

        });

        // Act
        double convertedAmount = currencyConverter.Convert("USD", "EUR", 100);

        // Assert
        Assert.Equal(86, convertedAmount);
    }

    [Fact]
    public void TestCurrencyConverter_WhenCurrencyConversionsReverse_ShouldReturnCorrectAmount()
    {
        // Arrange
        var currencyConverter = CurrencyConverter.Instance;
        currencyConverter.ClearConfiguration();
        currencyConverter.UpdateConfiguration(new List<Tuple<string, string, double>>
        {
            Tuple.Create("USD", "CAD", 1.34),
            Tuple.Create("CAD", "GBP", 0.58),
            Tuple.Create("USD", "EUR", 0.86)
        });

        // Act
        double convertedAmount = currencyConverter.Convert("CAD", "USD", 134);

        // Assert
        Assert.Equal(100, convertedAmount); 
    }

    [Fact]
    public void TestCurrencyConverer_WhenMultipleCurrencyConversions_ShouldReturnCorrectAmount()
    {
        // Arrange
        var currencyConverter = CurrencyConverter.Instance;
        currencyConverter.ClearConfiguration();
        currencyConverter.UpdateConfiguration(new List<Tuple<string, string, double>>
        {
            Tuple.Create("USD", "CAD", 1.34),
            Tuple.Create("CAD", "GBP", 0.58),
            Tuple.Create("USD", "EUR", 0.86)

        });

        // Act
        double convertedAmount = currencyConverter.Convert("CAD", "EUR", 134);

        // Assert
        Assert.Equal(86, convertedAmount);
    }

}