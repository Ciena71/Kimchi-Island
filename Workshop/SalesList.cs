using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ProductMaterials
{
    public int[] index;
    public int[] quantity;
}

public class SalesList : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
    public RectTransform rectSalesProductList;
    public Button btnSalesProductList;
    public EventTrigger eventSalesProductList;
    public TextMeshProUGUI textSalesProductList;
    public Text textSalesValue;

    List<SalesData> salesData = new List<SalesData>();

    private void Awake()
    {
        UserManager userManager = UserManager.instance;
        btnSalesProductList?.onClick.AddListener(() =>
        {
            Workshop.instance.SetNowSalesData(this);
        });
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) =>
        {
            if (salesData != null && salesData.Count == 1)
            {
                if (salesData[0].product != null && salesData[0].product.Length > 0)
                {
                    ResourceManager resourceManager = ResourceManager.instance;
                    Dictionary<int, int> quantity = new Dictionary<int, int>();
                    for (int i = 0; i < salesData[0].product.Length; ++i)
                    {
                        ProductMaterials mats = resourceManager.GetProductMaterials(salesData[0].product[i]);
                        for (int j = 0; j < mats.index.Length; ++j)
                        {
                            if (!quantity.ContainsKey(mats.index[j]))
                                quantity.Add(mats.index[j], mats.quantity[j] * (userManager.GetWorkshopActive() > 3 ? 3 : userManager.GetWorkshopActive()));
                            else
                                quantity[mats.index[j]] += mats.quantity[j] * (userManager.GetWorkshopActive() > 3 ? 3 : userManager.GetWorkshopActive());
                        }
                        mats = null;
                    }
                    if (salesData[0].product4 != null && salesData[0].product4.Length > 0)
                    {
                        for (int i = 0; i < salesData[0].product4.Length; ++i)
                        {
                            ProductMaterials mats = resourceManager.GetProductMaterials(salesData[0].product4[i]);
                            for (int j = 0; j < mats.index.Length; ++j)
                            {
                                if (!quantity.ContainsKey(mats.index[j]))
                                    quantity.Add(mats.index[j], mats.quantity[j]);
                                else
                                    quantity[mats.index[j]] += mats.quantity[j];
                            }
                            mats = null;
                        }
                    }
                    string text = "";
                    foreach (KeyValuePair<int, int> kv in quantity)
                    {
                        int value = kv.Value - userManager.GetInventory(kv.Key);
                        if (value < 0)
                            value = 0;
                        text += $"<sprite={kv.Key}>{resourceManager.GetMaterialName(kv.Key)} : {value}\n";
                    }
                    quantity.Clear();
                    quantity = null;
                    Tooltip.instance.ShowToolTip(text);
                }
            }
        });
        eventSalesProductList.triggers.Add(entry);
        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerExit;
        entry1.callback.AddListener((data) =>
        {
            Tooltip.instance.HideToolTip();
        });
        eventSalesProductList.triggers.Add(entry1);
    }

    public void SetData(SalesData _salesData)
    {
        salesData.Clear();
        salesData.Add(_salesData);
    }

    public void SetDatas(SalesData[] _salesData)
    {
        salesData.Clear();
        salesData.AddRange(_salesData);
    }

    public List<SalesData> GetData() => salesData;

    public void OnBeginDrag(PointerEventData data)
    {
        if (btnSalesProductList)
            Workshop.instance.scrollTopSalesList.OnBeginDrag(data);
    }

    public void OnDrag(PointerEventData data)
    {
        if (btnSalesProductList)
            Workshop.instance.scrollTopSalesList.OnDrag(data);
    }

    public void OnEndDrag(PointerEventData data)
    {
        if (btnSalesProductList)
            Workshop.instance.scrollTopSalesList.OnEndDrag(data);
    }

    public void OnScroll(PointerEventData data)
    {
        if (btnSalesProductList)
            Workshop.instance.scrollTopSalesList.OnScroll(data);
    }
}