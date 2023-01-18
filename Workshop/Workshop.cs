using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SalesData
{
    public SalesData()
    {
    }

    public SalesData(SalesData copy)
    {
        for(int i = 0;i<6;++i)
            product[i] = copy.product[i];
        productSize = copy.productSize;
    }

    public int[] product = new int[6];
    public int productSize = 0;
    public int[] value = new int[6];
    public int totalValue = 0;

    public void Clear()
    {
        product = null;
        productSize = default;
        value = null;
        totalValue = default;
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
    public Text textGroovePriority;
    public Toggle toggleGroovePriority;
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

    static List<Product> productList = new List<Product>();
    List<GameObject> salesList = new List<GameObject>();
    int cycle;
    bool[] reverse = new bool[5];
    int sort = 0;
    int totalValue;
    int maxEnableCount;

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
            long now = (DateTime.UtcNow.AddDays(-1).AddHours(-9).Ticks - 637961184000000000) / 1000000000;
            long season = now / 6048;
            //long day = (now % 6048) / 864 + 1;
            System.IO.DirectoryInfo path = new System.IO.DirectoryInfo($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/Kimchi Island Screenshot");
            if (!path.Exists)
                path.Create();
            if (cycle < 7)
                ScreenCapture.CaptureScreenshot($"{path.FullName}/S{season}-C{cycle + 1}-{(UserManager.instance.GetNetProfit() == true ? "Net_Profit" : "Native")}.png");
            else if (cycle == 7)
                ScreenCapture.CaptureScreenshot($"{path.FullName}/S{season}-C5~7-{(UserManager.instance.GetNetProfit() == true ? "Net_Profit" : "Native")}.png");
        }
    }
#endif

    void Awake()
    {
        instance = this;
        ResourceManager resourceManager = ResourceManager.instance;
        resourceManager.UpdateSupplyPattern();
        UserManager userManager = UserManager.instance;
        for (int i = 0; i < 7; ++i)
            totalValue += userManager.GetSalesData(i).totalValue;
        for (int i = 0; i < resourceManager.GetProductMax(); ++i)
        {
            GameObject productObject = Instantiate(Resources.Load("Prefab/Workshop/Product"),scrollProductList.content) as GameObject;
            Product product = productObject.GetComponent<Product>();
            product.SetDefaultData(i);
            productList.Add(product);
        }
        maxEnableCount = GetEnableList(false).Count;
        inputRank.text = userManager.GetPlayerRank().ToString();
        inputRank.onEndEdit.AddListener((data) =>
        {
            if (int.TryParse(data, out int value))
            {
                if (1 <= value && value <= 12)
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
        inputWorkshopRank.text = userManager.GetWorkshopRank().ToString();
        inputWorkshopRank.onEndEdit.AddListener((data) =>
        {
            if (int.TryParse(data, out int value))
            {
                if (1 <= value && value <= 3)
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
                if (1 <= value && value <= 3)
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
                if (userManager.GetSalesData(cycle).product != null && userManager.GetSalesData(cycle).productSize > 0)
                {
                    string productListString = "";
                    string productValueString = $"{userManager.GetSalesData(cycle).totalValue * userManager.GetWorkshopActive()} | {userManager.GetSalesData(cycle).totalValue} = ";
                    for (int i = 0; i < userManager.GetSalesData(cycle).productSize; ++i)
                    {
                        if (i > 0)
                        {
                            productListString += " + ";
                            productValueString += " + ";
                        }
                        productListString += $"<sprite={userManager.GetSalesData(cycle).product[i]}> {resourceManager.GetProductName(userManager.GetSalesData(cycle).product[i])}";
                        productValueString += userManager.GetSalesData(cycle).value[i].ToString();
                    }
                    nowSalesData.textSalesProductList.text = productListString;
                    nowSalesData.textSalesValue.text = productValueString;
                }
            }
            else
                CalculateAllData(true);
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
                if (userManager.GetSalesData(cycle).product != null && userManager.GetSalesData(cycle).productSize > 0)
                {
                    productListString = "";
                    productValueString = $"{userManager.GetSalesData(cycle).totalValue * userManager.GetWorkshopActive()} | {userManager.GetSalesData(cycle).totalValue} = ";
                    for (int i = 0; i < userManager.GetSalesData(cycle).productSize; ++i)
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
                        int dummy = (userManager.GetSalesData(i).product != null ? userManager.GetSalesData(i).productSize : 0);
                        groove += userManager.GetWorkshopActive() * (dummy >= 2 ? (dummy - 1) : 0);
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
                nowSalesData.SetData(new SalesData());
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
                        int dummy = (userManager.GetSalesData(i).product != null ? userManager.GetSalesData(i).productSize : 0);
                        groove += userManager.GetWorkshopActive() * (dummy >= 2 ? (dummy - 1) : 0);
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
        if (userManager.GetSalesData(0).product != null && userManager.GetSalesData(0).productSize > 0)
        {
            productListString = "";
            productValueString = $"{userManager.GetSalesData(0).totalValue * userManager.GetWorkshopActive()} | {userManager.GetSalesData(0).totalValue} = ";
            for (int i = 0; i < userManager.GetSalesData(0).productSize; ++i)
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
                List<SalesData> SalesDataList = GetResultList(cycle, new int[productList.Count], userManager.GetCurrentGroove(), false);
                SalesDataList.Sort((x1, x2) => x2.totalValue.CompareTo(x1.totalValue));
                foreach (SalesData dummySalesData in SalesDataList)
                {
                    if (salesList.Count < 100)
                    {
                        GameObject salesListObject = Instantiate(Resources.Load("Prefab/Workshop/SalesList"), scrollTopSalesList.content) as GameObject;
                        SalesList data = salesListObject.GetComponent<SalesList>();
                        data.SetData(dummySalesData);
                        string productListString = "";
                        string productValueString = $"{dummySalesData.totalValue * userManager.GetWorkshopActive()} | {dummySalesData.totalValue} = ";
                        for (int i = 0; i < dummySalesData.productSize; ++i)
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
            }
            else
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
                if (count != 2 && count != 3) return;
                SalesDataList highestResult = new SalesDataList();
                for (int i = 0; i < 3; ++i)
                    highestResult.salesData[i] = new SalesData();
                if (userManager.IsGPUCalculate())
                {
                    List<SalesData> dataList = GetEnableList(userManager.GetGroovePriority());
                    if (count >= 3)
                    {
                        EachResultList(4, new int[productList.Count], userManager.GetCurrentGroove(), false, (day5) =>
                        {
                            int[] stackA = new int[productList.Count];
                            for (int i = 0; i < day5.productSize; ++i)
                                stackA[day5.product[i]] += i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2;
                            EachResultList(5, stackA, userManager.GetCurrentGroove() + (day5.productSize - 1) * userManager.GetWorkshopActive(), false, (day6) =>
                            {
                                int[] stackB = new int[productList.Count];
                                for (int i = 0; i < day5.productSize; ++i)
                                    stackB[day5.product[i]] += i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2;
                                for (int i = 0; i < day6.productSize; ++i)
                                    stackB[day6.product[i]] += i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2;
                                EachResultList(6, stackB, userManager.GetCurrentGroove() + (day5.productSize + day6.productSize - 2) * userManager.GetWorkshopActive(), false, (day7) =>
                                {
                                    if (highestResult.totalValue < day5.totalValue + day6.totalValue + day7.totalValue)
                                    {
                                        highestResult.salesData[0].productSize = day5.productSize;
                                        for (int i = 0; i < day5.productSize; ++i)
                                            highestResult.salesData[0].product[i] = day5.product[i];
                                        for (int i = 0; i < day5.productSize; ++i)
                                            highestResult.salesData[0].value[i] = day5.value[i];
                                        highestResult.salesData[0].totalValue = day5.totalValue;
                                        highestResult.salesData[1].productSize = day6.productSize;
                                        for (int i = 0; i < day6.productSize; ++i)
                                            highestResult.salesData[1].product[i] = day6.product[i];
                                        for (int i = 0; i < day6.productSize; ++i)
                                            highestResult.salesData[1].value[i] = day6.value[i];
                                        highestResult.salesData[1].totalValue = day6.totalValue;
                                        highestResult.salesData[2].productSize = day7.productSize;
                                        for (int i = 0; i < day7.productSize; ++i)
                                            highestResult.salesData[2].product[i] = day7.product[i];
                                        for (int i = 0; i < day7.productSize; ++i)
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
                            for (int i = 0; i < day5.productSize; ++i)
                                stack[day5.product[i]] += (i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2);
                            SalesData day6 = GetHighestResultGPU(5, stack, userManager.GetCurrentGroove() + (day5.productSize - 1) * userManager.GetWorkshopActive(), dataList);
                            if (highestResult.totalValue < day5.totalValue + day6.totalValue)
                            {
                                highestResult.salesData[0].productSize = day5.productSize;
                                for (int i = 0; i < day5.productSize; ++i)
                                    highestResult.salesData[0].product[i] = day5.product[i];
                                for (int i = 0; i < day5.productSize; ++i)
                                    highestResult.salesData[0].value[i] = day5.value[i];
                                highestResult.salesData[0].totalValue = day5.totalValue;
                                highestResult.salesData[1].productSize = day6.productSize;
                                for (int i = 0; i < day6.productSize; ++i)
                                    highestResult.salesData[1].product[i] = day6.product[i];
                                for (int i = 0; i < day6.productSize; ++i)
                                    highestResult.salesData[1].value[i] = day6.value[i];
                                highestResult.salesData[1].totalValue = day6.totalValue;
                                highestResult.salesData[2].productSize = 0;
                                highestResult.salesData[2].totalValue = 0;
                                highestResult.totalValue = day5.totalValue + day6.totalValue;
                            }
                            day6.product = null;
                            day6.productSize = default;
                            day6.totalValue = default;
                            day6.value = null;
                            day6 = null;
                            SalesData day7 = GetHighestResultGPU(6, stack, userManager.GetCurrentGroove() + (day5.productSize - 1) * userManager.GetWorkshopActive(), dataList);
                            if (highestResult.totalValue < day5.totalValue + day7.totalValue)
                            {
                                highestResult.salesData[0].productSize = day5.productSize;
                                for (int i = 0; i < day5.productSize; ++i)
                                    highestResult.salesData[0].product[i] = day5.product[i];
                                for (int i = 0; i < day5.productSize; ++i)
                                    highestResult.salesData[0].value[i] = day5.value[i];
                                highestResult.salesData[0].totalValue = day5.totalValue;
                                highestResult.salesData[1].productSize = 0;
                                highestResult.salesData[1].totalValue = 0;
                                highestResult.salesData[2].productSize = day7.productSize;
                                for (int i = 0; i < day7.productSize; ++i)
                                    highestResult.salesData[2].product[i] = day7.product[i];
                                for (int i = 0; i < day7.productSize; ++i)
                                    highestResult.salesData[2].value[i] = day7.value[i];
                                highestResult.salesData[2].totalValue = day7.totalValue;
                                highestResult.totalValue = day5.totalValue + day7.totalValue;
                            }
                            day7.product = null;
                            day7.productSize = default;
                            day7.totalValue = default;
                            day7.value = null;
                            day7 = null;
                            stack = null;
                        });
                        EachResultList(5, new int[productList.Count], userManager.GetCurrentGroove(), false, (day6) =>
                        {
                            int[] stack = new int[productList.Count];
                            for (int i = 0; i < day6.productSize; ++i)
                                stack[day6.product[i]] += (i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2);
                            SalesData day7 = GetHighestResultGPU(6, stack, userManager.GetCurrentGroove() + (day6.productSize - 1) * userManager.GetWorkshopActive(), dataList);
                            if (highestResult.totalValue < day6.totalValue + day7.totalValue)
                            {
                                highestResult.salesData[0].productSize = 0;
                                highestResult.salesData[0].totalValue = 0;
                                highestResult.salesData[1].productSize = day6.productSize;
                                for (int i = 0; i < day6.productSize; ++i)
                                    highestResult.salesData[1].product[i] = day6.product[i];
                                for (int i = 0; i < day6.productSize; ++i)
                                    highestResult.salesData[1].value[i] = day6.value[i];
                                highestResult.salesData[1].totalValue = day6.totalValue;
                                highestResult.salesData[2].productSize = day7.productSize;
                                for (int i = 0; i < day7.productSize; ++i)
                                    highestResult.salesData[2].product[i] = day7.product[i];
                                for (int i = 0; i < day7.productSize; ++i)
                                    highestResult.salesData[2].value[i] = day7.value[i];
                                highestResult.salesData[2].totalValue = day7.totalValue;
                                highestResult.totalValue = day6.totalValue + day7.totalValue;
                            }
                            day7.product = null;
                            day7.productSize = default;
                            day7.totalValue = default;
                            day7.value = null;
                            day7 = null;
                            stack = null;
                        });
                    }
                }
                else
                {
                    List<SalesData> salesDataList = GetResultList(4, new int[productList.Count], userManager.GetCurrentGroove(), false);
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
                            for (int i = 0; i < day5.productSize; ++i)
                                stackA[day5.product[i]] += i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2;
                            List<SalesData> salesDataList6 = GetResultList(5, stackA, userManager.GetCurrentGroove() + (day5.productSize - 1) * userManager.GetWorkshopActive(), false);
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
                                    for (int i = 0; i < day5.productSize; ++i)
                                        stackB[day5.product[i]] += i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2;
                                    for (int i = 0; i < day6.productSize; ++i)
                                        stackB[day6.product[i]] += i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2;
                                    EachResultList(6, stackB, userManager.GetCurrentGroove() + (day5.productSize + day6.productSize - 2) * userManager.GetWorkshopActive(), false, (day7) =>
                                    {
                                        if (highestResult.totalValue < day5.totalValue + day6.totalValue + day7.totalValue)
                                        {
                                            highestResult.salesData[0].productSize = day5.productSize;
                                            for (int i = 0; i < day5.productSize; ++i)
                                                highestResult.salesData[0].product[i] = day5.product[i];
                                            for (int i = 0; i < day5.productSize; ++i)
                                                highestResult.salesData[0].value[i] = day5.value[i];
                                            highestResult.salesData[0].totalValue = day5.totalValue;
                                            highestResult.salesData[1].productSize = day6.productSize;
                                            for (int i = 0; i < day6.productSize; ++i)
                                                highestResult.salesData[1].product[i] = day6.product[i];
                                            for (int i = 0; i < day6.productSize; ++i)
                                                highestResult.salesData[1].value[i] = day6.value[i];
                                            highestResult.salesData[1].totalValue = day6.totalValue;
                                            highestResult.salesData[2].productSize = day7.productSize;
                                            for (int i = 0; i < day7.productSize; ++i)
                                                highestResult.salesData[2].product[i] = day7.product[i];
                                            for (int i = 0; i < day7.productSize; ++i)
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
                                EachResultList(5, stackA, userManager.GetCurrentGroove() + (day5.productSize - 1) * userManager.GetWorkshopActive(), false, (day6) =>
                                {
                                    int[] stackB = new int[productList.Count];
                                    for (int i = 0; i < day5.productSize; ++i)
                                        stackB[day5.product[i]] += i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2;
                                    for (int i = 0; i < day6.productSize; ++i)
                                        stackB[day6.product[i]] += i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2;
                                    EachResultList(6, stackB, userManager.GetCurrentGroove() + (day5.productSize + day6.productSize - 2) * userManager.GetWorkshopActive(), false, (day7) =>
                                    {
                                        if (highestResult.totalValue < day5.totalValue + day6.totalValue + day7.totalValue)
                                        {
                                            highestResult.salesData[0].productSize = day5.productSize;
                                            for (int i = 0; i < day5.productSize; ++i)
                                                highestResult.salesData[0].product[i] = day5.product[i];
                                            for (int i = 0; i < day5.productSize; ++i)
                                                highestResult.salesData[0].value[i] = day5.value[i];
                                            highestResult.salesData[0].totalValue = day5.totalValue;
                                            highestResult.salesData[1].productSize = day6.productSize;
                                            for (int i = 0; i < day6.productSize; ++i)
                                                highestResult.salesData[1].product[i] = day6.product[i];
                                            for (int i = 0; i < day6.productSize; ++i)
                                                highestResult.salesData[1].value[i] = day6.value[i];
                                            highestResult.salesData[1].totalValue = day6.totalValue;
                                            highestResult.salesData[2].productSize = day7.productSize;
                                            for (int i = 0; i < day7.productSize; ++i)
                                                highestResult.salesData[2].product[i] = day7.product[i];
                                            for (int i = 0; i < day7.productSize; ++i)
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
                            for (int i = 0; i < day5.productSize; ++i)
                                stack[day5.product[i]] += (i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2);
                            EachResultList(5, stack, userManager.GetCurrentGroove() + (day5.productSize - 1) * userManager.GetWorkshopActive(), false, (day6) =>
                            {
                                if (highestResult.totalValue < day5.totalValue + day6.totalValue)
                                {
                                    highestResult.salesData[0].productSize = day5.productSize;
                                    for (int i = 0; i < day5.productSize; ++i)
                                        highestResult.salesData[0].product[i] = day5.product[i];
                                    for (int i = 0; i < day5.productSize; ++i)
                                        highestResult.salesData[0].value[i] = day5.value[i];
                                    highestResult.salesData[0].totalValue = day5.totalValue;
                                    highestResult.salesData[1].productSize = day6.productSize;
                                    for (int i = 0; i < day6.productSize; ++i)
                                        highestResult.salesData[1].product[i] = day6.product[i];
                                    for (int i = 0; i < day6.productSize; ++i)
                                        highestResult.salesData[1].value[i] = day6.value[i];
                                    highestResult.salesData[1].totalValue = day6.totalValue;
                                    highestResult.salesData[2].productSize = 0;
                                    highestResult.salesData[2].totalValue = 0;
                                    highestResult.totalValue = day5.totalValue + day6.totalValue;
                                }
                            });
                            EachResultList(6, stack, userManager.GetCurrentGroove() + (day5.productSize - 1) * userManager.GetWorkshopActive(), false, (day7) =>
                            {
                                if (highestResult.totalValue < day5.totalValue + day7.totalValue)
                                {
                                    highestResult.salesData[0].productSize = day5.productSize;
                                    for (int i = 0; i < day5.productSize; ++i)
                                        highestResult.salesData[0].product[i] = day5.product[i];
                                    for (int i = 0; i < day5.productSize; ++i)
                                        highestResult.salesData[0].value[i] = day5.value[i];
                                    highestResult.salesData[0].totalValue = day5.totalValue;
                                    highestResult.salesData[1].productSize = 0;
                                    highestResult.salesData[1].totalValue = 0;
                                    highestResult.salesData[2].productSize = day7.productSize;
                                    for (int i = 0; i < day7.productSize; ++i)
                                        highestResult.salesData[2].product[i] = day7.product[i];
                                    for (int i = 0; i < day7.productSize; ++i)
                                        highestResult.salesData[2].value[i] = day7.value[i];
                                    highestResult.salesData[2].totalValue = day7.totalValue;
                                    highestResult.totalValue = day5.totalValue + day7.totalValue;
                                }
                            });
                            stack = null;
                        });
                        salesDataList = GetResultList(5, new int[productList.Count], userManager.GetCurrentGroove(), false);
                        if (userManager.GetLimitCount() > 0 && salesDataList.Count > userManager.GetLimitCount())
                        {
                            salesDataList.Sort((a, b) => b.totalValue.CompareTo(a.totalValue));
                            salesDataList.RemoveRange(userManager.GetLimitCount(), salesDataList.Count - userManager.GetLimitCount());
                        }
                        Parallel.ForEach(salesDataList, new ParallelOptions { MaxDegreeOfParallelism = userManager.GetCPUThread() }, day6 =>
                        {
                            int[] stack = new int[productList.Count];
                            for (int i = 0; i < day6.productSize; ++i)
                                stack[day6.product[i]] += (i == 0 ? userManager.GetWorkshopActive() : userManager.GetWorkshopActive() * 2);
                            EachResultList(6, stack, userManager.GetCurrentGroove() + (day6.productSize - 1) * userManager.GetWorkshopActive(), false, (day7) =>
                            {
                                if (highestResult.totalValue < day6.totalValue + day7.totalValue)
                                {
                                    highestResult.salesData[0].productSize = 0;
                                    highestResult.salesData[0].totalValue = 0;
                                    highestResult.salesData[1].productSize = day6.productSize;
                                    for (int i = 0; i < day6.productSize; ++i)
                                        highestResult.salesData[1].product[i] = day6.product[i];
                                    for (int i = 0; i < day6.productSize; ++i)
                                        highestResult.salesData[1].value[i] = day6.value[i];
                                    highestResult.salesData[1].totalValue = day6.totalValue;
                                    highestResult.salesData[2].productSize = day7.productSize;
                                    for (int i = 0; i < day7.productSize; ++i)
                                        highestResult.salesData[2].product[i] = day7.product[i];
                                    for (int i = 0; i < day7.productSize; ++i)
                                        highestResult.salesData[2].value[i] = day7.value[i];
                                    highestResult.salesData[2].totalValue = day7.totalValue;
                                    highestResult.totalValue = day6.totalValue + day7.totalValue;
                                }
                            });
                            stack = null;
                        });
                    }
                }
                GameObject salesListObject = Instantiate(Resources.Load("Prefab/Workshop/SalesList"), scrollTopSalesList.content) as GameObject;
                SalesList data = salesListObject.GetComponent<SalesList>();
                data.rectSalesProductList.offsetMax = new Vector2(data.rectSalesProductList.offsetMax.x, 50 * (highestResult.salesData.Length - 1));
                string productListString = "";
                string productValueString = "";
                for (int j = 0; j < highestResult.salesData.Length; ++j)
                {
                    if (highestResult.salesData[j].productSize > 0)
                    {
                        productValueString += $"{highestResult.salesData[j].totalValue * userManager.GetWorkshopActive()} | {highestResult.salesData[j].totalValue} = ";
                        for (int i = 0; i < highestResult.salesData[j].productSize; ++i)
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
                salesList.Add(salesListObject);
                SystemCore.instance.PlayAlarm();
            }
        });
        btnNowSales.onClick.AddListener(() =>
        {
            UserManager userManager = UserManager.instance;
            if (userManager.GetSalesData(cycle).product != null)
            {
                for (int i = 0; i < userManager.GetSalesData(cycle).productSize; ++i)
                {
                    int value = (i == 0 ? -1 : -2) * userManager.GetWorkshopActive();
                    for (int j = cycle + 1; j < 7; ++j)
                        productList[userManager.GetSalesData(cycle).product[i]].AddSupply(j, value);
                }
            }
            userManager.SetSalesData(cycle, new SalesData());
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
        textMaxCountValue.text = maxEnableCount.ToString();
        if (userManager.GetLimitCount() > maxEnableCount)
            userManager.SetLimitCount(maxEnableCount);
        inputLimitCount.text = userManager.GetLimitCount().ToString();
        inputLimitCount.onEndEdit.AddListener((value) =>
        {
            if (int.TryParse(value, out int result))
            {
                if (result < 0) result = 0;
                if (result > maxEnableCount) result = maxEnableCount;
                userManager.SetLimitCount(result);
            }
        });
        btnCrimeTime.onClick.AddListener(() =>
        {
            UserManager userManager = UserManager.instance;
            for (int k = 0; k < 6; ++k)
            {
                if (userManager.GetSalesData(k).product != null)
                {
                    for (int i = 0; i < userManager.GetSalesData(k).productSize; ++i)
                    {
                        int value = (i == 0 ? -1 : -2) * userManager.GetWorkshopActive();
                        for (int j = k + 1; j < 7; ++j)
                            productList[userManager.GetSalesData(k).product[i]].AddSupply(j, value);
                    }
                }
            }
            userManager.SetSalesData(0, new SalesData());
            userManager.SetSalesData(2, new SalesData());
            userManager.SetSalesData(4, new SalesData());
            userManager.SetSalesData(5, new SalesData());
            userManager.SetSalesData(6, new SalesData());
            SalesData day2 = new SalesData();
            day2.productSize = 6;
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
            SalesData day4 = new SalesData();
            day4.productSize = 6;
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
        });
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
        default: return 0;
        }
    }

    int GetProductValue(int _index, int _step, int _cycle, int _groove, int _stack)
    {
        UserManager userManager = UserManager.instance;
        int nowGroove = (_groove + userManager.GetWorkshopActive() * _step);
        if (nowGroove > userManager.GetMaxGroove())
            nowGroove = userManager.GetMaxGroove();
        return (_step > 0 ? 2 : 1) *
            (GetPopularityAndSupplyValue(productList[_index].GetPopularity(_cycle), productList[_index].GetSupply(_cycle, _stack)) *
            (productList[_index].GetValue() *
            GetWorkshopTierValueToInt(userManager.GetWorkshopRank()) *
            (100 + nowGroove) / 1000) / 100) - (userManager.GetNetProfit() == true ? productList[_index].GetSalesValue() : 0);
    }

    public void SetPacketData(byte[] data)
    {
        copySupplyPacket = "";
        copyPopularityPacket = "";
        ResourceManager resourceManager = ResourceManager.instance;
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
            int demandShift = 0;
            if ((data[35 + i] & (1 << 2)) == (1 << 2))
                demandShift = 5;
            else if ((data[35 + i] & (1 << 1) + (1 << 0)) == (1 << 1) + (1 << 0))
                demandShift = 4;
            else if ((data[35 + i] & (1 << 1)) == (1 << 1))
                demandShift = 3;
            else if ((data[35 + i] & (1 << 0)) == (1 << 0))
                demandShift = 2;
            else
                demandShift = 1;
            copySupplyPacket += $"{value}\t{3 - demandShift}";
            copyPopularityPacket += $"{resourceManager.GetStatusData(data[32], i)}\t{resourceManager.GetStatusData(data[33], i)}";
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

    public void SupplyPacketDataCopy() => GUIUtility.systemCopyBuffer = copySupplyPacket;

    public void PopularityPacketDataCopy() => GUIUtility.systemCopyBuffer = copyPopularityPacket;

    public int GetCycle() => cycle;

    public void ApplyLanguage()
    {
        ResourceManager resourceManager = ResourceManager.instance;
        UserManager userManager = UserManager.instance;
        textRank.text = resourceManager.GetText(1);
        textGrooveNow.text = resourceManager.GetText(2);
        textGrooveMax.text = resourceManager.GetText(3);
        textWorkshopRank.text = resourceManager.GetText(4);
        textWorkshopActive.text = resourceManager.GetText(6);
        textNetProfit.text = resourceManager.GetText(5);
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
                dropCycle.captionText.text = resourceManager.GetText(35).Replace("{0}", (cycle + 1).ToString()) + (userManager.GetSalesData(cycle).totalValue > 0 ? $" [{userManager.GetSalesData(cycle).totalValue * userManager.GetWorkshopActive()}]" : "");
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
                    form.text = resourceManager.GetText(35).Replace("{0}", (num + 1).ToString()) + (userManager.GetSalesData(num).totalValue > 0 ? $" [{userManager.GetSalesData(num).totalValue * userManager.GetWorkshopActive()}]" : "");
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
        textTotalValue.text = $"{resourceManager.GetText(50)} : {totalValue * userManager.GetWorkshopActive()}";
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

    List<SalesData> GetResultList(int _cycle, int[] _stack, int _groove, bool _hasPeak)
    {
        int[] stack = new int[_stack.Length];
        if (_cycle > 7)
            --_cycle;
        List<SalesData> SalesDataList = new List<SalesData>();
        SalesData salesdata;
        int checker;
        bool isPeak = false;
        bool isUsed = false;
        UserManager userManager = UserManager.instance;
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
        return SalesDataList;
    }

    void EachResultList(int _cycle, int[] _stack, int _groove, bool _hasPeak, Action<SalesData> result)
    {
        int[] stack = new int[_stack.Length];
        if (_cycle > 7)
            --_cycle;
        SalesData salesdata = new SalesData();
        int checker;
        bool isPeak = false;
        bool isUsed = false;
        UserManager userManager = UserManager.instance;
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
                                                                                    result.Invoke(salesdata);
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
                                                                                                result.Invoke(salesdata);
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
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
                                                                                    result.Invoke(salesdata);
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
                                                                                                    result.Invoke(salesdata);
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
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
                                                                                        result.Invoke(salesdata);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
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
                                                                        result.Invoke(salesdata);
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
                                                                                                        result.Invoke(salesdata);
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
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
                                                                                            result.Invoke(salesdata);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
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
                                                                            result.Invoke(salesdata);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
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
                                                            result.Invoke(salesdata);
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
                                                                                                    result.Invoke(salesdata);
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
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
                                                                                        result.Invoke(salesdata);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
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
                                                                        result.Invoke(salesdata);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
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
                                                                                            result.Invoke(salesdata);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
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
                                                                                result.Invoke(salesdata);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
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
                                                                result.Invoke(salesdata);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
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
    }

    List<SalesData> GetEnableList(bool _groovePriority)
    {
        List<SalesData> SalesDataList = new List<SalesData>();
        SalesData salesdata;
        int checker;
        UserManager userManager = UserManager.instance;
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
                                                                salesdata = new SalesData();
                                                                salesdata.product[0] = a;
                                                                salesdata.product[1] = b;
                                                                salesdata.product[2] = c;
                                                                salesdata.product[3] = d;
                                                                salesdata.product[4] = e;
                                                                salesdata.product[5] = f;
                                                                salesdata.productSize = 6;
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
                                                                                salesdata.product[0] = a;
                                                                                salesdata.product[1] = b;
                                                                                salesdata.product[2] = c;
                                                                                salesdata.product[3] = d;
                                                                                salesdata.product[4] = e;
                                                                                salesdata.product[5] = f;
                                                                                salesdata.productSize = 6;
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
                                                                                            salesdata.product[0] = a;
                                                                                            salesdata.product[1] = b;
                                                                                            salesdata.product[2] = c;
                                                                                            salesdata.product[3] = d;
                                                                                            salesdata.product[4] = e;
                                                                                            salesdata.product[5] = f;
                                                                                            salesdata.productSize = 6;
                                                                                            SalesDataList.Add(salesdata);
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                salesdata = new SalesData();
                                                                                salesdata.product[0] = a;
                                                                                salesdata.product[1] = b;
                                                                                salesdata.product[2] = c;
                                                                                salesdata.product[3] = d;
                                                                                salesdata.product[4] = e;
                                                                                salesdata.product[5] = -1;
                                                                                salesdata.productSize = 5;
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
                                                                                                salesdata.product[0] = a;
                                                                                                salesdata.product[1] = b;
                                                                                                salesdata.product[2] = c;
                                                                                                salesdata.product[3] = d;
                                                                                                salesdata.product[4] = e;
                                                                                                salesdata.product[5] = f;
                                                                                                salesdata.productSize = 6;
                                                                                                SalesDataList.Add(salesdata);
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    salesdata = new SalesData();
                                                                                    salesdata.product[0] = a;
                                                                                    salesdata.product[1] = b;
                                                                                    salesdata.product[2] = c;
                                                                                    salesdata.product[3] = d;
                                                                                    salesdata.product[4] = e;
                                                                                    salesdata.product[5] = -1;
                                                                                    salesdata.productSize = 5;
                                                                                    SalesDataList.Add(salesdata);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    salesdata = new SalesData();
                                                                    salesdata.product[0] = a;
                                                                    salesdata.product[1] = b;
                                                                    salesdata.product[2] = c;
                                                                    salesdata.product[3] = d;
                                                                    salesdata.product[4] = -1;
                                                                    salesdata.product[5] = -1;
                                                                    salesdata.productSize = 4;
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
                                                                                                    salesdata.product[0] = a;
                                                                                                    salesdata.product[1] = b;
                                                                                                    salesdata.product[2] = c;
                                                                                                    salesdata.product[3] = d;
                                                                                                    salesdata.product[4] = e;
                                                                                                    salesdata.product[5] = f;
                                                                                                    salesdata.productSize = 6;
                                                                                                    SalesDataList.Add(salesdata);
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        salesdata = new SalesData();
                                                                                        salesdata.product[0] = a;
                                                                                        salesdata.product[1] = b;
                                                                                        salesdata.product[2] = c;
                                                                                        salesdata.product[3] = d;
                                                                                        salesdata.product[4] = e;
                                                                                        salesdata.product[5] = -1;
                                                                                        salesdata.productSize = 5;
                                                                                        SalesDataList.Add(salesdata);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        salesdata = new SalesData();
                                                                        salesdata.product[0] = a;
                                                                        salesdata.product[1] = b;
                                                                        salesdata.product[2] = c;
                                                                        salesdata.product[3] = d;
                                                                        salesdata.product[4] = -1;
                                                                        salesdata.product[5] = -1;
                                                                        salesdata.productSize = 4;
                                                                        SalesDataList.Add(salesdata);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        salesdata = new SalesData();
                                                        salesdata.product[0] = a;
                                                        salesdata.product[1] = b;
                                                        salesdata.product[2] = c;
                                                        salesdata.product[3] = -1;
                                                        salesdata.product[4] = -1;
                                                        salesdata.product[5] = -1;
                                                        salesdata.productSize = 3;
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
                                                                                                salesdata.product[0] = a;
                                                                                                salesdata.product[1] = b;
                                                                                                salesdata.product[2] = c;
                                                                                                salesdata.product[3] = d;
                                                                                                salesdata.product[4] = e;
                                                                                                salesdata.product[5] = f;
                                                                                                salesdata.productSize = 6;
                                                                                                SalesDataList.Add(salesdata);
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    salesdata = new SalesData();
                                                                                    salesdata.product[0] = a;
                                                                                    salesdata.product[1] = b;
                                                                                    salesdata.product[2] = c;
                                                                                    salesdata.product[3] = d;
                                                                                    salesdata.product[4] = e;
                                                                                    salesdata.product[5] = -1;
                                                                                    salesdata.productSize = 5;
                                                                                    SalesDataList.Add(salesdata);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    salesdata = new SalesData();
                                                                    salesdata.product[0] = a;
                                                                    salesdata.product[1] = b;
                                                                    salesdata.product[2] = c;
                                                                    salesdata.product[3] = d;
                                                                    salesdata.product[4] = -1;
                                                                    salesdata.product[5] = -1;
                                                                    salesdata.productSize = 4;
                                                                    SalesDataList.Add(salesdata);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    salesdata = new SalesData();
                                                    salesdata.product[0] = a;
                                                    salesdata.product[1] = b;
                                                    salesdata.product[2] = c;
                                                    salesdata.product[3] = -1;
                                                    salesdata.product[4] = -1;
                                                    salesdata.product[5] = -1;
                                                    salesdata.productSize = 3;
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
                                                                                        salesdata.product[0] = a;
                                                                                        salesdata.product[1] = b;
                                                                                        salesdata.product[2] = c;
                                                                                        salesdata.product[3] = d;
                                                                                        salesdata.product[4] = e;
                                                                                        salesdata.product[5] = f;
                                                                                        salesdata.productSize = 6;
                                                                                        SalesDataList.Add(salesdata);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            salesdata = new SalesData();
                                                                            salesdata.product[0] = a;
                                                                            salesdata.product[1] = b;
                                                                            salesdata.product[2] = c;
                                                                            salesdata.product[3] = d;
                                                                            salesdata.product[4] = e;
                                                                            salesdata.product[5] = -1;
                                                                            salesdata.productSize = 5;
                                                                            SalesDataList.Add(salesdata);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            salesdata = new SalesData();
                                                            salesdata.product[0] = a;
                                                            salesdata.product[1] = b;
                                                            salesdata.product[2] = c;
                                                            salesdata.product[3] = d;
                                                            salesdata.product[4] = -1;
                                                            salesdata.product[5] = -1;
                                                            salesdata.productSize = 4;
                                                            SalesDataList.Add(salesdata);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            salesdata = new SalesData();
                                            salesdata.product[0] = a;
                                            salesdata.product[1] = b;
                                            salesdata.product[2] = c;
                                            salesdata.product[3] = -1;
                                            salesdata.product[4] = -1;
                                            salesdata.product[5] = -1;
                                            salesdata.productSize = 3;
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
        UserManager userManager = UserManager.instance;
        List<SalesData> dataList = GetEnableList(userManager.GetGroovePriority()).ConvertAll(form => new SalesData(form));
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
        UserManager userManager = UserManager.instance;
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
        SalesData highestData = new SalesData();
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
                    highestData.productSize = dataList[dataIndex].productSize;
                    highestData.product[0] = dataList[dataIndex].product[0];
                    highestData.product[1] = dataList[dataIndex].product[1];
                    highestData.product[2] = dataList[dataIndex].product[2];
                    highestData.product[3] = dataList[dataIndex].product[3];
                    highestData.product[4] = dataList[dataIndex].product[4];
                    highestData.product[5] = dataList[dataIndex].product[5];
                    highestData.value[0] = resultA[j];
                    highestData.value[1] = resultB[j];
                    highestData.value[2] = resultC[j];
                    highestData.value[3] = resultD[j];
                    highestData.value[4] = resultE[j];
                    highestData.value[5] = resultF[j];
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
                    form.SetPeak(int.Parse(data[0]), data[1] == "Strong" ? 1 : data[1] == "Weak" ? 2 : 0, pop[0], pop[1]);
                else
                {
                    if (data[1] == "4/5")
                        form.SetPeak(4, 0, pop[0], pop[1]);
                    else if (data[1] == "5")
                        form.SetPeak(5, 0, pop[0], pop[1]);
                    else if (data[1] == "6/7")
                        form.SetPeak(6, 0, pop[0], pop[1]);
                    else if (data[1] == "3/6/7")
                        form.SetPeak(3, -1, pop[0], pop[1]);
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
                if (userManager.GetSalesData(i).product != null && userManager.GetSalesData(i).productSize > 0)
                {
                    for (int j = 0; j < userManager.GetSalesData(i).productSize; ++j)
                    {
                        int value = (j == 0 ? 1 : 2) * userManager.GetWorkshopActive();
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
                    for (int i = 0; i < userManager.GetSalesData(cycle).productSize; ++i)
                    {
                        int value = (i == 0 ? -1 : -2) * userManager.GetWorkshopActive();
                        for (int j = cycle + 1; j < 7; ++j)
                            productList[userManager.GetSalesData(cycle).product[i]].AddSupply(j, value);
                    }
                }
            }
            userManager.SetSalesData(cycle, list.GetData()[0]);
            nowSalesData.SetData(list.GetData()[0]);
            nowSalesData.textSalesProductList.text = list.textSalesProductList.text;
            nowSalesData.textSalesValue.text = list.textSalesValue.text;
            if (cycle < 6)
            {
                for (int i = 0; i < list.GetData()[0].productSize; ++i)
                {
                    int value = (i == 0 ? 1 : 2) * userManager.GetWorkshopActive();
                    for (int j = cycle + 1; j < 7; ++j)
                        productList[list.GetData()[0].product[i]].AddSupply(j, value);
                }
            }
            CalculateAllData(false);
        }
    }

    void CalculateAllData(bool _all)
    {
        UserManager userManager = UserManager.instance;
        ResourceManager resourceManager = ResourceManager.instance;
        int nowGroove = 0;
        totalValue = 0;
        for (int i = 0; i < 7; ++i)
        {
            if (userManager.GetSalesData(i).product != null && userManager.GetSalesData(i).productSize > 0)
            {
                int dayValue = 0;
                SalesData form = userManager.GetSalesData(i);
                int[] stack = new int[productList.Count];
                for (int j = 0; j < userManager.GetSalesData(i).productSize; ++j)
                {
                    form.value[j] = GetProductValue(userManager.GetSalesData(i).product[j], j, i, nowGroove, stack[userManager.GetSalesData(i).product[j]]);
                    dayValue += form.value[j];
                    stack[userManager.GetSalesData(i).product[j]] += userManager.GetWorkshopActive() * (j == 0 ? 1 : 2);
                }
                form.totalValue = dayValue;
                totalValue += dayValue;
                userManager.SetSalesData(i, form);
                nowGroove += (userManager.GetSalesData(i).productSize - 1) * userManager.GetWorkshopActive();
                dropCycle.options[i].text = resourceManager.GetText(35).Replace("{0}", (i + 1).ToString()) + $" [{userManager.GetSalesData(i).totalValue * userManager.GetWorkshopActive()}]";
            }
            else
                dropCycle.options[i].text = resourceManager.GetText(35).Replace("{0}", (i + 1).ToString());
        }
        textTotalValue.text = $"{resourceManager.GetText(50)} : {totalValue * userManager.GetWorkshopActive()}";
        if (!_all)
            dropCycle.captionText.text = resourceManager.GetText(35).Replace("{0}", (cycle + 1).ToString()) + (userManager.GetSalesData(cycle).totalValue > 0 ? $" [{userManager.GetSalesData(cycle).totalValue * userManager.GetWorkshopActive()}]" : "");
    }
}