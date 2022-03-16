using System.Net.Http.Json;
using MiniBank.Core.Services.Interfaces;
using MiniBank.Core.Tools;
using MiniBank.Data.ResponseModels;

namespace MiniBank.Data.Services
{
    public class CurrencyRateProvider : ICurrencyRateProvider
    {
        private readonly HttpClient _client;

        public CurrencyRateProvider(HttpClient httpClient)
        {
            _client = httpClient;
        }

        public double GetCurrencyRate(string fromCurrencyCode, string toCurrencyCode)
        {
            double fromCurrencyRubleRate = GetCurrencyRubleRate(fromCurrencyCode);
            double toCurrencyRubleRate = GetCurrencyRubleRate(toCurrencyCode);

            return toCurrencyRubleRate / fromCurrencyRubleRate;
        }

        private double GetCurrencyRubleRate(string currencyCode)
        {
            CurrenciesModel? response = _client
                .GetFromJsonAsync<CurrenciesModel>("")
                .GetAwaiter()
                .GetResult();

            if (response is null)
            {
                throw new Exception("Can't get response");
            }

            if (!response.Valute.ContainsKey(currencyCode))
            {
                throw new ValidationException($"There is no such currency code: {currencyCode}");
            }
            
            CurrencyModel currency = response.Valute[currencyCode];
            return currency.Value / currency.Nominal;
        }
    }
}