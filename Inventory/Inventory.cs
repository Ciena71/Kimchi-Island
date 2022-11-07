using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    static public Inventory instance;
    public ScrollRect rectItem;
    public Text textItemList;

    List<InventoryItem> itemList = new List<InventoryItem>();

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < ResourceManager.instance.GetItemMax(); ++i)
        {
            GameObject obj = Instantiate(Resources.Load<GameObject>("Prefab/Inventory/InventoryItem"), rectItem.content);
            InventoryItem form = obj.GetComponent<InventoryItem>();
            form.SetDefault(i);
            itemList.Add(form);
        }
        ApplyLanguage();
    }

    public void SetItemQuantity(int index, int value) => itemList[index].SetQuantity(value);

    public void ApplyLanguage()
    {
        textItemList.text = ResourceManager.instance.GetText(47);
    }
}