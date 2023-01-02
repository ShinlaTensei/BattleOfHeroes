using System;
using System.Collections;
using System.Collections.Generic;
using Base.Logging;
using Base.Pattern;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.RemoteConfig;
using Newtonsoft.Json;
using UnityEngine;

namespace PaidRubik
{
    public class FirebaseAppService : IService
    {
        private FirebaseApp _firebaseApp;
        private FirebaseAuth _firebaseAuth;
        private FirebaseDatabase _firebaseDatabase;
        private FirebaseRemoteConfig _firebaseRemoteConfig;

        private const string DatabaseURL = "https://battleofheroes-48e86-default-rtdb.asia-southeast1.firebasedatabase.app";
        public bool IsFirebaseReady { get; set; }

        public string UserID => _firebaseAuth.CurrentUser.UserId;
        public FirebaseUser FirebaseUser => _firebaseAuth.CurrentUser;

        public DatabaseReference RootRef => _firebaseDatabase.RootReference;

        public DatabaseReference MyRef => _firebaseDatabase.GetReference($"Users/{UserID}");

        public FirebaseRemoteConfig RemoteConfig => _firebaseRemoteConfig;

        public void Init()
        {
            
        }

        public void DeInit()
        {
            _firebaseAuth.Dispose();
            _firebaseDatabase = null;
            _firebaseRemoteConfig.Info.Dispose();
            _firebaseApp.Dispose(true);
        }

        public async UniTask InitializeAsync()
        {
            BaseLogSystem.GetLogger().Info("[Firebase] Initializing...");
            DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (status == DependencyStatus.Available)
            {
                _firebaseApp = FirebaseApp.DefaultInstance;
                _firebaseAuth = FirebaseAuth.GetAuth(_firebaseApp);
                _firebaseDatabase = FirebaseDatabase.GetInstance(_firebaseApp, DatabaseURL);
                _firebaseRemoteConfig = FirebaseRemoteConfig.GetInstance(_firebaseApp);
                IsFirebaseReady = true;
                
                BaseLogSystem.GetLogger().Info("[Firebase] Initialize Completed");
            }
            else
            {
                IsFirebaseReady = false;
                
                BaseLogSystem.GetLogger().Info("[Firebase] Initialize Failed");
            }
        }

        public async UniTask LoginAnonymous()
        {
            if (!IsFirebaseReady) return;

            if (_firebaseAuth.CurrentUser != null)
            {
                await _firebaseAuth.CurrentUser.ReloadAsync();

                BaseLogSystem.GetLogger().Info("[Login] User: {0}", _firebaseAuth.CurrentUser);
            }

            if (_firebaseAuth.CurrentUser == null)
            {
                var result = await _firebaseAuth.SignInAnonymouslyAsync();

                if (result != null)
                {
                    BaseLogSystem.GetLogger().Info("[Login] UserID: {0}", result.UserId);
                }
            }
        }

        public async UniTask<DataSnapshot> LoadUserDataAsync(string key)
        {
            if (!IsFirebaseReady) return null;

            var result = await RootRef.Child("Users").Child(UserID).Child(key).GetValueAsync();

            if (result.Exists)
            {
                BaseLogSystem.GetLogger().Info($"[Database] Load ({key}) Data Successfully ...");
            }
            else
            {
                BaseLogSystem.GetLogger().Info($"[Database] ({key}) Data's not exist. Write data to server ...");
            }

            return result;
        }

        public async UniTask<bool> FetchRemoteConfig()
        {
            if (!IsFirebaseReady) return false;

            await _firebaseRemoteConfig.FetchAsync(TimeSpan.Zero);
            return await _firebaseRemoteConfig.ActivateAsync();
        }

        public async UniTask SetJsonAsync(string key, string data)
        {
            if (!IsFirebaseReady) return;

            await MyRef.Child(key).SetRawJsonValueAsync(data);
        }
    }
}
