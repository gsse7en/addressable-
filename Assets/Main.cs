using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Addressales.Main
{
    public class PrefabsFactory : MonoBehaviour
    {
        public GameObject GetPositionedPrefab(GameObject prefab)
        {
            GameObject gameOjectInstance = Instantiate(prefab);
            Vector3 position = new Vector3(Random.Range(-3, 3), Random.Range(-1, 3), 10);
            gameOjectInstance.transform.position = position;
            return gameOjectInstance;
        }
    }

    [System.Serializable]
    public class JsonSerializedObject
    {
        public string json_text;
    }

    class Main : MonoBehaviour
    {
        [SerializeField] private Text m_Text;
        [SerializeField] private Image m_Image;
        [SerializeField] private VideoPlayer m_VideoPlayer;
        [SerializeField] private Button m_AudioButton;
        [SerializeField] private Button m_VideoButton;
        [SerializeField] private Button m_SpriteButton;
        [SerializeField] private Button m_SpawnRandomPrefab;
        [SerializeField] private string m_PrefabsLabel;
        [SerializeField] private string m_MaterialAddress;
        [SerializeField] private string m_JsonAddress;
        [SerializeField] private string m_AudioAddress;
        [SerializeField] private string m_VideoAddress;
        [SerializeField] private string m_SpriteAddress;
        [SerializeField] private int m_SawnedObjectLifespan = 2000;
        private PrefabsFactory m_SpawnFactory = new PrefabsFactory();
        private IList<GameObject> m_Prefabs = new List<GameObject>();
        private AudioSource m_AudioSource;
        private AudioClip m_AddressableAudio;
        private Sprite m_AddressableSprite;

        #region Lifecycle
        private void Awake()
        {
            m_AudioButton?.onClick.AddListener(delegate { PlaySound(); });
            m_VideoButton?.onClick.AddListener(delegate { PlayVideo(); });
            m_SpriteButton?.onClick.AddListener(delegate { ShowPicture(); });
            m_SpawnRandomPrefab?.onClick.AddListener(delegate { SpawnPrefab(); });
        }

        void Start()
        {
            m_AudioSource = GetComponent<AudioSource>();

            LoadAssets();
        }

        private void OnDestroy()
        {
            m_AudioButton?.onClick.RemoveAllListeners();
            m_VideoButton?.onClick.RemoveAllListeners();
            m_SpriteButton?.onClick.RemoveAllListeners();
            m_SpawnRandomPrefab?.onClick.RemoveAllListeners();
        }
        #endregion

        #region Async
        async void LoadAssets()
        {
            await LoadAssetAsync();
        }

        async Task LoadAssetAsync()
        {
            m_AddressableAudio = await Addressables.LoadAssetAsync<AudioClip>(m_AudioAddress).Task;
            m_VideoPlayer.clip = await Addressables.LoadAssetAsync<VideoClip>(m_VideoAddress).Task;
            m_AddressableSprite = await Addressables.LoadAssetAsync<Sprite>(m_SpriteAddress).Task;
            TextAsset json_string = await Addressables.LoadAssetAsync<TextAsset>(m_JsonAddress).Task;
            m_Text.text += JsonUtility.FromJson<JsonSerializedObject>(json_string.ToString()).json_text;
            m_Prefabs = await Addressables.LoadAssetsAsync<GameObject>(m_PrefabsLabel, null).Task;
        }
        #endregion

        #region Delegates
        private void PlaySound()
        {
            m_AudioSource.PlayOneShot(m_AddressableAudio, 1f);
        }

        private void PlayVideo()
        {
            m_VideoPlayer.Play();
        }

        private void ShowPicture()
        {
            m_Image.sprite = m_AddressableSprite;
        }

        async void SpawnPrefab()
        {
            if (m_Prefabs.Count == 0) return;

            int randIndex = Random.Range(0, m_Prefabs.Count);
            var spawnedPrefab = m_SpawnFactory.GetPositionedPrefab(m_Prefabs[randIndex]);
            await Task.Delay(m_SawnedObjectLifespan);

            Destroy(spawnedPrefab);
        }
        #endregion
    }
}

