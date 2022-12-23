using System;
using System.Collections;
using System.Collections.Generic;
using Base.Helper;
using Base.Pattern;
using TMPro;
using UniRx;
using UnityEngine;


namespace PaidRubik
{
    public class HudHomeUI : UIView
    {
        [SerializeField] private TMP_Text valueText;

        private void Awake()
        {
            ServiceLocator.GetService<UserCurrencyService>().GetCurrencyByID(CurrencyEnum.Coin.ToString(), true).Amount.Subscribe(OnCurrencyChanged)
                .AddTo(this);
        }

        private void OnCurrencyChanged(int value)
        {
            valueText.text = value.ToString();
        }
    }
}

