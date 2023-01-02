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
    
    [Serializable]
    public class CurrencyDataBlueprint
    {
        public List<CurrencyRecord> Records;
    }
    
    [Serializable]
    public class CurrencyRecord
    {
        public string ID;
        public int Amount;
    }

    public enum CurrencyEnum
    {
        Coin = 0
    }

    public static class CurrencyID
    {
        public const string GoldKey = "GOLD";
        public const string StarKey = "STAR";
    }
    public class UserCurrencyService : IService<CurrencyData>, ISerialize<CurrencyDataBlueprint>
    {
        private Dictionary<string, CurrencyData> _currencyData = new Dictionary<string, CurrencyData>();

        public string CurrencyConfigKey = "CurrencyConfig";

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

        public void DeInit()
        {
            _currencyData.Clear();
        }

        public CurrencyDataBlueprint To()
        {
            CurrencyDataBlueprint currencyDataBlueprint = new CurrencyDataBlueprint();
            foreach (var currency in _currencyData.Values)
            {
                currencyDataBlueprint.Records.Add(new CurrencyRecord
                {
                    Amount = currency.Amount.Value,
                    ID = currency.ID.Value
                });
            }

            return currencyDataBlueprint;
        }

        public void From(CurrencyDataBlueprint data)
        {
            foreach (var record in data.Records)
            {
                CurrencyData currencyData = GetCurrencyByID(record.ID, true);
                currencyData.Amount.Value = record.Amount;
                currencyData.ID.Value = record.ID;
            }
        }

        public void Raise()
        {
           
        }
    }
}

