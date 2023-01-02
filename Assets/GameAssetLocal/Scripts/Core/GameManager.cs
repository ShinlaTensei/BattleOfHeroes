using System;
using System.Collections.Generic;
using System.Threading;
using Base;
using Base.Logging;
using Base.Pattern;
using Base.Services;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using Firebase.RemoteConfig;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace PaidRubik
{
    public interface ISerialize<T>
    {
        T To();
        void From(T data);

        void Raise();
    }

    public static class DatabaseKey
    {
        public const string UserData = "Data";
        public const string Currencies = "Currencies";
    }
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private ObjectPooler _pooler;
        [SerializeField] private AssetReference homeSceneRef;
        [SerializeField] private AssetReference gameSceneRef;
        
        private void Awake()
        {
            InitService().Forget();
            InitSignal();
        }

        private void OnApplicationQuit()
        {
            ReleaseSignal();
            ReleaseService();
        }
        
        private async UniTaskVoid InitService()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            try
            {
                await ServiceLocator.GetService<SceneLoadService>()!.LoadLoadingScene().AttachExternalCancellation(tokenSource.Token);

                var uiViewManager = ServiceLocator.GetService<UIViewManager>();
                uiViewManager!.Init();
                uiViewManager.GetView<LoadingUI>().SetStatus("Loading Services ...", 0);


                await ServiceLocator.GetService<FirebaseAppService>()!.InitializeAsync();
                await ServiceLocator.GetService<AddressableManager>()!.InitializeAsync(cancellationToken: tokenSource.Token);

                ServiceLocator.SetService(_pooler).Init();
                ServiceLocator.GetService<UserCurrencyService>()?.Init();
                ServiceLocator.GetService<SceneLoadService>()?.Init();
                ServiceLocator.GetService<MapService>()?.Init();
                ServiceLocator.GetService<BlueprintLocalization>()?.Init();

                uiViewManager.GetView<LoadingUI>().SetStatus("Loading Services ...", .1f);

                await LoginAndLoadConfig();

                await ServiceLocator.GetService<SceneLoadService>()!.UnLoadLoadingScene().AttachExternalCancellation(tokenSource.Token);

                await ServiceLocator.GetService<SceneLoadService>()!.LoadHomeScene(homeSceneRef)
                    .AttachExternalCancellation(tokenSource.Token);
            }
            catch (Exception e)
            {
                this.GetLogger().Error(e);
                tokenSource.Cancel();
            }
            finally
            {
                tokenSource.Dispose();
            }
            
        }

        private async UniTask LoginAndLoadConfig()
        {
            var firebaseService = ServiceLocator.GetService<FirebaseAppService>();
            var uiViewManager = ServiceLocator.GetService<UIViewManager>();
            
            // Login to Firebase
            uiViewManager!.GetView<LoadingUI>().SetStatus("Login ...", .2f);
            await firebaseService!.LoginAnonymous();
            uiViewManager.GetView<LoadingUI>().SetStatus("Login ...", .3f);
            
            uiViewManager.GetView<LoadingUI>().SetStatus("Loading config ...", .5f);
            // Load remote config
            await firebaseService.FetchRemoteConfig();
            
            if (firebaseService.RemoteConfig.Info.LastFetchStatus == LastFetchStatus.Success)
            {
                // Pass the map config to client
                MapService mapService = ServiceLocator.GetService<MapService>();
                firebaseService.RemoteConfig.AllValues.TryGetValue(mapService!.MapConfigKey, out ConfigValue result);
                List<MapDataConfig> data = JsonConvert.DeserializeObject<List<MapDataConfig>>(result.StringValue);
                mapService.From(data);
                
                // Pass the match config to client
            }

            uiViewManager.GetView<LoadingUI>().SetStatus("Loading config ...", .8f);
            
            // Load user data
            DataSnapshot userData = await firebaseService.LoadUserDataAsync(DatabaseKey.UserData);
            if (userData.Exists)
            {
                string json = userData.GetRawJsonValue();
                ServiceLocator.GetService<UserDataService>()!.From(JsonConvert.DeserializeObject<UserDataBlueprint>(json));
            }
            else
            {
                UserDataBlueprint pushData = new UserDataBlueprint
                {
                    ID = firebaseService.UserID,
                    SettingBlueprint = new UserSettingBlueprint { IsMusic = true, IsSound = true }
                };
                await firebaseService.SetJsonAsync(DatabaseKey.UserData, JsonConvert.SerializeObject(pushData));
            }
            DataSnapshot userCurrencies = await firebaseService.LoadUserDataAsync(DatabaseKey.Currencies);
            if (userCurrencies.Exists)
            {
                string json = userCurrencies.GetRawJsonValue();
                ServiceLocator.GetService<UserCurrencyService>()?
                    .From(JsonConvert.DeserializeObject<CurrencyDataBlueprint>(json));
            }
            else
            {
                UserCurrencyService currencyService = ServiceLocator.GetService<UserCurrencyService>();
                firebaseService.RemoteConfig.AllValues
                    .TryGetValue(currencyService!.CurrencyConfigKey, out ConfigValue currencyConfig);
                CurrencyDataBlueprint currencyDataBlueprint =
                    JsonConvert.DeserializeObject<CurrencyDataBlueprint>(currencyConfig.StringValue);
                currencyService.From(currencyDataBlueprint);

                string pushData = currencyConfig.StringValue;
                await firebaseService.SetJsonAsync(DatabaseKey.Currencies, pushData);
            }
            uiViewManager.GetView<LoadingUI>().SetStatus("Loading data ...", 1f);
        }

        private void InitSignal()
        {
            ServiceLocator.GetSignal<JoinMatchSoloSignal>()?.Subscribe(OnJoinMatchSoloSignal);
            ServiceLocator.GetSignal<LanguageChangedRequestSignal>()?.Subscribe(OnLanguageRequestChanged);
        }

        private void ReleaseSignal()
        {
            ServiceLocator.GetSignal<JoinMatchSoloSignal>()?.UnSubscribe(OnJoinMatchSoloSignal);
            ServiceLocator.GetSignal<LanguageChangedRequestSignal>()?.UnSubscribe(OnLanguageRequestChanged);
        }
        
        private void OnJoinMatchSoloSignal(int gameIndex)
        {
            ExecuteJoinGame().Forget();
        }

        private void OnLanguageRequestChanged(string langCode)
        {
            
        }
        
        /// <summary>
        /// Flow of join game
        /// </summary>
        private async UniTask ExecuteJoinGame()
        {
            await ServiceLocator.GetService<SceneLoadService>()!.UnloadHomeScene();

            await ServiceLocator.GetService<SceneLoadService>()!.LoadGameScene(gameSceneRef);

            // Pass config for level
        }

        private void ReleaseService()
        {
            ServiceLocator.GetService<UIViewManager>()?.DeInit();
            ServiceLocator.GetService<AddressableManager>()?.DeInit();
            ServiceLocator.GetService<ObjectPooler>()?.DeInit();
            ServiceLocator.GetService<UserCurrencyService>()?.DeInit();
            ServiceLocator.GetService<SceneLoadService>()?.DeInit();
            ServiceLocator.GetService<FirebaseAppService>()?.DeInit();
            ServiceLocator.GetService<MapService>()?.DeInit();
        }
    }
}

