using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour {

    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    public Slider musicVolumeSlider;
    public Slider soundEffectsVolumeSlider;

    public AudioSource musicSource;
    public AudioSource soundEffectsSource;
    public Resolution[] resolutions;
    GameSettings gameSettings;

    private void OnEnable()
    {
        gameSettings = new GameSettings();
        resolutions = Screen.resolutions;
        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChange(); });
        soundEffectsVolumeSlider.onValueChanged.AddListener(delegate { OnSoundEffectsVolumeChange(); });

        foreach (Resolution reso in resolutions)
            resolutionDropdown.options.Add(new Dropdown.OptionData(reso.ToString()));
    }
        
    public void OnFullscreenToggle ()
    {
        gameSettings.fullscreen = Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void OnResolutionChange()
    {
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
    }

    public void OnMusicVolumeChange()
    {
        musicSource.volume = gameSettings.musicVolume = musicVolumeSlider.value;
    }

    public void OnSoundEffectsVolumeChange()
    {
        soundEffectsSource.volume = gameSettings.soundEffectsVolume = soundEffectsVolumeSlider.value;
    }
}
