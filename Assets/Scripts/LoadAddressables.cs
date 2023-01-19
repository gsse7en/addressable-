using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Threading.Tasks;
using System.Collections.Generic;
using AddressablesSample.Spawner;

namespace AddressablesSample.Load
{
    [System.Serializable]
    public class JsonSerializedObject
    {
        public string[] labels;
    }

    class LoadAddressables : MonoBehaviour
    {
        [SerializeField] private SpawnerManager m_spawnerManager;
        [SerializeField] private Image m_Image;
        [SerializeField] private VideoPlayer m_VideoPlayer;
        [SerializeField] private Button m_VideoButton;
        [SerializeField] private Button m_SpriteButton;
        [SerializeField] private Button m_SpawnPrefabs;
        [SerializeField] private string m_MaterialAddress;
        [SerializeField] private string m_JsonAddress;
        [SerializeField] private string m_AudioAddress;
        [SerializeField] private string m_VideoAddress;
        [SerializeField] private string m_SpriteAddress;
        
        private IList<GameObject> m_Prefabs = new List<GameObject>();
        private AudioClip m_AddressableAudio;
        private Sprite m_AddressableSprite;
        private List<string> m_LabelsList = new List<string>();

        #region Lifecycle
        private void Awake()
        {
            m_VideoButton?.onClick.AddListener(delegate { PlayVideoDidClicked(); });
            m_SpriteButton?.onClick.AddListener(delegate { ShowPictureDidClicked(); });
            m_SpawnPrefabs?.onClick.AddListener(delegate { SpawnPrefabsDidClicked(); });
            LoadAssetsAsync().GetAwaiter();
        }

        private void OnDestroy()
        {
            m_VideoButton?.onClick.RemoveAllListeners();
            m_SpriteButton?.onClick.RemoveAllListeners();
            m_SpawnPrefabs?.onClick.RemoveAllListeners();
        }
        #endregion

        #region Async
        private async Task LoadAssetsAsync()
        {
            m_AddressableAudio = await Addressables.LoadAssetAsync<AudioClip>(m_AudioAddress).Task;
            m_spawnerManager.SetSpawnSound(m_AddressableAudio);
            m_VideoPlayer.clip = await Addressables.LoadAssetAsync<VideoClip>(m_VideoAddress).Task;
            m_AddressableSprite = await Addressables.LoadAssetAsync<Sprite>(m_SpriteAddress).Task;
            var json_string = await Addressables.LoadAssetAsync<TextAsset>(m_JsonAddress).Task;
            var labelsList = JsonUtility.FromJson<JsonSerializedObject>(json_string.ToString());
            m_LabelsList.AddRange(labelsList.labels);
            m_Prefabs = await Addressables.LoadAssetsAsync<GameObject>(m_LabelsList, AddToSpawnManager, Addressables.MergeMode.Union, false).Task;
        }
        #endregion

        #region Private
        private void AddToSpawnManager(GameObject prefab)
        {
            prefab.AddComponent<AudioSource>();
            m_spawnerManager.AddPrefab(prefab);
        }
        #endregion

        #region Delegates
        private void PlayVideoDidClicked()
        {
            m_VideoPlayer.Play();
        }

        private void ShowPictureDidClicked()
        {
            m_Image.sprite = m_AddressableSprite;
        }

        private void SpawnPrefabsDidClicked()
        {
            m_spawnerManager.gameObject.SetActive(!m_spawnerManager.gameObject.activeSelf);
        }
        #endregion
    }
}

