using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveMenuUI : MonoBehaviour
{
    [System.Serializable]
    public struct SlotCardUI
    {
        [Tooltip("Número de ranura: 1, 2 o 3")]
        public int slotIndex;
        
        [Header("Referencias UI de la tarjeta")]
        public TMP_Text slotTitleText;
        public TMP_Text progressText;
        public TMP_Text dateText;
        public Button selectButton;
        public Button deleteButton;
    }

    [Header("Configuración de Ranuras")]
    [SerializeField] private SlotCardUI[] _slotCards;

    private void OnEnable()
    {
        RefreshSlotCards();
    }

    /// <summary>
    /// Lee los archivos JSON de los 3 slots y actualiza los textos y botones de cada tarjeta.
    /// </summary>
    public void RefreshSlotCards()
    {
        if (_slotCards == null) return;

        for (int i = 0; i < _slotCards.Length; i++)
        {
            int slot = _slotCards[i].slotIndex;
            
            // Inspecciona la información del slot
            SaveData data = SaveSystem.LoadSlot(slot);

            // Asigna título
            if (_slotCards[i].slotTitleText != null)
            {
                _slotCards[i].slotTitleText.text = $"Ranura {slot}";
            }

            // Muestra progreso o estado vacío
            if (data != null && (data.highestUnlockedLevel > 1 || data.unlockedAchievements.Count > 0))
            {
                if (_slotCards[i].progressText != null)
                    _slotCards[i].progressText.text = $"Nivel Máximo: {data.highestUnlockedLevel}";

                if (_slotCards[i].dateText != null)
                    _slotCards[i].dateText.text = string.IsNullOrEmpty(data.lastSavedDate) ? "Partida Guardada" : data.lastSavedDate;

                if (_slotCards[i].deleteButton != null)
                    _slotCards[i].deleteButton.interactable = true;
            }
            else
            {
                if (_slotCards[i].progressText != null)
                    _slotCards[i].progressText.text = "Partida Nueva / Vacía";

                if (_slotCards[i].dateText != null)
                    _slotCards[i].dateText.text = "-- / -- / ----";

                if (_slotCards[i].deleteButton != null)
                    _slotCards[i].deleteButton.interactable = false;
            }

            // Eventos de botones
            int cachedSlot = slot;

            if (_slotCards[i].selectButton != null)
            {
                _slotCards[i].selectButton.onClick.RemoveAllListeners();
                _slotCards[i].selectButton.onClick.AddListener(() => OnSelectSlot(cachedSlot));
            }

            if (_slotCards[i].deleteButton != null)
            {
                _slotCards[i].deleteButton.onClick.RemoveAllListeners();
                _slotCards[i].deleteButton.onClick.AddListener(() => OnDeleteSlot(cachedSlot));
            }
        }

        // Restaura la carga del slot seleccionado actualmente en PlayerPrefs
        int currentActive = PlayerPrefs.GetInt("ActiveSlot", 1);
        SaveSystem.LoadSlot(currentActive);
    }

    private void OnSelectSlot(int slot)
    {
        PlayerPrefs.SetInt("ActiveSlot", slot);
        PlayerPrefs.Save();
        SaveSystem.LoadSlot(slot);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OpenLevelSelection();
        }
    }

    private void OnDeleteSlot(int slot)
    {
        // Borra usando directamente DeleteSlot(slot)
        SaveSystem.DeleteSlot(slot);

        RefreshSlotCards();
        Debug.Log($"[Guardado] Ranura {slot} eliminada correctamente.");
    }

    /// <summary>
    /// Asigna este método al botón "Volver" del panel de guardados.
    /// </summary>
    public void OnBackButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(GameState.MainMenu);
        }
    }
}