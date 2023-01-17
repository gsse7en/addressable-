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
        public string json_text;
    }

    class LoadAddressables : MonoBehaviour
    {
        [SerializeField] private Image m_Image;
        [SerializeField] private VideoPlayer m_VideoPlayer;
        [SerializeField] private Button m_VideoButton;
        [SerializeField] private Button m_SpriteButton;
        [SerializeField] private Button m_SpawnRandomPrefab;
        [SerializeField] private string m_PrefabsLabel;
        [SerializeField] private string m_MaterialAddress;
        [SerializeField] private string m_JsonAddress;
        [SerializeField] private string m_AudioAddress;
        [SerializeField] private string m_VideoAddress;
        [SerializeField] private string m_SpriteAddress;
        [SerializeField] private int m_SawnedObjectLifespan = 2;
        private IList<GameObject> m_Prefabs = new List<GameObject>();
        private AudioClip m_AddressableAudio;
        private Sprite m_AddressableSprite;

        #region Lifecycle
        private void Awake()
        {
            m_VideoButton?.onClick.AddListener(delegate { PlayVideo(); });
            m_SpriteButton?.onClick.AddListener(delegate { ShowPicture(); });
            m_SpawnRandomPrefab?.onClick.AddListener(delegate { ButtonSpawnPrefab(); });
        }

        private void Start()
        {
            Load();
        }

        private void OnDestroy()
        {
            m_VideoButton?.onClick.RemoveAllListeners();
            m_SpriteButton?.onClick.RemoveAllListeners();
            m_SpawnRandomPrefab?.onClick.RemoveAllListeners();
        }
        #endregion

        #region Async
        async void  Load()
        {
            try
            {
                await LoadAssetAsync();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        async Task LoadAssetAsync()
        {
            var json_string = await Addressables.LoadAssetAsync<TextAsset>(m_JsonAddress).Task;
            //m_Text.text += JsonUtility.FromJson<JsonSerializedObject>(json_string.ToString()).json_text;
            m_AddressableAudio = await Addressables.LoadAssetAsync<AudioClip>(m_AudioAddress).Task;
            m_VideoPlayer.clip = await Addressables.LoadAssetAsync<VideoClip>(m_VideoAddress).Task;
            m_AddressableSprite = await Addressables.LoadAssetAsync<Sprite>(m_SpriteAddress).Task;
            m_Prefabs = await Addressables.LoadAssetsAsync<GameObject>(m_PrefabsLabel, SpawnPrefab).Task;
        }

        async Task DelayAsync(int delay)
        {
            try
            {
                await Task.Delay(delay);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        #endregion

        #region Private
        async void DestroyPrefab(GameObject prefab, int lifeSpan)
        {
            await DelayAsync(lifeSpan);
            Destroy(prefab);
        }

        private void SpawnPrefab(GameObject prefab)
        {
            if (prefab == null) return;
            var position = new Vector3(Random.Range(-3, 3), Random.Range(-1, 3), 10);
            var prefabToDestroy = SpawnerManager.Spawn(prefab, position, m_AddressableAudio);

            SpawnerManager.DestroyPrefab(prefabToDestroy, m_SawnedObjectLifespan).Start();
        }
        #endregion

        #region Delegates
        private void PlayVideo()
        {
            m_VideoPlayer.Play();
        }

        private void ShowPicture()
        {
            m_Image.sprite = m_AddressableSprite;
        }

        private void ButtonSpawnPrefab()
        {
            if (m_Prefabs.Count == 0) return;

            var randIndex = Random.Range(0, m_Prefabs.Count);

            SpawnPrefab(m_Prefabs[randIndex]);
        }
        #endregion
    }
}

