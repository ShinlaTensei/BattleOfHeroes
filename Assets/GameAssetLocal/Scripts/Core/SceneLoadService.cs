using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Base;
using Base.Pattern;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace PaidRubik
{
    public class SceneLoadService : IService, IDisposable
    {
        private CancellationTokenSource _tokenOnDispose;
        public void Init()
        {
            _tokenOnDispose = new CancellationTokenSource();
        }

        public void Dispose()
        {
            if (!_tokenOnDispose.IsCancellationRequested)
            {
                _tokenOnDispose.Cancel();
                _tokenOnDispose.Dispose();
            }

            _tokenOnDispose = null;
        }

        public async UniTask LoadHomeScene(AssetReference sceneRef)
        {
            SceneInstance? sceneInstance = await ServiceLocator.GetService<AddressableManager>()
                .LoadSceneAsync(sceneRef, LoadSceneMode.Additive, cancellationToken: _tokenOnDispose.Token);

            if (sceneInstance.HasValue)
            {
                await sceneInstance.Value.ActivateAsync();
            }
        }

        public async UniTask LoadLoadingScene()
        {
            await SceneManager.LoadSceneAsync(SceneName.LoadingScene, LoadSceneMode.Additive).ToUniTask();
        }

        public async UniTask UnLoadLoadingScene()
        {
            await SceneManager.UnloadSceneAsync(SceneName.LoadingScene).ToUniTask();
        }
    }

    public static class SceneName
    {
        public const string HomeScene = "HomeScene";
        public const string GameScene = "GameScene";
        public const string LoadingScene = "LoadingScene";
    }
}

