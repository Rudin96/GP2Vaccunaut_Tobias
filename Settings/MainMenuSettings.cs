using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Team14;
using UnityEngine.UI;
using System.Linq;

public class MainMenuSettings : MonoBehaviour
{
    GameSettings _settings;

    public Slider musicSlider;
    public Slider masterSlider;
    public Slider sfxSlider;


    private List<Resolution> resolutions;
    public Dropdown resDropdown;

    void Start()
    {
        _settings = Settings.GetSettings();
        masterSlider.onValueChanged.AddListener(MasterVolume);
        musicSlider.onValueChanged.AddListener(MusicVolume);
        sfxSlider.onValueChanged.AddListener(SfxVolume);

        Resolution();

        Settings.SetSettings(_settings);
    }

    public void MasterVolume(float volume)
    {
        AudioManager.Instance.ChangeVolume(volume, "Master");
    }

    public void MusicVolume(float volume)
    {
        AudioManager.Instance.ChangeVolume(volume, "Music");
    }

    public void SfxVolume(float volume)
    {
        AudioManager.Instance.ChangeVolume(volume, "SFX");
    }

    public void Resolution()
    {

        List<Dropdown.OptionData> resOptions = new List<Dropdown.OptionData>();


        resolutions.ForEach(resolution => resOptions.Add(new Dropdown.OptionData(resolution.ToString())));

        resDropdown.AddOptions(resOptions);
        resDropdown.value = resOptions.FindIndex(r => r.text.Contains($"{Settings.ResX} x {Settings.ResY}"));

        resDropdown.AddOptions(resOptions);
    }




}
