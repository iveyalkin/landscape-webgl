using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace IV.UI.Popups
{
    public class MenuPopup : BasePopup
    {
        [SerializeField] private Button ambienceButton;
        [SerializeField] private Button sfxButton;

        [SerializeField] private Text ambienceTitle;
        [SerializeField] private Text sfxTitle;

        [SerializeField] private AudioMixer audioMixer;

        [SerializeField] private string ambienceVolume = "ambienceVolume";
        [SerializeField] private string sfxVolume = "sfxVolume";

        public void ToggleAmbience()
        {
            var isMuted = ToggleAudioGroup(ambienceVolume);
            UpdateButton(isMuted, ambienceButton, ambienceTitle);
        }

        public void ToggleSfx()
        {
            var isMuted = ToggleAudioGroup(sfxVolume);
            UpdateButton(isMuted, sfxButton, sfxTitle);
        }

        private bool IsMuted(string volumeParameter) => audioMixer.GetFloat(volumeParameter, out var volume)
                                                        && volume <= (-80f + float.Epsilon);

        protected override void Refresh()
        {
            UpdateButton(IsMuted(ambienceVolume), ambienceButton, ambienceTitle);
            UpdateButton(IsMuted(sfxVolume), sfxButton, sfxTitle);
        }

        private void UpdateButton(bool isMuted, Button button, Text titleText)
        {
            // var colors = button.colors;
            // colors.normalColor = isMuted ? Color.red : Color.green;
            // button.colors = colors;

            titleText.text = isMuted ? "muted" : "active";
        }

        private bool ToggleAudioGroup(string volumeParameter)
        {
            var isMuted = IsMuted(volumeParameter);
            audioMixer.SetFloat(volumeParameter, isMuted ? 0f : -80f);

            return !isMuted;
        }
    }
}