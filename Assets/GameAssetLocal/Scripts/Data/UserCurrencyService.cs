using System;
using System.Collections;
using System.Collections.Generic;
using Base.Data.Structure;
using Base.Module;
using Base.Pattern;
using UniRx;
using UnityEngine;

namespace PaidRubik
{
    public class CurrencyRecord
    {
        private StringReactiveProperty _id = new StringReactiveProperty();
        private IntReactiveProperty _amount = new IntReactiveProperty(0);

        public StringReactiveProperty ID => _id;
        public IntReactiveProperty Amount => _amount;
    }

    public static class CurrencyID
    {
        public const string GoldKey = "GOLD";
        public const string StarKey = "STAR";
    }
    public class UserCurrencyService : BaseBlueprint<PlayerProto.Types.CurrencyRecord>,IService<PlayerProto.Types.CurrencyData>
    {
        private Dictionary<string, CurrencyRecord> _currencyData = new Dictionary<string, CurrencyRecord>();

        public CurrencyRecord GetCurrencyByID(string id, bool createIfNotExist = false)
        {
            _currencyData.TryGetValue(id, out CurrencyRecord result);

            if (result == null && createIfNotExist)
            {
                result = new CurrencyRecord();
                result.ID.Value = id;
                result.Amount.Value = 0;
                
                _currencyData.Add(id, result);
            }
            
            return result;
        }
        public void UpdateData(PlayerProto.Types.CurrencyData data)
        {
            if (data == null) return;
            
            CurrencyRecord currencyRecord = GetCurrencyByID(data.Id, true);
            currencyRecord.Amount.Value = data.Amount;
            currencyRecord.ID.Value = data.Id;
        }

        public void Init()
        {
            
        }

        public void DeInit()
        {
            _currencyData.Clear();
        }

        public override void Load()
        {
            if (Data is not null)
            {
                _currencyData.Clear();
                foreach (var currency in Data.CurrencyData)
                {
                    CurrencyRecord @record = GetCurrencyByID(currency.Id, true);
                    @record.Amount.Value = currency.Amount;
                    @record.ID.Value = currency.Id;
                }
            }
        }

        public override void LoadDummyData()
        {
            throw new NotImplementedException();
        }
    }
}

