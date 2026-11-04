using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static int _activeSlot = 1;
    public static int ActiveSlot { get => _activeSlot; set => _activeSlot = Mathf.Clamp(value, 1, 3); }

    public static SaveData CurrentData { get; private set; }

    private static string GetFilePath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"save_slot_{slot}.json");
    }

    public static SaveData LoadSlot(int slot)
    {
        ActiveSlot = slot;
        string path = GetFilePath(slot);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            CurrentData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            CurrentData = new SaveData(slot);
            SaveCurrentSlot();
        }

        return CurrentData;
    }

    public static void SaveCurrentSlot()
    {
        if (CurrentData == null) CurrentData = new SaveData(ActiveSlot);

        CurrentData.lastSavedDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        string json = JsonUtility.ToJson(CurrentData, true);
        File.WriteAllText(GetFilePath(ActiveSlot), json);
    }

    public static void DeleteSlot(int slot)
    {
        string path = GetFilePath(slot);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        if (ActiveSlot == slot)
        {
            CurrentData = new SaveData(slot);
        }
    }

    public static void UnlockNextLevel(int completedLevelIndex)
    {
        if (CurrentData == null) LoadSlot(ActiveSlot);

        int nextLevel = completedLevelIndex + 1;
        if (nextLevel > CurrentData.highestUnlockedLevel)
        {
            CurrentData.highestUnlockedLevel = nextLevel;
            SaveCurrentSlot();
        }
    }
}