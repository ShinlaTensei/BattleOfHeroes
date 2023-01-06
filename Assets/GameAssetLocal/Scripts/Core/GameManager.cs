using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Base;
using Base.Data.Structure;
using Base.Helper;
using Base.Logging;
using Base.Pattern;
using Base.Services;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using Firebase.RemoteConfig;
using Google.Protobuf;
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

    public static class ConfigKey
    {
        public const string Currency = "CurrencyConfig";
    }
    public class GameManager : MonoBehaviour
    {
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
                
                ServiceLocator.GetService<SceneLoadService>()?.Init();
                ServiceLocator.GetService<MapService>()?.Init();
                ServiceLocator.GetService<BlueprintLocalization>()?.Init();

                uiViewManager.GetView<LoadingUI>().SetStatus("Loading Services ...", .1f);

                await LoginAndLoadConfig();
                
                ServiceLocator.GetService<UserCurrencyService>()?.InitBlueprint();

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
                ServiceLocator.GetBlueprint<UserDataService>()?.ReadBlueprint(Encoding.UTF8.GetBytes(json));
            }
            else
            {
                PlayerProto.Types.UserData pushData = new PlayerProto.Types.UserData
                {
                    Id = firebaseService.UserID,
                    IsMusic = true,
                    IsSound = true
                };
                ServiceLocator.GetService<UserDataService>()?.AddData(pushData);
                string json = ServiceLocator.GetService<UserDataService>()?.SerializeJson();
                await firebaseService.SetJsonAsync(DatabaseKey.UserData, json);
            }
            DataSnapshot userCurrencies = await firebaseService.LoadUserDataAsync(DatabaseKey.Currencies);
            if (userCurrencies.Exists)
            {
                string json = userCurrencies.GetRawJsonValue();
                ServiceLocator.GetService<UserCurrencyService>()?.ReadBlueprint(Encoding.UTF8.GetBytes(json));
            }
            else
            {
                firebaseService.RemoteConfig.AllValues
                    .TryGetValue(ConfigKey.Currency, out ConfigValue currencyConfig);

                string pushData = currencyConfig.StringValue;
                ServiceLocator.GetService<UserCurrencyService>()?.ReadBlueprint(Encoding.UTF8.GetBytes(pushData));
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
            ServiceLocator.GetService<UserCurrencyService>()?.DeInit();
            ServiceLocator.GetService<SceneLoadService>()?.DeInit();
            ServiceLocator.GetService<FirebaseAppService>()?.DeInit();
            ServiceLocator.GetService<MapService>()?.DeInit();
        }
    }
}

