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
            m_StartSceneButton?.onClick.AddListener(delegate
            {
                LoadSceneAsset();
            });
        }

        private void Update()
        {
            //TODO: switch to button
            if (Input.GetKeyDown(KeyCode.X))
            {
                m_StartSceneButton.gameObject.SetActive(true);
                m_PressXText.gameObject.SetActive(false);

                UnloadScene();
            }
        }

        private void OnDestroy()
        {
            m_StartSceneButton?.onClick.RemoveAllListeners();
        }
        #endregion

        #region Async
        private async void LoadSceneAsset()
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

        private async void UnloadScene()
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

