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

        public async Task<double> GetCurrencyRate(Currency fromCurrencyCode, Currency toCurrencyCode)
        {
            double fromCurrencyRubleRate = await GetCurrencyRubleRate(fromCurrencyCode);
            double toCurrencyRubleRate = await GetCurrencyRubleRate(toCurrencyCode);

            return fromCurrencyRubleRate / toCurrencyRubleRate;
        }

        private async Task<double> GetCurrencyRubleRate(Currency currencyCode)
        {
            CurrenciesModel response = await _client
                .GetFromJsonAsync<CurrenciesModel>("/daily_json.js");

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