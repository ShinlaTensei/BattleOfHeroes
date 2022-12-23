using System;
using System.Collections;
using System.Collections.Generic;
using Base.Helper;
using UnityEngine;
using UnityEngine.UI;

namespace PaidRubik
{
    [UIModel("MapSelectItemUI", SceneName.HomeScene)]
    public class MapSelectItemUI : BaseMono
    {
        [SerializeField] private Image background;
        [SerializeField] private Button button;

        private void Awake()
        {
            
        }
    }
}

