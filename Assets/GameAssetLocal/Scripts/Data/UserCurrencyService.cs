using System;
using System.Collections;
using System.Collections.Generic;
using Base.Pattern;
using UniRx;
using UnityEngine;

namespace PaidRubik
{
    public class CurrencyData
    {
        private StringReactiveProperty _id = new StringReactiveProperty();
        private IntReactiveProperty _amount = new IntReactiveProperty(0);

        public StringReactiveProperty ID => _id;
        public IntReactiveProperty Amount => _amount;
    }

    public enum CurrencyEnum
    {
        Coin = 0
    }
    public class UserCurrencyService : IService<CurrencyData>, IDisposable
    {
        private Dictionary<string, CurrencyData> _currencyData = new Dictionary<string, CurrencyData>();

        public CurrencyData GetCurrencyByID(string id, bool createIfNotExist = false)
        {
            _currencyData.TryGetValue(id, out CurrencyData result);

            if (result == null && createIfNotExist)
            {
                result = new CurrencyData();
                result.ID.Value = id;
                result.Amount.Value = 0;
                
                _currencyData.Add(id, result);
            }
            
            return result;
        }
        public void UpdateData(CurrencyData data)
        {
            if (data == null) return;
            
            CurrencyData currency = GetCurrencyByID(data.ID.Value, true);

            currency.Amount.Value = data.Amount.Value;
        }

        public void Init()
        {
            
        }

        public void Dispose()
        {
            _currencyData.Clear();
        }
    }
}

