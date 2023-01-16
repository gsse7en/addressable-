using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Threading.Tasks;
using TMPro;

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
        [SerializeField] private string m_PrefabAddress;
        [SerializeField] private string m_Prefab2Address;
        [SerializeField] private string m_MaterialAddress;
        [SerializeField] private string m_JsonAddress;
        [SerializeField] private string m_AudioAddress;
        [SerializeField] private string m_VideoAddress;
        [SerializeField] private string m_SpriteAddress;

        private GameObject m_Prefab;
        private AudioSource m_AudioSource;
        private AudioClip m_AddressableAudio;
        private Sprite m_AddressableSprite;

        #region Lifecycle
        private void Awake()
        {
            if (m_AudioButton != null)
            {
                m_AudioButton.onClick.AddListener(delegate
                {
                    PlaySound();
                });
            }

            if (m_VideoButton != null)
            {
                m_VideoButton.onClick.AddListener(delegate
                {
                    PlayVideo();
                });
            }

            if (m_SpriteButton != null)
            {
                m_SpriteButton.onClick.AddListener(delegate
                {
                    ShowPicture();
                });
            }
        }

        void Start()
        {
            m_AudioSource = GetComponent<AudioSource>();

            LoadAssets();
        }

        private void OnDestroy()
        {
            if (m_AudioButton != null)
            {
                m_AudioButton.onClick.RemoveAllListeners();
            }

            if (m_VideoButton != null)
            {
                m_VideoButton.onClick.RemoveAllListeners();
            }

            if (m_SpriteButton != null)
            {
                m_SpriteButton.onClick.RemoveAllListeners();
            }
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
            GameObject prefab = await Addressables.LoadAssetAsync<GameObject>(m_PrefabAddress).Task;
            Material material = await Addressables.LoadAssetAsync<Material>(m_MaterialAddress).Task;

            m_Prefab = Instantiate(prefab, transform);
            m_Prefab.GetComponent<MeshRenderer>().material = material;
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

