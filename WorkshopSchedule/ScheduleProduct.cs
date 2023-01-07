using UnityEngine;
using UnityEngine.UI;

public class ScheduleProduct : MonoBehaviour
{
    public Button btnProduct;
    public Image imgProductBG;
    public Image imgProductIcon;
    public Text textProductName;
    public Image imgDay1;
    Image[] imgDay = new Image[7];
    Text[] textDay = new Text[7];
    GameObject[,] objDaySup = new GameObject[7, 2];
    int[] supply = new int[7];

    int index;

    private void Awake()
    {
        for (int i = 0; i < 7; ++i)
        {
            imgDay[i] = transform.GetChild(1 + i).GetComponent<Image>();
            textDay[i] = transform.GetChild(1 + i).GetChild(2).GetComponent<Text>();
            objDaySup[i, 0] = transform.GetChild(1 + i).GetChild(1).gameObject;
            objDaySup[i, 1] = transform.GetChild(1 + i).GetChild(0).gameObject;
        }
        btnProduct.onClick.AddListener(() =>
        {
            WorkshopSchedule.instance.SetHighlight(index);
        });
    }

    public void SetDefaultData(int _index)
    {
        index = _index;
        ResourceManager resourceManager = ResourceManager.instance;
        imgProductIcon.sprite = Resources.Load<Sprite>($"Sprite/Product/{index}");
        textProductName.text = resourceManager.GetProductName(index);
    }

    public void UpdateSchedule()
    {
        ResourceManager resourceManager = ResourceManager.instance;
        supply[0] = resourceManager.GetSupplyPattern()[index]["Supply1"].ToString().Length > 0 ? (int)resourceManager.GetSupplyPattern()[index]["Supply1"] : 0;
        supply[1] = resourceManager.GetSupplyPattern()[index]["Supply2"].ToString().Length > 0 ? (int)resourceManager.GetSupplyPattern()[index]["Supply2"] : (int)resourceManager.GetSupplyPattern()[index]["Prediction2"];
        supply[2] = resourceManager.GetSupplyPattern()[index]["Supply3"].ToString().Length > 0 ? (int)resourceManager.GetSupplyPattern()[index]["Supply3"] : (int)resourceManager.GetSupplyPattern()[index]["Prediction3"];
        supply[3] = resourceManager.GetSupplyPattern()[index]["Supply4"].ToString().Length > 0 ? (int)resourceManager.GetSupplyPattern()[index]["Supply4"] : (int)resourceManager.GetSupplyPattern()[index]["Prediction4"];
        supply[4] = (int)resourceManager.GetSupplyPattern()[index]["Prediction5"];
        supply[5] = (int)resourceManager.GetSupplyPattern()[index]["Prediction6"];
        supply[6] = (int)resourceManager.GetSupplyPattern()[index]["Prediction7"];
        string[] pattern = resourceManager.GetSupplyPattern()[index]["Pattern"].ToString().Split(' ');
        int peakDay;
        if (pattern.Length == 2)
        {
            if (!int.TryParse(pattern[0], out peakDay))
            {
                if (pattern[1] == "4/5")
                    peakDay = -3;
                else if (pattern[1] == "6/7")
                    peakDay = -4;
                else
                    peakDay = -2;
            }
        }
        else
            peakDay = -1;
        bool checker = WorkshopSchedule.instance.GetDay1();
        for (int i = 0; i < 7; ++i)
        {
            if (i != 0 || !checker)
            {
                if (supply[i] >= 2)
                    objDaySup[i, 1].SetActive(true);
                else
                    objDaySup[i, 1].SetActive(false);
                if (supply[i] >= 1)
                    objDaySup[i, 0].SetActive(true);
                else
                    objDaySup[i, 0].SetActive(false);
                if (peakDay > 0)
                {
                    if (i + 1 == peakDay)
                    {
                        if (pattern[1] == "-2")
                            imgDay[i].color = new Color(201 / 255.0f, 218 / 255.0f, 250 / 255.0f);
                        else if (supply[i] == 0)
                            imgDay[i].color = new Color(244 / 255.0f, 204 / 255.0f, 204 / 255.0f);
                        else
                            imgDay[i].color = new Color(255 / 255.0f, 252 / 255.0f, 204 / 255.0f);
                    }
                    else
                    {
                        if (i + 1 < peakDay)
                            imgDay[i].color = new Color(220 / 255.0f, 220 / 255.0f, 220 / 255.0f);
                        else
                            imgDay[i].color = new Color(217 / 255.0f, 234 / 255.0f, 211 / 255.0f);
                    }
                }
                else
                {
                    if (peakDay == -1)
                        imgDay[i].color = new Color(220 / 255.0f, 220 / 255.0f, 220 / 255.0f);
                    else if (peakDay == -2)
                    {
                        if (i < 4)
                            imgDay[i].color = new Color(220 / 255.0f, 220 / 255.0f, 220 / 255.0f);
                        else if (i == 4)
                            imgDay[i].color = new Color(201 / 255.0f, 218 / 255.0f, 250 / 255.0f);
                        else
                            imgDay[i].color = new Color(217 / 255.0f, 234 / 255.0f, 211 / 255.0f);
                    }
                    else if (peakDay == -3)
                    {
                        if (i < 3)
                            imgDay[i].color = new Color(220 / 255.0f, 220 / 255.0f, 220 / 255.0f);
                        else if (3 <= i && i <= 4)
                            imgDay[i].color = new Color(201 / 255.0f, 218 / 255.0f, 250 / 255.0f);
                        else
                            imgDay[i].color = new Color(217 / 255.0f, 234 / 255.0f, 211 / 255.0f);
                    }
                    else if (peakDay == -4)
                    {
                        if (i < 5)
                            imgDay[i].color = new Color(220 / 255.0f, 220 / 255.0f, 220 / 255.0f);
                        else if (5 <= i && i <= 6)
                            imgDay[i].color = new Color(201 / 255.0f, 218 / 255.0f, 250 / 255.0f);
                        else
                            imgDay[i].color = new Color(217 / 255.0f, 234 / 255.0f, 211 / 255.0f);
                    }
                }
            }
        }
    }

    public void ApplyLanguage()
    {
        ResourceManager resourceManager = ResourceManager.instance;
        textProductName.text = resourceManager.GetProductName(index);
        bool checker = WorkshopSchedule.instance.GetDay1();
        if (checker)
            textDay[0].text = resourceManager.GetStatusName(Workshop.instance.GetProductList()[index].GetPopularity(0));
        for (int i = 0; i < 7; ++i)
        {
            if (i != 0 || !checker)
                textDay[i].text = resourceManager.GetSupplyName(supply[i] >= 0 ? supply[i] : (supply[i] == -1 ? 5 : (supply[i] == -2 ? 6 : (supply[i] == -3 ? 7 : 8))));
        }
    }

    public void SwapDay1(bool check)
    {
        if (check)
        {
            ResourceManager resourceManager = ResourceManager.instance;
            int status = Workshop.instance.GetProductList()[index].GetPopularity(0);
            textDay[0].text = resourceManager.GetStatusName(status);
            imgDay1.rectTransform.sizeDelta = new Vector2(50, 50);
            objDaySup[0, 1].SetActive(false);
            if (status != 0)
            {
                imgDay1.sprite = Resources.Load<Sprite>($"Sprite/Status/{status}");
                objDaySup[0, 0].SetActive(true);
            }
            else
                objDaySup[0, 0].SetActive(false);
        }
        else
        {
            ResourceManager resourceManager = ResourceManager.instance;
            textDay[0].text = resourceManager.GetSupplyName(supply[0] >= 0 ? supply[0] : (supply[0] == -1 ? 5 : (supply[0] == -2 ? 6 : (supply[0] == -3 ? 7 : 8))));
            imgDay1.rectTransform.sizeDelta = new Vector2(30, 30);
            imgDay1.sprite = Resources.Load<Sprite>("Sprite/Status/Supply");
            if (supply[0] >= 2)
                objDaySup[0, 1].SetActive(true);
            else
                objDaySup[0, 1].SetActive(false);
            if (supply[0] >= 1)
                objDaySup[0, 0].SetActive(true);
            else
                objDaySup[0, 0].SetActive(false);
        }
    }

    public int GetSupply(int day) => supply[day];

    public void SetHighlight(int _index)
    {
        if (_index == -1)
            imgProductBG.color = Color.white;
        else
        {
            if (index == _index)
                imgProductBG.color = new Color(217 / 255.0f, 234 / 255.0f, 211 / 255.0f);
            else
            {
                ResourceManager resourceManager = ResourceManager.instance;
                if ((resourceManager.GetProductCategory(index) & resourceManager.GetProductCategory(_index)) > 0)
                    imgProductBG.color = new Color(244 / 255.0f, 204 / 255.0f, 204 / 255.0f);
                else
                    imgProductBG.color = Color.white;
            }
        }
    }
}
