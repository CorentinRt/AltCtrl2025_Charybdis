using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AltCtrl.Charybdis
{
    public class TutoController : MonoBehaviour
    {
        #region Fields
        [Header("Islands")]
        [SerializeField] private List<GameObject> _islands;



        #endregion

        #region Properties


        #endregion


        public UnityEvent OnEnableAllIslands;
        public UnityEvent OnDisableAllIslands;

        protected virtual void SetEnabledAllIslands(bool enabled)
        {
            for (int i = 0;  i < _islands.Count; ++i)
            {
                _islands[i].SetActive(enabled);
            }

            if (enabled)
            {
                OnEnableAllIslands?.Invoke();
            }
            else
            {
                OnDisableAllIslands?.Invoke();
            }
        }

    }
}
