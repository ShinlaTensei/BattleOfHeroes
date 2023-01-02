using System;
using System.Collections;
using System.Collections.Generic;
using Base.Helper;
using Base.Pattern;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace PaidRubik
{
    public class NavigationButtonUI : BaseMono
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private Image icon;

        private Tweener _myTween;

        private void Awake()
        {
            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(bool value)
        {
            if (value)
            {
                ServiceLocator.GetSignal<NavigationChangedSignal>()?.Dispatch(this);
                toggle.isOn = true;
            }
        }

        public void SetToggle(bool value)
        {
            toggle.isOn = value;
        }
    }
}

