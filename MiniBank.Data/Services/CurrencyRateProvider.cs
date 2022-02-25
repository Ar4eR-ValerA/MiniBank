﻿using System;
using MiniBank.Core.Interfaces;

namespace MiniBank.Data.Services
{
    public class CurrencyRateProvider : ICurrencyRateProvider
    {
        private readonly Random _random;
        
        public CurrencyRateProvider()
        {
            _random = new Random();
        }
        
        public int GetCurrencyRate(string currencyCode)
        {
            return _random.Next();
        }
    }
}