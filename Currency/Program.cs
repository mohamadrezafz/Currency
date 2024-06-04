
using Currency;

var converter = CurrencyConverter.Instance;

converter.UpdateConfiguration(new List<Tuple<string, string, double>>
        {
            Tuple.Create("USD", "CAD", 1.34),
            Tuple.Create("CAD", "GBP", 0.58),
            Tuple.Create("USD", "EUR", 0.86)
        });

double amount = 134;
double convertedAmount = converter.Convert("CAD", "EUR", 134);

Console.WriteLine($"{amount} CAD equivalent {convertedAmount} EUR");