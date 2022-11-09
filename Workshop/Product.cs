using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Product : MonoBehaviour
{
    public Toggle toggleProductActive;
    public Image imgProductIcon;
    public Text textProductName;
    public Text textProductTime;
    public Text textProductQuantity;
    public Text textProductValue;
    public Image imgProductPopularity;
    public Text textProductPopularity;
    public Image imgProductSupply;
    public EventTrigger triggerProductSupply;
    public InputField inputProductSupply;
    public Text textProductSupply;
    public GameObject[] objProductSupply = new GameObject[4];
    public Text textProductCategory;

    int index;
    bool active;
    int time;
    int value;
    int quantity;
    int category;
    int rank;
    int[] pattern = { 0, 0 };
    byte[] popularity = { 3, 3, 3 };
    int[] supply = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    byte demandShift;
    int count;

    public void SetDefaultData(int _index)
    {
        index = _index;
        ResourceManager resourceManager = ResourceManager.instance;
        imgProductIcon.sprite = Resources.Load<Sprite>($"Sprite/Product/{index}");
        textProductName.text = resourceManager.GetProductName(index);
        time = resourceManager.GetProductTime(index);
        textProductTime.text = time.ToString();
        quantity = resourceManager.GetProductQuantity(index);
        textProductQuantity.text = quantity.ToString();
        value = resourceManager.GetProductValue(index);
        textProductValue.text = value.ToString();
        category = resourceManager.GetProductCategory(index);
        textProductCategory.text = resourceManager.GetProductCategoryName(index);
        rank = resourceManager.GetProductRank(index);
        UserManager userManager = UserManager.instance;
        active = userManager.GetProductBlacklist(index);
        toggleProductActive.isOn = active;
        toggleProductActive.onValueChanged.AddListener((_active) =>
        {
            active = _active;
            userManager.SetProductBlacklist(index, active);
        });
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => {
            inputProductSupply.textComponent.gameObject.SetActive(true);
            textProductSupply.gameObject.SetActive(false);
        });
        triggerProductSupply.triggers.Add(entry);
        inputProductSupply.onEndEdit.AddListener((stringvalue) =>
        {
            int value = int.Parse(stringvalue);
            int cycle = Workshop.instance.GetCycle();
            supply[cycle] = value;
            int sup = GetSupply(cycle);
            textProductSupply.text = resourceManager.GetSupplyName(sup - 1);
            for (int i = 0; i < 4; ++i)
            {
                if (i + 2 <= sup)
                    objProductSupply[i].SetActive(true);
                else
                    objProductSupply[i].SetActive(false);
            }
            inputProductSupply.textComponent.gameObject.SetActive(false);
            textProductSupply.gameObject.SetActive(true);
        });
    }
    /*Discarded
    public void SetPacketData(byte pop1, byte pop2, byte value, int cycle)
    {
        ResourceManager resourceManager = ResourceManager.instance;
        if ((value & (1 << 6)) == (1 << 6))
            supply[0] = 5;
        else if ((value & (1 << 5) + (1 << 4)) == (1 << 5) + (1 << 4))
            supply[0] = 4;
        else if ((value & (1 << 5)) == (1 << 5))
            supply[0] = 3;
        else if ((value & (1 << 4)) == (1 << 4))
            supply[0] = 2;
        else
            supply[0] = 1;
        popularity[0] = resourceManager.GetStatusData(pop1, index);
        popularity[1] = resourceManager.GetStatusData(pop2, index);
        if ((value & (1 << 2)) == (1 << 2))
            demandShift = 5;
        else if ((value & (1 << 1) + (1 << 0)) == (1 << 1) + (1 << 0))
            demandShift = 4;
        else if ((value & (1 << 1)) == (1 << 1))
            demandShift = 3;
        else if ((value & (1 << 0)) == (1 << 0))
            demandShift = 2;
        else
            demandShift = 1;
        textProductPopularity.text = resourceManager.GetStatusName(GetPopularity(cycle));
        imgProductPopularity.sprite = Resources.Load<Sprite>($"Sprite/Status/{GetPopularity(cycle)}");
    }*/

    public void SetActive(bool check) => active = check;
    public bool IsActive() => (UserManager.instance.GetPlayerRank() >= rank ? active : false);
    public int GetTime() => time;
    public int GetValue() => value;
    public int GetQuantity() => quantity;
    public int GetCategory() => category;
    public byte GetPopularity(int cycle)
    {
        switch (cycle)
        {
        case 0:
        case 1:
        case 2:
        case 3:
        case 4:
        case 5:
        case 6: return popularity[0];
        case 7: return popularity[1];
        case 8: return popularity[2];
        default: return 0;
        }
    }
    public int GetSupply(int cycle)
    {
        if (supply[cycle] + count >= 16)
            return 5;
        if (supply[cycle] + count >= 8)
            return 4;
        if (supply[cycle] + count >= 0)
            return 3;
        if (supply[cycle] + count >= -8)
            return 2;
        return 1;
    }

    public int GetSupplyValue(int cycle) => supply[cycle];

    public byte GetDemandShift() => demandShift;

    public void ApplyLanguage(int cycle)
    {
        ResourceManager resourceManager = ResourceManager.instance;
        textProductName.text = resourceManager.GetProductName(index);
        textProductCategory.text = resourceManager.GetProductCategoryName(index);
        textProductSupply.text = resourceManager.GetSupplyName(GetSupply(Workshop.instance.GetCycle()) - 1);
        textProductPopularity.text = resourceManager.GetStatusName(GetPopularity(cycle));
    }

    public void SetCycle(int cycle)
    {
        ResourceManager resourceManager = ResourceManager.instance;
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
                textProductPopularity.text = resourceManager.GetStatusName(popularity[0]);
                imgProductPopularity.sprite = Resources.Load<Sprite>($"Sprite/Status/{popularity[0]}");
                break;
            }
        case 7:
            {
                textProductPopularity.text = resourceManager.GetStatusName(popularity[1]);
                imgProductPopularity.sprite = Resources.Load<Sprite>($"Sprite/Status/{popularity[1]}");
                break;
            }
        case 8:
            {
                textProductPopularity.text = resourceManager.GetStatusName(popularity[2]);
                imgProductPopularity.sprite = Resources.Load<Sprite>($"Sprite/Status/{popularity[2]}");
                break;
            }
        }
        ShowSupply();
        ShowPeak();
    }
    
    public void ShowSupply()
    {
        int cycle = Workshop.instance.GetCycle();
        inputProductSupply.text = supply[cycle].ToString();
        textProductSupply.text = ResourceManager.instance.GetSupplyName(GetSupply(cycle) - 1);
        if (supply[cycle] >= 16)
            objProductSupply[3].SetActive(true);
        else
            objProductSupply[3].SetActive(false);
        if (supply[cycle] >= 8)
            objProductSupply[2].SetActive(true);
        else
            objProductSupply[2].SetActive(false);
        if (supply[cycle] >= 0)
            objProductSupply[1].SetActive(true);
        else
            objProductSupply[1].SetActive(false);
        if (supply[cycle] >= -8)
            objProductSupply[0].SetActive(true);
        else
            objProductSupply[0].SetActive(false);
    }
    
    public void SetPeak(int _day, int _peak, int _popularity, int _nextPopularity)
    {
        pattern[0] = _day;
        pattern[1] = _peak;
        popularity[0] = (byte)_popularity;
        popularity[1] = (byte)_nextPopularity;
        if (_day <= 7)
        {
            switch (_day)
            {
            case 2:
                {
                    if (_peak == 2 || _peak == 0)
                        supply = new int[] { -4, -8, 2, 2, 2, 2, 2, supply[7], supply[8] };
                    else
                        supply = new int[] { -8, -15, 0, 0, 0, 0, 0, supply[7], supply[8] };
                    break;
                }
            case 3:
                {
                    if (_peak == 2 || _peak == 0)
                        supply = new int[] { 0, -4, -8, 2, 2, 2, 2, supply[7], supply[8] };
                    else
                        supply = new int[] { 0, -8, -15, 0, 0, 0, 0, supply[7], supply[8] };
                    break;
                }
            case 4:
                {
                    if (_peak == 2)
                        supply = new int[] { 0, 0, -4, -8, 2, 2, 2, supply[7], supply[8] };
                    else if (_peak == 0)
                        supply = new int[] { 0, 0, 0, -8, -8, 2, 2, supply[7], supply[8] };
                    else
                        supply = new int[] { 0, 0, -8, -15, 0, 0, 0, supply[7], supply[8] };
                    break;
                }
            case 5:
                {
                    if (_peak == 2)
                        supply = new int[] { 0, 0, 0, -4, -8, 2, 2, supply[7], supply[8] };
                    else if (_peak == 0)
                        supply = new int[] { 0, 0, 0, -4, -8, 2, 2, supply[7], supply[8] };
                    else
                        supply = new int[] { 0, 0, 0, -8, -15, 0, 0, supply[7], supply[8] };
                    break;
                }
            case 6:
                {
                    if (_peak == 2)
                        supply = new int[] { 0, -1, 4, 0, -4, -8, 2, supply[7], supply[8] };
                    else if (_peak == 0)
                        supply = new int[] { 0, -1, 4, 0, 0, -8, -8, supply[7], supply[8] };
                    else
                        supply = new int[] { 0, -1, 7, 0, -8, -15, 0, supply[7], supply[8] };
                    break;
                }
            case 7:
                {
                    if (_peak == 2)
                        supply = new int[] { 0, -1, 7, 4, 0, -4, -8, supply[7], supply[8] };
                    else if (_peak == 0)
                        supply = new int[] { 0, -1, 4, 0, 0, -8, -8, supply[7], supply[8] };
                    else
                        supply = new int[] { 0, -1, 7, 7, 0, -8, -15, supply[7], supply[8] };
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < 7; ++i)
                supply[i] = 0;
        }
        SetCycle(Workshop.instance.GetCycle());
    }

    public void ShowPeak()
    {
        int cycle = Workshop.instance.GetCycle();
        if (cycle >= 7 || pattern[0] == 0)
        {
            imgProductSupply.color = new Color(220 / 255.0f, 220 / 255.0f, 220 / 255.0f);
            return;
        }
        if (pattern[0] > cycle + 1)
            imgProductSupply.color = new Color(220 / 255.0f, 220 / 255.0f, 220 / 255.0f);
        else if (pattern[0] == cycle + 1)
        {
            switch (pattern[1])
            {
            case 0:
                {
                    imgProductSupply.color = new Color(201 / 255.0f, 218 / 255.0f, 250 / 255.0f);
                    break;
                }
            case 1:
                {
                    imgProductSupply.color = new Color(244 / 255.0f, 204 / 255.0f, 204 / 255.0f);
                    break;
                }
            case 2:
                {
                    imgProductSupply.color = new Color(255 / 255.0f, 252 / 255.0f, 204 / 255.0f);
                    break;
                }
            }
        }
        else
        {
            if (pattern[1] == 0 && pattern[0] == cycle)
                imgProductSupply.color = new Color(201 / 255.0f, 218 / 255.0f, 250 / 255.0f);
            else
                imgProductSupply.color = new Color(217 / 255.0f, 234 / 255.0f, 211 / 255.0f);
        }
    }

    public void AddCount(int value) => count += value;

    public void ResetCount() => count = 0;

    public int GetIndex() => index;

    public void AddSupply(int cycle, int value) => supply[cycle] += value;
}
