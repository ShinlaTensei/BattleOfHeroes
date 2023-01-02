using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using Base.Helper;
using Base.Logging;
using Base.Services;
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
            ServiceLocator.GetSignal<NavigationChangedSignal>()?.Subscribe(OnNavigationChanged);
        }

        protected override void Start()
        {
            ChangeTab(_currentIndex = 2);
        }

        private void OnDestroy()
        {
            ServiceLocator.GetSignal<NavigationChangedSignal>()?.UnSubscribe(OnNavigationChanged);
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
            this.GetLogger().Info("[Navigation] Open Tab {0}", ((HomeTab)index).ToString());
            if (index == (int) HomeTab.ShopUI)
            {
                ServiceLocator.GetService<UIViewManager>()!.Show<ShopUI>().Forget();
            }
            else if (index == (int) HomeTab.HeroesUI)
            {
                ServiceLocator.GetService<UIViewManager>()!.Show<HeroesUI>().Forget();
            }
            else if (index == (int)HomeTab.MapUI)
            {
                MapService mapService = ServiceLocator.GetService<MapService>();
                ServiceLocator.GetService<UIViewManager>()!.Show<MapUI, MapDataBlueprint>(mapService!.CurrentMap()).Forget();
            }
            else if (index == (int)HomeTab.SettingUI)
            {
                ServiceLocator.GetService<UIViewManager>()!.Show<SettingUI>().Forget();
            }
        }
    }
    
    public enum HomeTab {ShopUI = 0, HeroesUI, MapUI, SettingUI}

    public class NavigationChangedSignal : Signal<NavigationButtonUI>
    {
        
    }
}

