using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementNotificationUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _headerText;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;

    [Header("Tiempos (segundos)")]
    [SerializeField] private float _fadeDuration = 0.3f;
    [SerializeField] private float _displayDuration = 3.5f;

    private readonly Queue<AchievementData> _notificationQueue = new Queue<AchievementData>();
    private bool _isShowing = false;

    private void Awake()
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }
    }

    private void Start()
    {
        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.OnAchievementUnlocked += EnqueueNotification;
        }
    }

    private void OnDestroy()
    {
        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.OnAchievementUnlocked -= EnqueueNotification;
        }
    }

    private void EnqueueNotification(AchievementData achievement)
    {
        _notificationQueue.Enqueue(achievement);

        if (!_isShowing)
        {
            StartCoroutine(ProcessQueueRoutine());
        }
    }

    private IEnumerator ProcessQueueRoutine()
    {
        _isShowing = true;

        while (_notificationQueue.Count > 0)
        {
            AchievementData data = _notificationQueue.Dequeue();
            SetupContent(data);

            // Fade In
            yield return FadeRoutine(0f, 1f);

            // Tiempo visible (ignora Time.timeScale = 0 al terminar nivel)
            yield return new WaitForSecondsRealtime(_displayDuration);

            // Fade Out
            yield return FadeRoutine(1f, 0f);
        }

        _isShowing = false;
    }

    private void SetupContent(AchievementData data)
    {
        if (_iconImage != null && data.icon != null)
            _iconImage.sprite = data.icon;

        if (LocalizationManager.Instance != null)
        {
            // Reutiliza "btn_achievements" ("LOGROS" / "ACHIEVEMENTS") que ya está en tu CSV
            if (_headerText != null)
                _headerText.text = LocalizationManager.Instance.GetText("btn_achievements");

            if (_titleText != null)
                _titleText.text = LocalizationManager.Instance.GetText(data.titleKey);

            if (_descriptionText != null)
                _descriptionText.text = LocalizationManager.Instance.GetText(data.descriptionKey);
        }
    }

    private IEnumerator FadeRoutine(float startAlpha, float endAlpha)
    {
        if (_canvasGroup == null) yield break;

        float elapsed = 0f;
        while (elapsed < _fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / _fadeDuration);
            yield return null;
        }
        _canvasGroup.alpha = endAlpha;
    }
}