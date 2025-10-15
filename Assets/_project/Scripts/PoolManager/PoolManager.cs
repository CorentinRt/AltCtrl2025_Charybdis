using System.Collections.Generic;
using UnityEngine;

namespace AltCtrl.Charybdis
{
    [System.Serializable]
    public struct PoolItem
    {
        public List<GameObject> Items;
        public GameObject Prefab;
        public Transform Parent;
    }

    public class PoolManager : MonoBehaviour
    {
        // ----- FIELDS ----- //
        public static PoolManager Instance;

        [Header("----- Ship Pool -----")]
        [SerializeField] private PoolItem _ship;

        [Header("----- Typhoon Pool -----")]
        [SerializeField] private PoolItem _typhoon;

        // plus tard : vfx

        [Header("----- Audio Source -----")]
        [SerializeField] private PoolItem _audioSource;
        private List<AudioSource> _audioSourceComponents = new List<AudioSource>();
        // ----- FIELDS ----- //

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;

            // Audio source components
            foreach (GameObject audioSource in _audioSource.Items)
                _audioSourceComponents.Add(audioSource.GetComponent<AudioSource>());
        }

        #region Get & setup child
        private GameObject GetFirstNonActiveChildInList(List<GameObject> list)
        {
            foreach (GameObject child in list)
            {
                if (!child.activeInHierarchy)
                {
                    return child;
                }
            }

            return null;
        }

        private GameObject CreateNewChild(GameObject prefab, Transform parent, string name, PoolItem poolItem)
        {
            GameObject newGameObject = Instantiate(prefab, parent);
            newGameObject.name = name;

            poolItem.Items.Add(newGameObject);

            return newGameObject;
        }

        private GameObject SetupChild(GameObject child, Vector3 position, Quaternion rotation, bool activate)
        {
            child.transform.SetPositionAndRotation(position, rotation);

            child.SetActive(activate);

            return child;
        }

        private string GetNewChildName(PoolItem poolItem)
        {
            string newChildName = poolItem.Items[0].gameObject.name.Substring(0, poolItem.Items[0].gameObject.name.Length - 2); 
            newChildName += poolItem.Items.Count.ToString("D2"); // ex : VFX_Destroy_02

            return newChildName;
        }

        private GameObject GetChild(PoolItem poolItem, Vector3 position, Quaternion rotation, bool activate)
        {
            GameObject child = GetFirstNonActiveChildInList(poolItem.Items);
            if (child != null)
            {
                return SetupChild(child, position, rotation, activate);
            }
            else
            {
                GameObject newChild = CreateNewChild(poolItem.Prefab, poolItem.Parent, GetNewChildName(poolItem), poolItem);

                return SetupChild(newChild, position, rotation, activate);
            }
        }
        #endregion

        #region Ship
        public GameObject ActivateShip(Vector3 position, Quaternion rotation, bool activate = true) 
        {
            PoolItem poolItem = _ship;

            return GetChild(poolItem, position, rotation, activate);
        }
        #endregion

        #region Typhoon
        public GameObject ActivateTyphoon(Vector3 position, Quaternion rotation, bool activate = true)
        {
            PoolItem poolItem = _typhoon;

            return GetChild(poolItem, position, rotation, activate);
        }
        #endregion

        #region Audio source
        public AudioSource GetAudioSource(bool activate = true)
        {
            AudioSource audioSource = GetNotPlayingAudioSource();

            if (audioSource == null)
            {
                audioSource = CreateNewChild(_audioSource.Prefab, _audioSource.Parent, GetNewChildName(_audioSource), _audioSource).GetComponent<AudioSource>();
                _audioSourceComponents.Add(audioSource);
            }

            //Debug.Log(audioSource);

            return audioSource;
        }

        public AudioSource GetNotPlayingAudioSource()
        {
            foreach (AudioSource audioSource in _audioSourceComponents)
            {
                if (!audioSource.isPlaying) return audioSource;
            }
            return null;
        }
        #endregion

        /*
        #region VFX
        public GameObject GetVFX(VFXType type, Vector3 position, Quaternion rotation, bool activate = true)
        {
            // Get pool item
            PoolItem poolItem = new PoolItem();

            switch (type)
            {
                case VFXType.DestroyFish:
                    poolItem = _destroyFish;
                    break;
                case VFXType.InkEffect:
                    poolItem = _inkEffect;
                    break;
                case VFXType.ExplosionEffect:
                    poolItem = _explosionEffect;
                    break;
                case VFXType.SmokeEffect:
                    poolItem = _smokeEffect;
                    break;
            }

            return GetChild(poolItem, position, rotation, activate);
        }
        #endregion
        */


    }
}
