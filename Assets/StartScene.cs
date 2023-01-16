using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;

namespace Addressales.StartScene
{
    public class StartScene : MonoBehaviour
    {
        public string m_AddressableSceneAddress;
        public Button m_StartSceneButton;
        public GameObject m_PressXText;
        private SceneInstance m_AddressableScene;

        #region Lifecycle
        private void Awake()
        {
            if (m_StartSceneButton != null)
            {
                m_StartSceneButton.onClick.AddListener(delegate
                {
                    LoadSceneAsset();
                });
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                m_StartSceneButton.gameObject.SetActive(true);
                m_PressXText.gameObject.SetActive(false);

                UnloadScene();
            }
        }

        private void OnDestroy()
        {
            if (m_StartSceneButton != null)
            {
                m_StartSceneButton.onClick.RemoveAllListeners();
            }
        }
        #endregion

        #region Async
        public async void LoadSceneAsset()
        {
            await LoadAssetAsync();
        }

        async void UnloadScene()
        {
            await Addressables.UnloadSceneAsync(m_AddressableScene, true).Task;
        }

        async Task LoadAssetAsync()
        {
            m_AddressableScene = await Addressables.LoadSceneAsync(m_AddressableSceneAddress, UnityEngine.SceneManagement.LoadSceneMode.Additive).Task;
            m_StartSceneButton.gameObject.SetActive(false);
            m_PressXText.gameObject.SetActive(true);
        }
        #endregion
    }
}

