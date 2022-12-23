using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using Base.Helper;
using Base.MessageSystem;
using Base.Pattern;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PaidRubik
{
    public class NavigationBarUI : UIView
    {
        [SerializeField] private NavigationButtonUI[] navigationButtonUI;

        private int _currentIndex = 0;
        private void Awake()
        {
            ServiceLocator.GetSignal<NavigationChangedSignal>().Subscribe(OnNavigationChanged);
        }

        private void OnApplicationQuit()
        {
            ServiceLocator.GetSignal<NavigationChangedSignal>().UnSubscribe(OnNavigationChanged);
        }

        private void OnNavigationChanged(NavigationButtonUI sender)
        {
            int index = Array.IndexOf(navigationButtonUI, sender);
            if (_currentIndex != index)
            {
                _currentIndex = index;
                ChangeTab(_currentIndex);
            }
        }

        private void ChangeTab(int index)
        {
            if (index == (int) HomeTab.ShopUI)
            {
                ServiceLocator.GetService<UIViewManager>().Show<ShopUI>().Forget();
            }
            else if (index == (int) HomeTab.HeroesUI)
            {
                ServiceLocator.GetService<UIViewManager>().Show<HeroesUI>().Forget();
            }
        }
    }
    
    public enum HomeTab {ShopUI = 0, HeroesUI, MapUI, SettingUI}

    public class NavigationChangedSignal : Signal<NavigationButtonUI>
    {
        
    }
}

