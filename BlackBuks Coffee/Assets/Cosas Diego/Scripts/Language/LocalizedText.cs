using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string _localizationKey;

    private Text _uiText;
    private TMP_Text _tmpText;
    private string _defaultInspectorText;

    private void Awake()
    {
        _uiText = GetComponent<Text>();
        _tmpText = GetComponent<TMP_Text>();

        if (_uiText != null) _defaultInspectorText = _uiText.text;
        if (_tmpText != null) _defaultInspectorText = _tmpText.text;
    }

    private void OnEnable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged += RefreshText;
            RefreshText();
        }
    }

    private void Start()
    {
        RefreshText();
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.OnLanguageChanged -= RefreshText;
        }
    }

    public void RefreshText()
    {
        if (LocalizationManager.Instance == null)
            return;

        if (string.IsNullOrEmpty(_localizationKey))
        {
            ApplyText(_defaultInspectorText);
            return;
        }

        string translatedValue = LocalizationManager.Instance.GetText(_localizationKey);
        ApplyText(translatedValue);
    }

    private void ApplyText(string value)
    {
        if (_uiText != null)
        {
            _uiText.text = value;
        }

        if (_tmpText != null)
        {
            _tmpText.text = value;
        }
    }
}