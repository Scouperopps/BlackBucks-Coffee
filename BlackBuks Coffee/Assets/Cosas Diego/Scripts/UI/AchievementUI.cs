using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private GameObject achievementItemPrefab;

    private void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        if (AchievementManager.Instance == null) return;

        foreach (var achievement in AchievementManager.Instance.Achievements)
        {
            GameObject item = Instantiate(achievementItemPrefab, container);
            bool unlocked = AchievementManager.Instance.IsAchievementUnlocked(achievement.id);

            TMP_Text titleText = item.transform.Find("Title")?.GetComponent<TMP_Text>();
            TMP_Text descText = item.transform.Find("Description")?.GetComponent<TMP_Text>();
            Image iconImage = item.transform.Find("Icon")?.GetComponent<Image>();
            Transform checkmark = item.transform.Find("Checkmark");
            CanvasGroup canvasGroup = item.GetComponent<CanvasGroup>();

            if (titleText != null)
                titleText.text = LocalizationManager.Instance.GetText(achievement.titleKey);

            if (descText != null)
                descText.text = LocalizationManager.Instance.GetText(achievement.descriptionKey);

            if (iconImage != null && achievement.icon != null)
                iconImage.sprite = achievement.icon;

            // Activa o desactiva la palomita según si ya se consiguió el logro
            if (checkmark != null)
                checkmark.gameObject.SetActive(unlocked);

            if (canvasGroup != null)
                canvasGroup.alpha = unlocked ? 1f : 0.4f;
        }
    }
}