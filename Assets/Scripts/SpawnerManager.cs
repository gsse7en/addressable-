using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AddressablesSample.Spawner
{
    public class SpawnerManager : MonoBehaviour
    {

        private List<GameObject> m_Prefabs = new List<GameObject>();
        private void Awake()
        {
            if (m_Prefabs.Count == 0) return;
            while (gameObject.activeSelf)
            {
                var randIndex = Random.Range(0, m_Prefabs.Count);
                var prefab = Spawn(m_Prefabs[randIndex], Vector3.zero);
            }
        }

        public void AddPrefab(GameObject prefab)
        {
            m_Prefabs.Add(prefab);
        }
        public GameObject Spawn(GameObject prefab, Vector3 position)
        {
            var gameOjectInstance = Instantiate(prefab);
            gameOjectInstance.transform.position = position;

            return gameOjectInstance;
        }

        //public void OnDestroy()
        //{
        //    public spawnPrefab
        //    private async destroyPrefab
        //    awake spawnPrefab+destroyPrefab
        //    onDestroy spawnPrefabdestroyPrefab
        //}
        //private void SpawnPrefabDidClickedAsync()
        //{
        //    if (m_Prefabs.Count == 0) return;

        //    var randIndex = Random.Range(0, m_Prefabs.Count);
        //    var prefab = SpawnPrefab(m_Prefabs[randIndex]);
        //    PlaySound(prefab, m_AddressableAudio);
        //    DestroyPrefabAsync(prefab, m_SawnedObjectLifespan).GetAwaiter();
        //}
    }
}
