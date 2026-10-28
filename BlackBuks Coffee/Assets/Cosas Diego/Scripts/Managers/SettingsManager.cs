using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    private static SettingsManager _instance;
    public static SettingsManager Instance => _instance;

    [SerializeField] private AudioMixer _mainAudioMixer;
    [SerializeField] private TMP_Dropdown _refreshRateDropdown;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private TMP_Dropdown _screenModeDropdown;
    [SerializeField] private TMP_Dropdown _languageDropdown;
    [SerializeField] private Slider _volumeSlider;

    private Resolution[] _availableResolutions;
    private readonly List<int> _availableRefreshRates = new List<int> { 30, 60, 120, 144, 240 };

    private const string REFRESH_RATE_KEY = "TargetRefreshRate";
    private const string RESOLUTION_KEY = "ResolutionIndex";
    private const string SCREEN_MODE_KEY = "ScreenModeIndex";
    private const string LANGUAGE_KEY = "LanguageIndex";
    private const string MASTER_VOLUME_KEY = "MasterVolume";

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetupRefreshRateDropdown();
        SetupResolutionDropdown();
        SetupScreenModeDropdown();
        LoadSettings();
    }

    private void SetupRefreshRateDropdown()
    {
        if (_refreshRateDropdown == null)
        {
            return;
        }

        _refreshRateDropdown.ClearOptions();
        List<string> options = new List<string>();

        foreach (int rate in _availableRefreshRates)
        {
            options.Add($"{rate}Hz");
        }

        _refreshRateDropdown.AddOptions(options);
    }

    private void SetupResolutionDropdown()
    {
        if (_resolutionDropdown == null)
        {
            return;
        }

        _availableResolutions = Screen.resolutions;
        _resolutionDropdown.ClearOptions();

        int defaultResolutionIndex = 0;
        List<string> options = new List<string>();

        for (int i = 0; i < _availableResolutions.Length; i++)
        {
            string option = $"{_availableResolutions[i].width}x{_availableResolutions[i].height}";
            options.Add(option);

            if (_availableResolutions[i].width == Screen.currentResolution.width &&
                _availableResolutions[i].height == Screen.currentResolution.height)
            {
                defaultResolutionIndex = i;
            }
        }

        _resolutionDropdown.AddOptions(options);
        _resolutionDropdown.value = PlayerPrefs.GetInt(RESOLUTION_KEY, defaultResolutionIndex);
        _resolutionDropdown.RefreshShownValue();
    }

    private void SetupScreenModeDropdown()
    {
        if (_screenModeDropdown == null)
        {
            return;
        }

        _screenModeDropdown.ClearOptions();
        List<string> options = new List<string> { "Full Screen", "Windowed", "Borderless" };
        _screenModeDropdown.AddOptions(options);
    }

    public void SetRefreshRate(int index)
    {
        if (index >= 0 && index < _availableRefreshRates.Count)
        {
            int targetRate = _availableRefreshRates[index];
            Application.targetFrameRate = targetRate;
            PlayerPrefs.SetInt(REFRESH_RATE_KEY, index);
        }
    }

    public void SetResolution(int index)
    {
        if (_availableResolutions != null && index >= 0 && index < _availableResolutions.Length)
        {
            Resolution selectedResolution = _availableResolutions[index];
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreenMode);
            PlayerPrefs.SetInt(RESOLUTION_KEY, index);
        }
    }

    public void SetScreenMode(int index)
    {
        switch (index)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }
        PlayerPrefs.SetInt(SCREEN_MODE_KEY, index);
    }

    public void SetLanguage(int index)
    {
        PlayerPrefs.SetInt(LANGUAGE_KEY, index);
    
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.SetLanguageByIndex(index);
        }
    }

    public void SetMasterVolume(float volume)
    {
        float dbVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;

        if (_mainAudioMixer != null)
        {
            _mainAudioMixer.SetFloat("MasterVolumeParam", dbVolume);
        }

        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, volume);
    }

    private void LoadSettings()
    {
        int savedRefreshIndex = PlayerPrefs.GetInt(REFRESH_RATE_KEY, 1);
        int savedScreenModeIndex = PlayerPrefs.GetInt(SCREEN_MODE_KEY, 0);
        int savedLanguageIndex = PlayerPrefs.GetInt(LANGUAGE_KEY, 0);
        float savedVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 0.8f);

        if (_refreshRateDropdown != null) _refreshRateDropdown.value = savedRefreshIndex;
        if (_screenModeDropdown != null) _screenModeDropdown.value = savedScreenModeIndex;
        if (_languageDropdown != null) _languageDropdown.value = savedLanguageIndex;
        if (_volumeSlider != null) _volumeSlider.value = savedVolume;

        SetRefreshRate(savedRefreshIndex);
        SetScreenMode(savedScreenModeIndex);
        SetLanguage(savedLanguageIndex);
        SetMasterVolume(savedVolume);
    }
}