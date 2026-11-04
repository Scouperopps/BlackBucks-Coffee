using UnityEngine;
using TMPro;

public class ColorblindManager : MonoBehaviour
{
    public static ColorblindManager Instance { get; private set; }

    [SerializeField] private Material colorblindMaterial;
    [SerializeField] private TMP_Dropdown modeDropdown;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        int savedMode = PlayerPrefs.GetInt("ColorblindMode", 0);
        SetColorblindMode(savedMode);

        if (modeDropdown != null)
        {
            modeDropdown.value = savedMode;
            modeDropdown.onValueChanged.AddListener(SetColorblindMode);
        }
    }

    public void SetColorblindMode(int modeIndex)
    {
        if (colorblindMaterial != null)
        {
            colorblindMaterial.SetInt("_Mode", modeIndex);
        }

        PlayerPrefs.SetInt("ColorblindMode", modeIndex);
        PlayerPrefs.Save();
    }
}