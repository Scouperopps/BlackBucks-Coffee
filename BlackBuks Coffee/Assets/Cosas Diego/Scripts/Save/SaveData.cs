using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int slotId = 1;
    public int highestUnlockedLevel = 1; // 0 = Tutorial, 1 = Nivel 1, 2 = Nivel 2, 3 = Nivel 3
    public List<string> unlockedAchievements = new List<string>();
    public string lastSavedDate;

    public SaveData(int slot)
    {
        slotId = slot;
        highestUnlockedLevel = 1;
        unlockedAchievements = new List<string>();
        lastSavedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }
}