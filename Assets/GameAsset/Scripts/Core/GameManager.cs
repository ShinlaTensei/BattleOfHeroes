using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using Base.Pattern;
using UnityEngine;

namespace PaidRubik
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private ObjectPooler _pooler;
        private void Start()
        {
            InitService();
        }

        private void OnDestroy()
        {
            ReleaseService();
        }

        private void InitService()
        {
            ServiceLocator.GetService<UIViewManager>().Init();
            ServiceLocator.GetService<AddressableManager>().Init();
            ServiceLocator.SetService(_pooler).Init();
        }

        private void ReleaseService()
        {
            ServiceLocator.GetService<UIViewManager>().Dispose();
            ServiceLocator.GetService<AddressableManager>().Dispose();
            ServiceLocator.GetService<ObjectPooler>().Dispose();
        }
    }
}

