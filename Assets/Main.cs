using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Video;

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
    [SerializeField]
    private VideoPlayer m_VideoPlayer;

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

    //void Start()
    //{
    //    // Will attach a VideoPlayer to the main camera.
    //    GameObject camera = GameObject.Find("Main Camera");

    //    // VideoPlayer automatically targets the camera backplane when it is added
    //    // to a camera object, no need to change videoPlayer.targetCamera.
    //    var videoPlayer = camera.AddComponent<UnityEngine.Video.VideoPlayer>();

    //    // Play on awake defaults to true. Set it to false to avoid the url set
    //    // below to auto-start playback since we're in Start().
    //    videoPlayer.playOnAwake = false;

    //    // By default, VideoPlayers added to a camera will use the far plane.
    //    // Let's target the near plane instead.
    //    videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;

    //    // This will cause our Scene to be visible through the video being played.
    //    videoPlayer.targetCameraAlpha = 0.5F;

    //    // Set the video to play. URL supports local absolute or relative paths.
    //    // Here, using absolute.
    //    videoPlayer.url = "/Users/graham/movie.mov";

    //    // Skip the first 100 frames.
    //    videoPlayer.frame = 100;

    //    // Restart from beginning when done.
    //    videoPlayer.isLooping = true;

    //    // Each time we reach the end, we slow down the playback by a factor of 10.
    //    videoPlayer.loopPointReached += EndReached;

    //    // Start playback. This means the VideoPlayer may have to prepare (reserve
    //    // resources, pre-load a few frames, etc.). To better control the delays
    //    // associated with this preparation one can use videoPlayer.Prepare() along with
    //    // its prepareCompleted event.
    //    videoPlayer.Play();
    //}

    public void PlaySound()
    {
        m_AudioSource.PlayOneShot(m_AddressableAudio, 1f);
    }

    public void PlayVideo()
    {
        m_VideoPlayer.Play();
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