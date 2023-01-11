using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[RequireComponent(typeof(AudioSource))]
class Main : MonoBehaviour
{
    
    [SerializeField]
    private TextMeshProUGUI m_Text;
    [SerializeField]
    private string m_PrefabAddress;
    [SerializeField]
    private string m_MaterialAddress;
    [SerializeField]
    private string m_JsonAddress;
    [SerializeField]
    private string m_AudioAddress;

    private AsyncOperationHandle<Material> materialHandle;
    private AsyncOperationHandle<GameObject> prefabHandle;
    private AsyncOperationHandle<TextAsset> jsonHandle;
    private AsyncOperationHandle<AudioClip> audioHandle;
    private GameObject m_Prefab;
    private AudioSource m_AudioSource;
    private AudioClip m_AddressableAudio;

    void Start()
    {
        prefabHandle = Addressables.LoadAssetAsync<GameObject>(m_PrefabAddress);
        prefabHandle.Completed += PrefabHandle_Completed;
        audioHandle = Addressables.LoadAssetAsync<AudioClip>(m_AudioAddress);
        audioHandle.Completed += AudioHandle_Completed;

        jsonHandle = Addressables.LoadAssetAsync<TextAsset>(m_JsonAddress);
        jsonHandle.Completed += JsonHandle_Completed;

        m_AudioSource = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        m_AudioSource.PlayOneShot(m_AddressableAudio, 1f);
    }

    private void PrefabHandle_Completed(AsyncOperationHandle<GameObject> operation)
    {
        if (operation.Status == AsyncOperationStatus.Succeeded)
        {
            m_Prefab = Instantiate(operation.Result, transform);
            materialHandle = Addressables.LoadAssetAsync<Material>(m_MaterialAddress);
            materialHandle.Completed += MaterialHandle_Completed;
        }
        else
        {
            Debug.LogError($"Asset for {m_PrefabAddress} failed to load.");
        }
    }

    private void AudioHandle_Completed(AsyncOperationHandle<AudioClip> operation)
    {
        if (operation.Status == AsyncOperationStatus.Succeeded)
        {
            m_AddressableAudio = operation.Result;
        }
        else
        {
            Debug.LogError($"Asset for {m_AudioAddress} failed to load.");
        }
    }

    private void MaterialHandle_Completed(AsyncOperationHandle<Material> operation)
    {
        if (operation.Status == AsyncOperationStatus.Succeeded)
        {
            m_Prefab.GetComponent<MeshRenderer>().material = operation.Result;
        }
        else
        {
            Debug.LogError($"Asset for {m_MaterialAddress} failed to load.");
        }
    }

    private void JsonHandle_Completed(AsyncOperationHandle<TextAsset> operation)
    {
        if (operation.Status == AsyncOperationStatus.Succeeded)
        {
            m_Text.text += JsonUtility.FromJson<JsonSerializedObject>(operation.Result.text).json_text;
        }
        else
        {
            Debug.LogError($"Asset for {m_JsonAddress} failed to load.");
        }
    }

    private void OnDestroy()
    {
        Addressables.Release(materialHandle);
        Addressables.Release(prefabHandle);
        Addressables.Release(jsonHandle);
    }
}

[System.Serializable]
public class JsonSerializedObject
{
    public string json_text;
}