using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    public string AddressableSceneAddress;
    public Button m_StartSceneButton;
    private SceneInstance m_AddressableScene;

    public async void Caller()
    {
        await LoadAssetAsync();
    }

    async void UnloadScene()
    {
        await Addressables.UnloadSceneAsync(m_AddressableScene, true).Task;
    }

    async Task LoadAssetAsync()
    {
        m_AddressableScene = await Addressables.LoadSceneAsync(AddressableSceneAddress, UnityEngine.SceneManagement.LoadSceneMode.Additive).Task;
        m_StartSceneButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            m_StartSceneButton.gameObject.SetActive(true);
            UnloadScene();
        }
    }
}
