using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AltCtrl.Charybdis
{
    public class TutoController : MonoBehaviour
    {
        #region Fields
        [Header("Islands")]
        [SerializeField] private GameObject _islandHolder;

        [Header("UI Tuto")]
        [SerializeField] private UI_Tutorials _uiTutorials;

        #endregion

        #region Properties


        #endregion


        public UnityEvent OnEnableAllIslands;
        public UnityEvent OnDisableAllIslands;


        protected virtual void Start()
        {
            StartTuto();
        }

        protected virtual void OnDestroy()
        {
            
        }

        protected virtual void StartTuto()
        {
            Debug.Log("Start Tuto", this);
        }


        protected void SetEnabledAllIslands(bool enabled)
        {
            _islandHolder.SetActive(enabled);

            if (enabled)
            {
                OnEnableAllIslands?.Invoke();
            }
            else
            {
                OnDisableAllIslands?.Invoke();
            }
        }

        protected void SetEnabledTutorialUI(bool enabled)
        {
            if (enabled)
            {
                _uiTutorials.ShowTuto();
            }
            else
            {
                _uiTutorials.HideTuto();
            }
        }

        protected void GoToTutorialUIPart(string triggerPartName)
        {
            _uiTutorials.TriggerAnimTuto(triggerPartName);
        }

    }
}
