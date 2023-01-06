using System.Collections;
using System.Collections.Generic;
using Base.Data.Structure;
using Base.Module;
using UnityEngine;

namespace PaidRubik
{
    [BlueprintReader("Currencies", DataFormat.Json)]
    public class BlueprintCurrency : BaseBlueprint<PlayerProto.Types.CurrencyRecord>
    {
        private IDictionary<string, PlayerProto.Types.CurrencyData> _currencyData = 
            new Dictionary<string, PlayerProto.Types.CurrencyData>();
        public override void Load()
        {
            if (Data is not null)
            {
                _currencyData.Clear();
                int length = Data.CurrencyData.Count;
                for (int i = 0; i < length; ++i)
                {
                    _currencyData.TryAdd(Data.CurrencyData[i].Id, Data.CurrencyData[i]);
                }
            }
        }

        public PlayerProto.Types.CurrencyData GetCurrencyByID(string ID)
        {
            if (_currencyData.TryGetValue(ID, out PlayerProto.Types.CurrencyData value))
            {
                return value;
            }

            return null;
        }

        public override void LoadDummyData()
        {
            
        }
    }
}

