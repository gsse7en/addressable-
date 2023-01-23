using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AddressablesSample.Spawner
{
    [System.Serializable]
    public class JsonSerializedObject
    {
        public List<string> labels;
    }

    [System.Serializable]
    public struct DelayData
    {
        [Range(0, 1000)]
        public int From;
        [Range(1000, 5000)]
        public int To;
    }

    public class SpawnerManager : MonoBehaviour
    {
        [SerializeField] private string m_JsonAddress;
        [SerializeField] private string m_AudioAddress;
        [SerializeField] private DelayData m_Delay;
        [SerializeField] private float m_xPosRange = 3;
        [SerializeField] private float m_yPosRange = 2;
        [SerializeField] private float m_zPos = 10;
        private AudioClip m_AddressableAudio;
        private List<GameObject> m_SpawnedObjects = new List<GameObject>();

        #region Lifecycle
        private void OnEnable()
        {
            if (m_SpawnedObjects.Count > 0)
            {
                DestroyAll();
            }

            LoadAssetsAsync();
        }

        private void OnDisable()
        {
            DestroyAll();
        }
        #endregion

        #region Async
        private async Task LoadAssetsAsync()
        {
            var json_string = await Addressables.LoadAssetAsync<TextAsset>(m_JsonAddress).Task;
            var labelsList = JsonUtility.FromJson<JsonSerializedObject>(json_string.ToString());
            await Addressables.LoadAssetsAsync<GameObject>(labelsList.labels, PrefabLoaded, Addressables.MergeMode.Union, false).Task;
        }
        #endregion

        #region Private
        private void PrefabLoaded(GameObject prefab)
        {
            CreatePrefabAsync(prefab);
        }

        private async Task CreatePrefabAsync(GameObject prefab)
        {
            await Task.Delay(Random.Range(m_Delay.From, m_Delay.To));
            var position = new Vector3(Random.Range(-m_xPosRange, m_xPosRange), Random.Range(-m_yPosRange + 1, m_yPosRange + 1), m_zPos);
            var obj = Instantiate(prefab);
            obj.transform.position = position;
            obj.AddComponent<AudioSource>();
            m_AddressableAudio = await Addressables.LoadAssetAsync<AudioClip>(m_AudioAddress).Task;
            obj.GetComponent<AudioSource>().PlayOneShot(m_AddressableAudio);
            m_SpawnedObjects.Add(obj);
        }

        private void DestroyAll()
        {
            foreach (GameObject prefab in m_SpawnedObjects) Destroy(prefab);
            m_SpawnedObjects.Clear();
        }
        #endregion
    }
}
