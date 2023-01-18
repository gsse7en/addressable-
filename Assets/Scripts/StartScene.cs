using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;

namespace AddressablesSample.StartScene
{
    public class StartScene : MonoBehaviour
    {
        public string m_AddressableSceneAddress;
        public Button m_StartSceneButton;
        public Button m_PressXText;
        private SceneInstance m_AddressableScene;

        #region Lifecycle
        private void Awake()
        {
            m_StartSceneButton?.onClick.AddListener(async delegate
            {
                await LoadSceneDidClickedAsync();
            });

            m_PressXText?.onClick.AddListener(async delegate
            {
                m_StartSceneButton.gameObject.SetActive(true);
                m_PressXText.gameObject.SetActive(false);
                await UnloadSceneDidClickedAsync();
            });
        }

        private void OnDestroy()
        {
            m_StartSceneButton?.onClick.RemoveAllListeners();
            m_PressXText?.onClick.RemoveAllListeners();
        }
        #endregion

        #region Delegates
        private async Task LoadSceneDidClickedAsync()
        {
            m_StartSceneButton.gameObject.SetActive(false);
            m_PressXText.gameObject.SetActive(true);

            try
            {
                m_AddressableScene = await Addressables.LoadSceneAsync(m_AddressableSceneAddress, UnityEngine.SceneManagement.LoadSceneMode.Additive).Task;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private async Task UnloadSceneDidClickedAsync()
        {
            try
            {
                await Addressables.UnloadSceneAsync(m_AddressableScene, true).Task;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        #endregion
    }
}

