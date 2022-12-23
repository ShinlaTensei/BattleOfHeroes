using System;
using System.Threading;
using Base;
using Base.Logging;
using Base.Pattern;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace PaidRubik
{
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


                //await ServiceLocator.GetService<FirebaseAppService>().InitializeAsync();
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
            await UniTask.Yield();
        }

        private void ReleaseService()
        {
            ServiceLocator.GetService<UIViewManager>().Dispose();
            ServiceLocator.GetService<AddressableManager>().Dispose();
            ServiceLocator.GetService<ObjectPooler>().Dispose();
            ServiceLocator.GetService<UserCurrencyService>().Dispose();
            ServiceLocator.GetService<SceneLoadService>().Dispose();
            //ServiceLocator.GetService<FirebaseAppService>().Dispose();
        }
    }
}

