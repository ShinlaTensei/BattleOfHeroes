using Base;
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
            ServiceLocator.GetService<UIViewManager>().Init();
            
            await ServiceLocator.GetService<AddressableManager>().InitializeAsync();
            
            ServiceLocator.SetService(_pooler).Init();
            ServiceLocator.GetService<UserCurrencyService>().Init();
            ServiceLocator.GetService<SceneLoadService>().Init();
            
            await ServiceLocator.GetService<SceneLoadService>().LoadHomeScene(homeSceneRef);
        }

        private void ReleaseService()
        {
            ServiceLocator.GetService<UIViewManager>().Dispose();
            ServiceLocator.GetService<AddressableManager>().Dispose();
            ServiceLocator.GetService<ObjectPooler>().Dispose();
            ServiceLocator.GetService<UserCurrencyService>().Dispose();
            ServiceLocator.GetService<SceneLoadService>().Dispose();
        }
    }
}

