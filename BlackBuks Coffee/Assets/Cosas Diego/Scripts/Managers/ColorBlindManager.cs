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

        if (modeDropdown != null)
        {
            modeDropdown.value = savedMode;
            modeDropdown.onValueChanged.AddListener(SetColorblindMode);
        }

        SetColorblindMode(savedMode);
    }

    public void SetColorblindMode(int modeIndex)
    {
        Debug.Log($"[ColorblindManager] Cambiando modo a: {modeIndex}");
    
        if (colorblindMaterial != null)
        {
            colorblindMaterial.SetInt("_Type", modeIndex);
        }
        else
        {
            Debug.LogError("[ColorblindManager] ¡ATENCIÓN! 'colorblindMaterial' está NULL en el Inspector.");
        }
    
        PlayerPrefs.SetInt("ColorblindMode", modeIndex);
        PlayerPrefs.Save();
    }
}