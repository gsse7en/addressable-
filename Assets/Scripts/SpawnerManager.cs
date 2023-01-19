using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AddressablesSample.Spawner
{
    public class SpawnerManager : MonoBehaviour
    {
        [SerializeField] private int m_Delay = 1000;
        [SerializeField] private float m_xPosRange = 3;
        [SerializeField] private float m_yPosRange = 2;
        [SerializeField] private float m_zPos = 10;
        private List<GameObject> m_Prefabs = new List<GameObject>();
        private List<GameObject> m_SpawnedPrefabs = new List<GameObject>();
        private AudioClip m_clip;

        #region Lifecycle
        private void Awake()
        {
            StartSpawnerAync().GetAwaiter();
        }

        private void OnDestroy()
        {
            foreach (GameObject prefab in m_SpawnedPrefabs) Destroy(prefab);
            m_SpawnedPrefabs.Clear();
        }
        #endregion

        #region Public
        public void AddPrefab(GameObject prefab)
        {
            m_Prefabs.Add(prefab);
        }

        public void SetSpawnSound(AudioClip clip)
        {
            m_clip = clip;
        }
        #endregion

        #region Async
        private async Task StartSpawnerAync()
        {
            while (true)
            {
                if (gameObject.activeSelf) CreateInstance();
                else Destroy();

                await Task.Delay(m_Delay);
            }
        }
        #endregion

        #region Private
        private void CreateInstance()
        {
            if (m_Prefabs.Count > 0)
            {
                var gameOjectInstance = SpawnRandomPrefab();
                PlaySound(gameOjectInstance);
                m_SpawnedPrefabs.Add(gameOjectInstance);
            }
        }

        private void Destroy()
        {
            if (m_SpawnedPrefabs.Count > 0)
            {
                Destroy(m_SpawnedPrefabs[m_SpawnedPrefabs.Count - 1]);
                m_SpawnedPrefabs.RemoveAt(m_SpawnedPrefabs.Count - 1);
            } else
            {
                m_SpawnedPrefabs.Clear();
            }
        }

        private GameObject SpawnRandomPrefab()
        {
            var randIndex = Random.Range(0, m_Prefabs.Count);
            var position = new Vector3(Random.Range(-m_xPosRange, m_xPosRange), Random.Range(-m_yPosRange + 1, m_yPosRange + 1), m_zPos);
            var obj = Instantiate(m_Prefabs[randIndex]);
            obj.transform.position = position;
            return obj;
        }

        private void PlaySound(GameObject prefab)
        {
            if (m_clip == null) return;

            var objectSoundSource = prefab.GetComponent<AudioSource>();
            objectSoundSource.clip = m_clip;
            objectSoundSource.Play();
        }
        #endregion
    }
}
