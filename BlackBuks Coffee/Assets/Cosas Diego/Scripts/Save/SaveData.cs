[System.Serializable]
public class SaveData
{
    public int slotId = 1;
    public int highestUnlockedLevel = 1;
    public System.Collections.Generic.List<string> unlockedAchievements = new System.Collections.Generic.List<string>();
    public int colorblindMode = 0;
    public string lastSavedDate; // <-- Debe estar presente aquí

    public SaveData(int slot)
    {
        slotId = slot;
        highestUnlockedLevel = 1;
        unlockedAchievements = new System.Collections.Generic.List<string>();
        colorblindMode = 0;
        lastSavedDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }
}