using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AltCtrl.Charybdis
{
    public class UI_SliderValueDisplayer : MonoBehaviour
    {
        #region Fields
        [Header("Associated Slider")]
        [SerializeField] private Slider _associatedSlider;
        [SerializeField] private TextMeshProUGUI _associatedLabel;

        [SerializeField] private bool _shouldDisplayAfterComa = false;
        [SerializeField, ShowIf("_shouldDisplayAfterComa")] private int _numberAfterComa = 2;

        #endregion

        #region Properties


        #endregion

        private void Awake()
        {
            BindSliderChangeValueToDisplay();
        }

        private void OnDestroy()
        {
            UnbindSliderChangeValueToDisplay();
        }

        private void BindSliderChangeValueToDisplay()
        {
            _associatedSlider.onValueChanged.AddListener(delegate { UpdateSliderLabelValue(); });
        }

        private void UnbindSliderChangeValueToDisplay()
        {
            _associatedSlider.onValueChanged.RemoveListener(delegate { UpdateSliderLabelValue(); });
        }

        public void UpdateSliderLabelValue()
        {
            float value = _associatedSlider.value;

            if (_shouldDisplayAfterComa)
            {
                string numberAfterComaString = "F" + _numberAfterComa.ToString();

                _associatedLabel.text = value.ToString(numberAfterComaString);
            }
            else
            {
                _associatedLabel.text = value.ToString("F0");
            }

        }

    }
}
