using System.Net.Http.Json;
using MiniBank.Core.Domain.Currencies;
using MiniBank.Core.Domain.Currencies.Providers;
using MiniBank.Core.Tools;
using MiniBank.Data.CurrencyRates.Models;

namespace MiniBank.Data.CurrencyRates.Providers
{
    public class CurrencyRateProvider : ICurrencyRateProvider
    {
        private readonly HttpClient _client;

        public CurrencyRateProvider(HttpClient httpClient)
        {
            _client = httpClient;
        }

        public double GetCurrencyRate(Currency fromCurrencyCode, Currency toCurrencyCode)
        {
            double fromCurrencyRubleRate = GetCurrencyRubleRate(fromCurrencyCode);
            double toCurrencyRubleRate = GetCurrencyRubleRate(toCurrencyCode);

            return fromCurrencyRubleRate / toCurrencyRubleRate;
        }

        private double GetCurrencyRubleRate(Currency currencyCode)
        {
            CurrenciesModel response = _client
                .GetFromJsonAsync<CurrenciesModel>("")
                .GetAwaiter()
                .GetResult();

            if (response is null)
            {
                throw new Exception("Can't get response");
            }

            if (currencyCode == Currency.RUB)
            {
                return 1;
            }
            
            if (!response.Valute.ContainsKey(currencyCode.ToString()))
            {
                throw new Exception($"There is no such currency code: {currencyCode}");
            }

            CurrencyModel currency = response.Valute[currencyCode.ToString()];
            return currency.Value / currency.Nominal;
        }
    }
}