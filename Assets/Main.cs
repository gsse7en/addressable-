using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UnityEngine.Video;

[System.Serializable]
public class JsonSerializedObject
{
    public string json_text;
}

[RequireComponent(typeof(AudioSource))]
class Main : MonoBehaviour
{
    
    [SerializeField]
    private TextMeshProUGUI m_Text;
    [SerializeField]
    private Image m_Image;
    [SerializeField]
    private VideoPlayer m_VideoPlayer;
    [SerializeField]
    private string m_PrefabAddress;
    [SerializeField]
    private string m_MaterialAddress;
    [SerializeField]
    private string m_JsonAddress;
    [SerializeField]
    private string m_AudioAddress;
    [SerializeField]
    private string m_VideoAddress;
    [SerializeField]
    private string m_SpriteAddress;


    private AsyncOperationHandle<Material> materialHandle;
    private AsyncOperationHandle<GameObject> prefabHandle;
    private AsyncOperationHandle<TextAsset> jsonHandle;
    private AsyncOperationHandle<AudioClip> audioHandle;
    private AsyncOperationHandle<VideoClip> videoHandle;
    private AsyncOperationHandle<Sprite> spriteHandle;
    private GameObject m_Prefab;
    private AudioSource m_AudioSource;
    private AudioClip m_AddressableAudio;
    private Sprite m_AddressableSprite;

    void Start()
    {
        prefabHandle = Addressables.LoadAssetAsync<GameObject>(m_PrefabAddress);
        prefabHandle.Completed += PrefabHandle_Completed;
        audioHandle = Addressables.LoadAssetAsync<AudioClip>(m_AudioAddress);
        audioHandle.Completed += AudioHandle_Completed;
        videoHandle = Addressables.LoadAssetAsync<VideoClip>(m_VideoAddress);
        videoHandle.Completed += VideoHandle_Completed;
        spriteHandle = Addressables.LoadAssetAsync<Sprite>(m_SpriteAddress);
        spriteHandle.Completed += SpriteHandle_Completed;
        jsonHandle = Addressables.LoadAssetAsync<TextAsset>(m_JsonAddress);
        jsonHandle.Completed += JsonHandle_Completed;

        m_AudioSource = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        m_AudioSource.PlayOneShot(m_AddressableAudio, 1f);
    }

    public void PlayVideo()
    {
        m_VideoPlayer.Play();
    }

    public void ShowPicture()
    {
        m_Image.sprite = m_AddressableSprite;
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

    private void SpriteHandle_Completed(AsyncOperationHandle<Sprite> operation)
    {
        if (operation.Status == AsyncOperationStatus.Succeeded)
        {
            m_AddressableSprite = operation.Result;
        }
        else
        {
            Debug.LogError($"Asset for {m_AudioAddress} failed to load.");
        }
    }

    private void VideoHandle_Completed(AsyncOperationHandle<VideoClip> operation)
    {
        if (operation.Status == AsyncOperationStatus.Succeeded)
        {
            m_VideoPlayer.clip = operation.Result;
        }
        else
        {
            Debug.LogError($"Asset for {m_AudioAddress} failed to load.");
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
        Addressables.Release(audioHandle);
        Addressables.Release(videoHandle);
        Addressables.Release(spriteHandle);
    }
}
