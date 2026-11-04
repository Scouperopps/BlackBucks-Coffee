using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    private Language _currentLanguage = Language.Spanish;
    private readonly Dictionary<string, Dictionary<Language, string>> _textTable = new Dictionary<string, Dictionary<Language, string>>();

    public Language CurrentLanguage => _currentLanguage;
    public event Action OnLanguageChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadTranslationsFromCSV();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetLanguage(Language language)
    {
        if (_currentLanguage == language) return;
        _currentLanguage = language;
        OnLanguageChanged?.Invoke();
    }

    public void SetLanguageByIndex(int index)
    {
        if (Enum.IsDefined(typeof(Language), index))
        {
            SetLanguage((Language)index);
        }
    }

    public string GetText(string key)
    {
        if (_textTable.TryGetValue(key, out var translations))
        {
            if (translations.TryGetValue(_currentLanguage, out var localizedText))
            {
                return localizedText;
            }
        }
        return key;
    }

    private void LoadTranslationsFromCSV()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("localization");
        if (csvFile == null)
        {
            Debug.LogError("No se encontró el archivo localization.csv en Resources!");
            return;
        }

        string[] lines = csvFile.text.Split('\n');
        if (lines.Length <= 1) return;

        // Separación por comas respetando textos encerrados entre comillas
        string[] headers = ParseCSVLine(lines[0]);

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] row = ParseCSVLine(lines[i]);
            if (row.Length == 0) continue;

            string key = row[0];

            if (!_textTable.ContainsKey(key))
            {
                _textTable[key] = new Dictionary<Language, string>();
            }

            for (int j = 1; j < row.Length && j < headers.Length; j++)
            {
                if (Enum.TryParse<Language>(headers[j], true, out Language lang))
                {
                    _textTable[key][lang] = row[j];
                }
            }
        }
    }

    // Método auxiliar para dividir comas sin romper frases con texto entre comillas
    private string[] ParseCSVLine(string line)
    {
        string[] fields = Regex.Split(line.Trim(), ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
        for (int i = 0; i < fields.Length; i++)
        {
            fields[i] = fields[i].Trim(' ', '"', '\r');
        }
        return fields;
    }
}