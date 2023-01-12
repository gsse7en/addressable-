using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;


public class StartScene : MonoBehaviour, ISingleton
{
    public static StartScene Instance { get; private set; }
    public string AddressableSceneAddress;
    public Button m_StartSceneButton;
    private AsyncOperationHandle<SceneInstance> sceneHandle;

    #region Lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (Instance.m_StartSceneButton != null)
        {
            Instance.m_StartSceneButton.onClick.AddListener(delegate
            {
                Addressables.LoadSceneAsync(AddressableSceneAddress, UnityEngine.SceneManagement.LoadSceneMode.Additive).Completed += SceneLoadComplete;
            });
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Instance.m_StartSceneButton.gameObject.SetActive(true);
            Instance.UnloadScene();
        }
    }
    #endregion

    #region Private
    void UnloadScene()
    {
        if (Instance.sceneHandle.IsValid())
            Addressables.UnloadSceneAsync(Instance.sceneHandle, true).Completed += op =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                    Debug.Log("Unloaded Scene");
            };
        else
            Debug.LogError("sceneHandle n/v");
    }

    void SceneLoadComplete(AsyncOperationHandle<SceneInstance> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Loaded Scene: " + obj.Result.Scene.name);
            Instance.sceneHandle = obj;
            Instance.m_StartSceneButton.gameObject.SetActive(false);
        }
    }
    #endregion
}
