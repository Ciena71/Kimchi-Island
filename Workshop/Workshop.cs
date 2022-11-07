using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct SalesData
{
    public int[] product;
    public int[] value;
    public int totalValue;
}

public class Workshop : MonoBehaviour
{
    public static Workshop instance;

    public Text textRank;
    public InputField inputRank;
    public Text textGrooveNow;
    public InputField inputGrooveNow;
    public Text textGrooveMax;
    public InputField inputGrooveMax;
    public Text textTier1;
    public InputField inputTier1;
    public Text textTier2;
    public InputField inputTier2;
    public Text textTier3;
    public InputField inputTier3;
    public Text textGroovePriority;
    public Toggle toggleGroovePriority;
    public Text textCycle;
    public Dropdown dropCycle;
    public ScrollRect scrollProductList;
    public Text textProductName;
    public Button btnProductName;
    public Text textTime;
    public Button btnTime;
    public Text textQuantity;
    public Text textValue;
    public Button btnValue;
    public Text textPopularity;
    public Button btnPopularity;
    public Text textSupply;
    public Button btnSupply;
    public Text textCategory;
    public ScrollRect scrollTopSalesList;
    public Text textProductList;
    public Text textExpectedValue;
    public Text textCalculate;
    public Button btnCalculate;
    public SalesList nowSalesData;
    public Button btnNowSales;

    static List<Product> productList = new List<Product>();
    List<GameObject> salesList = new List<GameObject>();
    int cycle;
    bool[] reverse = new bool[5];
    int sort = 0;

    string copySupplyPacket;
    string copyPopularityPacket;

    void Awake()
    {
        instance = this;
        ResourceManager resourceManager = ResourceManager.instance;
        resourceManager.UpdateSupplyPattern();
        UserManager userManager = UserManager.instance;
        for (int i = 0; i < resourceManager.GetProductMax(); ++i)
        {
            GameObject productObject = Instantiate(Resources.Load("Prefab/Workshop/Product"),scrollProductList.content) as GameObject;
            Product product = productObject.GetComponent<Product>();
            product.SetDefaultData(i);
            productList.Add(product);
        }
        inputRank.text = userManager.GetPlayerRank().ToString();
        inputRank.onEndEdit.AddListener((data) =>
        {
            if (int.TryParse(data, out int value))
            {
                if (1 <= value && value <= 10)
                    userManager.SetPlayerRank(value);
                else
                    inputRank.text = userManager.GetPlayerRank().ToString();
            }
        });
        inputGrooveNow.text = userManager.GetCurrentGroove().ToString();
        inputGrooveNow.onEndEdit.AddListener((data) =>
        {
            if (int.TryParse(data, out int value))
            {
                if (0 <= value && value <= userManager.GetMaxGroove())
                    userManager.SetCurrentGroove(value);
                else
                    inputGrooveNow.text = userManager.GetCurrentGroove().ToString();
            }
        });
        inputGrooveMax.text = userManager.GetMaxGroove().ToString();
        inputGrooveMax.onEndEdit.AddListener((data) =>
        {
            if (int.TryParse(data, out int value))
            {
                if (10 <= value && value <= 35)
                {
                    userManager.SetMaxGroove(value);
                    if(userManager.GetCurrentGroove() > userManager.GetMaxGroove())
                    {
                        userManager.SetCurrentGroove(userManager.GetMaxGroove());
                        inputGrooveNow.text = userManager.GetCurrentGroove().ToString();
                    }
                }
                else
                    inputGrooveMax.text = userManager.GetMaxGroove().ToString();
            }
        });
        inputTier1.text = userManager.GetWorkshopTier(0).ToString();
        inputTier1.onEndEdit.AddListener((data) =>
        {
            if (int.TryParse(data, out int value))
            {
                if (1 <= value && value <= 3)
                    userManager.SetWorkshopTier(0, value);
                else
                    inputTier1.text = userManager.GetWorkshopTier(0).ToString();
            }
        });
        inputTier2.text = userManager.GetWorkshopTier(1).ToString();
        inputTier2.onEndEdit.AddListener((data) =>
        {
            if (int.TryParse(data, out int value))
            {
                if (0 <= value && value <= 3)
                    userManager.SetWorkshopTier(1, value);
                else
                    inputTier2.text = userManager.GetWorkshopTier(1).ToString();
            }
        });
        inputTier3.text = userManager.GetWorkshopTier(2).ToString();
        inputTier3.onEndEdit.AddListener((data) =>
        {
            if (int.TryParse(data, out int value))
            {
                if (0 <= value && value <= 3)
                    userManager.SetWorkshopTier(2, value);
                else
                    inputTier3.text = userManager.GetWorkshopTier(2).ToString();
            }
        });
        toggleGroovePriority.isOn = userManager.GetGroovePriority();
        toggleGroovePriority.onValueChanged.AddListener((value) =>
        {
            userManager.SetGroovePriority(value);
        });
        dropCycle.onValueChanged.AddListener((value) =>
        {
            salesList.ForEach((data) =>
            {
                Destroy(data);
            });
            salesList.Clear();
            int oldCycle = cycle;
            cycle = value;
            if(cycle < 7)
            {
                nowSalesData.SetData(userManager.GetSalesData(cycle));
                string productListString = "";
                string productValueString = "";
                if (userManager.GetSalesData(cycle).product != null && userManager.GetSalesData(cycle).product.Length > 0)
                {
                    productListString = "";
                    productValueString = $"{userManager.GetSalesData(cycle).totalValue * GetActiveWorkshop()} | {userManager.GetSalesData(cycle).totalValue} = ";
                    for (int i = 0; i < userManager.GetSalesData(cycle).product.Length; ++i)
                    {
                        if (i > 0)
                        {
                            productListString += " + ";
                            productValueString += " + ";
                        }
                        productListString += $"<sprite={userManager.GetSalesData(cycle).product[i]}> {resourceManager.GetProductName(userManager.GetSalesData(cycle).product[i])}";
                        productValueString += userManager.GetSalesData(cycle).value[i].ToString();
                    }
                }
                int groove = 0;
                if(cycle > 0)
                {
                    for (int i = 0; i < cycle; ++i)
                    {
                        int dummy = (userManager.GetSalesData(i).product != null ? userManager.GetSalesData(i).product.Length : 0);
                        groove += GetActiveWorkshop() * (dummy >= 2 ? (dummy - 1) : 0);
                    }
                }
                if (groove > userManager.GetMaxGroove())
                    groove = userManager.GetMaxGroove();
                userManager.SetCurrentGroove(groove);
                inputGrooveNow.text = groove.ToString();
                nowSalesData.textSalesProductList.text = productListString;
                nowSalesData.textSalesValue.text = productValueString;
            }
            else
            {
                nowSalesData.SetData(new SalesData());
                nowSalesData.textSalesProductList.text = "";
                nowSalesData.textSalesValue.text = "";
            }
            for (int i = 0; i < productList.Count; ++i)
                productList[i].SetCycle(value);
        });
        btnProductName.onClick.AddListener(() =>
        {
            if (sort == 0)
                reverse[0] = !reverse[0];
            else
                sort = 0;
            int index = 0;
            if (reverse[0])
            {
                for (int i = productList.Count - 1; i >= 0; --i)
                {
                    productList[i].transform.SetSiblingIndex(index);
                    ++index;
                }
            }
            else
            {
                productList.ForEach((form) =>
                {
                    form.transform.SetSiblingIndex(index);
                    ++index;
                });
            }
        });
        btnTime.onClick.AddListener(() =>
        {
            if (sort == 1)
                reverse[1] = !reverse[1];
            else
                sort = 1;
            if (reverse[1])
            {
                int index = 0;
                productList.ForEach((form) =>
                {
                    if (form.GetTime() == 8)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                productList.ForEach((form) =>
                {
                    if (form.GetTime() == 6)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                productList.ForEach((form) =>
                {
                    if (form.GetTime() == 4)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
            }
            else
            {
                int index = 0;
                productList.ForEach((form) =>
                {
                    if (form.GetTime() == 4)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                productList.ForEach((form) =>
                {
                    if (form.GetTime() == 6)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                productList.ForEach((form) =>
                {
                    if (form.GetTime() == 8)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
            }
        });
        btnValue.onClick.AddListener(() =>
        {
            if (sort == 2)
                reverse[2] = !reverse[2];
            else
                sort = 2;
            if (reverse[2])
            {
                productList.Sort((a, b) => a.GetValue().CompareTo(b.GetValue()));
                int index = 0;
                productList.ForEach((form) =>
                {
                    form.transform.SetSiblingIndex(index);
                    ++index;
                });
                productList.Sort((a, b) => a.GetIndex().CompareTo(b.GetIndex()));
            }
            else
            {
                productList.Sort((a, b) => b.GetValue().CompareTo(a.GetValue()));
                int index = 0;
                productList.ForEach((form) =>
                {
                    form.transform.SetSiblingIndex(index);
                    ++index;
                });
                productList.Sort((a, b) => a.GetIndex().CompareTo(b.GetIndex()));
            }
        });
        btnPopularity.onClick.AddListener(() =>
        {
            if (sort == 3)
                reverse[3] = !reverse[3];
            else
                sort = 3;
            if (reverse[3])
            {
                productList.Sort((a, b) => a.GetPopularity(cycle).CompareTo(b.GetPopularity(cycle)));
                int index = 0;
                productList.ForEach((form) =>
                {
                    form.transform.SetSiblingIndex(index);
                    ++index;
                });
                productList.Sort((a, b) => a.GetIndex().CompareTo(b.GetIndex()));
            }
            else
            {
                productList.Sort((a, b) => b.GetPopularity(cycle).CompareTo(a.GetPopularity(cycle)));
                int index = 0;
                productList.ForEach((form) =>
                {
                    form.transform.SetSiblingIndex(index);
                    ++index;
                });
                productList.Sort((a, b) => a.GetIndex().CompareTo(b.GetIndex()));
            }
        });
        btnSupply.onClick.AddListener(() =>
        {
            if (sort == 4)
                reverse[4] = !reverse[4];
            else
                sort = 4;
            if (reverse[4])
            {
                productList.Sort((a, b) => a.GetSupplyValue(cycle).CompareTo(b.GetSupplyValue(cycle)));
                int index = 0;
                productList.ForEach((form) =>
                {
                    form.transform.SetSiblingIndex(index);
                    ++index;
                });
                productList.Sort((a, b) => a.GetIndex().CompareTo(b.GetIndex()));
            }
            else
            {
                productList.Sort((a, b) => b.GetSupplyValue(cycle).CompareTo(a.GetSupplyValue(cycle)));
                int index = 0;
                productList.ForEach((form) =>
                {
                    form.transform.SetSiblingIndex(index);
                    ++index;
                });
                productList.Sort((a, b) => a.GetIndex().CompareTo(b.GetIndex()));
            }
        });
        nowSalesData.SetData(userManager.GetSalesData(0));
        string productListString = "";
        string productValueString = "";
        if (userManager.GetSalesData(0).product != null && userManager.GetSalesData(0).product.Length > 0)
        {
            productListString = "";
            productValueString = $"{userManager.GetSalesData(0).totalValue * GetActiveWorkshop()} | {userManager.GetSalesData(0).totalValue} = ";
            for (int i = 0; i < userManager.GetSalesData(0).product.Length; ++i)
            {
                if (i > 0)
                {
                    productListString += " + ";
                    productValueString += " + ";
                }
                productListString += $"<sprite={userManager.GetSalesData(0).product[i]}> {resourceManager.GetProductName(userManager.GetSalesData(0).product[i])}";
                productValueString += userManager.GetSalesData(0).value[i].ToString();
            }
        }
        nowSalesData.textSalesProductList.text = productListString;
        nowSalesData.textSalesValue.text = productValueString;
        btnCalculate.onClick.AddListener(() =>
        {
            salesList.ForEach((data) =>
            {
                Destroy(data);
            });
            salesList.Clear();
            List<SalesData> SalesDataList = GetResultList();
            SalesDataList.Sort((x1, x2) => x2.totalValue.CompareTo(x1.totalValue));
            foreach (var dummySalesData in SalesDataList)
            {
                if (salesList.Count < 100)
                {
                    GameObject salesListObject = Instantiate(Resources.Load("Prefab/Workshop/SalesList"), scrollTopSalesList.content) as GameObject;
                    SalesList data = salesListObject.GetComponent<SalesList>();
                    data.SetData(dummySalesData);
                    string productListString = "";
                    string productValueString = $"{dummySalesData.totalValue * GetActiveWorkshop()} | {dummySalesData.totalValue} = ";
                    for (int i = 0; i < dummySalesData.product.Length; ++i)
                    {
                        if (i > 0)
                        {
                            productListString += " + ";
                            productValueString += " + ";
                        }
                        productListString += $"<sprite={dummySalesData.product[i]}> {resourceManager.GetProductName(dummySalesData.product[i])}";
                        productValueString += dummySalesData.value[i].ToString();
                    }
                    data.textSalesProductList.text = productListString;
                    data.textSalesValue.text = productValueString;
                    salesList.Add(salesListObject);
                }
                else
                    break;
            }
            SalesDataList.Clear();
            SalesDataList = null;
        });
        btnNowSales.onClick.AddListener(() =>
        {
            UserManager userManager = UserManager.instance;
            if (userManager.GetSalesData(cycle).product != null)
            {
                for (int i = 0; i < userManager.GetSalesData(cycle).product.Length; ++i)
                {
                    int value = (i == 0 ? -1 : -2) * GetActiveWorkshop();
                    for (int j = cycle + 1; j < 7; ++j)
                        productList[userManager.GetSalesData(cycle).product[i]].AddSupply(j, value);
                }
            }
            userManager.SetSalesData(cycle, new SalesData());
            nowSalesData.textSalesProductList.text = "";
            nowSalesData.textSalesValue.text = "";
        });
        ApplyLanguage();
    }

    public int GetActiveWorkshop()
    {
        UserManager userManager = UserManager.instance;
        return 1 + (userManager.GetWorkshopTier(1) > 0 ? 1 : 0) + (userManager.GetWorkshopTier(2) > 0 ? 1 : 0);
    }

    int GetHighestWorkshopTier()
    {
        UserManager userManager = UserManager.instance;
        int value = userManager.GetWorkshopTier(0);
        if (value < userManager.GetWorkshopTier(1))
            value = userManager.GetWorkshopTier(1);
        if (value < userManager.GetWorkshopTier(2))
            value = userManager.GetWorkshopTier(2);
        return value;
    }

    float GetPopularityValue(int pop)
    {
        switch(pop)
        {
        case 1: return 1.4f;
        case 2: return 1.2f;
        case 3: return 1;
        case 4: return 0.8f;
        default: return 0;
        }
    }

    float GetSupplyValue(int sup)
    {
        switch (sup)
        {
        case 1: return 1.6f;
        case 2: return 1.3f;
        case 3: return 1;
        case 4: return 0.8f;
        case 5: return 0.6f;
        default: return 0;
        }
    }

    float GetWorkshopTierValue(int tier)
    {
        switch (tier)
        {
        case 1: return 1;
        case 2: return 1.1f;
        case 3: return 1.2f;
        default: return 0;
        }
    }

    int GetNowValue(int index, int step)
    {
        UserManager userManager = UserManager.instance;
        int nowGroove = (userManager.GetCurrentGroove() + GetActiveWorkshop() * step);
        return (step > 0 ? 2 : 1) * 
            Mathf.FloorToInt(GetPopularityValue(productList[index].GetPopularity(cycle)) *
            GetSupplyValue(productList[index].GetSupply(cycle)) * 
            Mathf.FloorToInt(productList[index].GetValue() *
            GetWorkshopTierValue(GetHighestWorkshopTier()) * 
            (1 + (((nowGroove < userManager.GetMaxGroove()) ? nowGroove : userManager.GetMaxGroove()) / 100.0f))));
    }

    public void SetPacketData(byte[] data)
    {
        if (data.Length == 96 && data[data.Length - 1] == 50)
        {
            copySupplyPacket = "";
            copyPopularityPacket = "";
            for (int i = 0; i < productList.Count; ++i)
            {
                int value = 0;
                if ((data[35 + i] & (1 << 6)) == (1 << 6))
                    value = 4;
                else if ((data[35 + i] & (1 << 5) + (1 << 4)) == (1 << 5) + (1 << 4))
                    value = 3;
                else if ((data[35 + i] & (1 << 5)) == (1 << 5))
                    value = 2;
                else if ((data[35 + i] & (1 << 4)) == (1 << 4))
                    value = 1;
                copySupplyPacket += $"{value}\t{3 - productList[i].GetDemandShift()}";
                copyPopularityPacket += $"{productList[i].GetPopularity(0)}\t{productList[i].GetPopularity(7)}";
                if (i < productList.Count - 1)
                {
                    copySupplyPacket += "\n";
                    copyPopularityPacket += "\n";
                }
            }
#if UNITY_EDITOR
            Debug.Log(copySupplyPacket + "\n");
            Debug.Log(copyPopularityPacket + "\n");
#endif
        }
    }

    public void SupplyPacketDataCopy() => GUIUtility.systemCopyBuffer = copySupplyPacket;

    public void PopularityPacketDataCopy() => GUIUtility.systemCopyBuffer = copyPopularityPacket;

    public int GetCycle() => cycle;

    public void ApplyLanguage()
    {
        ResourceManager resourceManager = ResourceManager.instance;
        textRank.text = resourceManager.GetText(1);
        textGrooveNow.text = resourceManager.GetText(2);
        textGrooveMax.text = resourceManager.GetText(3);
        textTier1.text = resourceManager.GetText(4);
        textTier2.text = resourceManager.GetText(5);
        textTier3.text = resourceManager.GetText(6);
        textGroovePriority.text = resourceManager.GetText(19);
        switch (cycle)
        {
        case 0:
        case 1:
        case 2:
        case 3:
        case 4:
        case 5:
        case 6:
            {
                dropCycle.captionText.text = resourceManager.GetText(35).Replace("{0}", (cycle + 1).ToString());
                break;
            }
        case 7:
            {
                dropCycle.captionText.text = resourceManager.GetText(27);
                break;
            }
        case 8:
            {
                dropCycle.captionText.text = resourceManager.GetText(31);
                break;
            }
        }
        textCycle.text = resourceManager.GetText(32);
        int num = 0;
        dropCycle.options.ForEach((form) =>
        {
            switch(num)
            {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
                {
                    form.text = resourceManager.GetText(35).Replace("{0}", (num + 1).ToString());
                    break;
                }
            case 7:
                {
                    form.text = resourceManager.GetText(27);
                    break;
                }
            case 8:
                {
                    form.text = resourceManager.GetText(31);
                    break;
                }
            }
            ++num;
        });
        textProductName.text = resourceManager.GetText(7);
        textTime.text = resourceManager.GetText(8);
        textQuantity.text = resourceManager.GetText(9);
        textValue.text = resourceManager.GetText(10);
        textPopularity.text = resourceManager.GetText(11);
        textSupply.text = resourceManager.GetText(12);
        textCategory.text = resourceManager.GetText(13);
        textProductList.text = resourceManager.GetText(7);
        textExpectedValue.text = resourceManager.GetText(14);
        textCalculate.text = resourceManager.GetText(15);
        productList.ForEach((product) =>
        {
            product.ApplyLanguage(cycle);
        });
    }

    List<SalesData> GetResultList()
    {
        List<SalesData> SalesDataList = new List<SalesData>();
        SalesData salesdata;
        int checker;
        UserManager userManager = UserManager.instance;
        if (userManager.GetCurrentGroove() < userManager.GetMaxGroove() && userManager.GetGroovePriority())
        {
            if (userManager.GetCurrentGroove() + (GetActiveWorkshop() * 6) <= userManager.GetMaxGroove() + (GetActiveWorkshop() - 1))
            {
                for (int a = 0; a < productList.Count; ++a)
                {
                    if (productList[a].IsActive() && productList[a].GetTime() == 4)
                    {
                        for (int b = 0; b < productList.Count; ++b)
                        {
                            if (a != b && productList[b].IsActive() && productList[b].GetTime() == 4 && ((productList[a].GetCategory() & productList[b].GetCategory()) > 0))
                            {
                                for (int c = 0; c < productList.Count; ++c)
                                {
                                    if (b != c && productList[c].IsActive() && productList[c].GetTime() == 4 && ((productList[b].GetCategory() & productList[c].GetCategory()) > 0))
                                    {
                                        for (int d = 0; d < productList.Count; ++d)
                                        {
                                            if (c != d && productList[d].IsActive() && productList[d].GetTime() == 4 && ((productList[c].GetCategory() & productList[d].GetCategory()) > 0))
                                            {
                                                for (int e = 0; e < productList.Count; ++e)
                                                {
                                                    if (d != e && productList[e].IsActive() && productList[e].GetTime() == 4 && ((productList[d].GetCategory() & productList[e].GetCategory()) > 0))
                                                    {
                                                        for (int f = 0; f < productList.Count; ++f)
                                                        {
                                                            if (e != f && productList[f].IsActive() && productList[f].GetTime() == 4 && ((productList[e].GetCategory() & productList[f].GetCategory()) > 0))
                                                            {
                                                                salesdata = new SalesData();
                                                                salesdata.product = new int[6];
                                                                salesdata.product[0] = a;
                                                                salesdata.product[1] = b;
                                                                salesdata.product[2] = c;
                                                                salesdata.product[3] = d;
                                                                salesdata.product[4] = e;
                                                                salesdata.product[5] = f;
                                                                productList[a].ResetCount();
                                                                productList[b].ResetCount();
                                                                productList[c].ResetCount();
                                                                productList[d].ResetCount();
                                                                productList[e].ResetCount();
                                                                productList[f].ResetCount();
                                                                salesdata.value = new int[6];
                                                                for (int i = 0; i < 6; ++i)
                                                                {
                                                                    salesdata.value[i] += GetNowValue(salesdata.product[i], i);
                                                                    salesdata.totalValue += salesdata.value[i];
                                                                    productList[salesdata.product[i]].AddCount(GetActiveWorkshop() * (i == 0 ? 1 : 2));
                                                                }
                                                                SalesDataList.Add(salesdata);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int a = 0; a < productList.Count; ++a)
                {
                    if (productList[a].IsActive() && productList[a].GetTime() == 4)
                    {
                        if (userManager.GetCurrentGroove() + GetActiveWorkshop() < userManager.GetMaxGroove())
                        {
                            for (int b = 0; b < productList.Count; ++b)
                            {
                                if (a != b && productList[b].IsActive() && productList[b].GetTime() == 4 && ((productList[a].GetCategory() & productList[b].GetCategory()) > 0))
                                {
                                    if (userManager.GetCurrentGroove() + GetActiveWorkshop() * 2 < userManager.GetMaxGroove())
                                    {
                                        for (int c = 0; c < productList.Count; ++c)
                                        {
                                            if (b != c && productList[c].IsActive() && productList[c].GetTime() == 4 && ((productList[b].GetCategory() & productList[c].GetCategory()) > 0))
                                            {
                                                if (userManager.GetCurrentGroove() + GetActiveWorkshop() * 3 < userManager.GetMaxGroove())
                                                {
                                                    for (int d = 0; d < productList.Count; ++d)
                                                    {
                                                        if (c != d && productList[d].IsActive() && productList[d].GetTime() == 4 && ((productList[c].GetCategory() & productList[d].GetCategory()) > 0))
                                                        {
                                                            if (userManager.GetCurrentGroove() + GetActiveWorkshop() * 4 < userManager.GetMaxGroove())
                                                            {
                                                                for (int e = 0; e < productList.Count; ++e)
                                                                {
                                                                    if (d != e && productList[e].IsActive() && productList[e].GetTime() == 4 && ((productList[d].GetCategory() & productList[e].GetCategory()) > 0))
                                                                    {
                                                                        for (int f = 0; f < productList.Count; ++f)
                                                                        {
                                                                            if (e != f && productList[f].IsActive() && productList[f].GetTime() == 4 && ((productList[e].GetCategory() & productList[f].GetCategory()) > 0))
                                                                            {
                                                                                salesdata = new SalesData();
                                                                                salesdata.product = new int[6];
                                                                                salesdata.product[0] = a;
                                                                                salesdata.product[1] = b;
                                                                                salesdata.product[2] = c;
                                                                                salesdata.product[3] = d;
                                                                                salesdata.product[4] = e;
                                                                                salesdata.product[5] = f;
                                                                                productList[a].ResetCount();
                                                                                productList[b].ResetCount();
                                                                                productList[c].ResetCount();
                                                                                productList[d].ResetCount();
                                                                                productList[e].ResetCount();
                                                                                productList[f].ResetCount();
                                                                                salesdata.value = new int[6];
                                                                                for (int i = 0; i < 6; ++i)
                                                                                {
                                                                                    salesdata.value[i] += GetNowValue(salesdata.product[i], i);
                                                                                    salesdata.totalValue += salesdata.value[i];
                                                                                    productList[salesdata.product[i]].AddCount(GetActiveWorkshop() * (i == 0 ? 1 : 2));
                                                                                }
                                                                                SalesDataList.Add(salesdata);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                for (int e = 0; e < productList.Count; ++e)
                                                                {
                                                                    if (d != e && productList[e].IsActive() && ((productList[d].GetCategory() & productList[e].GetCategory()) > 0))
                                                                    {
                                                                        checker = productList[a].GetTime() + productList[b].GetTime() + productList[c].GetTime() + productList[d].GetTime() + productList[e].GetTime();
                                                                        if (checker <= 24)
                                                                        {
                                                                            if (checker <= 20)
                                                                            {
                                                                                for (int f = 0; f < productList.Count; ++f)
                                                                                {
                                                                                    if (e != f && productList[f].IsActive() && ((productList[e].GetCategory() & productList[f].GetCategory()) > 0))
                                                                                    {
                                                                                        checker = productList[a].GetTime() + productList[b].GetTime() + productList[c].GetTime() + productList[d].GetTime() + productList[e].GetTime() + productList[f].GetTime();
                                                                                        if (checker <= 24)
                                                                                        {
                                                                                            salesdata = new SalesData();
                                                                                            salesdata.product = new int[6];
                                                                                            salesdata.product[0] = a;
                                                                                            salesdata.product[1] = b;
                                                                                            salesdata.product[2] = c;
                                                                                            salesdata.product[3] = d;
                                                                                            salesdata.product[4] = e;
                                                                                            salesdata.product[5] = f;
                                                                                            productList[a].ResetCount();
                                                                                            productList[b].ResetCount();
                                                                                            productList[c].ResetCount();
                                                                                            productList[d].ResetCount();
                                                                                            productList[e].ResetCount();
                                                                                            productList[f].ResetCount();
                                                                                            salesdata.value = new int[6];
                                                                                            for (int i = 0; i < 6; ++i)
                                                                                            {
                                                                                                salesdata.value[i] += GetNowValue(salesdata.product[i], i);
                                                                                                salesdata.totalValue += salesdata.value[i];
                                                                                                productList[salesdata.product[i]].AddCount(GetActiveWorkshop() * (i == 0 ? 1 : 2));
                                                                                            }
                                                                                            SalesDataList.Add(salesdata);
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                salesdata = new SalesData();
                                                                                salesdata.product = new int[5];
                                                                                salesdata.product[0] = a;
                                                                                salesdata.product[1] = b;
                                                                                salesdata.product[2] = c;
                                                                                salesdata.product[3] = d;
                                                                                salesdata.product[4] = e;
                                                                                productList[a].ResetCount();
                                                                                productList[b].ResetCount();
                                                                                productList[c].ResetCount();
                                                                                productList[d].ResetCount();
                                                                                productList[e].ResetCount();
                                                                                salesdata.value = new int[5];
                                                                                for (int i = 0; i < 5; ++i)
                                                                                {
                                                                                    salesdata.value[i] += GetNowValue(salesdata.product[i], i);
                                                                                    salesdata.totalValue += salesdata.value[i];
                                                                                    productList[salesdata.product[i]].AddCount(GetActiveWorkshop() * (i == 0 ? 1 : 2));
                                                                                }
                                                                                SalesDataList.Add(salesdata);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    for (int d = 0; d < productList.Count; ++d)
                                                    {
                                                        if (c != d && productList[d].IsActive() && ((productList[c].GetCategory() & productList[d].GetCategory()) > 0))
                                                        {
                                                            checker = productList[a].GetTime() + productList[b].GetTime() + productList[c].GetTime() + productList[d].GetTime();
                                                            if (checker <= 24)
                                                            {
                                                                if (checker <= 20)
                                                                {
                                                                    for (int e = 0; e < productList.Count; ++e)
                                                                    {
                                                                        if (d != e && productList[e].IsActive() && ((productList[d].GetCategory() & productList[e].GetCategory()) > 0))
                                                                        {
                                                                            checker = productList[a].GetTime() + productList[b].GetTime() + productList[c].GetTime() + productList[d].GetTime() + productList[e].GetTime();
                                                                            if (checker <= 24)
                                                                            {
                                                                                if (checker <= 20)
                                                                                {
                                                                                    for (int f = 0; f < productList.Count; ++f)
                                                                                    {
                                                                                        if (e != f && productList[f].IsActive() && ((productList[e].GetCategory() & productList[f].GetCategory()) > 0))
                                                                                        {
                                                                                            checker = productList[a].GetTime() + productList[b].GetTime() + productList[c].GetTime() + productList[d].GetTime() + productList[e].GetTime() + productList[f].GetTime();
                                                                                            if (checker <= 24)
                                                                                            {
                                                                                                salesdata = new SalesData();
                                                                                                salesdata.product = new int[6];
                                                                                                salesdata.product[0] = a;
                                                                                                salesdata.product[1] = b;
                                                                                                salesdata.product[2] = c;
                                                                                                salesdata.product[3] = d;
                                                                                                salesdata.product[4] = e;
                                                                                                salesdata.product[5] = f;
                                                                                                productList[a].ResetCount();
                                                                                                productList[b].ResetCount();
                                                                                                productList[c].ResetCount();
                                                                                                productList[d].ResetCount();
                                                                                                productList[e].ResetCount();
                                                                                                productList[f].ResetCount();
                                                                                                salesdata.value = new int[6];
                                                                                                for (int i = 0; i < 6; ++i)
                                                                                                {
                                                                                                    salesdata.value[i] += GetNowValue(salesdata.product[i], i);
                                                                                                    salesdata.totalValue += salesdata.value[i];
                                                                                                    productList[salesdata.product[i]].AddCount(GetActiveWorkshop() * (i == 0 ? 1 : 2));
                                                                                                }
                                                                                                SalesDataList.Add(salesdata);
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    salesdata = new SalesData();
                                                                                    salesdata.product = new int[5];
                                                                                    salesdata.product[0] = a;
                                                                                    salesdata.product[1] = b;
                                                                                    salesdata.product[2] = c;
                                                                                    salesdata.product[3] = d;
                                                                                    salesdata.product[4] = e;
                                                                                    productList[a].ResetCount();
                                                                                    productList[b].ResetCount();
                                                                                    productList[c].ResetCount();
                                                                                    productList[d].ResetCount();
                                                                                    productList[e].ResetCount();
                                                                                    salesdata.value = new int[5];
                                                                                    for (int i = 0; i < 5; ++i)
                                                                                    {
                                                                                        salesdata.value[i] += GetNowValue(salesdata.product[i], i);
                                                                                        salesdata.totalValue += salesdata.value[i];
                                                                                        productList[salesdata.product[i]].AddCount(GetActiveWorkshop() * (i == 0 ? 1 : 2));
                                                                                    }
                                                                                    SalesDataList.Add(salesdata);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    salesdata = new SalesData();
                                                                    salesdata.product = new int[4];
                                                                    salesdata.product[0] = a;
                                                                    salesdata.product[1] = b;
                                                                    salesdata.product[2] = c;
                                                                    salesdata.product[3] = d;
                                                                    productList[a].ResetCount();
                                                                    productList[b].ResetCount();
                                                                    productList[c].ResetCount();
                                                                    productList[d].ResetCount();
                                                                    salesdata.value = new int[4];
                                                                    for (int i = 0; i < 4; ++i)
                                                                    {
                                                                        salesdata.value[i] += GetNowValue(salesdata.product[i], i);
                                                                        salesdata.totalValue += salesdata.value[i];
                                                                        productList[salesdata.product[i]].AddCount(GetActiveWorkshop() * (i == 0 ? 1 : 2));
                                                                    }
                                                                    SalesDataList.Add(salesdata);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int c = 0; c < productList.Count; ++c)
                                        {
                                            if (b != c && productList[c].IsActive() && ((productList[b].GetCategory() & productList[c].GetCategory()) > 0))
                                            {
                                                checker = productList[a].GetTime() + productList[b].GetTime() + productList[c].GetTime();
                                                if (checker <= 24)
                                                {
                                                    if (checker <= 20)
                                                    {
                                                        for (int d = 0; d < productList.Count; ++d)
                                                        {
                                                            if (c != d && productList[d].IsActive() && ((productList[c].GetCategory() & productList[d].GetCategory()) > 0))
                                                            {
                                                                checker = productList[a].GetTime() + productList[b].GetTime() + productList[c].GetTime() + productList[d].GetTime();
                                                                if (checker <= 24)
                                                                {
                                                                    if (checker <= 20)
                                                                    {
                                                                        for (int e = 0; e < productList.Count; ++e)
                                                                        {
                                                                            if (d != e && productList[e].IsActive() && ((productList[d].GetCategory() & productList[e].GetCategory()) > 0))
                                                                            {
                                                                                checker = productList[a].GetTime() + productList[b].GetTime() + productList[c].GetTime() + productList[d].GetTime() + productList[e].GetTime();
                                                                                if (checker <= 24)
                                                                                {
                                                                                    if (checker <= 20)
                                                                                    {
                                                                                        for (int f = 0; f < productList.Count; ++f)
                                                                                        {
                                                                                            if (e != f && productList[f].IsActive() && ((productList[e].GetCategory() & productList[f].GetCategory()) > 0))
                                                                                            {
                                                                                                checker = productList[a].GetTime() + productList[b].GetTime() + productList[c].GetTime() + productList[d].GetTime() + productList[e].GetTime() + productList[f].GetTime();
                                                                                                if (checker <= 24)
                                                                                                {
                                                                                                    salesdata = new SalesData();
                                                                                                    salesdata.product = new int[6];
                                                                                                    salesdata.product[0] = a;
                                                                                                    salesdata.product[1] = b;
                                                                                                    salesdata.product[2] = c;
                                                                                                    salesdata.product[3] = d;
                                                                                                    salesdata.product[4] = e;
                                                                                                    salesdata.product[5] = f;
                                                                                                    productList[a].ResetCount();
                                                                                                    productList[b].ResetCount();
                                                                                                    productList[c].ResetCount();
                                                                                                    productList[d].ResetCount();
                                                                                                    productList[e].ResetCount();
                                                                                                    productList[f].ResetCount();
                                                                                                    salesdata.value = new int[6];
                                                                                                    for (int i = 0; i < 6; ++i)
                                                                                                    {
                                                                                                        salesdata.value[i] += GetNowValue(salesdata.product[i], i);
                                                                                                        salesdata.totalValue += salesdata.value[i];
                                                                                                        productList[salesdata.product[i]].AddCount(GetActiveWorkshop() * (i == 0 ? 1 : 2));
                                                                                                    }
                                                                                                    SalesDataList.Add(salesdata);
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        salesdata = new SalesData();
                                                                                        salesdata.product = new int[5];
                                                                                        salesdata.product[0] = a;
                                                                                        salesdata.product[1] = b;
                                                                                        salesdata.product[2] = c;
                                                                                        salesdata.product[3] = d;
                                                                                        salesdata.product[4] = e;
                                                                                        productList[a].ResetCount();
                                                                                        productList[b].ResetCount();
                                                                                        productList[c].ResetCount();
                                                                                        productList[d].ResetCount();
                                                                                        productList[e].ResetCount();
                                                                                        salesdata.value = new int[5];
                                                                                        for (int i = 0; i < 5; ++i)
                                                                                        {
                                                                                            salesdata.value[i] += GetNowValue(salesdata.product[i], i);
                                                                                            salesdata.totalValue += salesdata.value[i];
                                                                                            productList[salesdata.product[i]].AddCount(GetActiveWorkshop() * (i == 0 ? 1 : 2));
                                                                                        }
                                                                                        SalesDataList.Add(salesdata);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        salesdata = new SalesData();
                                                                        salesdata.product = new int[4];
                                                                        salesdata.product[0] = a;
                                                                        salesdata.product[1] = b;
                                                                        salesdata.product[2] = c;
                                                                        salesdata.product[3] = d;
                                                                        productList[a].ResetCount();
                                                                        productList[b].ResetCount();
                                                                        productList[c].ResetCount();
                                                                        productList[d].ResetCount();
                                                                        salesdata.value = new int[4];
                                                                        for (int i = 0; i < 4; ++i)
                                                                        {
                                                                            salesdata.value[i] += GetNowValue(salesdata.product[i], i);
                                                                            salesdata.totalValue += salesdata.value[i];
                                                                            productList[salesdata.product[i]].AddCount(GetActiveWorkshop() * (i == 0 ? 1 : 2));
                                                                        }
                                                                        SalesDataList.Add(salesdata);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        salesdata = new SalesData();
                                                        salesdata.product = new int[3];
                                                        salesdata.product[0] = a;
                                                        salesdata.product[1] = b;
                                                        salesdata.product[2] = c;
                                                        productList[a].ResetCount();
                                                        productList[b].ResetCount();
                                                        productList[c].ResetCount();
                                                        salesdata.value = new int[3];
                                                        for (int i = 0; i < 3; ++i)
                                                        {
                                                            salesdata.value[i] += GetNowValue(salesdata.product[i], i);
                                                            salesdata.totalValue += salesdata.value[i];
                                                            productList[salesdata.product[i]].AddCount(GetActiveWorkshop() * (i == 0 ? 1 : 2));
                                                        }
                                                        SalesDataList.Add(salesdata);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int b = 0; b < productList.Count; ++b)
                            {
                                if (a != b && productList[b].IsActive() && ((productList[a].GetCategory() & productList[b].GetCategory()) > 0))
                                {
                                    for (int c = 0; c < productList.Count; ++c)
                                    {
                                        if (b != c && productList[c].IsActive() && ((productList[b].GetCategory() & productList[c].GetCategory()) > 0))
                                        {
                                            checker = productList[a].GetTime() + productList[b].GetTime() + productList[c].GetTime();
                                            if (checker <= 24)
                                            {
                                                if (checker <= 20)
                                                {
                                                    for (int d = 0; d < productList.Count; ++d)
                                                    {
                                                        if (c != d && productList[d].IsActive() && ((productList[c].GetCategory() & productList[d].GetCategory()) > 0))
                                                        {
                                                            checker = productList[a].GetTime() + productList[b].GetTime() + productList[c].GetTime() + productList[d].GetTime();
                                                            if (checker <= 24)
                                                            {
                                                                if (checker <= 20)
                                                                {
                                                                    for (int e = 0; e < productList.Count; ++e)
                                                                    {
                                                                        if (d != e && productList[e].IsActive() && ((productList[d].GetCategory() & productList[e].GetCategory()) > 0))
                                                                        {
                                                                            checker = productList[a].GetTime() + productList[b].GetTime() + productList[c].GetTime() + productList[d].GetTime() + productList[e].GetTime();
                                                                            if (checker <= 24)
                                                                            {
                                                                                if (checker <= 20)
                                                                                {
                                                                                    for (int f = 0; f < productList.Count; ++f)
                                                                                    {
                                                                                        if (e != f && productList[f].IsActive() && ((productList[e].GetCategory() & productList[f].GetCategory()) > 0))
                                                                                        {
                                                                                            checker = productList[a].GetTime() + productList[b].GetTime() + productList[c].GetTime() + productList[d].GetTime() + productList[e].GetTime() + productList[f].GetTime();
                                                                                            if (checker <= 24)
                                                                                            {
                                                                                                salesdata = new SalesData();
                                                                                                salesdata.product = new int[6];
                                                                                                salesdata.product[0] = a;
                                                                                                salesdata.product[1] = b;
                                                                                                salesdata.product[2] = c;
                                                                                                salesdata.product[3] = d;
                                                                                                salesdata.product[4] = e;
                                                                                                salesdata.product[5] = f;
                                                                                                productList[a].ResetCount();
                                                                                                productList[b].ResetCount();
                                                                                                productList[c].ResetCount();
                                                                                                productList[d].ResetCount();
                                                                                                productList[e].ResetCount();
                                                                                                productList[f].ResetCount();
                                                                                                salesdata.value = new int[6];
                                                                                                for (int i = 0; i < 6; ++i)
                                                                                                {
                                                                                                    salesdata.value[i] += GetNowValue(salesdata.product[i], i);
                                                                                                    salesdata.totalValue += salesdata.value[i];
                                                                                                    productList[salesdata.product[i]].AddCount(GetActiveWorkshop() * (i == 0 ? 1 : 2));
                                                                                                }
                                                                                                SalesDataList.Add(salesdata);
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    salesdata = new SalesData();
                                                                                    salesdata.product = new int[5];
                                                                                    salesdata.product[0] = a;
                                                                                    salesdata.product[1] = b;
                                                                                    salesdata.product[2] = c;
                                                                                    salesdata.product[3] = d;
                                                                                    salesdata.product[4] = e;
                                                                                    productList[a].ResetCount();
                                                                                    productList[b].ResetCount();
                                                                                    productList[c].ResetCount();
                                                                                    productList[d].ResetCount();
                                                                                    productList[e].ResetCount();
                                                                                    salesdata.value = new int[5];
                                                                                    for (int i = 0; i < 5; ++i)
                                                                                    {
                                                                                        salesdata.value[i] += GetNowValue(salesdata.product[i], i);
                                                                                        salesdata.totalValue += salesdata.value[i];
                                                                                        productList[salesdata.product[i]].AddCount(GetActiveWorkshop() * (i == 0 ? 1 : 2));
                                                                                    }
                                                                                    SalesDataList.Add(salesdata);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    salesdata = new SalesData();
                                                                    salesdata.product = new int[4];
                                                                    salesdata.product[0] = a;
                                                                    salesdata.product[1] = b;
                                                                    salesdata.product[2] = c;
                                                                    salesdata.product[3] = d;
                                                                    productList[a].ResetCount();
                                                                    productList[b].ResetCount();
                                                                    productList[c].ResetCount();
                                                                    productList[d].ResetCount();
                                                                    salesdata.value = new int[4];
                                                                    for (int i = 0; i < 4; ++i)
                                                                    {
                                                                        salesdata.value[i] += GetNowValue(salesdata.product[i], i);
                                                                        salesdata.totalValue += salesdata.value[i];
                                                                        productList[salesdata.product[i]].AddCount(GetActiveWorkshop() * (i == 0 ? 1 : 2));
                                                                    }
                                                                    SalesDataList.Add(salesdata);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    salesdata = new SalesData();
                                                    salesdata.product = new int[3];
                                                    salesdata.product[0] = a;
                                                    salesdata.product[1] = b;
                                                    salesdata.product[2] = c;
                                                    productList[a].ResetCount();
                                                    productList[b].ResetCount();
                                                    productList[c].ResetCount();
                                                    salesdata.value = new int[3];
                                                    for (int i = 0; i < 3; ++i)
                                                    {
                                                        salesdata.value[i] += GetNowValue(salesdata.product[i], i);
                                                        salesdata.totalValue += salesdata.value[i];
                                                        productList[salesdata.product[i]].AddCount(GetActiveWorkshop() * (i == 0 ? 1 : 2));
                                                    }
                                                    SalesDataList.Add(salesdata);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            for (int a = 0; a < productList.Count; ++a)
            {
                if (productList[a].IsActive())
                {
                    for (int b = 0; b < productList.Count; ++b)
                    {
                        if (a != b && productList[b].IsActive() && ((productList[a].GetCategory() & productList[b].GetCategory()) > 0))
                        {
                            for (int c = 0; c < productList.Count; ++c)
                            {
                                if (b != c && productList[c].IsActive() && ((productList[b].GetCategory() & productList[c].GetCategory()) > 0))
                                {
                                    checker = productList[a].GetTime() + productList[b].GetTime() + productList[c].GetTime();
                                    if (checker <= 24)
                                    {
                                        if (checker <= 20)
                                        {
                                            for (int d = 0; d < productList.Count; ++d)
                                            {
                                                if (c != d && productList[d].IsActive() && ((productList[c].GetCategory() & productList[d].GetCategory()) > 0))
                                                {
                                                    checker = productList[a].GetTime() + productList[b].GetTime() + productList[c].GetTime() + productList[d].GetTime();
                                                    if (checker <= 24)
                                                    {
                                                        if (checker <= 20)
                                                        {
                                                            for (int e = 0; e < productList.Count; ++e)
                                                            {
                                                                if (d != e && productList[e].IsActive() && ((productList[d].GetCategory() & productList[e].GetCategory()) > 0))
                                                                {
                                                                    checker = productList[a].GetTime() + productList[b].GetTime() + productList[c].GetTime() + productList[d].GetTime() + productList[e].GetTime();
                                                                    if (checker <= 24)
                                                                    {
                                                                        if (checker <= 20)
                                                                        {
                                                                            for (int f = 0; f < productList.Count; ++f)
                                                                            {
                                                                                if (e != f && productList[f].IsActive() && ((productList[e].GetCategory() & productList[f].GetCategory()) > 0))
                                                                                {
                                                                                    checker = productList[a].GetTime() + productList[b].GetTime() + productList[c].GetTime() + productList[d].GetTime() + productList[e].GetTime() + productList[f].GetTime();
                                                                                    if (checker <= 24)
                                                                                    {
                                                                                        salesdata = new SalesData();
                                                                                        salesdata.product = new int[6];
                                                                                        salesdata.product[0] = a;
                                                                                        salesdata.product[1] = b;
                                                                                        salesdata.product[2] = c;
                                                                                        salesdata.product[3] = d;
                                                                                        salesdata.product[4] = e;
                                                                                        salesdata.product[5] = f;
                                                                                        productList[a].ResetCount();
                                                                                        productList[b].ResetCount();
                                                                                        productList[c].ResetCount();
                                                                                        productList[d].ResetCount();
                                                                                        productList[e].ResetCount();
                                                                                        productList[f].ResetCount();
                                                                                        salesdata.value = new int[6];
                                                                                        for (int i = 0; i < 6; ++i)
                                                                                        {
                                                                                            salesdata.value[i] += GetNowValue(salesdata.product[i], i);
                                                                                            salesdata.totalValue += salesdata.value[i];
                                                                                            productList[salesdata.product[i]].AddCount(GetActiveWorkshop() * (i == 0 ? 1 : 2));
                                                                                        }
                                                                                        SalesDataList.Add(salesdata);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            salesdata = new SalesData();
                                                                            salesdata.product = new int[5];
                                                                            salesdata.product[0] = a;
                                                                            salesdata.product[1] = b;
                                                                            salesdata.product[2] = c;
                                                                            salesdata.product[3] = d;
                                                                            salesdata.product[4] = e;
                                                                            productList[a].ResetCount();
                                                                            productList[b].ResetCount();
                                                                            productList[c].ResetCount();
                                                                            productList[d].ResetCount();
                                                                            productList[e].ResetCount();
                                                                            salesdata.value = new int[5];
                                                                            for (int i = 0; i < 5; ++i)
                                                                            {
                                                                                salesdata.value[i] += GetNowValue(salesdata.product[i], i);
                                                                                salesdata.totalValue += salesdata.value[i];
                                                                                productList[salesdata.product[i]].AddCount(GetActiveWorkshop() * (i == 0 ? 1 : 2));
                                                                            }
                                                                            SalesDataList.Add(salesdata);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            salesdata = new SalesData();
                                                            salesdata.product = new int[4];
                                                            salesdata.product[0] = a;
                                                            salesdata.product[1] = b;
                                                            salesdata.product[2] = c;
                                                            salesdata.product[3] = d;
                                                            productList[a].ResetCount();
                                                            productList[b].ResetCount();
                                                            productList[c].ResetCount();
                                                            productList[d].ResetCount();
                                                            salesdata.value = new int[4];
                                                            for (int i = 0; i < 4; ++i)
                                                            {
                                                                salesdata.value[i] += GetNowValue(salesdata.product[i], i);
                                                                salesdata.totalValue += salesdata.value[i];
                                                                productList[salesdata.product[i]].AddCount(GetActiveWorkshop() * (i == 0 ? 1 : 2));
                                                            }
                                                            SalesDataList.Add(salesdata);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            salesdata = new SalesData();
                                            salesdata.product = new int[3];
                                            salesdata.product[0] = a;
                                            salesdata.product[1] = b;
                                            salesdata.product[2] = c;
                                            productList[a].ResetCount();
                                            productList[b].ResetCount();
                                            productList[c].ResetCount();
                                            salesdata.value = new int[3];
                                            for (int i = 0; i < 3; ++i)
                                            {
                                                salesdata.value[i] += GetNowValue(salesdata.product[i], i);
                                                salesdata.totalValue += salesdata.value[i];
                                                productList[salesdata.product[i]].AddCount(GetActiveWorkshop() * (i == 0 ? 1 : 2));
                                            }
                                            SalesDataList.Add(salesdata);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        productList.ForEach((form) =>
        {
            form.ResetCount();
        });
        return SalesDataList;
    }

    public void UpdatePeakData()
    {
        ResourceManager resourceManager = ResourceManager.instance;
        int index = 0;
        productList.ForEach((form) =>
        {
            string[] data = resourceManager.GetSupplyPattern()[index]["Pattern"].ToString().Split(' ');
            int[] pop = { 0, 0 };
            int.TryParse(resourceManager.GetSupplyPattern()[index]["Popularity"].ToString(), out pop[0]);
            int.TryParse(resourceManager.GetSupplyPattern()[index]["Next Popularity"].ToString(), out pop[1]);
            if (data.Length == 2)
            {
                if (data[0] != "Cycle")
                    form.SetPeak(int.Parse(data[0]), data[1] == "Strong" ? 1 : 2, pop[0], pop[1]);
                else
                {
                    if (data[1] == "4/5")
                        form.SetPeak(4, 0, pop[0], pop[1]);
                    else if (data[1] == "5")
                        form.SetPeak(5, 0, pop[0], pop[1]);
                    else
                        form.SetPeak(6, 0, pop[0], pop[1]);
                }
            }
            else
                form.SetPeak(0, 0, pop[0], pop[1]);
            ++index;
        });
        UserManager userManager = UserManager.instance;
        if (cycle < 6)
        {
            for (int i = 0; i < 6; ++i)
            {
                if (userManager.GetSalesData(i).product != null && userManager.GetSalesData(i).product.Length > 0)
                {
                    for (int j = 0; j < userManager.GetSalesData(i).product.Length; ++j)
                    {
                        int value = (j == 0 ? 1 : 2) * GetActiveWorkshop();
                        for (int k = i + 1; k < 7; ++k)
                            productList[userManager.GetSalesData(i).product[j]].AddSupply(k, value);
                    }
                }
            }
        }
    }

    public List<Product> GetProductList() => productList;

    public void SetNowSalesData(SalesList list)
    {
        if (cycle < 7)
        {
            UserManager userManager = UserManager.instance;
            if (userManager.GetSalesData(cycle).product != null)
            {
                if (cycle < 6)
                {
                    for (int i = 0; i < userManager.GetSalesData(cycle).product.Length; ++i)
                    {
                        int value = (i == 0 ? -1 : -2) * GetActiveWorkshop();
                        for (int j = cycle + 1; j < 7; ++j)
                            productList[userManager.GetSalesData(cycle).product[i]].AddSupply(j, value);
                    }
                }
            }
            userManager.SetSalesData(cycle, list.GetData());
            nowSalesData.SetData(list.GetData());
            nowSalesData.textSalesProductList.text = list.textSalesProductList.text;
            nowSalesData.textSalesValue.text = list.textSalesValue.text;
            if (cycle < 6)
            {
                for (int i = 0; i < list.GetData().product.Length; ++i)
                {
                    int value = (i == 0 ? 1 : 2) * GetActiveWorkshop();
                    for (int j = cycle + 1; j < 7; ++j)
                        productList[list.GetData().product[i]].AddSupply(j, value);
                }
            }
        }
    }
}