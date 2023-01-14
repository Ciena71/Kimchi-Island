using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    static public Inventory instance;
    public ScrollRect rectMaterial;
    public Text textMaterial;
    public ScrollRect rectGardening;
    public Text textGardening;
    public ScrollRect rectProduce;
    public Text textProduce;
    public ScrollRect rectLeaving;
    public Text textLeaving;

    List<InventoryItem> itemList = new List<InventoryItem>();

    private void Awake()
    {
        instance = this;
        ResourceManager resourceManager = ResourceManager.instance;
        for (int i = 0; i < resourceManager.GetMaterialMax(); ++i)
        {
            GameObject obj = null;
            switch (resourceManager.GetMaterialCategory(i))
            {
            case 0:
                {
                    obj = Instantiate(Resources.Load<GameObject>("Prefab/Inventory/InventoryItem"), rectMaterial.content);
                    break;
                }
            case 1:
                {
                    obj = Instantiate(Resources.Load<GameObject>("Prefab/Inventory/InventoryItem"), rectGardening.content);
                    break;
                }
            case 2:
                {
                    obj = Instantiate(Resources.Load<GameObject>("Prefab/Inventory/InventoryItem"), rectProduce.content);
                    break;
                }
            case 3:
                {
                    obj = Instantiate(Resources.Load<GameObject>("Prefab/Inventory/InventoryItem"), rectLeaving.content);
                    break;
                }
            }
            InventoryItem form = obj.GetComponent<InventoryItem>();
            form.SetDefault(i);
            itemList.Add(form);
        }
        ApplyLanguage();
    }

    public void SetItemQuantity(int index, int value) => itemList[index].SetQuantity(value);

    public void ApplyLanguage()
    {
        textMaterial.text = ResourceManager.instance.GetText(47);
        textGardening.text = ResourceManager.instance.GetText(63);
        textProduce.text = ResourceManager.instance.GetText(64);
        textLeaving.text = ResourceManager.instance.GetText(65);
    }
}