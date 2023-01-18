using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AddressablesSample.Spawner
{
    public class SpawnerManager : MonoBehaviour
    {
        public GameObject Spawn(GameObject prefab, Vector3 position)
        {
            var gameOjectInstance = Instantiate(prefab);
            gameOjectInstance.transform.position = position;

            return gameOjectInstance;
        }
    }
}
