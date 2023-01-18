using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Addressales.Spawner
{
    public class SpawnerManager : MonoBehaviour
    {
        public GameObject Spawn(GameObject prefab, AudioClip playSound, Vector3 position)
        {
            var gameOjectInstance = Instantiate(prefab);
            gameOjectInstance.transform.position = position;
            PlaySound(gameOjectInstance, playSound);

            return gameOjectInstance;
        }

        private void PlaySound(GameObject prefab, AudioClip clip)
        {
            var objectSoundSource = prefab.GetComponent<AudioSource>();
            objectSoundSource.clip = clip;
            objectSoundSource.Play();
        }

        public async Task DestroyPrefab(GameObject prefab, int waitTime)
        {
            await Task.Delay(waitTime);
            Destroy(prefab);
        }
    }
}
