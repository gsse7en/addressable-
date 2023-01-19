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
        private List<string> m_LabelsList = new List<string>();

        #region Lifecycle
        private void Awake()
        {
            m_VideoButton?.onClick.AddListener(delegate { PlayVideoDidClicked(); });
            m_SpriteButton?.onClick.AddListener(delegate { ShowPictureDidClicked(); });
            m_SpawnRandomPrefab?.onClick.AddListener(delegate { SpawnPrefabDidClickedAsync(); });
            LoadAssetsAsync().GetAwaiter();
        }

        private void OnDestroy()
        {
            m_VideoButton?.onClick.RemoveAllListeners();
            m_SpriteButton?.onClick.RemoveAllListeners();
            m_SpawnRandomPrefab?.onClick.RemoveAllListeners();
        }
        #endregion

        #region Async
        private async Task LoadAssetsAsync()
        {
            m_AddressableAudio = await Addressables.LoadAssetAsync<AudioClip>(m_AudioAddress).Task;
            m_VideoPlayer.clip = await Addressables.LoadAssetAsync<VideoClip>(m_VideoAddress).Task;
            m_AddressableSprite = await Addressables.LoadAssetAsync<Sprite>(m_SpriteAddress).Task;
            var json_string = await Addressables.LoadAssetAsync<TextAsset>(m_JsonAddress).Task;
            var labelsList = JsonUtility.FromJson<JsonSerializedObject>(json_string.ToString());
            m_LabelsList.AddRange(labelsList.labels);
            m_Prefabs = await Addressables.LoadAssetsAsync<GameObject>(m_LabelsList, AddAudioSource, Addressables.MergeMode.Union, false).Task;
        }

        public async Task DestroyPrefabAsync(GameObject prefab, int waitTime)
        {
            await Task.Delay(waitTime);
            DestroyImmediate(prefab);
        }
        #endregion

        #region Private
        private GameObject SpawnPrefab(GameObject prefab)
        {
            if (prefab == null) return null;

            var position = new Vector3(Random.Range(-m_xPosRange, m_xPosRange), Random.Range(-m_yPosRange + 1, m_yPosRange + 1), m_zPos);
            var prefabToDestroy = m_spawnerManager.Spawn(prefab, position);
            return prefabToDestroy;
        }

        private void PlaySound(GameObject prefab, AudioClip clip)
        {
            var objectSoundSource = prefab.GetComponent<AudioSource>();
            objectSoundSource.clip = clip;
            objectSoundSource.Play();
        }

        private void AddAudioSource(GameObject prefab)
        {
            prefab.AddComponent<AudioSource>();
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

        private void SpawnPrefabDidClickedAsync()
        {
            if (m_Prefabs.Count == 0) return;

            var randIndex = Random.Range(0, m_Prefabs.Count);
            var prefab = SpawnPrefab(m_Prefabs[randIndex]);
            PlaySound(prefab, m_AddressableAudio);
            DestroyPrefabAsync(prefab, m_SawnedObjectLifespan).GetAwaiter();
        }
        #endregion
    }
}

