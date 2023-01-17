using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Addressales.Spawner
{
    public class SpawnerManager : MonoBehaviour
    {
        public static GameObject Spawn(GameObject prefab, Vector3 position, AudioClip playSound)
        {
            var gameOjectInstance = Instantiate(prefab);
            gameOjectInstance.transform.position = position;
            PlaySound(gameOjectInstance, playSound);
            return gameOjectInstance;
        }

        private static void PlaySound(GameObject prefab, AudioClip clip)
        {
            var objectSoundSource = prefab.GetComponent<AudioSource>();
            objectSoundSource.clip = clip;
            objectSoundSource.Play();
        }

        public static async Task DestroyPrefab(GameObject prefab, int time)
        {
            await Task.Delay(time);
            Destroy(prefab);
        }
    }
}
