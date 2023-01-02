using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using Base.Helper;
using Base.Pattern;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PaidRubik
{
    public class LoadingUI : UIView
    {
        [Space] [SerializeField] private Slider progressSlider;
        [SerializeField] private TMP_Text statusText;

        private void Awake()
        {
            ServiceLocator.GetService<UIViewManager>()?.Add(this);

            progressSlider.value = 1;
        }

        public void SetStatus(string text, float progress)
        {
            statusText.text = text;
            progressSlider.value = 1 - progress;
        }
    }
}

