using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemSlot : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI quantityText;

    public void SetItemData(ItemData itemData, uint quantity)
    {
        iconImage.sprite = itemData.Icon;
        iconImage.color = Color.white;
        quantityText.text = quantity.ToString();
    }

    public void SetEmpty()
    {
        iconImage.sprite = null;
        iconImage.color = Color.clear;
        quantityText.text = "";
    }
}