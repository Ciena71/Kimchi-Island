using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
/*
[Serializable]
public class SalesData
{
    public SalesData(int size)
    {
        if (size > 0)
        {
            product = new int[size];
            value = new int[size];
        }
    }

    public SalesData(SalesData copy)
    {
        for (int i = 0; i < copy.product.Length; ++i)
            product[i] = copy.product[i];
    }

    public int[] product;
    public int[] value;
    public int totalValue = 0;
    public int bonusGrooveValue = 0;

    public void Clear()
    {
        product = null;
        value = null;
        totalValue = default;
        bonusGrooveValue = default;
    }
}
*/
[Serializable]
public class SalesData
{
    public SalesData(int size)
    {
        if (size > 0)
        {
            product = new int[size];
            value = new int[size];
        }
        product4 = new int[0];
        value4 = new int[0];
    }

    public SalesData(int size3, int size4)
    {
        if (size3 > 0)
        {
            product = new int[size3];
            value = new int[size3];
        }
        if (size4 > 0)
        {
            product4 = new int[size4];
            value4 = new int[size4];
        }
    }
    
    public SalesData(SalesData copyA, SalesData copyB)
    {
        product = new int[copyA.product.Length];
        value = new int[copyA.product.Length];
        product4 = new int[copyB.product.Length];
        value4 = new int[copyB.product.Length];
        for (int i = 0; i < copyA.product.Length; ++i)
            product[i] = copyA.product[i];
        for (int i = 0; i < copyB.product.Length; ++i)
            product4[i] = copyB.product[i];
    }
    
    public SalesData(SalesData copy)
    {
        product = new int[copy.product.Length];
        value = new int[copy.product.Length];
        product4 = new int[copy.product4.Length];
        value4 = new int[copy.product4.Length];
        for (int i = 0; i < copy.product.Length; ++i)
        {
            product[i] = copy.product[i];
            value[i] = copy.value[i];
        }
        for (int i = 0; i < copy.product4.Length; ++i)
        {
            product4[i] = copy.product4[i];
            value4[i] = copy.value4[i];
        }
        totalValue = copy.totalValue;
        bonusGrooveTotalValue = copy.bonusGrooveTotalValue;
    }

    public int[] product;
    public int[] value;
    public int[] product4;
    public int[] value4;
    public int totalValue = 0;
    public int bonusGrooveTotalValue = 0;

    public void Clear()
    {
        product = null;
        value = null;
        product4 = null;
        value4 = null;
        totalValue = default;
        bonusGrooveTotalValue = default;
    }
}

public class SalesDataList
{
    public SalesData[] salesData = new SalesData[3];
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
    public Text textWorkshopRank;
    public InputField inputWorkshopRank;
    public Text textWorkshopActive;
    public InputField inputWorkshopActive;
    public Text textNetProfit;
    public Toggle toggleNetProfit;
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
    public RectTransform rectTopSalesList;
    public ScrollRect scrollTopSalesList;
    public Text textProductList;
    public Text textTotalValue;
    public Text textExpectedValue;
    public Text textCalculate;
    public Button btnCalculate;
    public SalesList nowSalesData;
    public Button btnNowSales;
    public GameObject objRangeCycle;
    public Text textRangeCycle;
    public Text textCPUThreadMax;
    public Text textCPUThreadMaxValue;
    public Text textCPUThread;
    public InputField inputCPUThread;
    public Text textGPU;
    public Toggle toggleGPU;
    public Text textMaxCount;
    public Text textMaxCountValue;
    public Text textLimitCount;
    public InputField inputLimitCount;
    public Button btnCrimeTime;
    public Text textCrimeTime;

    UserManager userManager;
    ResourceManager resourceManager;

    static List<Product> productList = new List<Product>();
    List<GameObject> salesList = new List<GameObject>();
    List<SalesData> allowedSalesList;
    int cycle;
    bool[] reverse = new bool[5];
    int sort = 0;
    int totalValue;

    string copySupplyPacket;
    string copyPopularityPacket;

    [SerializeField]
    ComputeShader gpuCalculate;
    ComputeBuffer gpuCalculateValue;
    ComputeBuffer gpuCalculateSupply;
    ComputeBuffer gpuCalculatePopularity;
    ComputeBuffer gpuCalculateItemA;
    ComputeBuffer gpuCalculateItemB;
    ComputeBuffer gpuCalculateItemC;
    ComputeBuffer gpuCalculateItemD;
    ComputeBuffer gpuCalculateItemE;
    ComputeBuffer gpuCalculateItemF;
    ComputeBuffer gpuCalculateResultA;
    ComputeBuffer gpuCalculateResultB;
    ComputeBuffer gpuCalculateResultC;
    ComputeBuffer gpuCalculateResultD;
    ComputeBuffer gpuCalculateResultE;
    ComputeBuffer gpuCalculateResultF;
    ComputeBuffer gpuCalculateResultTotal;

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            long now = (DateTime.UtcNow.AddDays(-1).AddHours(-8).Ticks - 637961184000000000) / 10000000;
            long season = now / 604800;
            /*long day = (now % 604800) / 86400 + 1;
            long second = (now % 604800) % 86400;
            Debug.Log($"{now} {season} {day} {second / 3600} : {(second % 3600) / 60} : {second % 60}");*/
            System.IO.DirectoryInfo path = new System.IO.DirectoryInfo($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/Kimchi Island Screenshot");
            if (!path.Exists)
                path.Create();
            if (cycle < 7)
                ScreenCapture.CaptureScreenshot($"{path.FullName}/S{season}-C{cycle + 1}-R{userManager.GetPlayerRank()}-{(userManager.GetNetProfit() == true ? "Net_Profit" : "Native")}.png");
            else if (cycle == 7)
                ScreenCapture.CaptureScreenshot($"{path.FullName}/S{season}-C5~7-R{userManager.GetPlayerRank()}-{(userManager.GetNetProfit() == true ? "Net_Profit" : "Native")}.png");
        }
    }
#endif

    void Awake()
    {
        instance = this;
        resourceManager = ResourceManager.instance;
        userManager = UserManager.instance;
        resourceManager.UpdateSupplyPattern();
        for (int i = 0; i < 7; ++i)
            totalValue += userManager.GetSalesData(i).totalValue;
        for (int i = 0; i < resourceManager.GetProductMax(); ++i)
        {
            Product product = Instantiate(Resources.Load<Product>("Prefab/Workshop/Product"),scrollProductList.content);
            product.SetDefaultData(i);
            productList.Add(product);
        }
        inputRank.text = userManager.GetPlayerRank().ToString();
        inputRank.onEndEdit.AddListener((data) =>
        {
            if (int.TryParse(data, out int value))
            {
                if (1 <= value && value <= 20)
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
                if (10 <= value && value <= 45)
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
        inputWorkshopRank.text = userManager.GetWorkshopRank().ToString();
        inputWorkshopRank.onEndEdit.AddListener((data) =>
        {
            if (int.TryParse(data, out int value))
            {
                if (1 <= value && value <= 5)
                    userManager.SetWorkshopRank(value);
                else
                    inputWorkshopRank.text = userManager.GetWorkshopRank().ToString();
            }
        });
        inputWorkshopActive.text = userManager.GetWorkshopActive().ToString();
        inputWorkshopActive.onEndEdit.AddListener((data) =>
        {
            if (int.TryParse(data, out int value))
            {
                if (1 <= value && value <= 4)
                    userManager.SetWorkshopActive(value);
                else
                    inputWorkshopActive.text = userManager.GetWorkshopActive().ToString();
            }
        });
        toggleNetProfit.isOn = userManager.GetNetProfit();
        toggleNetProfit.onValueChanged.AddListener((value) =>
        {
            userManager.SetNetProfit(value);
            if (cycle < 7)
            {
                CalculateAllData(false);
                if (userManager.GetSalesData(cycle).product != null && userManager.GetSalesData(cycle).product.Length > 0)
                {
                    string productListString = "";
                    string productValueString = "";
                    int valueTotal = 0;
                    if (userManager.GetWorkshopActive() < 4)
                    {
                        for (int i = 0; i < userManager.GetSalesData(cycle).product.Length; ++i)
                            valueTotal += userManager.GetSalesData(cycle).value[i];
                        productValueString = $"{valueTotal * userManager.GetWorkshopActive()} | {valueTotal} = ";
                        for (int i = 0; i < userManager.GetSalesData(cycle).product.Length; ++i)
                        {
                            if (i > 0)
                            {
                                productListString += " + ";
                                productValueString += " + ";
                            }
                            productListString += $"<sprite={userManager.GetSalesData(cycle).product[i]}> {resourceManager.GetProductName(userManager.GetSalesData(cycle).product[i])}";
                            productValueString += userManager.GetSalesData(cycle).value[i].ToString();
                            valueTotal += userManager.GetSalesData(cycle).value[i];
                        }
                        productValueString += $"\n{userManager.GetSalesData(cycle).totalValue} | {(userManager.GetSalesData(cycle).product.Length - 1) * userManager.GetWorkshopActive()} | {userManager.GetSalesData(cycle).bonusGrooveTotalValue} | {userManager.GetSalesData(cycle).totalValue + userManager.GetSalesData(cycle).bonusGrooveTotalValue}";
                    }
                    else
                    {
                        for (int i = 0; i < userManager.GetSalesData(cycle).product.Length; ++i)
                            valueTotal += userManager.GetSalesData(cycle).value[i];
                        productValueString = $"{valueTotal * 3} | {valueTotal} = ";
                        for (int i = 0; i < userManager.GetSalesData(cycle).product.Length; ++i)
                        {
                            if (i > 0)
                            {
                                productListString += " + ";
                                productValueString += " + ";
                            }
                            productListString += $"<sprite={userManager.GetSalesData(cycle).product[i]}> {resourceManager.GetProductName(userManager.GetSalesData(cycle).product[i])}";
                            productValueString += userManager.GetSalesData(cycle).value[i].ToString();
                            valueTotal += userManager.GetSalesData(cycle).value[i];
                        }
                        productListString += "\n";
                        productValueString += "\n";
                        valueTotal = 0;
                        for (int i = 0; i < userManager.GetSalesData(cycle).product4.Length; ++i)
                            valueTotal += userManager.GetSalesData(cycle).value4[i];
                        productValueString += $"{valueTotal} | {valueTotal} = ";
                        for (int i = 0; i < userManager.GetSalesData(cycle).product4.Length; ++i)
                        {
                            if (i > 0)
                            {
                                productListString += " + ";
                                productValueString += " + ";
                            }
                            productListString += $"<sprite={userManager.GetSalesData(cycle).product4[i]}> {resourceManager.GetProductName(userManager.GetSalesData(cycle).product4[i])}";
                            productValueString += userManager.GetSalesData(cycle).value4[i].ToString();
                            valueTotal += userManager.GetSalesData(cycle).value4[i];
                        }
                        productValueString += $"\n{userManager.GetSalesData(cycle).totalValue} | {(userManager.GetSalesData(cycle).product.Length - 1) * 3 + userManager.GetSalesData(cycle).product4.Length - 1} | {userManager.GetSalesData(cycle).bonusGrooveTotalValue} | {userManager.GetSalesData(cycle).totalValue + userManager.GetSalesData(cycle).bonusGrooveTotalValue}";
                    }
                    productListString += "\n ";
                    nowSalesData.textSalesProductList.text = productListString;
                    nowSalesData.textSalesValue.text = productValueString;
                }
            }
            else
                CalculateAllData(true);
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
            if (cycle < 7)
            {
                SalesData form = userManager.GetSalesData(cycle);
                nowSalesData.SetData(form);
                string productListString = "";
                string productValueString = "";
                if (form.product != null && form.product.Length > 0)
                {
                    if (userManager.GetWorkshopActive() > 3 && form.product4 != null && form.product4.Length > 0)
                    {
                        int dummyValue = 0;
                        for (int i = 0; i < form.value.Length; ++i)
                            dummyValue += form.value[i];
                        productValueString = $"{dummyValue * 3} | {dummyValue} = ";
                        for (int i = 0; i < form.product.Length; ++i)
                        {
                            if (i > 0)
                            {
                                productListString += " + ";
                                productValueString += " + ";
                            }
                            productListString += $"<sprite={form.product[i]}> {resourceManager.GetProductName(form.product[i])}";
                            productValueString += form.value[i].ToString();
                        }
                        productListString += "\n";
                        productValueString += "\n";
                        dummyValue = 0;
                        for (int i = 0; i < form.value4.Length; ++i)
                            dummyValue += form.value4[i];
                        productValueString += $"{dummyValue} | {dummyValue} = ";
                        for (int i = 0; i < form.product4.Length; ++i)
                        {
                            if (i > 0)
                            {
                                productListString += " + ";
                                productValueString += " + ";
                            }
                            productListString += $"<sprite={form.product4[i]}> {resourceManager.GetProductName(form.product4[i])}";
                            productValueString += form.value4[i].ToString();
                        }
                        productListString += "\n ";
                        productValueString += $"\n{form.totalValue} | {(form.product.Length - 1) * 3 + form.product4.Length - 1} | {form.bonusGrooveTotalValue} | {form.totalValue + form.bonusGrooveTotalValue}";
                    }
                    else
                    {
                        int dummyValue = 0;
                        for (int i = 0; i < form.product.Length; ++i)
                            dummyValue += form.value[i];
                        productValueString = $"{dummyValue * userManager.GetWorkshopActive()} | {dummyValue} = ";
                        for (int i = 0; i < form.product.Length; ++i)
                        {
                            if (i > 0)
                            {
                                productListString += " + ";
                                productValueString += " + ";
                            }
                            productListString += $"<sprite={form.product[i]}> {resourceManager.GetProductName(form.product[i])}";
                            productValueString += form.value[i].ToString();
                        }
                        productListString += "\n ";
                        productValueString += $"\n{form.totalValue} | {(form.product.Length - 1) * userManager.GetWorkshopActive()} | {form.bonusGrooveTotalValue} | {form.totalValue + form.bonusGrooveTotalValue}";
                    }
                }
                int groove = 0;
                if (cycle > 0)
                {
                    for (int i = 0; i < cycle; ++i)
                    {
                        int dummy = (userManager.GetSalesData(i).product != null ? userManager.GetSalesData(i).product.Length : 0);
                        groove += (userManager.GetWorkshopActive() > 3 ? 3 : userManager.GetWorkshopActive()) * (dummy >= 2 ? (dummy - 1) : 0);
                        if (userManager.GetWorkshopActive() > 3 && userManager.GetSalesData(i).product4 != null && userManager.GetSalesData(i).product4.Length >= 2)
                            groove += userManager.GetSalesData(i).product4.Length - 1;
                    }
                }
                if (groove > userManager.GetMaxGroove())
                    groove = userManager.GetMaxGroove();
                userManager.SetCurrentGroove(groove);
                inputGrooveNow.text = groove.ToString();
                nowSalesData.textSalesProductList.text = productListString;
                nowSalesData.textSalesValue.text = productValueString;
                nowSalesData.gameObject.SetActive(true);
            }
            else
            {
                nowSalesData.SetData(new SalesData(0));
                nowSalesData.textSalesProductList.text = "";
                nowSalesData.textSalesValue.text = "";
                nowSalesData.gameObject.SetActive(false);
            }
            if (cycle == 7)
            {
                int groove = 0;
                if (cycle > 0)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        int dummy = (userManager.GetSalesData(i).product != null ? userManager.GetSalesData(i).product.Length : 0);
                        groove += (userManager.GetWorkshopActive() > 3 ? 3 : userManager.GetWorkshopActive()) * (dummy >= 2 ? (dummy - 1) : 0);
                        if (userManager.GetWorkshopActive() > 3 && userManager.GetSalesData(i).product4 != null && userManager.GetSalesData(i).product4.Length >= 2)
                            groove += userManager.GetSalesData(i).product4.Length - 1;
                    }
                }
                if (groove > userManager.GetMaxGroove())
                    groove = userManager.GetMaxGroove();
                userManager.SetCurrentGroove(groove);
                inputGrooveNow.text = groove.ToString();
                scrollProductList.gameObject.SetActive(false);
                objRangeCycle.SetActive(true);
                rectTopSalesList.offsetMax = new Vector2(rectTopSalesList.offsetMax.x, -400);
            }
            else
            {
                if (oldCycle == 7)
                {
                    rectTopSalesList.offsetMax = new Vector2(rectTopSalesList.offsetMax.x, -700);
                    objRangeCycle.SetActive(false);
                    scrollProductList.gameObject.SetActive(true);
                }
                for (int i = 0; i < productList.Count; ++i)
                    productList[i].SetCycle(value);
            }
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
                productList.Sort((a, b) => a.GetSupplyValue(cycle < 7 ? cycle : cycle - 1).CompareTo(b.GetSupplyValue(cycle < 7 ? cycle : cycle - 1)));
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
                productList.Sort((a, b) => b.GetSupplyValue(cycle < 7 ? cycle : cycle - 1).CompareTo(a.GetSupplyValue(cycle < 7 ? cycle : cycle - 1)));
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
            productValueString = $"{userManager.GetSalesData(0).totalValue} | {userManager.GetSalesData(0).totalValue / userManager.GetWorkshopActive()} = ";
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
            if (cycle != 7)
            {
                if (userManager.GetWorkshopActive() <= 3)
                {
                    List<SalesData> SalesDataList = GetResultListOn3Workshop(cycle, new int[productList.Count], userManager.GetCurrentGroove(), false);
                    SalesDataList.Sort((x1, x2) => (x2.bonusGrooveTotalValue + x2.totalValue).CompareTo(x1.bonusGrooveTotalValue + x1.totalValue));
                    foreach (SalesData dummySalesData in SalesDataList)
                    {
                        if (salesList.Count < 100)
                        {
                            SalesList salesListObject = Instantiate(Resources.Load<SalesList>("Prefab/Workshop/SalesList"), scrollTopSalesList.content);
                            salesListObject.SetData(dummySalesData);
                            string productListString = "";
                            string productValueString = "";
                            int valueTotal = 0;
                            for (int i = 0; i < dummySalesData.product.Length; ++i)
                                valueTotal += dummySalesData.value[i];
                            productValueString = $"{valueTotal * userManager.GetWorkshopActive()} | {valueTotal} = ";
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
                            productListString += "\n ";
                            productValueString += $"\n{dummySalesData.totalValue} | {(dummySalesData.product.Length - 1) * userManager.GetWorkshopActive()} | {dummySalesData.bonusGrooveTotalValue} | {dummySalesData.totalValue + dummySalesData.bonusGrooveTotalValue}";
                            salesListObject.textSalesProductList.text = productListString;
                            salesListObject.textSalesValue.text = productValueString;
                            salesList.Add(salesListObject.gameObject);
                        }
                        else
                            break;
                    }
                    SalesDataList.Clear();
                    SalesDataList = null;
                }
                else
                {
                    List<SalesData> SalesDataList = GetResultListOn4Workshop(cycle, new int[productList.Count], userManager.GetCurrentGroove(), false);
                    SalesDataList.Sort((x1, x2) => (x2.bonusGrooveTotalValue + x2.totalValue).CompareTo(x1.bonusGrooveTotalValue + x1.totalValue));
                    foreach (SalesData dummySalesData in SalesDataList)
                    {
                        if (salesList.Count < 100)
                        {
                            SalesList salesListObject = Instantiate(Resources.Load<SalesList>("Prefab/Workshop/SalesList"), scrollTopSalesList.content);
                            salesListObject.rectSalesProductList.offsetMax = new Vector2(salesListObject.rectSalesProductList.offsetMax.x, 75);
                            salesListObject.SetData(dummySalesData);
                            string productListString = "";
                            int value = 0;
                            for (int i = 0; i < dummySalesData.value.Length; ++i)
                                value += dummySalesData.value[i];
                            string productValueString = $"{value * 3} | {value} = ";
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
                            productListString += "\n"; value = 0;
                            for (int i = 0; i < dummySalesData.value4.Length; ++i)
                                value += dummySalesData.value4[i];
                            productValueString += $"\n{value} | {value} = ";
                            for (int i = 0; i < dummySalesData.product4.Length; ++i)
                            {
                                if (i > 0)
                                {
                                    productListString += " + ";
                                    productValueString += " + ";
                                }
                                productListString += $"<sprite={dummySalesData.product4[i]}> {resourceManager.GetProductName(dummySalesData.product4[i])}";
                                productValueString += dummySalesData.value4[i].ToString();
                            }
                            productListString += "\n ";
                            productValueString += $"\n{dummySalesData.totalValue} | {(dummySalesData.product.Length - 1) * 3 + dummySalesData.product4.Length - 1} | {dummySalesData.bonusGrooveTotalValue} | {dummySalesData.totalValue + dummySalesData.bonusGrooveTotalValue}";
                            salesListObject.textSalesProductList.text = productListString;
                            salesListObject.textSalesValue.text = productValueString;
                            salesList.Add(salesListObject.gameObject);
                        }
                        else
                            break;
                    }
                    SalesDataList.Clear();
                    SalesDataList = null;
                    SystemCore.instance.PlayAlarm();
                }
            }
            else
            {
                if (userManager.GetWorkshopActive() <= 3)
                {
                    int count = 5;
                    for (int i = 0; i < 7; ++i)
                    {
                        if (userManager.GetSalesData(i).totalValue > 0)
                            --count;
                        if (i >= 5 && userManager.GetSalesData(i).totalValue > 0)
                        {
                            count = 0;
                            break;
                        }
                    }
                    if (count < 2) return;
                    SalesDataList highestResult = new SalesDataList();
                    for (int i = 0; i < 3; ++i)
                        highestResult.salesData[i] = new SalesData(0);
                    if (userManager.IsGPUCalculate())
                    {
                        List<SalesData> dataList = GetEnableList(false);
                        if (count >= 3)
                        {
                            EachResultList(4, new int[productList.Count], userManager.GetCurrentGroove(), false, (day5) =>
                            {
                                int[] stackA = new int[productList.Count];
                                for (int i = 0; i < day5.product.Length; ++i)
                                    stackA[day5.product[i]] += i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2;
                                EachResultList(5, stackA, userManager.GetCurrentGroove() + (day5.product.Length - 1) * userManager.GetWorkshopActive(), false, (day6) =>
                                {
                                    int[] stackB = new int[productList.Count];
                                    for (int i = 0; i < day5.product.Length; ++i)
                                        stackB[day5.product[i]] += i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2;
                                    for (int i = 0; i < day6.product.Length; ++i)
                                        stackB[day6.product[i]] += i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2;
                                    EachResultList(6, stackB, userManager.GetCurrentGroove() + (day5.product.Length + day6.product.Length - 2) * userManager.GetWorkshopActive(), false, (day7) =>
                                    {
                                        if (highestResult.totalValue < day5.totalValue + day6.totalValue + day7.totalValue)
                                        {
                                            for (int i = 0; i < day5.product.Length; ++i)
                                                highestResult.salesData[0].product[i] = day5.product[i];
                                            for (int i = 0; i < day5.product.Length; ++i)
                                                highestResult.salesData[0].value[i] = day5.value[i];
                                            highestResult.salesData[0].totalValue = day5.totalValue;
                                            for (int i = 0; i < day6.product.Length; ++i)
                                                highestResult.salesData[1].product[i] = day6.product[i];
                                            for (int i = 0; i < day6.product.Length; ++i)
                                                highestResult.salesData[1].value[i] = day6.value[i];
                                            highestResult.salesData[1].totalValue = day6.totalValue;
                                            for (int i = 0; i < day7.product.Length; ++i)
                                                highestResult.salesData[2].product[i] = day7.product[i];
                                            for (int i = 0; i < day7.product.Length; ++i)
                                                highestResult.salesData[2].value[i] = day7.value[i];
                                            highestResult.salesData[2].totalValue = day7.totalValue;
                                            highestResult.totalValue = day5.totalValue + day6.totalValue + day7.totalValue;
                                        }
                                    });
                                    stackB = null;
                                });
                                stackA = null;
                            });
                        }
                        else if (count == 2)
                        {
                            EachResultList(4, new int[productList.Count], userManager.GetCurrentGroove(), false, (day5) =>
                            {
                                int[] stack = new int[productList.Count];
                                for (int i = 0; i < day5.product.Length; ++i)
                                    stack[day5.product[i]] += (i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2);
                                SalesData day6 = GetHighestResultGPU(5, stack, userManager.GetCurrentGroove() + (day5.product.Length - 1) * userManager.GetWorkshopActive(), dataList);
                                if (highestResult.totalValue < day5.totalValue + day6.totalValue)
                                {
                                    for (int i = 0; i < day5.product.Length; ++i)
                                        highestResult.salesData[0].product[i] = day5.product[i];
                                    for (int i = 0; i < day5.product.Length; ++i)
                                        highestResult.salesData[0].value[i] = day5.value[i];
                                    highestResult.salesData[0].totalValue = day5.totalValue;
                                    for (int i = 0; i < day6.product.Length; ++i)
                                        highestResult.salesData[1].product[i] = day6.product[i];
                                    for (int i = 0; i < day6.product.Length; ++i)
                                        highestResult.salesData[1].value[i] = day6.value[i];
                                    highestResult.salesData[1].totalValue = day6.totalValue;
                                    highestResult.salesData[2].totalValue = 0;
                                    highestResult.totalValue = day5.totalValue + day6.totalValue;
                                }
                                day6.product = null;
                                day6.totalValue = default;
                                day6.value = null;
                                day6 = null;
                                SalesData day7 = GetHighestResultGPU(6, stack, userManager.GetCurrentGroove() + (day5.product.Length - 1) * userManager.GetWorkshopActive(), dataList);
                                if (highestResult.totalValue < day5.totalValue + day7.totalValue)
                                {
                                    for (int i = 0; i < day5.product.Length; ++i)
                                        highestResult.salesData[0].product[i] = day5.product[i];
                                    for (int i = 0; i < day5.product.Length; ++i)
                                        highestResult.salesData[0].value[i] = day5.value[i];
                                    highestResult.salesData[0].totalValue = day5.totalValue;
                                    highestResult.salesData[1].totalValue = 0;
                                    for (int i = 0; i < day7.product.Length; ++i)
                                        highestResult.salesData[2].product[i] = day7.product[i];
                                    for (int i = 0; i < day7.product.Length; ++i)
                                        highestResult.salesData[2].value[i] = day7.value[i];
                                    highestResult.salesData[2].totalValue = day7.totalValue;
                                    highestResult.totalValue = day5.totalValue + day7.totalValue;
                                }
                                day7.product = null;
                                day7.totalValue = default;
                                day7.value = null;
                                day7 = null;
                                stack = null;
                            });
                            EachResultList(5, new int[productList.Count], userManager.GetCurrentGroove(), false, (day6) =>
                            {
                                int[] stack = new int[productList.Count];
                                for (int i = 0; i < day6.product.Length; ++i)
                                    stack[day6.product[i]] += (i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2);
                                SalesData day7 = GetHighestResultGPU(6, stack, userManager.GetCurrentGroove() + (day6.product.Length - 1) * userManager.GetWorkshopActive(), dataList);
                                if (highestResult.totalValue < day6.totalValue + day7.totalValue)
                                {
                                    highestResult.salesData[0].totalValue = 0;
                                    for (int i = 0; i < day6.product.Length; ++i)
                                        highestResult.salesData[1].product[i] = day6.product[i];
                                    for (int i = 0; i < day6.product.Length; ++i)
                                        highestResult.salesData[1].value[i] = day6.value[i];
                                    highestResult.salesData[1].totalValue = day6.totalValue;
                                    for (int i = 0; i < day7.product.Length; ++i)
                                        highestResult.salesData[2].product[i] = day7.product[i];
                                    for (int i = 0; i < day7.product.Length; ++i)
                                        highestResult.salesData[2].value[i] = day7.value[i];
                                    highestResult.salesData[2].totalValue = day7.totalValue;
                                    highestResult.totalValue = day6.totalValue + day7.totalValue;
                                }
                                day7.product = null;
                                day7.totalValue = default;
                                day7.value = null;
                                day7 = null;
                                stack = null;
                            });
                        }
                    }
                    else
                    {
                        List<SalesData> salesDataList = GetResultListOn3Workshop(4, new int[productList.Count], userManager.GetCurrentGroove(), false);
                        if (userManager.GetLimitCount() > 0 && salesDataList.Count > userManager.GetLimitCount())
                        {
                            salesDataList.Sort((a, b) => b.totalValue.CompareTo(a.totalValue));
                            salesDataList.RemoveRange(userManager.GetLimitCount(), salesDataList.Count - userManager.GetLimitCount());
                        }
                        if (count >= 3)
                        {
                            Parallel.ForEach(salesDataList, new ParallelOptions { MaxDegreeOfParallelism = userManager.GetCPUThread() }, day5 =>
                            {
                                int[] stackA = new int[productList.Count];
                                for (int i = 0; i < day5.product.Length; ++i)
                                    stackA[day5.product[i]] += i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2;
                                List<SalesData> salesDataList6 = GetResultListOn3Workshop(5, stackA, userManager.GetCurrentGroove() + (day5.product.Length - 1) * userManager.GetWorkshopActive(), false);
                                if (userManager.GetLimitCount() > 0 && salesDataList6.Count > userManager.GetLimitCount())
                                {
                                    salesDataList6.Sort((a, b) => b.totalValue.CompareTo(a.totalValue));
                                    for (int x = userManager.GetLimitCount(); x < salesDataList6.Count - userManager.GetLimitCount(); ++x)
                                    {
                                        salesDataList6[x].Clear();
                                        salesDataList6[x] = null;
                                    }
                                    salesDataList6.RemoveRange(userManager.GetLimitCount(), salesDataList6.Count - userManager.GetLimitCount());
                                    salesDataList6.ForEach((day6) =>
                                    {
                                        int[] stackB = new int[productList.Count];
                                        for (int i = 0; i < day5.product.Length; ++i)
                                            stackB[day5.product[i]] += i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2;
                                        for (int i = 0; i < day6.product.Length; ++i)
                                            stackB[day6.product[i]] += i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2;
                                        EachResultList(6, stackB, userManager.GetCurrentGroove() + (day5.product.Length + day6.product.Length - 2) * userManager.GetWorkshopActive(), false, (day7) =>
                                        {
                                            if (highestResult.totalValue < day5.totalValue + day6.totalValue + day7.totalValue)
                                            {
                                                for (int i = 0; i < day5.product.Length; ++i)
                                                    highestResult.salesData[0].product[i] = day5.product[i];
                                                for (int i = 0; i < day5.product.Length; ++i)
                                                    highestResult.salesData[0].value[i] = day5.value[i];
                                                highestResult.salesData[0].totalValue = day5.totalValue;
                                                for (int i = 0; i < day6.product.Length; ++i)
                                                    highestResult.salesData[1].product[i] = day6.product[i];
                                                for (int i = 0; i < day6.product.Length; ++i)
                                                    highestResult.salesData[1].value[i] = day6.value[i];
                                                highestResult.salesData[1].totalValue = day6.totalValue;
                                                for (int i = 0; i < day7.product.Length; ++i)
                                                    highestResult.salesData[2].product[i] = day7.product[i];
                                                for (int i = 0; i < day7.product.Length; ++i)
                                                    highestResult.salesData[2].value[i] = day7.value[i];
                                                highestResult.salesData[2].totalValue = day7.totalValue;
                                                highestResult.totalValue = day5.totalValue + day6.totalValue + day7.totalValue;
                                            }
                                        });
                                        day6.Clear();
                                        day6 = null;
                                        stackB = null;
                                    });
                                    salesDataList6.Clear();
                                }
                                else
                                {
                                    salesDataList6.Clear();
                                    EachResultList(5, stackA, userManager.GetCurrentGroove() + (day5.product.Length - 1) * userManager.GetWorkshopActive(), false, (day6) =>
                                    {
                                        int[] stackB = new int[productList.Count];
                                        for (int i = 0; i < day5.product.Length; ++i)
                                            stackB[day5.product[i]] += i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2;
                                        for (int i = 0; i < day6.product.Length; ++i)
                                            stackB[day6.product[i]] += i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2;
                                        EachResultList(6, stackB, userManager.GetCurrentGroove() + (day5.product.Length + day6.product.Length - 2) * userManager.GetWorkshopActive(), false, (day7) =>
                                        {
                                            if (highestResult.totalValue < day5.totalValue + day6.totalValue + day7.totalValue)
                                            {
                                                for (int i = 0; i < day5.product.Length; ++i)
                                                    highestResult.salesData[0].product[i] = day5.product[i];
                                                for (int i = 0; i < day5.product.Length; ++i)
                                                    highestResult.salesData[0].value[i] = day5.value[i];
                                                highestResult.salesData[0].totalValue = day5.totalValue;
                                                for (int i = 0; i < day6.product.Length; ++i)
                                                    highestResult.salesData[1].product[i] = day6.product[i];
                                                for (int i = 0; i < day6.product.Length; ++i)
                                                    highestResult.salesData[1].value[i] = day6.value[i];
                                                highestResult.salesData[1].totalValue = day6.totalValue;
                                                for (int i = 0; i < day7.product.Length; ++i)
                                                    highestResult.salesData[2].product[i] = day7.product[i];
                                                for (int i = 0; i < day7.product.Length; ++i)
                                                    highestResult.salesData[2].value[i] = day7.value[i];
                                                highestResult.salesData[2].totalValue = day7.totalValue;
                                                highestResult.totalValue = day5.totalValue + day6.totalValue + day7.totalValue;
                                            }
                                        });
                                        stackB = null;
                                    });
                                }
                                stackA = null;
                            });
                        }
                        else if (count == 2)
                        {
                            Parallel.ForEach(salesDataList, new ParallelOptions { MaxDegreeOfParallelism = userManager.GetCPUThread() }, day5 =>
                            {
                                int[] stack = new int[productList.Count];
                                for (int i = 0; i < day5.product.Length; ++i)
                                    stack[day5.product[i]] += (i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2);
                                EachResultList(5, stack, userManager.GetCurrentGroove() + (day5.product.Length - 1) * userManager.GetWorkshopActive(), false, (day6) =>
                                {
                                    if (highestResult.totalValue < day5.totalValue + day6.totalValue)
                                    {
                                        for (int i = 0; i < day5.product.Length; ++i)
                                            highestResult.salesData[0].product[i] = day5.product[i];
                                        for (int i = 0; i < day5.product.Length; ++i)
                                            highestResult.salesData[0].value[i] = day5.value[i];
                                        highestResult.salesData[0].totalValue = day5.totalValue;
                                        for (int i = 0; i < day6.product.Length; ++i)
                                            highestResult.salesData[1].product[i] = day6.product[i];
                                        for (int i = 0; i < day6.product.Length; ++i)
                                            highestResult.salesData[1].value[i] = day6.value[i];
                                        highestResult.salesData[1].totalValue = day6.totalValue;
                                        highestResult.salesData[2].totalValue = 0;
                                        highestResult.totalValue = day5.totalValue + day6.totalValue;
                                    }
                                });
                                EachResultList(6, stack, userManager.GetCurrentGroove() + (day5.product.Length - 1) * userManager.GetWorkshopActive(), false, (day7) =>
                                {
                                    if (highestResult.totalValue < day5.totalValue + day7.totalValue)
                                    {
                                        for (int i = 0; i < day5.product.Length; ++i)
                                            highestResult.salesData[0].product[i] = day5.product[i];
                                        for (int i = 0; i < day5.product.Length; ++i)
                                            highestResult.salesData[0].value[i] = day5.value[i];
                                        highestResult.salesData[0].totalValue = day5.totalValue;
                                        highestResult.salesData[1].totalValue = 0;
                                        for (int i = 0; i < day7.product.Length; ++i)
                                            highestResult.salesData[2].product[i] = day7.product[i];
                                        for (int i = 0; i < day7.product.Length; ++i)
                                            highestResult.salesData[2].value[i] = day7.value[i];
                                        highestResult.salesData[2].totalValue = day7.totalValue;
                                        highestResult.totalValue = day5.totalValue + day7.totalValue;
                                    }
                                });
                                stack = null;
                            });
                            salesDataList = GetResultListOn3Workshop(5, new int[productList.Count], userManager.GetCurrentGroove(), false);
                            if (userManager.GetLimitCount() > 0 && salesDataList.Count > userManager.GetLimitCount())
                            {
                                salesDataList.Sort((a, b) => b.totalValue.CompareTo(a.totalValue));
                                salesDataList.RemoveRange(userManager.GetLimitCount(), salesDataList.Count - userManager.GetLimitCount());
                            }
                            Parallel.ForEach(salesDataList, new ParallelOptions { MaxDegreeOfParallelism = userManager.GetCPUThread() }, day6 =>
                            {
                                int[] stack = new int[productList.Count];
                                for (int i = 0; i < day6.product.Length; ++i)
                                    stack[day6.product[i]] += (i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2);
                                EachResultList(6, stack, userManager.GetCurrentGroove() + (day6.product.Length - 1) * userManager.GetWorkshopActive(), false, (day7) =>
                                {
                                    if (highestResult.totalValue < day6.totalValue + day7.totalValue)
                                    {
                                        highestResult.salesData[0].totalValue = 0;
                                        for (int i = 0; i < day6.product.Length; ++i)
                                            highestResult.salesData[1].product[i] = day6.product[i];
                                        for (int i = 0; i < day6.product.Length; ++i)
                                            highestResult.salesData[1].value[i] = day6.value[i];
                                        highestResult.salesData[1].totalValue = day6.totalValue;
                                        for (int i = 0; i < day7.product.Length; ++i)
                                            highestResult.salesData[2].product[i] = day7.product[i];
                                        for (int i = 0; i < day7.product.Length; ++i)
                                            highestResult.salesData[2].value[i] = day7.value[i];
                                        highestResult.salesData[2].totalValue = day7.totalValue;
                                        highestResult.totalValue = day6.totalValue + day7.totalValue;
                                    }
                                });
                                stack = null;
                            });
                        }
                    }
                    SalesList data = Instantiate(Resources.Load<SalesList>("Prefab/Workshop/SalesList"), scrollTopSalesList.content);
                    data.rectSalesProductList.offsetMax = new Vector2(data.rectSalesProductList.offsetMax.x, 50 * (highestResult.salesData.Length - 1));
                    string productListString = "";
                    string productValueString = "";
                    for (int j = 0; j < highestResult.salesData.Length; ++j)
                    {
                        if (highestResult.salesData[j].product.Length > 0)
                        {
                            productValueString += $"{highestResult.salesData[j].totalValue * userManager.GetWorkshopActive()} | {highestResult.salesData[j].totalValue} = ";
                            for (int i = 0; i < highestResult.salesData[j].product.Length; ++i)
                            {
                                if (i > 0)
                                {
                                    productListString += " + ";
                                    productValueString += " + ";
                                }
                                productListString += $"<sprite={highestResult.salesData[j].product[i]}> {resourceManager.GetProductName(highestResult.salesData[j].product[i])}";
                                productValueString += highestResult.salesData[j].value[i].ToString();
                            }
                        }
                        productListString += "\n";
                        productValueString += "\n";
                    }
                    productListString += " ";
                    productValueString += $"{highestResult.totalValue * userManager.GetWorkshopActive()} | {(highestResult.totalValue + totalValue) * userManager.GetWorkshopActive()}";
                    data.textSalesProductList.text = productListString;
                    data.textSalesValue.text = productValueString;
                    salesList.Add(data.gameObject);
                    SystemCore.instance.PlayAlarm();
                }
            }
        });
        btnNowSales.onClick.AddListener(() =>
        {
            if (userManager.GetSalesData(cycle).product != null)
            {
                for (int i = 0; i < userManager.GetSalesData(cycle).product.Length; ++i)
                {
                    int value = (i == 0 ? -1 : -2) * userManager.GetWorkshopActive();
                    for (int j = cycle + 1; j < 7; ++j)
                        productList[userManager.GetSalesData(cycle).product[i]].AddSupply(j, value);
                }
            }
            userManager.SetSalesData(cycle, new SalesData(0));
            nowSalesData.textSalesProductList.text = "";
            nowSalesData.textSalesValue.text = "";
            CalculateAllData(false);
        });
        textCPUThreadMaxValue.text = Environment.ProcessorCount.ToString();
        inputCPUThread.text = userManager.GetCPUThread().ToString();
        inputCPUThread.onEndEdit.AddListener((value) =>
        {
            if (int.TryParse(value, out int result))
            {
                if (result <= 0) result = 1;
                if (result > Environment.ProcessorCount) result = Environment.ProcessorCount;
                userManager.SetCPUThread(result);
            }
        });
        toggleGPU.isOn = userManager.IsGPUCalculate();
        toggleGPU.onValueChanged.AddListener((value) =>
        {
            userManager.SetGPUCalculate(value);
        });
        UpdateEnableList();
        if (userManager.GetLimitCount() > allowedSalesList.Count)
            userManager.SetLimitCount(allowedSalesList.Count);
        inputLimitCount.text = userManager.GetLimitCount().ToString();
        inputLimitCount.onEndEdit.AddListener((value) =>
        {
            if (int.TryParse(value, out int result))
            {
                if (result < 0) result = 0;
                if (result > allowedSalesList.Count) result = allowedSalesList.Count;
                userManager.SetLimitCount(result);
            }
        });/*
        btnCrimeTime.onClick.AddListener(() =>
        {
            for (int k = 0; k < 6; ++k)
            {
                if (userManager.GetSalesData(k).product != null)
                {
                    for (int i = 0; i < userManager.GetSalesData(k).product.Length; ++i)
                    {
                        int value = (i == 0 ? -1 : -2) * userManager.GetWorkshopActive();
                        for (int j = k + 1; j < 7; ++j)
                            productList[userManager.GetSalesData(k).product[i]].AddSupply(j, value);
                    }
                }
            }
            userManager.SetSalesData(0, new SalesData(0));
            userManager.SetSalesData(2, new SalesData(0));
            userManager.SetSalesData(4, new SalesData(0));
            userManager.SetSalesData(5, new SalesData(0));
            userManager.SetSalesData(6, new SalesData(0));
            SalesData day2 = new SalesData(6);
            day2.product[0] = 13;
            day2.product[1] = 9;
            day2.product[2] = 25;
            day2.product[3] = 3;
            day2.product[4] = 32;
            day2.product[5] = 35;
            for (int i = 0; i < 6; ++i)
            {
                day2.value[i] = GetProductValue(day2.product[i], i, 1, 0, 0);
                day2.totalValue += day2.value[i];
                int value = (i == 0 ? 1 : 2) * userManager.GetWorkshopActive();
                for (int j = 2; j < 7; ++j)
                    productList[day2.product[i]].AddSupply(j, value);
            }
            userManager.SetSalesData(1, day2);
            SalesData day4 = new SalesData(6);
            day4.product[0] = 13;
            day4.product[1] = 9;
            day4.product[2] = 25;
            day4.product[3] = 3;
            day4.product[4] = 32;
            day4.product[5] = 35;
            for (int i = 0; i < 6; ++i)
            {
                day4.value[i] = GetProductValue(day4.product[i], i, 3, 15, 0);
                day4.totalValue += day4.value[i];
                int value = (i == 0 ? 1 : 2) * userManager.GetWorkshopActive();
                for (int j = 4; j < 7; ++j)
                    productList[day4.product[i]].AddSupply(j, value);
            }
            userManager.SetSalesData(3, day4);
            CalculateAllData(true);
        });*/
        ApplyLanguage();
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
    /*
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
    }*/

    int GetPopularityAndSupplyValue(int pop, int sup)
    {
        switch (pop)
        {
        case 1:
            {
                switch (sup)
                {
                case 1: return 224;
                case 2: return 182;
                case 3: return 140;
                case 4: return 112;
                case 5: return 84;
                default: return 0;
                }
            }
        case 2:
            {
                switch (sup)
                {
                case 1: return 192;
                case 2: return 156;
                case 3: return 120;
                case 4: return 96;
                case 5: return 72;
                default: return 0;
                }
            }
        case 3:
            {
                switch (sup)
                {
                case 1: return 160;
                case 2: return 130;
                case 3: return 100;
                case 4: return 80;
                case 5: return 60;
                default: return 0;
                }
            }
        case 4:
            {
                switch (sup)
                {
                case 1: return 128;
                case 2: return 104;
                case 3: return 80;
                case 4: return 64;
                case 5: return 48;
                default: return 0;
                }
            }
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
        case 4: return 1.3f;
        case 5: return 1.4f;
        default: return 0;
        }
    }

    int GetWorkshopTierValueToInt(int tier)
    {
        switch (tier)
        {
        case 1: return 10;
        case 2: return 11;
        case 3: return 12;
        case 4: return 13;
        case 5: return 14;
        default: return 0;
        }
    }

    int GetProductValue4(int _index, int _step, int _cycle, int _groove, int _stack)
    {
        if (_groove > userManager.GetMaxGroove())
            _groove = userManager.GetMaxGroove();
        return (_step > 0 ? 2 : 1) *
            (GetPopularityAndSupplyValue(productList[_index].GetPopularity(_cycle), productList[_index].GetSupply(_cycle, _stack)) *
            (productList[_index].GetValue() *
            GetWorkshopTierValueToInt(userManager.GetWorkshopRank()) *
            (100 + _groove) / 1000) / 100) - (userManager.GetNetProfit() == true ? productList[_index].GetSalesValue() : 0);
    }

    int GetProductValue(int _index, int _step, int _cycle, int _groove, int _stack)
    {
        int nowGroove = (_groove + userManager.GetWorkshopActive() * _step);
        if (nowGroove > userManager.GetMaxGroove())
            nowGroove = userManager.GetMaxGroove();
        return (_step > 0 ? 2 : 1) *
            (GetPopularityAndSupplyValue(productList[_index].GetPopularity(_cycle), productList[_index].GetSupply(_cycle, _stack)) *
            (productList[_index].GetValue() *
            GetWorkshopTierValueToInt(userManager.GetWorkshopRank()) *
            (100 + nowGroove) / 1000) / 100) - (userManager.GetNetProfit() == true ? productList[_index].GetSalesValue() : 0);
    }

    void GetTime(byte[] data, DateTime time)
    {
        IList<IList<object>> form = null;
        int allow = 0;
        long season;
        long day = 0;
        if (userManager.GetContribute())
        {
            SpreadSheetData spreadSheetData = SpreadSheetData.instance;
            long now = (time.AddDays(-1).AddHours(-8).Ticks - 637961184000000000) / 10000000;
            season = now / 604800;
            day = (now % 604800) / 86400 + 1;
            spreadSheetData.SelectData("Z3", out form);
            if (int.Parse(form[0][0].ToString()) != season)
            {
                form[0][0] = season;
                spreadSheetData.InsertData("Z3", form);
                spreadSheetData.SelectData("R3:R83", out form);
                spreadSheetData.InsertData("S3:S83", form);
                spreadSheetData.SelectData("V3:W3", out form);
                form[0][0] = data[32];
                form[0][1] = data[33];
                spreadSheetData.InsertData("V3:W3", form);
                form = new IList<object>[81];
                for (int i = 0; i < form.Count; ++i)
                {
                    form[i] = new object[8];
                    for (int j = 0; j < form[i].Count; ++j)
                        form[i][j] = "";
                }
                spreadSheetData.InsertData("C3:J83", form);
            }
            switch (day)
            {
            case 1:
                {
                    spreadSheetData.SelectData("C3:D83", out form);
                    if (form == null)
                    {
                        form = new IList<object>[81];
                        for (int i = 0; i < form.Count; ++i)
                            form[i] = new object[2];
                    }
                    else if (form.Count < 81)
                    {
                        int size = form.Count;
                        for (int i = size; i < 81; ++i)
                            form.Add(new object[2]);
                    }
                    allow = 1;
                    break;
                }
            case 2:
                {
                    spreadSheetData.SelectData("E3:F83", out form);
                    if (form == null)
                    {
                        form = new IList<object>[81];
                        for (int i = 0; i < form.Count; ++i)
                            form[i] = new object[2];
                    }
                    else if (form.Count < 81)
                    {
                        int size = form.Count;
                        for (int i = size; i < 81; ++i)
                            form.Add(new object[2]);
                    }
                    allow = 1;
                    break;
                }
            case 3:
                {
                    spreadSheetData.SelectData("G3:H83", out form);
                    if (form == null)
                    {
                        form = new IList<object>[81];
                        for (int i = 0; i < form.Count; ++i)
                            form[i] = new object[2];
                    }
                    else if (form.Count < 81)
                    {
                        int size = form.Count;
                        for (int i = size; i < 81; ++i)
                            form.Add(new object[2]);
                    }
                    allow = 1;
                    break;
                }
            case 4:
                {
                    spreadSheetData.SelectData("I3:J83", out form);
                    if (form == null)
                    {
                        form = new IList<object>[81];
                        for (int i = 0; i < form.Count; ++i)
                            form[i] = new object[2];
                    }
                    else if (form.Count < 81)
                    {
                        int size = form.Count;
                        for (int i = size; i < 81; ++i)
                            form.Add(new object[2]);
                    }
                    allow = 1;
                    break;
                }
            }
        }
        copySupplyPacket = "";
        copyPopularityPacket = "";
        for (int i = 0; i < (data.Length == 112 ? 60 : (data.Length == 120 ? 72 : 81)); ++i)
        {
            if (i > 0)
                copySupplyPacket += "\n";
            int value = 0;
            if ((data[35 + i] & (1 << 6)) == (1 << 6))
                value = 4;
            else if ((data[35 + i] & (1 << 5) + (1 << 4)) == (1 << 5) + (1 << 4))
                value = 3;
            else if ((data[35 + i] & (1 << 5)) == (1 << 5))
                value = 2;
            else if ((data[35 + i] & (1 << 4)) == (1 << 4))
                value = 1;
            int demandShift = 3;
            if ((data[35 + i] & (1 << 2)) == (1 << 2))
                demandShift = -2;
            else if ((data[35 + i] & (1 << 1) + (1 << 0)) == (1 << 1) + (1 << 0))
                demandShift = -1;
            else if ((data[35 + i] & (1 << 1)) == (1 << 1))
                demandShift = 0;
            else if ((data[35 + i] & (1 << 0)) == (1 << 0))
                demandShift = 1;
            else
                demandShift = 2;
            copySupplyPacket += $"{value}\t{demandShift}";
            if (allow > 0)
            {
                if (form.Count > i)
                {
                    if (form[i][0] != null && int.TryParse(form[i][0].ToString(), out int result1))
                    {
                        if (value < result1)
                        {
                            form[i][0] = value.ToString();
                            allow = 2;
                        }
                    }
                    else
                    {
                        form[i][0] = value.ToString();
                        allow = 2;
                    }
                    if (form[i][1] != null && int.TryParse(form[i][1].ToString(), out int result2))
                    {
                        if (demandShift > result2)
                        {
                            form[i][1] = demandShift.ToString();
                            allow = 2;
                        }
                    }
                    else
                    {
                        form[i][1] = demandShift.ToString();
                        allow = 2;
                    }
                }
            }
        }
        if (allow == 2)
        {
            SpreadSheetData spreadSheetData = SpreadSheetData.instance;
            switch (day)
            {
            case 1:
                {
                    spreadSheetData.InsertData("C3:D83", form);
                    break;
                }
            case 2:
                {
                    spreadSheetData.InsertData("E3:F83", form);
                    break;
                }
            case 3:
                {
                    spreadSheetData.InsertData("G3:H83", form);
                    break;
                }
            case 4:
                {
                    spreadSheetData.InsertData("I3:J83", form);
                    break;
                }
            }
            resourceManager.UpdateSupplyPattern();
        }
        copyPopularityPacket = $"{data[32]}\t{data[33]}";
#if UNITY_EDITOR
        Debug.Log(copySupplyPacket + "\n");
        SupplyPacketDataCopy();
        Debug.Log(copyPopularityPacket + "\n");
#endif
    }

    public void SetPacketData(byte[] data)
    {
        resourceManager.GetRealDate(data, GetTime);
    }

    public void SupplyPacketDataCopy() => GUIUtility.systemCopyBuffer = copySupplyPacket;

    public void PopularityPacketDataCopy() => GUIUtility.systemCopyBuffer = copyPopularityPacket;

    public int GetCycle() => cycle;

    public void ApplyLanguage()
    {
        textRank.text = resourceManager.GetText(1);
        textGrooveNow.text = resourceManager.GetText(2);
        textGrooveMax.text = resourceManager.GetText(3);
        textWorkshopRank.text = resourceManager.GetText(4);
        textWorkshopActive.text = resourceManager.GetText(6);
        textNetProfit.text = resourceManager.GetText(5);
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
                dropCycle.captionText.text = resourceManager.GetText(35).Replace("{0}", (cycle + 1).ToString()) + (userManager.GetSalesData(cycle).totalValue > 0 ? $" [{userManager.GetSalesData(cycle).totalValue}]" : "");
                break;
            }
        case 7:
            {
                dropCycle.captionText.text = resourceManager.GetText(53);
                break;
            }
        case 8:
            {
                dropCycle.captionText.text = resourceManager.GetText(27);
                break;
            }
        case 9:
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
                    form.text = resourceManager.GetText(35).Replace("{0}", (num + 1).ToString()) + (userManager.GetSalesData(num).totalValue > 0 ? $" [{userManager.GetSalesData(num).totalValue}]" : "");
                    break;
                }
            case 7:
                {
                    form.text = resourceManager.GetText(53);
                    break;
                }
            case 8:
                {
                    form.text = resourceManager.GetText(27);
                    break;
                }
            case 9:
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
        textTotalValue.text = $"{resourceManager.GetText(50)} : {totalValue}";
        textExpectedValue.text = resourceManager.GetText(14);
        textCalculate.text = resourceManager.GetText(15);
        textRangeCycle.text = resourceManager.GetText(56);
        textCPUThreadMax.text = resourceManager.GetText(54);
        textCPUThread.text = resourceManager.GetText(55);
        textGPU.text = resourceManager.GetText(57);
        textMaxCount.text = resourceManager.GetText(61);
        textLimitCount.text = resourceManager.GetText(62);
        textCrimeTime.text = resourceManager.GetText(58);
        productList.ForEach((product) =>
        {
            product.ApplyLanguage(cycle);
        });
    }

    List<SalesData> GetResultListOn4Workshop(int _cycle, int[] _stack, int _groove, bool _hasPeak)
    {
        if (_cycle > 7)
            --_cycle;
        List<SalesData> TopSalesDataList = new List<SalesData>();
        ConcurrentBag<SalesData> SalesDataList = new ConcurrentBag<SalesData>();
        int groove = 0;
        foreach (SalesData form in allowedSalesList)
        {
            int[] stack = new int[_stack.Length];
            for (int i = 0; i < stack.Length; ++i)
                stack[i] = _stack[i];
            form.totalValue = 0;
            groove = 0;
            for (int i = 0; i < form.product.Length; ++i)
            {
                form.value[i] = GetProductValue4(form.product[i], i, _cycle, _groove + groove, stack[form.product[i]]);
                form.totalValue += form.value[i];
                stack[form.product[i]] += 3 * (i == 0 ? 1 : 2);
                groove += 3;
            }
            SalesData copyForm = new SalesData(form);
            TopSalesDataList.Add(copyForm);
        }
        TopSalesDataList.Sort((x1, x2) => x2.totalValue.CompareTo(x1.totalValue));
        TopSalesDataList.RemoveRange(100, TopSalesDataList.Count - 100);
        int grooveBonusValue = 4;
        if (_cycle < 7)
        {
            for (int i = 0; i < _cycle; ++i)
            {
                if (userManager.GetSalesData(i).product != null && userManager.GetSalesData(i).product.Length > 0)
                    --grooveBonusValue;
            }
        }
        if (grooveBonusValue < 0)
            grooveBonusValue = 0;
        Parallel.ForEach(TopSalesDataList, new ParallelOptions { MaxDegreeOfParallelism = userManager.GetCPUThread() }, formA =>
        {
            int[] stack = new int[_stack.Length];
            List<SalesData> dummyList = new List<SalesData>();
            foreach (SalesData formB in allowedSalesList)
            {
                for (int i = 0; i < stack.Length; ++i)
                    stack[i] = _stack[i];
                int timeA = 0;
                int timeB = 0;
                int turnA = 0;
                int turnB = 0;
                int nowTime = 0;
                int groovePlus = 0;
                SalesData form = new SalesData(formA, formB);
                while (turnA < formA.product.Length || turnB < formB.product.Length)
                {
                    if (nowTime == timeA)
                    {
                        form.value[turnA] = GetProductValue4(formA.product[turnA], turnA, _cycle, _groove + groovePlus, stack[formA.product[turnA]]);
                        form.totalValue += form.value[turnA] * 3;
                    }
                    if (nowTime == timeB)
                    {
                        form.value4[turnB] = GetProductValue4(formB.product[turnB], turnB, _cycle, _groove + groovePlus, stack[formB.product[turnB]]);
                        form.totalValue += form.value4[turnB];
                    }
                    if (nowTime == timeA)
                    {
                        stack[formA.product[turnA]] += 3 * (turnA == 0 ? 1 : 2);
                        timeA += resourceManager.GetProductTime(formA.product[turnA]);
                        groovePlus += 3;
                        ++turnA;
                    }
                    if (nowTime == timeB)
                    {
                        stack[formB.product[turnB]] += (turnB == 0 ? 1 : 2);
                        timeB += resourceManager.GetProductTime(formB.product[turnB]);
                        ++groovePlus;
                        ++turnB;
                    }
                    if (timeA < timeB)
                        nowTime = timeA;
                    else
                        nowTime = timeB;
                }
                int bonusGroove = (form.product.Length - 1) * 3 + (form.product4.Length - 1);
                if (bonusGroove + userManager.GetCurrentGroove() > userManager.GetMaxGroove())
                    bonusGroove -= bonusGroove + userManager.GetCurrentGroove() - userManager.GetMaxGroove();
                form.bonusGrooveTotalValue = Mathf.FloorToInt(bonusGroove * 11.7f * grooveBonusValue);
                dummyList.Add(form);
                if (dummyList.Count > 10)
                {
                    dummyList.Sort((x1, x2) => (x2.bonusGrooveTotalValue + x2.totalValue).CompareTo(x1.bonusGrooveTotalValue + x1.totalValue));
                    dummyList[10].Clear();
                    dummyList.RemoveAt(10);
                }
                timeA = default;
                timeB = default;
                turnA = default;
                turnB = default;
                nowTime = default;
                groovePlus = default;
            }
            dummyList.ForEach((form) =>
            {
                SalesDataList.Add(form);
            });
            dummyList.Clear();
            dummyList = null;
        });
        List<SalesData> SalesDataListFinal = new List<SalesData>();
        foreach (SalesData form in SalesDataList)
            SalesDataListFinal.Add(form);
        return SalesDataListFinal;
    }

    List<SalesData> GetResultListOn3Workshop(int _cycle, int[] _stack, int _groove, bool _hasPeak)
    {
        int[] stack = new int[_stack.Length];
        if (_cycle > 7)
            --_cycle;
        List<SalesData> SalesDataList = new List<SalesData>();
        SalesData salesdata;
        int checker;
        bool isPeak = false;
        bool isUsed = false;
        int grooveBonusValue = 4;
        if (_cycle < 7)
        {
            for (int i = 0; i < _cycle; ++i)
            {
                if (userManager.GetSalesData(i).product != null && userManager.GetSalesData(i).product.Length > 0)
                    --grooveBonusValue;
            }
        }
        if (grooveBonusValue < 0)
            grooveBonusValue = 0;
        foreach (SalesData form in allowedSalesList)
        {
            for (int i = 0; i < stack.Length; ++i)
                stack[i] = _stack[i];
            form.totalValue = 0;
            for (int i = 0; i < form.product.Length; ++i)
            {
                form.value[i] = GetProductValue(form.product[i], i, _cycle, _groove, stack[form.product[i]]);
                form.totalValue += form.value[i] * userManager.GetWorkshopActive();
                stack[form.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                if (productList[form.product[i]].GetSupplyValue(_cycle) < -8)
                    isPeak = true;
                if (productList[form.product[i]].GetSupplyValue(_cycle) >= 8)
                    isUsed = true;
            }
            int bonusGroove = (form.product.Length - 1) * userManager.GetWorkshopActive();
            if (bonusGroove + userManager.GetCurrentGroove() > userManager.GetMaxGroove())
                bonusGroove -= bonusGroove + userManager.GetCurrentGroove() - userManager.GetMaxGroove();
            form.bonusGrooveTotalValue = Mathf.FloorToInt(bonusGroove * 11.7f * grooveBonusValue);
            SalesData copyForm = new SalesData(form);
            SalesDataList.Add(copyForm);
        }/*
        if (userManager.GetCurrentGroove() < userManager.GetMaxGroove() && userManager.GetGroovePriority())
        {
            if (userManager.GetCurrentGroove() + (userManager.GetWorkshopActive() * 6) <= userManager.GetMaxGroove() + (userManager.GetWorkshopActive() - 1))
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
                                                                salesdata.productSize = 6;
                                                                salesdata.product[0] = a;
                                                                salesdata.product[1] = b;
                                                                salesdata.product[2] = c;
                                                                salesdata.product[3] = d;
                                                                salesdata.product[4] = e;
                                                                salesdata.product[5] = f;
                                                                for (int i = 0; i < stack.Length; ++i)
                                                                    stack[i] = _stack[i];
                                                                isPeak = false;
                                                                isUsed = false;
                                                                salesdata.totalValue = 0;
                                                                for (int i = 0; i < 6; ++i)
                                                                {
                                                                    salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                                    salesdata.totalValue += salesdata.value[i];
                                                                    stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                                    if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                                        isPeak = true;
                                                                    if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                                        isUsed = true;
                                                                }
                                                                if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
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
                        if (userManager.GetCurrentGroove() + userManager.GetWorkshopActive() < userManager.GetMaxGroove())
                        {
                            for (int b = 0; b < productList.Count; ++b)
                            {
                                if (a != b && productList[b].IsActive() && productList[b].GetTime() == 4 && ((productList[a].GetCategory() & productList[b].GetCategory()) > 0))
                                {
                                    if (userManager.GetCurrentGroove() + userManager.GetWorkshopActive() * 2 < userManager.GetMaxGroove())
                                    {
                                        for (int c = 0; c < productList.Count; ++c)
                                        {
                                            if (b != c && productList[c].IsActive() && productList[c].GetTime() == 4 && ((productList[b].GetCategory() & productList[c].GetCategory()) > 0))
                                            {
                                                if (userManager.GetCurrentGroove() + userManager.GetWorkshopActive() * 3 < userManager.GetMaxGroove())
                                                {
                                                    for (int d = 0; d < productList.Count; ++d)
                                                    {
                                                        if (c != d && productList[d].IsActive() && productList[d].GetTime() == 4 && ((productList[c].GetCategory() & productList[d].GetCategory()) > 0))
                                                        {
                                                            if (userManager.GetCurrentGroove() + userManager.GetWorkshopActive() * 4 < userManager.GetMaxGroove())
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
                                                                                salesdata.productSize = 6;
                                                                                salesdata.product[0] = a;
                                                                                salesdata.product[1] = b;
                                                                                salesdata.product[2] = c;
                                                                                salesdata.product[3] = d;
                                                                                salesdata.product[4] = e;
                                                                                salesdata.product[5] = f;
                                                                                for (int i = 0; i < stack.Length; ++i)
                                                                                    stack[i] = _stack[i];
                                                                                isPeak = false;
                                                                                isUsed = false;
                                                                                salesdata.totalValue = 0;
                                                                                for (int i = 0; i < 6; ++i)
                                                                                {
                                                                                    salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                                                    salesdata.totalValue += salesdata.value[i];
                                                                                    stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                                                    if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                                                        isPeak = true;
                                                                                    if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                                                        isUsed = true;
                                                                                }
                                                                                if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
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
                                                                                            salesdata.productSize = 6;
                                                                                            salesdata.product[0] = a;
                                                                                            salesdata.product[1] = b;
                                                                                            salesdata.product[2] = c;
                                                                                            salesdata.product[3] = d;
                                                                                            salesdata.product[4] = e;
                                                                                            salesdata.product[5] = f;
                                                                                            for (int i = 0; i < stack.Length; ++i)
                                                                                                stack[i] = _stack[i];
                                                                                            isPeak = false;
                                                                                            isUsed = false;
                                                                                            salesdata.totalValue = 0;
                                                                                            for (int i = 0; i < 6; ++i)
                                                                                            {
                                                                                                salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                                                                salesdata.totalValue += salesdata.value[i];
                                                                                                stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                                                                if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                                                                    isPeak = true;
                                                                                                if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                                                                    isUsed = true;
                                                                                            }
                                                                                            if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
                                                                                                SalesDataList.Add(salesdata);
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                salesdata = new SalesData();
                                                                                salesdata.productSize = 5;
                                                                                salesdata.product[0] = a;
                                                                                salesdata.product[1] = b;
                                                                                salesdata.product[2] = c;
                                                                                salesdata.product[3] = d;
                                                                                salesdata.product[4] = e;
                                                                                for (int i = 0; i < stack.Length; ++i)
                                                                                    stack[i] = _stack[i];
                                                                                isPeak = false;
                                                                                isUsed = false;
                                                                                salesdata.totalValue = 0;
                                                                                for (int i = 0; i < 5; ++i)
                                                                                {
                                                                                    salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                                                    salesdata.totalValue += salesdata.value[i];
                                                                                    stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                                                    if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                                                        isPeak = true;
                                                                                    if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                                                        isUsed = true;
                                                                                }
                                                                                if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
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
                                                                                                salesdata.productSize = 6;
                                                                                                salesdata.product[0] = a;
                                                                                                salesdata.product[1] = b;
                                                                                                salesdata.product[2] = c;
                                                                                                salesdata.product[3] = d;
                                                                                                salesdata.product[4] = e;
                                                                                                salesdata.product[5] = f;
                                                                                                for (int i = 0; i < stack.Length; ++i)
                                                                                                    stack[i] = _stack[i];
                                                                                                isPeak = false;
                                                                                                isUsed = false;
                                                                                                salesdata.totalValue = 0;
                                                                                                for (int i = 0; i < 6; ++i)
                                                                                                {
                                                                                                    salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                                                                    salesdata.totalValue += salesdata.value[i];
                                                                                                    stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                                                                    if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                                                                        isPeak = true;
                                                                                                    if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                                                                        isUsed = true;
                                                                                                }
                                                                                                if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
                                                                                                    SalesDataList.Add(salesdata);
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    salesdata = new SalesData();
                                                                                    salesdata.productSize = 5;
                                                                                    salesdata.product[0] = a;
                                                                                    salesdata.product[1] = b;
                                                                                    salesdata.product[2] = c;
                                                                                    salesdata.product[3] = d;
                                                                                    salesdata.product[4] = e;
                                                                                    for (int i = 0; i < stack.Length; ++i)
                                                                                        stack[i] = _stack[i];
                                                                                    isPeak = false;
                                                                                    isUsed = false;
                                                                                    salesdata.totalValue = 0;
                                                                                    for (int i = 0; i < 5; ++i)
                                                                                    {
                                                                                        salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                                                        salesdata.totalValue += salesdata.value[i];
                                                                                        stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                                                        if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                                                            isPeak = true;
                                                                                        if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                                                            isUsed = true;
                                                                                    }
                                                                                    if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
                                                                                        SalesDataList.Add(salesdata);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    salesdata = new SalesData();
                                                                    salesdata.productSize = 4;
                                                                    salesdata.product[0] = a;
                                                                    salesdata.product[1] = b;
                                                                    salesdata.product[2] = c;
                                                                    salesdata.product[3] = d;
                                                                    for (int i = 0; i < stack.Length; ++i)
                                                                        stack[i] = _stack[i];
                                                                    isPeak = false;
                                                                    isUsed = false;
                                                                    salesdata.totalValue = 0;
                                                                    for (int i = 0; i < 4; ++i)
                                                                    {
                                                                        salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                                        salesdata.totalValue += salesdata.value[i];
                                                                        stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                                        if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                                            isPeak = true;
                                                                        if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                                            isUsed = true;
                                                                    }
                                                                    if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
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
                                                                                                    salesdata.productSize = 6;
                                                                                                    salesdata.product[0] = a;
                                                                                                    salesdata.product[1] = b;
                                                                                                    salesdata.product[2] = c;
                                                                                                    salesdata.product[3] = d;
                                                                                                    salesdata.product[4] = e;
                                                                                                    salesdata.product[5] = f;
                                                                                                    for (int i = 0; i < stack.Length; ++i)
                                                                                                        stack[i] = _stack[i];
                                                                                                    isPeak = false;
                                                                                                    isUsed = false;
                                                                                                    salesdata.totalValue = 0;
                                                                                                    for (int i = 0; i < 6; ++i)
                                                                                                    {
                                                                                                        salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                                                                        salesdata.totalValue += salesdata.value[i];
                                                                                                        stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                                                                        if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                                                                            isPeak = true;
                                                                                                        if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                                                                            isUsed = true;
                                                                                                    }
                                                                                                    if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
                                                                                                        SalesDataList.Add(salesdata);
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        salesdata = new SalesData();
                                                                                        salesdata.productSize = 5;
                                                                                        salesdata.product[0] = a;
                                                                                        salesdata.product[1] = b;
                                                                                        salesdata.product[2] = c;
                                                                                        salesdata.product[3] = d;
                                                                                        salesdata.product[4] = e;
                                                                                        for (int i = 0; i < stack.Length; ++i)
                                                                                            stack[i] = _stack[i];
                                                                                        isPeak = false;
                                                                                        isUsed = false;
                                                                                        salesdata.totalValue = 0;
                                                                                        for (int i = 0; i < 5; ++i)
                                                                                        {
                                                                                            salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                                                            salesdata.totalValue += salesdata.value[i];
                                                                                            stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                                                            if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                                                                isPeak = true;
                                                                                            if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                                                                isUsed = true;
                                                                                        }
                                                                                        if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
                                                                                            SalesDataList.Add(salesdata);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        salesdata = new SalesData();
                                                                        salesdata.productSize = 4;
                                                                        salesdata.product[0] = a;
                                                                        salesdata.product[1] = b;
                                                                        salesdata.product[2] = c;
                                                                        salesdata.product[3] = d;
                                                                        for (int i = 0; i < stack.Length; ++i)
                                                                            stack[i] = _stack[i];
                                                                        isPeak = false;
                                                                        isUsed = false;
                                                                        salesdata.totalValue = 0;
                                                                        for (int i = 0; i < 4; ++i)
                                                                        {
                                                                            salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                                            salesdata.totalValue += salesdata.value[i];
                                                                            stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                                            if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                                                isPeak = true;
                                                                            if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                                                isUsed = true;
                                                                        }
                                                                        if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
                                                                            SalesDataList.Add(salesdata);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        salesdata = new SalesData();
                                                        salesdata.productSize = 3;
                                                        salesdata.product[0] = a;
                                                        salesdata.product[1] = b;
                                                        salesdata.product[2] = c;
                                                        for (int i = 0; i < stack.Length; ++i)
                                                            stack[i] = _stack[i];
                                                        isPeak = false;
                                                        isUsed = false;
                                                        salesdata.totalValue = 0;
                                                        for (int i = 0; i < 3; ++i)
                                                        {
                                                            salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                            salesdata.totalValue += salesdata.value[i];
                                                            stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                            if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                                isPeak = true;
                                                            if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                                isUsed = true;
                                                        }
                                                        if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
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
                                                                                                salesdata.productSize = 6;
                                                                                                salesdata.product[0] = a;
                                                                                                salesdata.product[1] = b;
                                                                                                salesdata.product[2] = c;
                                                                                                salesdata.product[3] = d;
                                                                                                salesdata.product[4] = e;
                                                                                                salesdata.product[5] = f;
                                                                                                for (int i = 0; i < stack.Length; ++i)
                                                                                                    stack[i] = _stack[i];
                                                                                                isPeak = false;
                                                                                                isUsed = false;
                                                                                                salesdata.totalValue = 0;
                                                                                                for (int i = 0; i < 6; ++i)
                                                                                                {
                                                                                                    salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                                                                    salesdata.totalValue += salesdata.value[i];
                                                                                                    stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                                                                    if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                                                                        isPeak = true;
                                                                                                    if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                                                                        isUsed = true;
                                                                                                }
                                                                                                if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
                                                                                                    SalesDataList.Add(salesdata);
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    salesdata = new SalesData();
                                                                                    salesdata.productSize = 5;
                                                                                    salesdata.product[0] = a;
                                                                                    salesdata.product[1] = b;
                                                                                    salesdata.product[2] = c;
                                                                                    salesdata.product[3] = d;
                                                                                    salesdata.product[4] = e;
                                                                                    for (int i = 0; i < stack.Length; ++i)
                                                                                        stack[i] = _stack[i];
                                                                                    isPeak = false;
                                                                                    isUsed = false;
                                                                                    salesdata.totalValue = 0;
                                                                                    for (int i = 0; i < 5; ++i)
                                                                                    {
                                                                                        salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                                                        salesdata.totalValue += salesdata.value[i];
                                                                                        stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                                                        if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                                                            isPeak = true;
                                                                                        if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                                                            isUsed = true;
                                                                                    }
                                                                                    if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
                                                                                        SalesDataList.Add(salesdata);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    salesdata = new SalesData();
                                                                    salesdata.productSize = 4;
                                                                    salesdata.product[0] = a;
                                                                    salesdata.product[1] = b;
                                                                    salesdata.product[2] = c;
                                                                    salesdata.product[3] = d;
                                                                    for (int i = 0; i < stack.Length; ++i)
                                                                        stack[i] = _stack[i];
                                                                    isPeak = false;
                                                                    isUsed = false;
                                                                    salesdata.totalValue = 0;
                                                                    for (int i = 0; i < 4; ++i)
                                                                    {
                                                                        salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                                        salesdata.totalValue += salesdata.value[i];
                                                                        stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                                        if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                                            isPeak = true;
                                                                        if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                                            isUsed = true;
                                                                    }
                                                                    if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
                                                                        SalesDataList.Add(salesdata);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    salesdata = new SalesData();
                                                    salesdata.productSize = 3;
                                                    salesdata.product[0] = a;
                                                    salesdata.product[1] = b;
                                                    salesdata.product[2] = c;
                                                    for (int i = 0; i < stack.Length; ++i)
                                                        stack[i] = _stack[i];
                                                    isPeak = false;
                                                    isUsed = false;
                                                    salesdata.totalValue = 0;
                                                    for (int i = 0; i < 3; ++i)
                                                    {
                                                        salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                        salesdata.totalValue += salesdata.value[i];
                                                        stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                        if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                            isPeak = true;
                                                        if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                            isUsed = true;
                                                    }
                                                    if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
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
                                                                                        salesdata.productSize = 6;
                                                                                        salesdata.product[0] = a;
                                                                                        salesdata.product[1] = b;
                                                                                        salesdata.product[2] = c;
                                                                                        salesdata.product[3] = d;
                                                                                        salesdata.product[4] = e;
                                                                                        salesdata.product[5] = f;
                                                                                        for (int i = 0; i < stack.Length; ++i)
                                                                                            stack[i] = _stack[i];
                                                                                        isPeak = false;
                                                                                        isUsed = false;
                                                                                        salesdata.totalValue = 0;
                                                                                        for (int i = 0; i < 6; ++i)
                                                                                        {
                                                                                            salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                                                            salesdata.totalValue += salesdata.value[i];
                                                                                            stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                                                            if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                                                                isPeak = true;
                                                                                            if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                                                                isUsed = true;
                                                                                        }
                                                                                        if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
                                                                                            SalesDataList.Add(salesdata);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            salesdata = new SalesData();
                                                                            salesdata.productSize = 5;
                                                                            salesdata.product[0] = a;
                                                                            salesdata.product[1] = b;
                                                                            salesdata.product[2] = c;
                                                                            salesdata.product[3] = d;
                                                                            salesdata.product[4] = e;
                                                                            for (int i = 0; i < stack.Length; ++i)
                                                                                stack[i] = _stack[i];
                                                                            isPeak = false;
                                                                            isUsed = false;
                                                                            salesdata.totalValue = 0;
                                                                            for (int i = 0; i < 5; ++i)
                                                                            {
                                                                                salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                                                salesdata.totalValue += salesdata.value[i];
                                                                                stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                                                if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                                                    isPeak = true;
                                                                                if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                                                    isUsed = true;
                                                                            }
                                                                            if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
                                                                                SalesDataList.Add(salesdata);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            salesdata = new SalesData();
                                                            salesdata.productSize = 4;
                                                            salesdata.product[0] = a;
                                                            salesdata.product[1] = b;
                                                            salesdata.product[2] = c;
                                                            salesdata.product[3] = d;
                                                            for (int i = 0; i < stack.Length; ++i)
                                                                stack[i] = _stack[i];
                                                            isPeak = false;
                                                            isUsed = false;
                                                            salesdata.totalValue = 0;
                                                            for (int i = 0; i < 4; ++i)
                                                            {
                                                                salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                                salesdata.totalValue += salesdata.value[i];
                                                                stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                                if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                                    isPeak = true;
                                                                if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                                    isUsed = true;
                                                            }
                                                            if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
                                                                SalesDataList.Add(salesdata);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            salesdata = new SalesData();
                                            salesdata.productSize = 3;
                                            salesdata.product[0] = a;
                                            salesdata.product[1] = b;
                                            salesdata.product[2] = c;
                                            for (int i = 0; i < stack.Length; ++i)
                                                stack[i] = _stack[i];
                                            isPeak = false;
                                            isUsed = false;
                                            salesdata.totalValue = 0;
                                            for (int i = 0; i < 3; ++i)
                                            {
                                                salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                salesdata.totalValue += salesdata.value[i];
                                                stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                    isPeak = true;
                                                if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                    isUsed = true;
                                            }
                                            if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
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
        */
        return SalesDataList;
    }

    void EachResultList(int _cycle, int[] _stack, int _groove, bool _hasPeak, Action<SalesData> result)
    {
        int[] stack = new int[_stack.Length];
        if (_cycle > 7)
            --_cycle;
        SalesData salesdata = new SalesData(0);
        int checker;
        bool isPeak = false;
        bool isUsed = false;
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
                                                                                    Array.Resize(ref salesdata.product, 6);
                                                                                    salesdata.product[0] = a;
                                                                                    salesdata.product[1] = b;
                                                                                    salesdata.product[2] = c;
                                                                                    salesdata.product[3] = d;
                                                                                    salesdata.product[4] = e;
                                                                                    salesdata.product[5] = f;
                                                                                    for (int i = 0; i < stack.Length; ++i)
                                                                                        stack[i] = _stack[i];
                                                                                    isPeak = false;
                                                                                    isUsed = false;
                                                                                    salesdata.totalValue = 0;
                                                                                    for (int i = 0; i < 6; ++i)
                                                                                    {
                                                                                        salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                                                        salesdata.totalValue += salesdata.value[i];
                                                                                        stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                                                        if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                                                            isPeak = true;
                                                                                        if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                                                            isUsed = true;
                                                                                    }
                                                                                    if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
                                                                                        result.Invoke(salesdata);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        Array.Resize(ref salesdata.product, 5);
                                                                        salesdata.product[0] = a;
                                                                        salesdata.product[1] = b;
                                                                        salesdata.product[2] = c;
                                                                        salesdata.product[3] = d;
                                                                        salesdata.product[4] = e;
                                                                        for (int i = 0; i < stack.Length; ++i)
                                                                            stack[i] = _stack[i];
                                                                        isPeak = false;
                                                                        isUsed = false;
                                                                        salesdata.totalValue = 0;
                                                                        for (int i = 0; i < 5; ++i)
                                                                        {
                                                                            salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                                            salesdata.totalValue += salesdata.value[i];
                                                                            stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                                            if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                                                isPeak = true;
                                                                            if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                                                isUsed = true;
                                                                        }
                                                                        if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
                                                                            result.Invoke(salesdata);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Array.Resize(ref salesdata.product, 4);
                                                        salesdata.product[0] = a;
                                                        salesdata.product[1] = b;
                                                        salesdata.product[2] = c;
                                                        salesdata.product[3] = d;
                                                        for (int i = 0; i < stack.Length; ++i)
                                                            stack[i] = _stack[i];
                                                        isPeak = false;
                                                        isUsed = false;
                                                        salesdata.totalValue = 0;
                                                        for (int i = 0; i < 4; ++i)
                                                        {
                                                            salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                                            salesdata.totalValue += salesdata.value[i];
                                                            stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                                            if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                                isPeak = true;
                                                            if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                                isUsed = true;
                                                        }
                                                        if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
                                                            result.Invoke(salesdata);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Array.Resize(ref salesdata.product, 3);
                                        salesdata.product[0] = a;
                                        salesdata.product[1] = b;
                                        salesdata.product[2] = c;
                                        for (int i = 0; i < stack.Length; ++i)
                                            stack[i] = _stack[i];
                                        isPeak = false;
                                        isUsed = false;
                                        salesdata.totalValue = 0;
                                        for (int i = 0; i < 3; ++i)
                                        {
                                            salesdata.value[i] = GetProductValue(salesdata.product[i], i, _cycle, _groove, stack[salesdata.product[i]]);
                                            salesdata.totalValue += salesdata.value[i];
                                            stack[salesdata.product[i]] += userManager.GetWorkshopActive() * (i == 0 ? 1 : 2);
                                            if (productList[salesdata.product[i]].GetSupplyValue(_cycle) < -8)
                                                isPeak = true;
                                            if (productList[salesdata.product[i]].GetSupplyValue(_cycle) >= 8)
                                                isUsed = true;
                                        }
                                        if (!_hasPeak || (_hasPeak && isPeak && !isUsed))
                                            result.Invoke(salesdata);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    List<SalesData> GetEnableList(bool _groovePriority)
    {
        List<SalesData> SalesDataList = new List<SalesData>();
        SalesData salesdata;
        int checker;
        if (userManager.GetCurrentGroove() < userManager.GetMaxGroove() && _groovePriority)
        {
            if (userManager.GetCurrentGroove() + (userManager.GetWorkshopActive() * 6) <= userManager.GetMaxGroove() + (userManager.GetWorkshopActive() - 1))
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
                                                                salesdata = new SalesData(6);
                                                                salesdata.product[0] = a;
                                                                salesdata.product[1] = b;
                                                                salesdata.product[2] = c;
                                                                salesdata.product[3] = d;
                                                                salesdata.product[4] = e;
                                                                salesdata.product[5] = f;
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
                        if (userManager.GetCurrentGroove() + userManager.GetWorkshopActive() < userManager.GetMaxGroove())
                        {
                            for (int b = 0; b < productList.Count; ++b)
                            {
                                if (a != b && productList[b].IsActive() && productList[b].GetTime() == 4 && ((productList[a].GetCategory() & productList[b].GetCategory()) > 0))
                                {
                                    if (userManager.GetCurrentGroove() + userManager.GetWorkshopActive() * 2 < userManager.GetMaxGroove())
                                    {
                                        for (int c = 0; c < productList.Count; ++c)
                                        {
                                            if (b != c && productList[c].IsActive() && productList[c].GetTime() == 4 && ((productList[b].GetCategory() & productList[c].GetCategory()) > 0))
                                            {
                                                if (userManager.GetCurrentGroove() + userManager.GetWorkshopActive() * 3 < userManager.GetMaxGroove())
                                                {
                                                    for (int d = 0; d < productList.Count; ++d)
                                                    {
                                                        if (c != d && productList[d].IsActive() && productList[d].GetTime() == 4 && ((productList[c].GetCategory() & productList[d].GetCategory()) > 0))
                                                        {
                                                            if (userManager.GetCurrentGroove() + userManager.GetWorkshopActive() * 4 < userManager.GetMaxGroove())
                                                            {
                                                                for (int e = 0; e < productList.Count; ++e)
                                                                {
                                                                    if (d != e && productList[e].IsActive() && productList[e].GetTime() == 4 && ((productList[d].GetCategory() & productList[e].GetCategory()) > 0))
                                                                    {
                                                                        for (int f = 0; f < productList.Count; ++f)
                                                                        {
                                                                            if (e != f && productList[f].IsActive() && productList[f].GetTime() == 4 && ((productList[e].GetCategory() & productList[f].GetCategory()) > 0))
                                                                            {
                                                                                salesdata = new SalesData(6);
                                                                                salesdata.product[0] = a;
                                                                                salesdata.product[1] = b;
                                                                                salesdata.product[2] = c;
                                                                                salesdata.product[3] = d;
                                                                                salesdata.product[4] = e;
                                                                                salesdata.product[5] = f;
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
                                                                                            salesdata = new SalesData(6);
                                                                                            salesdata.product[0] = a;
                                                                                            salesdata.product[1] = b;
                                                                                            salesdata.product[2] = c;
                                                                                            salesdata.product[3] = d;
                                                                                            salesdata.product[4] = e;
                                                                                            salesdata.product[5] = f;
                                                                                            SalesDataList.Add(salesdata);
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                salesdata = new SalesData(5);
                                                                                salesdata.product[0] = a;
                                                                                salesdata.product[1] = b;
                                                                                salesdata.product[2] = c;
                                                                                salesdata.product[3] = d;
                                                                                salesdata.product[4] = e;
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
                                                                                                salesdata = new SalesData(6);
                                                                                                salesdata.product[0] = a;
                                                                                                salesdata.product[1] = b;
                                                                                                salesdata.product[2] = c;
                                                                                                salesdata.product[3] = d;
                                                                                                salesdata.product[4] = e;
                                                                                                salesdata.product[5] = f;
                                                                                                SalesDataList.Add(salesdata);
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    salesdata = new SalesData(5);
                                                                                    salesdata.product[0] = a;
                                                                                    salesdata.product[1] = b;
                                                                                    salesdata.product[2] = c;
                                                                                    salesdata.product[3] = d;
                                                                                    salesdata.product[4] = e;
                                                                                    SalesDataList.Add(salesdata);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    salesdata = new SalesData(4);
                                                                    salesdata.product[0] = a;
                                                                    salesdata.product[1] = b;
                                                                    salesdata.product[2] = c;
                                                                    salesdata.product[3] = d;
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
                                                                                                    salesdata = new SalesData(6);
                                                                                                    salesdata.product[0] = a;
                                                                                                    salesdata.product[1] = b;
                                                                                                    salesdata.product[2] = c;
                                                                                                    salesdata.product[3] = d;
                                                                                                    salesdata.product[4] = e;
                                                                                                    salesdata.product[5] = f;
                                                                                                    SalesDataList.Add(salesdata);
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        salesdata = new SalesData(5);
                                                                                        salesdata.product[0] = a;
                                                                                        salesdata.product[1] = b;
                                                                                        salesdata.product[2] = c;
                                                                                        salesdata.product[3] = d;
                                                                                        salesdata.product[4] = e;
                                                                                        SalesDataList.Add(salesdata);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        salesdata = new SalesData(4);
                                                                        salesdata.product[0] = a;
                                                                        salesdata.product[1] = b;
                                                                        salesdata.product[2] = c;
                                                                        salesdata.product[3] = d;
                                                                        SalesDataList.Add(salesdata);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        salesdata = new SalesData(3);
                                                        salesdata.product[0] = a;
                                                        salesdata.product[1] = b;
                                                        salesdata.product[2] = c;
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
                                                                                                salesdata = new SalesData(6);
                                                                                                salesdata.product[0] = a;
                                                                                                salesdata.product[1] = b;
                                                                                                salesdata.product[2] = c;
                                                                                                salesdata.product[3] = d;
                                                                                                salesdata.product[4] = e;
                                                                                                salesdata.product[5] = f;
                                                                                                SalesDataList.Add(salesdata);
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    salesdata = new SalesData(5);
                                                                                    salesdata.product[0] = a;
                                                                                    salesdata.product[1] = b;
                                                                                    salesdata.product[2] = c;
                                                                                    salesdata.product[3] = d;
                                                                                    salesdata.product[4] = e;
                                                                                    SalesDataList.Add(salesdata);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    salesdata = new SalesData(4);
                                                                    salesdata.product[0] = a;
                                                                    salesdata.product[1] = b;
                                                                    salesdata.product[2] = c;
                                                                    salesdata.product[3] = d;
                                                                    SalesDataList.Add(salesdata);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    salesdata = new SalesData(3);
                                                    salesdata.product[0] = a;
                                                    salesdata.product[1] = b;
                                                    salesdata.product[2] = c;
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
                                                                                        salesdata = new SalesData(6);
                                                                                        salesdata.product[0] = a;
                                                                                        salesdata.product[1] = b;
                                                                                        salesdata.product[2] = c;
                                                                                        salesdata.product[3] = d;
                                                                                        salesdata.product[4] = e;
                                                                                        salesdata.product[5] = f;
                                                                                        SalesDataList.Add(salesdata);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            salesdata = new SalesData(5);
                                                                            salesdata.product[0] = a;
                                                                            salesdata.product[1] = b;
                                                                            salesdata.product[2] = c;
                                                                            salesdata.product[3] = d;
                                                                            salesdata.product[4] = e;
                                                                            SalesDataList.Add(salesdata);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            salesdata = new SalesData(4);
                                                            salesdata.product[0] = a;
                                                            salesdata.product[1] = b;
                                                            salesdata.product[2] = c;
                                                            salesdata.product[3] = d;
                                                            SalesDataList.Add(salesdata);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            salesdata = new SalesData(3);
                                            salesdata.product[0] = a;
                                            salesdata.product[1] = b;
                                            salesdata.product[2] = c;
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
        return SalesDataList;
    }

    public List<SalesData> GetResultListGPU(int _cycle, int[] _stack, int _groove, List<SalesData> enableList)
    {
        List<SalesData> dataList = GetEnableList(false).ConvertAll(form => new SalesData(form));
        int gpuKernel = gpuCalculate.FindKernel("CSMain");
        int[] value = new int[productList.Count];
        for (int i = 0; i < value.Length; ++i)
            value[i] = productList[i].GetValue();
        gpuCalculateValue = new ComputeBuffer(productList.Count, sizeof(uint));
        gpuCalculateValue.SetData(value);
        gpuCalculate.SetBuffer(gpuKernel, "value", gpuCalculateValue);
        int[] supply = new int[productList.Count];
        for (int i = 0; i < supply.Length; ++i)
            supply[i] = productList[i].GetSupplyValue(_cycle) + _stack[i];
        gpuCalculateSupply = new ComputeBuffer(productList.Count, sizeof(int));
        gpuCalculateSupply.SetData(supply);
        gpuCalculate.SetBuffer(gpuKernel, "supply", gpuCalculateSupply);
        float[] popularity = new float[productList.Count];
        for (int i = 0; i < popularity.Length; ++i)
            popularity[i] = GetPopularityValue(productList[i].GetPopularity(_cycle));
        gpuCalculatePopularity = new ComputeBuffer(productList.Count, sizeof(float));
        gpuCalculatePopularity.SetData(popularity);
        gpuCalculate.SetBuffer(gpuKernel, "popularity", gpuCalculatePopularity);

        gpuCalculate.SetInt("grooveNow", _groove);
        gpuCalculate.SetInt("grooveMax", userManager.GetMaxGroove());
        gpuCalculate.SetInt("workshopActive", userManager.GetWorkshopActive());
        gpuCalculate.SetFloat("workshopRank", GetWorkshopTierValue(userManager.GetWorkshopRank()));
        gpuCalculateItemA = new ComputeBuffer(1024, sizeof(int));
        gpuCalculateItemB = new ComputeBuffer(1024, sizeof(int));
        gpuCalculateItemC = new ComputeBuffer(1024, sizeof(int));
        gpuCalculateItemD = new ComputeBuffer(1024, sizeof(int));
        gpuCalculateItemE = new ComputeBuffer(1024, sizeof(int));
        gpuCalculateItemF = new ComputeBuffer(1024, sizeof(int));
        gpuCalculateResultA = new ComputeBuffer(1024, sizeof(uint));
        gpuCalculateResultB = new ComputeBuffer(1024, sizeof(uint));
        gpuCalculateResultC = new ComputeBuffer(1024, sizeof(uint));
        gpuCalculateResultD = new ComputeBuffer(1024, sizeof(uint));
        gpuCalculateResultE = new ComputeBuffer(1024, sizeof(uint));
        gpuCalculateResultF = new ComputeBuffer(1024, sizeof(uint));
        gpuCalculateResultTotal = new ComputeBuffer(1024, sizeof(uint));
        int[] itemA = new int[1024];
        int[] itemB = new int[1024];
        int[] itemC = new int[1024];
        int[] itemD = new int[1024];
        int[] itemE = new int[1024];
        int[] itemF = new int[1024];
        int highestValue = 0;
        int lastIndex = (dataList.Count % 1024 == 0 ? 0 : 1) + dataList.Count / 1024;
        int dataIndex;
        for (int i = 0; i < lastIndex; ++i)
        {
            if (i == lastIndex - 1)
            {
                for (int j = 0; j < dataList.Count % 1024; ++j)
                {
                    dataIndex = 1024 * i + j;
                    itemA[j] = dataList[dataIndex].product[0];
                    itemB[j] = dataList[dataIndex].product[1];
                    itemC[j] = dataList[dataIndex].product[2];
                    itemD[j] = dataList[dataIndex].product[3];
                    itemE[j] = dataList[dataIndex].product[4];
                    itemF[j] = dataList[dataIndex].product[5];
                }
                for (int j = dataList.Count % 1024; j < 1024; ++j)
                {
                    itemA[j] = -1;
                    itemB[j] = -1;
                    itemC[j] = -1;
                    itemD[j] = -1;
                    itemE[j] = -1;
                    itemF[j] = -1;
                }
            }
            else
            {
                for (int j = 0; j < 1024; ++j)
                {
                    dataIndex = 1024 * i + j;
                    itemA[j] = dataList[dataIndex].product[0];
                    itemB[j] = dataList[dataIndex].product[1];
                    itemC[j] = dataList[dataIndex].product[2];
                    itemD[j] = dataList[dataIndex].product[3];
                    itemE[j] = dataList[dataIndex].product[4];
                    itemF[j] = dataList[dataIndex].product[5];
                }
            }
            gpuCalculateItemA.SetData(itemA);
            gpuCalculate.SetBuffer(gpuKernel, "ItemA", gpuCalculateItemA);
            gpuCalculateItemB.SetData(itemB);
            gpuCalculate.SetBuffer(gpuKernel, "ItemB", gpuCalculateItemB);
            gpuCalculateItemC.SetData(itemC);
            gpuCalculate.SetBuffer(gpuKernel, "ItemC", gpuCalculateItemC);
            gpuCalculateItemD.SetData(itemD);
            gpuCalculate.SetBuffer(gpuKernel, "ItemD", gpuCalculateItemD);
            gpuCalculateItemE.SetData(itemE);
            gpuCalculate.SetBuffer(gpuKernel, "ItemE", gpuCalculateItemE);
            gpuCalculateItemF.SetData(itemF);
            gpuCalculate.SetBuffer(gpuKernel, "ItemF", gpuCalculateItemF);

            int[] resultA = new int[1024];
            gpuCalculateResultA.SetData(resultA);
            gpuCalculate.SetBuffer(gpuKernel, "ResultA", gpuCalculateResultA);
            int[] resultB = new int[1024];
            gpuCalculateResultB.SetData(resultB);
            gpuCalculate.SetBuffer(gpuKernel, "ResultB", gpuCalculateResultB);
            int[] resultC = new int[1024];
            gpuCalculateResultC.SetData(resultC);
            gpuCalculate.SetBuffer(gpuKernel, "ResultC", gpuCalculateResultC);
            int[] resultD = new int[1024];
            gpuCalculateResultD.SetData(resultD);
            gpuCalculate.SetBuffer(gpuKernel, "ResultD", gpuCalculateResultD);
            int[] resultE = new int[1024];
            gpuCalculateResultE.SetData(resultE);
            gpuCalculate.SetBuffer(gpuKernel, "ResultE", gpuCalculateResultE);
            int[] resultF = new int[1024];
            gpuCalculateResultF.SetData(resultF);
            gpuCalculate.SetBuffer(gpuKernel, "ResultF", gpuCalculateResultF);
            int[] resultTotal = new int[1024];
            gpuCalculateResultTotal.SetData(resultTotal);
            gpuCalculate.SetBuffer(gpuKernel, "ResultTotal", gpuCalculateResultTotal);

            gpuCalculate.Dispatch(gpuKernel, 1024, 1, 1);

            gpuCalculateResultA.GetData(resultA);
            gpuCalculateResultB.GetData(resultB);
            gpuCalculateResultC.GetData(resultC);
            gpuCalculateResultD.GetData(resultD);
            gpuCalculateResultE.GetData(resultE);
            gpuCalculateResultF.GetData(resultF);
            gpuCalculateResultTotal.GetData(resultTotal);
            for (int j = 0; j < (i == lastIndex - 1 ? dataList.Count % 1024 : resultTotal.Length); ++j)
            {
                dataIndex = 1024 * i + j;
                dataList[dataIndex].value[0] = resultA[j];
                dataList[dataIndex].value[1] = resultB[j];
                dataList[dataIndex].value[2] = resultC[j];
                dataList[dataIndex].value[3] = resultD[j];
                dataList[dataIndex].value[4] = resultE[j];
                dataList[dataIndex].value[5] = resultF[j];
                dataList[dataIndex].totalValue = resultTotal[j];
                if (highestValue < resultTotal[j])
                    highestValue = resultTotal[j];
            }
            resultA = null;
            resultB = null;
            resultC = null;
            resultD = null;
            resultE = null;
            resultF = null;
            resultTotal = null;
        }
        itemA = null;
        itemB = null;
        itemC = null;
        itemD = null;
        itemE = null;
        itemF = null;
        gpuCalculateValue.Release();
        gpuCalculateSupply.Release();
        gpuCalculatePopularity.Release();
        gpuCalculateItemA.Release();
        gpuCalculateItemB.Release();
        gpuCalculateItemC.Release();
        gpuCalculateItemD.Release();
        gpuCalculateItemE.Release();
        gpuCalculateItemF.Release();
        gpuCalculateResultA.Release();
        gpuCalculateResultB.Release();
        gpuCalculateResultC.Release();
        gpuCalculateResultD.Release();
        gpuCalculateResultE.Release();
        gpuCalculateResultF.Release();
        gpuCalculateResultTotal.Release();
        return dataList;
    }

    public SalesData GetHighestResultGPU(int _cycle, int[] _stack, int _groove, List<SalesData> dataList)
    {
        int gpuKernel = gpuCalculate.FindKernel("CSMain");
        int[] value = new int[productList.Count];
        for (int i = 0; i < value.Length; ++i)
            value[i] = productList[i].GetValue();
        gpuCalculateValue = new ComputeBuffer(productList.Count, sizeof(uint));
        gpuCalculateValue.SetData(value);
        gpuCalculate.SetBuffer(gpuKernel, "value", gpuCalculateValue);
        int[] supply = new int[productList.Count];
        for (int i = 0; i < supply.Length; ++i)
            supply[i] = productList[i].GetSupplyValue(_cycle) + _stack[i];
        gpuCalculateSupply = new ComputeBuffer(productList.Count, sizeof(int));
        gpuCalculateSupply.SetData(supply);
        gpuCalculate.SetBuffer(gpuKernel, "supply", gpuCalculateSupply);
        float[] popularity = new float[productList.Count];
        for (int i = 0; i < popularity.Length; ++i)
            popularity[i] = GetPopularityValue(productList[i].GetPopularity(_cycle));
        gpuCalculatePopularity = new ComputeBuffer(productList.Count, sizeof(float));
        gpuCalculatePopularity.SetData(popularity);
        gpuCalculate.SetBuffer(gpuKernel, "popularity", gpuCalculatePopularity);

        gpuCalculate.SetInt("grooveNow", _groove);
        gpuCalculate.SetInt("grooveMax", userManager.GetMaxGroove());
        gpuCalculate.SetInt("workshopActive", userManager.GetWorkshopActive());
        gpuCalculate.SetFloat("workshopRank", GetWorkshopTierValue(userManager.GetWorkshopRank()));
        gpuCalculateItemA = new ComputeBuffer(1024, sizeof(int));
        gpuCalculateItemB = new ComputeBuffer(1024, sizeof(int));
        gpuCalculateItemC = new ComputeBuffer(1024, sizeof(int));
        gpuCalculateItemD = new ComputeBuffer(1024, sizeof(int));
        gpuCalculateItemE = new ComputeBuffer(1024, sizeof(int));
        gpuCalculateItemF = new ComputeBuffer(1024, sizeof(int));
        gpuCalculateResultA = new ComputeBuffer(1024, sizeof(uint));
        gpuCalculateResultB = new ComputeBuffer(1024, sizeof(uint));
        gpuCalculateResultC = new ComputeBuffer(1024, sizeof(uint));
        gpuCalculateResultD = new ComputeBuffer(1024, sizeof(uint));
        gpuCalculateResultE = new ComputeBuffer(1024, sizeof(uint));
        gpuCalculateResultF = new ComputeBuffer(1024, sizeof(uint));
        gpuCalculateResultTotal = new ComputeBuffer(1024, sizeof(uint));
        int[] itemA = new int[1024];
        int[] itemB = new int[1024];
        int[] itemC = new int[1024];
        int[] itemD = new int[1024];
        int[] itemE = new int[1024];
        int[] itemF = new int[1024];
        int lastIndex = (dataList.Count % 1024 == 0 ? 0 : 1) + dataList.Count / 1024;
        int dataIndex;
        SalesData highestData = new SalesData(0);
        for (int i = 0; i < lastIndex; ++i)
        {
            if (i == lastIndex - 1)
            {
                for (int j = 0; j < dataList.Count % 1024; ++j)
                {
                    dataIndex = 1024 * i + j;
                    itemA[j] = dataList[dataIndex].product[0];
                    itemB[j] = dataList[dataIndex].product[1];
                    itemC[j] = dataList[dataIndex].product[2];
                    itemD[j] = dataList[dataIndex].product[3];
                    itemE[j] = dataList[dataIndex].product[4];
                    itemF[j] = dataList[dataIndex].product[5];
                }
                for (int j = dataList.Count % 1024; j < 1024; ++j)
                {
                    itemA[j] = -1;
                    itemB[j] = -1;
                    itemC[j] = -1;
                    itemD[j] = -1;
                    itemE[j] = -1;
                    itemF[j] = -1;
                }
            }
            else
            {
                for (int j = 0; j < 1024; ++j)
                {
                    dataIndex = 1024 * i + j;
                    itemA[j] = dataList[dataIndex].product[0];
                    itemB[j] = dataList[dataIndex].product[1];
                    itemC[j] = dataList[dataIndex].product[2];
                    itemD[j] = dataList[dataIndex].product[3];
                    itemE[j] = dataList[dataIndex].product[4];
                    itemF[j] = dataList[dataIndex].product[5];
                }
            }
            gpuCalculateItemA.SetData(itemA);
            gpuCalculate.SetBuffer(gpuKernel, "ItemA", gpuCalculateItemA);
            gpuCalculateItemB.SetData(itemB);
            gpuCalculate.SetBuffer(gpuKernel, "ItemB", gpuCalculateItemB);
            gpuCalculateItemC.SetData(itemC);
            gpuCalculate.SetBuffer(gpuKernel, "ItemC", gpuCalculateItemC);
            gpuCalculateItemD.SetData(itemD);
            gpuCalculate.SetBuffer(gpuKernel, "ItemD", gpuCalculateItemD);
            gpuCalculateItemE.SetData(itemE);
            gpuCalculate.SetBuffer(gpuKernel, "ItemE", gpuCalculateItemE);
            gpuCalculateItemF.SetData(itemF);
            gpuCalculate.SetBuffer(gpuKernel, "ItemF", gpuCalculateItemF);

            int[] resultA = new int[1024];
            gpuCalculateResultA.SetData(resultA);
            gpuCalculate.SetBuffer(gpuKernel, "ResultA", gpuCalculateResultA);
            int[] resultB = new int[1024];
            gpuCalculateResultB.SetData(resultB);
            gpuCalculate.SetBuffer(gpuKernel, "ResultB", gpuCalculateResultB);
            int[] resultC = new int[1024];
            gpuCalculateResultC.SetData(resultC);
            gpuCalculate.SetBuffer(gpuKernel, "ResultC", gpuCalculateResultC);
            int[] resultD = new int[1024];
            gpuCalculateResultD.SetData(resultD);
            gpuCalculate.SetBuffer(gpuKernel, "ResultD", gpuCalculateResultD);
            int[] resultE = new int[1024];
            gpuCalculateResultE.SetData(resultE);
            gpuCalculate.SetBuffer(gpuKernel, "ResultE", gpuCalculateResultE);
            int[] resultF = new int[1024];
            gpuCalculateResultF.SetData(resultF);
            gpuCalculate.SetBuffer(gpuKernel, "ResultF", gpuCalculateResultF);
            int[] resultTotal = new int[1024];
            gpuCalculateResultTotal.SetData(resultTotal);
            gpuCalculate.SetBuffer(gpuKernel, "ResultTotal", gpuCalculateResultTotal);

            gpuCalculate.Dispatch(gpuKernel, 1024, 1, 1);

            gpuCalculateResultA.GetData(resultA);
            gpuCalculateResultB.GetData(resultB);
            gpuCalculateResultC.GetData(resultC);
            gpuCalculateResultD.GetData(resultD);
            gpuCalculateResultE.GetData(resultE);
            gpuCalculateResultF.GetData(resultF);
            gpuCalculateResultTotal.GetData(resultTotal);
            for (int j = 0; j < (i == lastIndex - 1 ? dataList.Count % 1024 : resultTotal.Length); ++j)
            {
                if (highestData.totalValue < resultTotal[j])
                {
                    dataIndex = 1024 * i + j;
                    int size = dataList[dataIndex].product.Length;
                    Array.Resize(ref highestData.product, size);
                    for (int k = 0; k < size; ++k)
                    {
                        highestData.product[k] = dataList[dataIndex].product[k];
                    }
                    switch(size)
                    {
                    case 3:
                        {
                            highestData.value[0] = resultA[j];
                            highestData.value[1] = resultB[j];
                            highestData.value[2] = resultC[j];
                            break;
                        }
                    case 4:
                        {
                            highestData.value[0] = resultA[j];
                            highestData.value[1] = resultB[j];
                            highestData.value[2] = resultC[j];
                            highestData.value[3] = resultD[j];
                            break;
                        }
                    case 5:
                        {
                            highestData.value[0] = resultA[j];
                            highestData.value[1] = resultB[j];
                            highestData.value[2] = resultC[j];
                            highestData.value[3] = resultD[j];
                            highestData.value[4] = resultE[j];
                            break;
                        }
                    case 6:
                        {
                            highestData.value[0] = resultA[j];
                            highestData.value[1] = resultB[j];
                            highestData.value[2] = resultC[j];
                            highestData.value[3] = resultD[j];
                            highestData.value[4] = resultE[j];
                            highestData.value[5] = resultF[j];
                            break;
                        }
                    }
                    highestData.totalValue = resultTotal[j];
                }
            }
            resultA = null;
            resultB = null;
            resultC = null;
            resultD = null;
            resultE = null;
            resultF = null;
            resultTotal = null;
        }
        itemA = null;
        itemB = null;
        itemC = null;
        itemD = null;
        itemE = null;
        itemF = null;
        value = null;
        supply = null;
        popularity = null;
        gpuCalculateValue.Release();
        gpuCalculateSupply.Release();
        gpuCalculatePopularity.Release();
        gpuCalculateItemA.Release();
        gpuCalculateItemB.Release();
        gpuCalculateItemC.Release();
        gpuCalculateItemD.Release();
        gpuCalculateItemE.Release();
        gpuCalculateItemF.Release();
        gpuCalculateResultA.Release();
        gpuCalculateResultB.Release();
        gpuCalculateResultC.Release();
        gpuCalculateResultD.Release();
        gpuCalculateResultE.Release();
        gpuCalculateResultF.Release();
        gpuCalculateResultTotal.Release();
        gpuKernel = default;
        return highestData;
    }

    public void UpdateEnableList()
    {
        if (allowedSalesList != null)
        {
            allowedSalesList.ForEach((form) =>
            {
                form.Clear();
            });
            allowedSalesList.Clear();
        }
        allowedSalesList = GetEnableList(false);
        textMaxCountValue.text = allowedSalesList.Count.ToString();
    }

    public void UpdatePeakData()
    {
        int index = 0;
        productList.ForEach((form) =>
        {
            string[] data = resourceManager.GetSupplyPattern()[index]["Pattern"].ToString().Split(' ');
            int[] pop = { 0, 0 };
            int.TryParse(resourceManager.GetSupplyPattern()[0]["Popularity"].ToString(), out pop[0]);
            int.TryParse(resourceManager.GetSupplyPattern()[0]["Next Popularity"].ToString(), out pop[1]);
            if (data.Length == 2)
            {
                if (data[0] != "Cycle")
                    form.SetPeak(int.Parse(data[0]), data[1] == "Strong" ? 1 : data[1] == "Weak" ? 2 : 0, resourceManager.GetStatusData(pop[0],form.GetIndex()), resourceManager.GetStatusData(pop[1], form.GetIndex()));
                else
                {
                    if (data[1] == "4/5")
                        form.SetPeak(4, 0, resourceManager.GetStatusData(pop[0], form.GetIndex()), resourceManager.GetStatusData(pop[1], form.GetIndex()));
                    else if (data[1] == "5")
                        form.SetPeak(5, 0, resourceManager.GetStatusData(pop[0], form.GetIndex()), resourceManager.GetStatusData(pop[1], form.GetIndex()));
                    else if (data[1] == "6/7")
                        form.SetPeak(6, 0, resourceManager.GetStatusData(pop[0], form.GetIndex()), resourceManager.GetStatusData(pop[1], form.GetIndex()));
                    else if (data[1] == "3/6/7")
                        form.SetPeak(3, -1, resourceManager.GetStatusData(pop[0], form.GetIndex()), resourceManager.GetStatusData(pop[1], form.GetIndex()));
                }
            }
            else
                form.SetPeak(0, 0, resourceManager.GetStatusData(pop[0], form.GetIndex()), resourceManager.GetStatusData(pop[1], form.GetIndex()));
            ++index;
        });
        SalesData form;
        if (cycle < 6)
        {
            for (int i = 0; i < 6; ++i)
            {
                form = userManager.GetSalesData(i);
                if (form.product != null && form.product.Length > 0)
                {
                    for (int j = 0; j < form.product.Length; ++j)
                    {
                        int value = (j == 0 ? 1 : 2) * (userManager.GetWorkshopActive() > 3 ? 3 : userManager.GetWorkshopActive());
                        for (int k = i + 1; k < 7; ++k)
                            productList[form.product[j]].AddSupply(k, value);
                    }
                }
                if (form.product4 != null && form.product4.Length > 0)
                {
                    for (int j = 0; j < form.product4.Length; ++j)
                    {
                        int value = (j == 0 ? 1 : 2);
                        for (int k = i + 1; k < 7; ++k)
                            productList[form.product4[j]].AddSupply(k, value);
                    }
                }
            }
        }
    }

    public List<Product> GetProductList => productList;

    public void SetNowSalesData(SalesList list)
    {
        if (cycle < 7)
        {
            if (userManager.GetSalesData(cycle).product != null)
            {
                if (cycle < 6)
                {
                    for (int i = 0; i < userManager.GetSalesData(cycle).product.Length; ++i)
                    {
                        int value = (i == 0 ? -1 : -2) * (userManager.GetWorkshopActive() > 3 ? 3 : userManager.GetWorkshopActive());
                        for (int j = cycle + 1; j < 7; ++j)
                            productList[userManager.GetSalesData(cycle).product[i]].AddSupply(j, value);
                    }
                    if (userManager.GetWorkshopActive() == 4 && userManager.GetSalesData(cycle).product4 != null)
                    {
                        for (int i = 0; i < userManager.GetSalesData(cycle).product4.Length; ++i)
                        {
                            int value = (i == 0 ? -1 : -2);
                            for (int j = cycle + 1; j < 7; ++j)
                                productList[userManager.GetSalesData(cycle).product4[i]].AddSupply(j, value);
                        }
                    }
                }
            }
            userManager.SetSalesData(cycle, list.GetData()[0]);
            nowSalesData.SetData(list.GetData()[0]);
            nowSalesData.textSalesProductList.text = list.textSalesProductList.text;
            nowSalesData.textSalesValue.text = list.textSalesValue.text;
            if (cycle < 6)
            {
                for (int i = 0; i < list.GetData()[0].product.Length; ++i)
                {
                    int value = (i == 0 ? 1 : 2) * (userManager.GetWorkshopActive() > 3 ? 3 : userManager.GetWorkshopActive());
                    for (int j = cycle + 1; j < 7; ++j)
                        productList[list.GetData()[0].product[i]].AddSupply(j, value);
                }
                if (userManager.GetWorkshopActive() == 4 && list.GetData()[0].product4 != null)
                {
                    for (int i = 0; i < list.GetData()[0].product4.Length; ++i)
                    {
                        int value = (i == 0 ? 1 : 2);
                        for (int j = cycle + 1; j < 7; ++j)
                            productList[list.GetData()[0].product4[i]].AddSupply(j, value);
                    }
                }
            }
            CalculateAllData(false);
        }
    }

    void CalculateAllData(bool _all)
    {
        int nowGroove = 0;
        totalValue = 0;
        for (int i = 0; i < 7; ++i)
        {
            if (userManager.GetSalesData(i).product != null && userManager.GetSalesData(i).product.Length > 0)
            {
                SalesData form = userManager.GetSalesData(i);
                form.totalValue = 0;
                int[] stack = new int[productList.Count];
                if (userManager.GetSalesData(i).product4 != null && userManager.GetSalesData(i).product4.Length > 0)
                {
                    int timeA = 0;
                    int timeB = 0;
                    int turnA = 0;
                    int turnB = 0;
                    int nowTime = 0;
                    int groovePlus = 0;
                    while (turnA < form.product.Length || turnB < form.product4.Length)
                    {
                        if (nowTime == timeA)
                        {
                            form.value[turnA] = GetProductValue4(form.product[turnA], turnA, i, nowGroove + groovePlus, stack[form.product[turnA]]);
                            form.totalValue += form.value[turnA] * 3;
                        }
                        if (nowTime == timeB)
                        {
                            form.value4[turnB] = GetProductValue4(form.product4[turnB], turnB, i, nowGroove + groovePlus, stack[form.product4[turnB]]);
                            form.totalValue += form.value4[turnB];
                        }
                        if (nowTime == timeA)
                        {
                            stack[form.product[turnA]] += 3 * (turnA == 0 ? 1 : 2);
                            timeA += resourceManager.GetProductTime(form.product[turnA]);
                            groovePlus += 3;
                            ++turnA;
                        }
                        if (nowTime == timeB)
                        {
                            stack[form.product4[turnB]] += (turnB == 0 ? 1 : 2);
                            timeB += resourceManager.GetProductTime(form.product4[turnB]);
                            ++groovePlus;
                            ++turnB;
                        }
                        if (timeA < timeB)
                            nowTime = timeA;
                        else
                            nowTime = timeB;
                    }
                    nowGroove += (form.product.Length - 1) * 3 + form.product4.Length - 1;
                }
                else
                {
                    for (int j = 0; j < userManager.GetSalesData(i).product.Length; ++j)
                    {
                        form.value[j] = GetProductValue(userManager.GetSalesData(i).product[j], j, i, nowGroove, stack[userManager.GetSalesData(i).product[j]]);
                        form.totalValue += form.value[j] * userManager.GetWorkshopActive();
                        stack[userManager.GetSalesData(i).product[j]] += userManager.GetWorkshopActive() * (j == 0 ? 1 : 2);
                    }
                    nowGroove += (userManager.GetSalesData(i).product.Length - 1) * userManager.GetWorkshopActive();
                }
                totalValue += form.totalValue;
                userManager.SetSalesData(i, form);
                dropCycle.options[i].text = resourceManager.GetText(35).Replace("{0}", (i + 1).ToString()) + $" [{userManager.GetSalesData(i).totalValue}]";
            }
            else
                dropCycle.options[i].text = resourceManager.GetText(35).Replace("{0}", (i + 1).ToString());
        }
        textTotalValue.text = $"{resourceManager.GetText(50)} : {totalValue}";
        if (!_all)
            dropCycle.captionText.text = resourceManager.GetText(35).Replace("{0}", (cycle + 1).ToString()) + (userManager.GetSalesData(cycle).totalValue > 0 ? $" [{userManager.GetSalesData(cycle).totalValue}]" : "");
    }
}