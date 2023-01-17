using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Addressales.SpawnManager
{
    public class Spawner : MonoBehaviour
    {
        public static GameObject Spawn(GameObject prefab, Vector3 position)
        {
            var gameOjectInstance = Instantiate(prefab);
            gameOjectInstance.transform.position = position;
            return gameOjectInstance;
        }
    }
}
