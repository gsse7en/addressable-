using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Addressales.Main
{
    [System.Serializable]
    public class JsonSerializedObject
    {
        public string json_text;
    }

    class Main : MonoBehaviour
    {
        [SerializeField] private Text m_Text;
        [SerializeField] private Image m_Image;
        [SerializeField] private VideoPlayer m_VideoPlayer;
        [SerializeField] private Button m_AudioButton;
        [SerializeField] private Button m_VideoButton;
        [SerializeField] private Button m_SpriteButton;
        [SerializeField] private string m_PrefabsLabel;
        [SerializeField] private string m_MaterialAddress;
        [SerializeField] private string m_JsonAddress;
        [SerializeField] private string m_AudioAddress;
        [SerializeField] private string m_VideoAddress;
        [SerializeField] private string m_SpriteAddress;

        private List<GameObject> m_Prefabs = new List<GameObject>();
        private AudioSource m_AudioSource;
        private AudioClip m_AddressableAudio;
        private Sprite m_AddressableSprite;

        #region Lifecycle
        private void Awake()
        {
            m_AudioButton?.onClick.AddListener(delegate
            {
                PlaySound();
            });

            m_VideoButton?.onClick.AddListener(delegate
            {
                PlayVideo();
            });

            m_SpriteButton?.onClick.AddListener(delegate
            {
                ShowPicture();
            });
        }

        void Start()
        {
            m_AudioSource = GetComponent<AudioSource>();

            LoadAssets();
        }

        private void OnDestroy()
        {
            m_AudioButton?.onClick.RemoveAllListeners();
            m_VideoButton?.onClick.RemoveAllListeners();
            m_SpriteButton?.onClick.RemoveAllListeners();
        }
        #endregion

        #region Async
        async void LoadAssets()
        {
            await LoadAssetAsync();
        }

        async Task LoadAssetAsync()
        {
            m_AddressableAudio = await Addressables.LoadAssetAsync<AudioClip>(m_AudioAddress).Task;
            m_VideoPlayer.clip = await Addressables.LoadAssetAsync<VideoClip>(m_VideoAddress).Task;
            m_AddressableSprite = await Addressables.LoadAssetAsync<Sprite>(m_SpriteAddress).Task;

            TextAsset json_string = await Addressables.LoadAssetAsync<TextAsset>(m_JsonAddress).Task;
            IList<GameObject> m_Prefabs = await Addressables.LoadAssetsAsync<GameObject>(m_PrefabsLabel, null).Task;
            foreach (var obj in m_Prefabs)
            {
                Instantiate(obj, transform);
            }
            //Material material = await Addressables.LoadAssetAsync<Material>(m_MaterialAddress).Task;
            //m_Prefab = Instantiate(prefab, transform);
            //m_Prefab.GetComponent<MeshRenderer>().material = material;
            m_Text.text += JsonUtility.FromJson<JsonSerializedObject>(json_string.ToString()).json_text;
        }


        #endregion

        #region Delegates
        public void PlaySound()
        {
            m_AudioSource.PlayOneShot(m_AddressableAudio, 1f);
        }

        public void PlayVideo()
        {
            m_VideoPlayer.Play();
        }

        public void ShowPicture()
        {
            m_Image.sprite = m_AddressableSprite;
        }
        #endregion
    }
}

