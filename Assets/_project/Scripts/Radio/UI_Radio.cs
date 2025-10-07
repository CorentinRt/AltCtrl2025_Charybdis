using UnityEngine;
using TMPro;

namespace AltCtrl.Charybdis
{
    public class UI_Radio : MonoBehaviour
    {
        #region Fields
        [SerializeField] private TextMeshProUGUI _frequencyLabel;

        #endregion

        #region Properties


        #endregion


        private void Start()
        {
            Init();
        }
        private void Init()
        {
            if (RadioManager.Exist)
            {
                RadioManager.Instance.OnChangeFrequency += UpdateFrequencyLabel;

                UpdateFrequencyLabel(RadioManager.Instance.GetCurrentFrequency());
            }
        }


        private void OnDestroy()
        {
            if (RadioManager.Exist)
            {
                RadioManager.Instance.OnChangeFrequency -= UpdateFrequencyLabel;
            }
        }

        private void UpdateFrequencyLabel((int, int) frequency)
        {
            _frequencyLabel.text = $"{ frequency.Item1 } : { frequency.Item2 }";
        }
    }
}
