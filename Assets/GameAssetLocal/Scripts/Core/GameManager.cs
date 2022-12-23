using System;
using System.Threading;
using Base;
using Base.Logging;
using Base.Pattern;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using Newtonsoft.Json;
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

        private SceneInstance? _sceneOperation;
        private void Awake()
        {
            InitService().Forget();
        }

        private void OnApplicationQuit()
        {
            ReleaseService();
        }

        private async UniTaskVoid InitService()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            try
            {
                await ServiceLocator.GetService<SceneLoadService>().LoadLoadingScene().AttachExternalCancellation(tokenSource.Token);

                var uiViewManager = ServiceLocator.GetService<UIViewManager>();
                uiViewManager.Init();
                uiViewManager.GetView<LoadingUI>().SetStatus("Loading Services ...", 0);


                await ServiceLocator.GetService<FirebaseAppService>().InitializeAsync();
                await ServiceLocator.GetService<AddressableManager>().InitializeAsync(cancellationToken: tokenSource.Token);

                ServiceLocator.SetService(_pooler).Init();
                ServiceLocator.GetService<UserCurrencyService>().Init();
                ServiceLocator.GetService<SceneLoadService>().Init();

                uiViewManager.GetView<LoadingUI>().SetStatus("Loading Services ...", .1f);

                await LoginAndLoadConfig();
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
            uiViewManager.GetView<LoadingUI>().SetStatus("Login ...", .2f);
            await firebaseService.LoginAnonymous();
            uiViewManager.GetView<LoadingUI>().SetStatus("Login ...", .3f);
            DataSnapshot userData = await firebaseService.LoadUserDataAsync(DatabaseKey.UserData);
            if (userData.Exists)
            {
                string json = userData.GetRawJsonValue();
                ServiceLocator.GetService<UserDataService>().From(JsonConvert.DeserializeObject<UserDataBlueprint>(json));
            }
            else
            {
                UserDataBlueprint pushData = ServiceLocator.GetService<UserDataService>().To();
                await firebaseService.SetJsonAsync(DatabaseKey.UserData, JsonConvert.SerializeObject(pushData));
            }
            DataSnapshot userCurrencies = await firebaseService.LoadUserDataAsync(DatabaseKey.Currencies);
            if (userCurrencies.Exists)
            {
                string json = userCurrencies.GetRawJsonValue();
                ServiceLocator.GetService<UserCurrencyService>()
                    .From(JsonConvert.DeserializeObject<CurrencyDataBlueprint>(json));
            }
            else
            {
                CurrencyDataBlueprint pushData = ServiceLocator.GetService<UserCurrencyService>().To();
                await firebaseService.SetJsonAsync(DatabaseKey.Currencies, JsonConvert.SerializeObject(pushData));
            }
            uiViewManager.GetView<LoadingUI>().SetStatus("Loading data ...", .5f);
        }

        private void ReleaseService()
        {
            ServiceLocator.GetService<UIViewManager>().Dispose();
            ServiceLocator.GetService<AddressableManager>().Dispose();
            ServiceLocator.GetService<ObjectPooler>().Dispose();
            ServiceLocator.GetService<UserCurrencyService>().Dispose();
            ServiceLocator.GetService<SceneLoadService>().Dispose();
            ServiceLocator.GetService<FirebaseAppService>().Dispose();
        }
    }
}

