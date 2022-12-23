using System;
using System.Collections;
using System.Collections.Generic;
using Base.Logging;
using Base.Pattern;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;

namespace PaidRubik
{
    public class FirebaseAppService : IService, IDisposable
    {
        private FirebaseApp _firebaseApp;
        private FirebaseAuth _firebaseAuth;
        
        public bool IsFirebaseReady { get; set; }
        public void Init()
        {
            
        }

        public void Dispose()
        {
            _firebaseApp.Dispose();
            _firebaseAuth.Dispose();
        }

        public async UniTask InitializeAsync()
        {
            BaseLogSystem.GetLogger().Info("[Firebase] Initializing...");
            DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (status == DependencyStatus.Available)
            {
                _firebaseApp = FirebaseApp.DefaultInstance;
                _firebaseAuth = FirebaseAuth.GetAuth(_firebaseApp);

                IsFirebaseReady = true;
                
                BaseLogSystem.GetLogger().Info("[Firebase] Initialize Completed");
            }
            else
            {
                IsFirebaseReady = false;
                
                BaseLogSystem.GetLogger().Info("[Firebase] Initialize Failed");
            }
        }
    }
}
