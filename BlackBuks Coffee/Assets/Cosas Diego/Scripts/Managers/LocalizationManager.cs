using System;
using System.Collections.Generic;
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
            InitializeTranslations();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetLanguage(Language language)
    {
        if (_currentLanguage == language)
            return;

        _currentLanguage = language;
        OnLanguageChanged?.Invoke();
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

    private void InitializeTranslations()
    {
        AddTranslation("btn_play", new Dictionary<Language, string>
        {
            { Language.English, "PLAY" },
            { Language.Spanish, "JUGAR" },
            { Language.Portuguese, "JOGAR" },
            { Language.French, "JOUER" },
            { Language.German, "SPIELEN" },
            { Language.Danish, "SPIL" },
            { Language.Swedish, "SPELA" },
            { Language.Norwegian, "SPIL" },
            { Language.Japanese, "プレイ" },
            { Language.Russian, "ИГРАТЬ" },
            { Language.Mandarin, "开始游戏" }
        });

        AddTranslation("btn_resume", new Dictionary<Language, string>
        {
            { Language.English, "RESUME" },
            { Language.Spanish, "REANUDAR" },
            { Language.Portuguese, "CONTINUAR" },
            { Language.French, "REPRENDRE" },
            { Language.German, "FORTSETZEN" },
            { Language.Danish, "GENOPTAG" },
            { Language.Swedish, "FORTSÄTT" },
            { Language.Norwegian, "FORTSETT" },
            { Language.Japanese, "再開" },
            { Language.Russian, "ПРОДОЛЖИТЬ" },
            { Language.Mandarin, "继续游戏" }
        });

        AddTranslation("btn_restart", new Dictionary<Language, string>
        {
            { Language.English, "RESTART" },
            { Language.Spanish, "REINICIAR" },
            { Language.Portuguese, "REINICIAR" },
            { Language.French, "RECOMMENCER" },
            { Language.German, "NEUSTART" },
            { Language.Danish, "GENSTART" },
            { Language.Swedish, "STARTA OM" },
            { Language.Norwegian, "START PÅ NYTT" },
            { Language.Japanese, "リスタート" },
            { Language.Russian, "ПЕРЕЗАПУСК" },
            { Language.Mandarin, "重新开始" }
        });

        AddTranslation("btn_main_menu", new Dictionary<Language, string>
        {
            { Language.English, "MAIN MENU" },
            { Language.Spanish, "MENÚ PRINCIPAL" },
            { Language.Portuguese, "MENU PRINCIPAL" },
            { Language.French, "MENU PRINCIPAL" },
            { Language.German, "HAUPTMENÜ" },
            { Language.Danish, "HOVEDMENU" },
            { Language.Swedish, "HUVUDMENY" },
            { Language.Norwegian, "HOVEDMENY" },
            { Language.Japanese, "メインメニュー" },
            { Language.Russian, "ГЛАВНОЕ МЕНЮ" },
            { Language.Mandarin, "主菜单" }
        });

        AddTranslation("btn_settings", new Dictionary<Language, string>
        {
            { Language.English, "SETTINGS" },
            { Language.Spanish, "AJUSTES" },
            { Language.Portuguese, "CONFIGURAÇÕES" },
            { Language.French, "PARAMÈTRES" },
            { Language.German, "EINSTELLUNGEN" },
            { Language.Danish, "INDSTILLINGER" },
            { Language.Swedish, "INSTÄLLNINGAR" },
            { Language.Norwegian, "INNSTILLINGER" },
            { Language.Japanese, "設定" },
            { Language.Russian, "НАСТРОЙКИ" },
            { Language.Mandarin, "设置" }
        });

        AddTranslation("btn_tutorial", new Dictionary<Language, string>
        {
            { Language.English, "TUTORIAL" },
            { Language.Spanish, "TUTORIAL" },
            { Language.Portuguese, "TUTORIAL" },
            { Language.French, "TUTORIEL" },
            { Language.German, "TUTORIAL" },
            { Language.Danish, "TUTORIAL" },
            { Language.Swedish, "TUTORIAL" },
            { Language.Norwegian, "TUTORIAL" },
            { Language.Japanese, "チュートリアル" },
            { Language.Russian, "ОБУЧЕНИЕ" },
            { Language.Mandarin, "教程" }
        });

        AddTranslation("btn_quit", new Dictionary<Language, string>
        {
            { Language.English, "QUIT" },
            { Language.Spanish, "SALIR" },
            { Language.Portuguese, "SAIR" },
            { Language.French, "QUITTER" },
            { Language.German, "BEENDEN" },
            { Language.Danish, "AFSLUT" },
            { Language.Swedish, "AVSLUTA" },
            { Language.Norwegian, "AVSLUTT" },
            { Language.Japanese, "終了" },
            { Language.Russian, "ВЫХОД" },
            { Language.Mandarin, "退出" }
        });

        AddTranslation("btn_back", new Dictionary<Language, string>
        {
            { Language.English, "BACK" },
            { Language.Spanish, "ATRÁS" },
            { Language.Portuguese, "VOLTAR" },
            { Language.French, "RETOUR" },
            { Language.German, "ZURÜCK" },
            { Language.Danish, "TILBAGE" },
            { Language.Swedish, "TILLBAKA" },
            { Language.Norwegian, "TILBAKE" },
            { Language.Japanese, "戻る" },
            { Language.Russian, "НАЗАД" },
            { Language.Mandarin, "返回" }
        });

        AddTranslation("txt_victory", new Dictionary<Language, string>
        {
            { Language.English, "VICTORY!" },
            { Language.Spanish, "¡VICTORIA!" },
            { Language.Portuguese, "VITÓRIA!" },
            { Language.French, "VICTOIRE !" },
            { Language.German, "SIEG!" },
            { Language.Danish, "SEJR!" },
            { Language.Swedish, "SEGER!" },
            { Language.Norwegian, "SEIER!" },
            { Language.Japanese, "勝利！" },
            { Language.Russian, "ПОБЕДА!" },
            { Language.Mandarin, "胜利！" }
        });

        AddTranslation("txt_defeat", new Dictionary<Language, string>
        {
            { Language.English, "DEFEAT!" },
            { Language.Spanish, "¡DERROTA!" },
            { Language.Portuguese, "DERROTA!" },
            { Language.French, "DÉFAITE !" },
            { Language.German, "NIEDERLAGE!" },
            { Language.Danish, "NEDERLAG!" },
            { Language.Swedish, "FÖRLUST!" },
            { Language.Norwegian, "NEDERLAG!" },
            { Language.Japanese, "敗北…" },
            { Language.Russian, "ПОРАЖЕНИЕ!" },
            { Language.Mandarin, "失败！" }
        });

        AddTranslation("txt_settings_title", new Dictionary<Language, string>
        {
            { Language.English, "SETTINGS" },
            { Language.Spanish, "AJUSTES" },
            { Language.Portuguese, "CONFIGURAÇÕES" },
            { Language.French, "PARAMÈTRES" },
            { Language.German, "EINSTELLUNGEN" },
            { Language.Danish, "INDSTILLINGER" },
            { Language.Swedish, "INSTÄLLNINGAR" },
            { Language.Norwegian, "INNSTILLINGER" },
            { Language.Japanese, "設定" },
            { Language.Russian, "НАСТРОЙКИ" },
            { Language.Mandarin, "设置" }
        });

        AddTranslation("txt_title", new Dictionary<Language, string>
        {
            { Language.English, "BASIC CONTROLS" },
            { Language.Spanish, "CONTROLES BÁSICOS" },
            { Language.Portuguese, "CONTROLES BÁSICOS" },
            { Language.French, "COMMANDES DE BASE" },
            { Language.German, "GRUNDSTEUERUNG" },
            { Language.Danish, "GRUNDLÆGGENDE BETJENING" },
            { Language.Swedish, "GRUNDLÄGGANDE KONTROLLER" },
            { Language.Norwegian, "GRUNNLEGGENDE KONTROLLER" },
            { Language.Japanese, "基本操作" },
            { Language.Russian, "ОСНОВНОЕ УПРАВЛЕНИЕ" },
            { Language.Mandarin, "基础操作" }
        });
    }

    private void AddTranslation(string key, Dictionary<Language, string> translations)
    {
        _textTable[key] = translations;
    }
}