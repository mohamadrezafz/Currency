using Currency.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

public class CurrencyConverter : ICurrencyConverter
{
    private static readonly Lazy<CurrencyConverter> lazyInstance = new Lazy<CurrencyConverter>(() => new CurrencyConverter());

    public static CurrencyConverter Instance => lazyInstance.Value;

    private readonly Dictionary<string, Dictionary<string, double>> rates;
    private readonly object lockObject = new object();

    private CurrencyConverter()
    {
        rates = new Dictionary<string, Dictionary<string, double>>();
    }

    public void ClearConfiguration()
    {
        lock (lockObject)
            rates.Clear();
    }

    public void UpdateConfiguration(IEnumerable<Tuple<string, string, double>> conversionRates)
    {
        lock (lockObject)
        {
            foreach (var rate in conversionRates)
            {
                if (!rates.ContainsKey(rate.Item1))
                    rates[rate.Item1] = new Dictionary<string, double>();

                rates[rate.Item1][rate.Item2] = rate.Item3;

                // for reverse rates
                if (!rates.ContainsKey(rate.Item2))
                    rates[rate.Item2] = new Dictionary<string, double>();

                rates[rate.Item2][rate.Item1] = 1.0 / rate.Item3;
            }
        }
    }

    public double Convert(string fromCurrency, string toCurrency, double amount)
    {
        if (fromCurrency == toCurrency)
            return amount;

        lock (lockObject)
        {
            if (!rates.ContainsKey(fromCurrency) || !rates.ContainsKey(toCurrency))
                throw new ArgumentException("Conversion rate not available for the given currencies.");

            var path = FindShortPath(fromCurrency, toCurrency);
            if (path == null)
                throw new ArgumentException("No conversion path available for the given currencies.");

            double convertedAmount = amount;
            for (int i = 0; i < path.Count - 1; i++)
            {
                convertedAmount = Math.Round(convertedAmount * rates[path[i]][path[i + 1]], 4);
            }
            return convertedAmount;
        }
    }

    private List<string> FindShortPath(string start, string end)
    {
        var previous = new Dictionary<string, string>();
        var distances = new Dictionary<string, double>();
        var nodes = new List<string>();

        foreach (var currency in rates.Keys)
        {
            if (currency == start)
                distances[currency] = 0;
            else
                distances[currency] = double.MaxValue;

            nodes.Add(currency);
        }

        while (nodes.Count != 0)
        {
            nodes.Sort((x, y) => distances[x].CompareTo(distances[y]));
            var smallest = nodes[0];
            nodes.Remove(smallest);

            if (smallest == end)
            {
                var path = new List<string>();
                while (previous.ContainsKey(smallest))
                {
                    path.Add(smallest);
                    smallest = previous[smallest];
                }
                path.Add(start);
                path.Reverse();
                return path;
            }

            if (distances[smallest] == double.MaxValue)
                break;

            foreach (var neighbor in rates[smallest].Keys)
            {
                var alt = distances[smallest] + 1 / rates[smallest][neighbor];
                if (alt < distances[neighbor])
                {
                    distances[neighbor] = alt;
                    previous[neighbor] = smallest;
                }
            }
        }

        return null;
    }
}
