using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    private TextMeshProUGUI origamiText;

    // Start is called before the first frame update
    void Start()
    {
        origamiText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateOrigamiText(PlayerInventory playerInventory)
    {
        origamiText.text = playerInventory.NumberOfOrigamis.ToString();
    }
}
