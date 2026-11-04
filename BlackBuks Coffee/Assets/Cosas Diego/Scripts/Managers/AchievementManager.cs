using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AchievementData
{
    public string id;
    public string title;
    public string description;
    public Sprite icon;
}

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    [SerializeField] private List<AchievementData> _achievements = new List<AchievementData>();
    public List<AchievementData> Achievements => _achievements;

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

    public bool IsAchievementUnlocked(string id)
    {
        if (SaveSystem.CurrentData == null) SaveSystem.LoadSlot(SaveSystem.ActiveSlot);
        return SaveSystem.CurrentData.unlockedAchievements.Contains(id);
    }

    public void UnlockAchievement(string id)
    {
        if (IsAchievementUnlocked(id)) return;

        SaveSystem.CurrentData.unlockedAchievements.Add(id);
        SaveSystem.SaveCurrentSlot();
    }

    public void CheckLevelAchievements(int levelIndex)
    {
        if (levelIndex == 1) UnlockAchievement("FIRST_LEVEL_PASS");
        if (levelIndex == 2) UnlockAchievement("SECOND_LEVEL_PASS");
        if (levelIndex == 3) UnlockAchievement("MASTER_BARISTA");
    }
}