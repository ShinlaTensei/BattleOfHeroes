using System;
using System.Collections;
using System.Collections.Generic;
using Base.Helper;
using Base.Services;
using Base.Pattern;
using UnityEngine;
using UnityEngine.UI;

namespace PaidRubik
{
    public enum MapItemStatus
    {
        Locked, Ready, Completed
    }
    public class JoinMatchSoloSignal : Signal<int> {}

    [UIModel("MapSelectItemUI", SceneName.HomeScene)]
    public class MapSelectItemUI : BaseMono
    {
        [SerializeField] private Image background;
        [SerializeField] private Button button;
        [SerializeField] private MapItemStatus status;

        private int _currentIndex;
        private string _currentMapID;

        private void Awake()
        {
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            ServiceLocator.GetSignal<JoinMatchSoloSignal>()?.Dispatch(_currentIndex);
        }
        
        public void Install(MapItemSelectData itemData)
        {
            Active = true;
            background.gameObject.SetActive(itemData.Index != 10);
            _currentIndex = itemData.Index;
        }
    }
}

