using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Slider))]
    public class VolumeSlider : MonoBehaviour
    {
        public AudioMixer audioMixer;
        public string volumeParameterName = "Master Volume";

        public float maxVolume = 10;
        public float minVolume = -30;

        public TMPro.TextMeshProUGUI percentLabel;

        private Slider _slider;

        public void Awake()
        {
            _slider = GetComponent<Slider>();
            if (audioMixer == null) throw new UnityException($"Volume Slider {name} missing an AudioMixer");
            float curVolume = 0;
            if (!audioMixer.GetFloat(volumeParameterName, out curVolume))
            {
                Debug.LogError($"No Exposed Parameter named {volumeParameterName} in {audioMixer.name}", audioMixer);
            }
            _slider.minValue = 0;
            _slider.maxValue = 1;
            _slider.value = Mathf.InverseLerp(minVolume, maxVolume, curVolume);
            
            _slider.onValueChanged.AddListener(t =>
            {
                var newVolume = Mathf.Lerp(minVolume, maxVolume, t);

                audioMixer.SetFloat(volumeParameterName, newVolume);
            });


        }
    }

}