using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Threading.Tasks;
using System.Collections.Generic;
using Addressales.Spawner;

namespace Addressales.Load
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
        [SerializeField] private Button m_SpawnRandomPrefab;
        [SerializeField] private string m_MaterialAddress;
        [SerializeField] private string m_JsonAddress;
        [SerializeField] private string m_AudioAddress;
        [SerializeField] private string m_VideoAddress;
        [SerializeField] private string m_SpriteAddress;
        [SerializeField] private int m_SawnedObjectLifespan = 2;
        [SerializeField] private float m_xPosRange = 3;
        [SerializeField] private float m_yPosRange = 2;
        [SerializeField] private float m_zPos = 10;
        private IList<GameObject> m_Prefabs = new List<GameObject>();
        private AudioClip m_AddressableAudio;
        private Sprite m_AddressableSprite;
        private List<string> m_labelsList = new List<string>();

        #region Lifecycle
        private async void Awake()
        {
            m_VideoButton?.onClick.AddListener(delegate { PlayVideoDidClicked(); });
            m_SpriteButton?.onClick.AddListener(delegate { ShowPictureDidClicked(); });
            m_SpawnRandomPrefab?.onClick.AddListener(delegate { SpawnPrefabDidClicked(); });
            await LoadAssetAsync();
        }

        private void OnDestroy()
        {
            m_VideoButton?.onClick.RemoveAllListeners();
            m_SpriteButton?.onClick.RemoveAllListeners();
            m_SpawnRandomPrefab?.onClick.RemoveAllListeners();
        }
        #endregion

        #region Private
        private void AddAudioSource(GameObject prefab)
        {
            prefab.AddComponent<AudioSource>();
        }
        #endregion

        #region Async
        private async Task LoadAssetAsync()
        {
            m_AddressableAudio = await Addressables.LoadAssetAsync<AudioClip>(m_AudioAddress).Task;
            m_VideoPlayer.clip = await Addressables.LoadAssetAsync<VideoClip>(m_VideoAddress).Task;
            m_AddressableSprite = await Addressables.LoadAssetAsync<Sprite>(m_SpriteAddress).Task;
            var json_string = await Addressables.LoadAssetAsync<TextAsset>(m_JsonAddress).Task;
            HandleJson(json_string.ToString());
            m_Prefabs = await Addressables.LoadAssetsAsync<GameObject>(m_labelsList, AddAudioSource, Addressables.MergeMode.Union, false).Task;
        }

        private void HandleJson(string json)
        {
            var labelsList = JsonUtility.FromJson<JsonSerializedObject>(json.ToString());
            m_labelsList.AddRange(labelsList.labels);
        }

        private async Task SpawnPrefab(GameObject prefab)
        {
            if (prefab == null) return;

            var position = new Vector3(Random.Range(-m_xPosRange, m_xPosRange), Random.Range(-m_yPosRange+1, m_yPosRange+1), m_zPos);
            var prefabToDestroy = m_spawnerManager.Spawn(prefab, m_AddressableAudio, position);

            try
            {
                await m_spawnerManager.DestroyPrefab(prefabToDestroy, m_SawnedObjectLifespan);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
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

        private async void SpawnPrefabDidClicked()
        {
            if (m_Prefabs.Count == 0) return;

            var randIndex = Random.Range(0, m_Prefabs.Count);

            try
            {
                await SpawnPrefab(m_Prefabs[randIndex]);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        #endregion
    }
}

