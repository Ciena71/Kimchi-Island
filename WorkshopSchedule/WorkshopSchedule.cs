using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkshopSchedule : MonoBehaviour
{
    public static WorkshopSchedule instance;

    public Text textProductName;
    public Button btnProductName;
    public Text textDay1;
    public Button btnDay1;
    public Text textDay2;
    public Button btnDay2;
    public Text textDay3;
    public Button btnDay3;
    public Text textDay4;
    public Button btnDay4;
    public Text textDay5;
    public Button btnDay5;
    public Text textDay6;
    public Button btnDay6;
    public Text textDay7;
    public Button btnDay7;
    public ScrollRect scrollProductList;

    bool day1;
    bool[] reverse = new bool[7];
    int sort = 0;
    int highlight = -1;

    List<ScheduleProduct> scheduleProductList = new List<ScheduleProduct>();

    void Awake()
    {
        instance = this;
        ResourceManager resourceManager = ResourceManager.instance;
        UserManager userManager = UserManager.instance;
        for (int i = 0; i < resourceManager.GetProductMax(); ++i)
        {
            GameObject productObject = Instantiate(Resources.Load("Prefab/WorkshopSchedule/ScheduleProduct"), scrollProductList.content) as GameObject;
            ScheduleProduct product = productObject.GetComponent<ScheduleProduct>();
            product.SetDefaultData(i);
            scheduleProductList.Add(product);
        }
        btnProductName.onClick.AddListener(() =>
        {
            if (sort == 0)
                reverse[0] = !reverse[0];
            else
                sort = 0;
            int index = 0;
            if (reverse[0])
            {
                for (int i = scheduleProductList.Count - 1; i >= 0; --i)
                {
                    scheduleProductList[i].transform.SetSiblingIndex(index);
                    ++index;
                }
            }
            else
            {
                scheduleProductList.ForEach((form) =>
                {
                    form.transform.SetSiblingIndex(index);
                    ++index;
                });
            }
        });
        btnDay1.onClick.AddListener(() =>
        {
            day1 = !day1;
            scheduleProductList.ForEach((form) =>
            {
                form.SwapDay1(day1);
            });
        });
        btnDay2.onClick.AddListener(() =>
        {
            if (sort == 1)
                reverse[1] = !reverse[1];
            else
                sort = 1;
            int index = 0;
            if (reverse[1])
            {
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(1) == 2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(1) == -1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(1) == 1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(1) == -2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(1) == 0)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
            }
            else
            {
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(1) == 0)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(1) == -2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(1) == 1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(1) == -1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(1) == 2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
            }
        });
        btnDay3.onClick.AddListener(() =>
        {
            if (sort == 2)
                reverse[2] = !reverse[2];
            else
                sort = 2;
            int index = 0;
            if (reverse[2])
            {
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(2) == 2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(2) == -1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(2) == 1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(2) == -2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(2) == 0)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
            }
            else
            {
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(2) == 0)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(2) == -2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(2) == 1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(2) == -1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(2) == 2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
            }
        });
        btnDay4.onClick.AddListener(() =>
        {
            if (sort == 3)
                reverse[3] = !reverse[3];
            else
                sort = 3;
            int index = 0;
            if (reverse[3])
            {
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(3) == 2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(3) == -1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(3) == 1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(3) == -3)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(3) == -2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(3) == 0)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
            }
            else
            {
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(3) == 0)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(3) == -2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(3) == -3)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(3) == 1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(3) == -1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(3) == 2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
            }
        });
        btnDay5.onClick.AddListener(() =>
        {
            if (sort == 4)
                reverse[4] = !reverse[4];
            else
                sort = 4;
            int index = 0;
            if (reverse[4])
            {
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(4) == 2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(4) == -1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(4) == 1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(4) == -3)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(4) == -2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(4) == 0)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
            }
            else
            {
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(4) == 0)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(4) == -2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(4) == -3)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(4) == 1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(4) == -1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(4) == 2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
            }
        });
        btnDay6.onClick.AddListener(() =>
        {
            if (sort == 5)
                reverse[5] = !reverse[5];
            else
                sort = 5;
            int index = 0;
            if (reverse[5])
            {
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(5) == 2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(5) == -1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(5) == 1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(5) == -4)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(5) == -2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(5) == 0)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
            }
            else
            {
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(5) == 0)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(5) == -2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(5) == -4)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(5) == 1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(5) == -1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(5) == 2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
            }
        });
        btnDay7.onClick.AddListener(() =>
        {
            if (sort == 6)
                reverse[6] = !reverse[6];
            else
                sort = 6;
            int index = 0;
            if (reverse[6])
            {
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(6) == 2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(6) == -1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(6) == 1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(6) == -4)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(6) == -2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(6) == 0)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
            }
            else
            {
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(6) == 0)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(6) == -2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(6) == -4)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(6) == 1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(6) == -1)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
                scheduleProductList.ForEach((form) =>
                {
                    if (form.GetSupply(6) == 2)
                    {
                        form.transform.SetSiblingIndex(index);
                        ++index;
                    }
                });
            }
        });
        ApplyLanguage();
    }

    private void OnEnable()
    {
        ResourceManager.instance.UpdateSupplyPattern();
    }

    public void UpdateSupplyPattern()
    {
        scheduleProductList.ForEach((form) =>
        {
            form.UpdateSchedule();
            form.ApplyLanguage();
        });
    }

    public void ApplyLanguage()
    {
        ResourceManager resourceManager = ResourceManager.instance;
        textProductName.text = resourceManager.GetText(7);
        textDay1.text = $"{resourceManager.GetText(35).Replace("{0}", "1")} / {resourceManager.GetText(11)}";
        textDay2.text = resourceManager.GetText(35).Replace("{0}", "2");
        textDay3.text = resourceManager.GetText(35).Replace("{0}", "3");
        textDay4.text = resourceManager.GetText(35).Replace("{0}", "4");
        textDay5.text = resourceManager.GetText(35).Replace("{0}", "5");
        textDay6.text = resourceManager.GetText(35).Replace("{0}", "6");
        textDay7.text = resourceManager.GetText(35).Replace("{0}", "7");
        scheduleProductList.ForEach((form) =>
        {
            form.ApplyLanguage();
        });
    }

    public bool GetDay1() => day1;

    public void SetHighlight(int index)
    {
        if (index != highlight)
            highlight = index;
        else
            highlight = -1;
        scheduleProductList.ForEach((form) =>
        {
            form.SetHighlight(highlight);
        });
    }

    public void SetDay1()
    {
        scheduleProductList.ForEach((form) =>
        {
            form.SwapDay1(day1);
        });
    }
}
