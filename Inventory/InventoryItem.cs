using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public Image imgItem;
    public Text textItemQuantity;

    int index;

    public void SetDefault(int _index)
    {
        index = _index;
        imgItem.sprite = Resources.Load<Sprite>($"Sprite/Material/M{index}");
        textItemQuantity.text = UserManager.instance.GetInventory(index).ToString();
    }

    public void SetQuantity(int value)
    {
        UserManager.instance.SetInventory(index, value);
        textItemQuantity.text = value.ToString();
    }
}