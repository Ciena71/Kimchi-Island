﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class ResourceManager
{
    public static ResourceManager instance;

    List<Dictionary<string, object>> productData;
    List<Dictionary<string, object>> categoryData;
    List<Dictionary<string, object>> supplyData;
    List<Dictionary<string, object>> popularityData;
    List<Dictionary<string, object>> demandShiftData;
    List<Dictionary<string, object>> statusData;
    List<Dictionary<string, object>> animalData;
    List<Dictionary<string, object>> sizeData;
    List<Dictionary<string, object>> weatherData;
    List<Dictionary<string, object>> textData;
    List<Dictionary<string, object>> supplyPattern;
    List<Dictionary<string, object>> updateVersionData;
    List<Dictionary<string, object>> materialData;
    List<Dictionary<string, object>> gatherNodeData;
    List<Dictionary<string, object>> urlData;

    List<Sprite> mapSprite = new List<Sprite>();

    const string API_URL = "http://worldtimeapi.org/api/ip";

    public ResourceManager()
    {
        instance = this;
        productData = CSVReader.Read("CSV/Product", true);
        categoryData = CSVReader.Read("CSV/Category", true);
        supplyData = CSVReader.Read("CSV/Supply", true);
        popularityData = CSVReader.Read("CSV/PopularityData", true);
        demandShiftData = CSVReader.Read("CSV/Demand Shift", true);
        statusData = CSVReader.Read("CSV/Status", true);
        animalData = CSVReader.Read("CSV/Animal", true);
        sizeData = CSVReader.Read("CSV/Size", true);
        weatherData = CSVReader.Read("CSV/Weather", true);
        textData = CSVReader.Read("CSV/Text", true);
        materialData = CSVReader.Read("CSV/Material", true);
        gatherNodeData = CSVReader.Read("CSV/GatherNode", true);
        urlData = CSVReader.Read("CSV/URL", true);
        for (int i = 0; i < 2; ++i)
            mapSprite.Add(Resources.Load<Sprite>($"Sprite/Map{i}"));
        UpdateVersion();
    }

    ~ResourceManager()
    {
        productData.Clear();
        productData = null;
        categoryData.Clear();
        categoryData = null;
        supplyData.Clear();
        supplyData = null;
        popularityData.Clear();
        popularityData = null;
        demandShiftData.Clear();
        demandShiftData = null;
        statusData.Clear();
        statusData = null;
        animalData.Clear();
        animalData = null;
        weatherData.Clear();
        weatherData = null;
        textData.Clear();
        textData = null;
        materialData.Clear();
        materialData = null;
        gatherNodeData.Clear();
        gatherNodeData = null;
        urlData.Clear();
        urlData = null;
    }

    public int GetProductMax() => productData.Count;

    public string GetProductName(int index)
    {
        if (index >= 0 && index < productData.Count)
            return productData[index][UserManager.instance.GetDataLanguage().ToString()].ToString();
        else
            return null;
    }

    public int GetProductTime(int index)
    {
        if (index >= 0 && index < productData.Count)
            return (int)productData[index]["Time"];
        else
            return -1;
    }

    public int GetProductQuantity(int index)
    {
        if (index >= 0 && index < productData.Count)
            return (int)productData[index]["Quantity"];
        else
            return -1;
    }

    public int GetProductValue(int index)
    {
        if (index >= 0 && index < productData.Count)
            return (int)productData[index]["Value"];
        else
            return -1;
    }

    public int GetProductSalesValue(int index)
    {
        if (index >= 0 && index < productData.Count)
        {
            int value = (int)materialData[(int)productData[index]["Material 1"]]["Value"] * (int)productData[index]["Material Required 1"];
            value += (int)materialData[(int)productData[index]["Material 2"]]["Value"] * (int)productData[index]["Material Required 2"];
            if (productData[index]["Material 3"].ToString().Length > 0)
                value += (int)materialData[(int)productData[index]["Material 3"]]["Value"] * (int)productData[index]["Material Required 3"];
            return value;
        }
        else
            return -1;
    }

    public int GetProductCategory(int index)
    {
        if (index >= 0 && index < productData.Count)
            return (int)productData[index]["Category"];
        else
            return -1;
    }

    public string GetProductCategoryName(int index)
    {
        int value = GetProductCategory(index);
        string data = null;
        for (int i = 0; i < categoryData.Count; ++i)
        {
            if ((value & (1 << i)) == (1 << i))
            {
                if (data != null)
                    data += ", ";
                data += GetCategoryName(i);
            }
        }
        return data;
    }

    public ProductMaterials GetProductMaterials(int product)
    {
        ProductMaterials mat = new ProductMaterials();
        if (int.TryParse(productData[product]["Material 3"].ToString(), out int index))
        {
            if (int.TryParse(productData[product]["Material 4"].ToString(), out int index1))
            {
                mat.index = new int[4];
                mat.quantity = new int[4];
                mat.index[2] = index;
                mat.quantity[2] = (int)productData[product]["Material Required 3"];
                mat.index[3] = index1;
                mat.quantity[3] = (int)productData[product]["Material Required 4"];
            }
            else
            {
                mat.index = new int[3];
                mat.quantity = new int[3];
                mat.index[2] = index;
                mat.quantity[2] = (int)productData[product]["Material Required 3"];
            }
        }
        else
        {
            mat.index = new int[2];
            mat.quantity = new int[2];
        }
        mat.index[0] = (int)productData[product]["Material 1"];
        mat.quantity[0] = (int)productData[product]["Material Required 1"];
        mat.index[1] = (int)productData[product]["Material 2"];
        mat.quantity[1] = (int)productData[product]["Material Required 2"];
        return mat;
    }

    public string GetCategoryName(int index)
    {
        if (index >= 0 && index < categoryData.Count)
            return categoryData[index][UserManager.instance.GetDataLanguage().ToString()].ToString();
        else
            return null;
    }

    public int GetProductRank(int index)
    {
        if (index >= 0 && index < productData.Count)
            return (int)productData[index]["Rank"];
        else
            return -1;
    }

    public string GetSupplyName(int index)
    {
        if (index >= 0 && index < supplyData.Count)
            return supplyData[index][UserManager.instance.GetLanguage().ToString()].ToString();
        else
            return null;
    }

    public string GetDemandShiftName(int index)
    {
        if (index >= 0 && index < demandShiftData.Count)
            return demandShiftData[index][UserManager.instance.GetLanguage().ToString()].ToString();
        else
            return null;
    }

    public string GetStatusName(int index)
    {
        if (index >= 0 && index < statusData.Count)
            return statusData[index][UserManager.instance.GetLanguage().ToString()].ToString();
        else
            return null;
    }

    public int GetAnimalMax() => animalData.Count;

    public void GetAnimalData(int index, out int time, out int weather, out int type, out Vector2 pos)
    {
        if (index >= 0 && index < animalData.Count)
        {
            time = (int)animalData[index]["Time"];
            weather = (int)animalData[index]["Weather"];
            type = (int)animalData[index]["Type"];
            pos = new Vector2(float.Parse(animalData[index]["X"].ToString(), CultureInfo.InvariantCulture), float.Parse(animalData[index]["Y"].ToString(), CultureInfo.InvariantCulture));
        }
        else
        {
            time = -1;
            weather = -1;
            type = -1;
            pos = Vector2.zero;
        }
    }

    public string GetAnimalName(int index) => animalData[index][UserManager.instance.GetDataLanguage().ToString()].ToString();

    public int GetAnimalNormalLeaving(int index) => (int)animalData[index]["Material 1"];

    public int GetAnimalRareLeaving(int index) => (int)animalData[index]["Material 2"];

    public string GetSize(int index) => sizeData[index][UserManager.instance.GetLanguage().ToString()].ToString();

    public string GetWeather(int index) => weatherData[index][UserManager.instance.GetDataLanguage().ToString()].ToString();

    public string GetText(int index) => textData[index][UserManager.instance.GetLanguage().ToString()].ToString();

    public byte GetStatusData(int line, int index) => byte.Parse(popularityData[line][$"{index + 1}"].ToString());

    public string GetMaterialName(int index) => materialData[index][UserManager.instance.GetDataLanguage().ToString()].ToString();

    public int GetMaterialCategory(int index) => (int)materialData[index]["Category"];

    public int GetMaterialMax() => materialData.Count;

    public void GetGatherNodeItems(int index, out int item1, out int item2, out int item3)
    {
        item1 = (int)gatherNodeData[index]["Item1"];
        if (gatherNodeData[index]["Item2"].ToString().Length > 0)
            item2 = (int)gatherNodeData[index]["Item2"];
        else
            item2 = -1;
        if (gatherNodeData[index]["Item3"].ToString().Length > 0)
            item3 = (int)gatherNodeData[index]["Item3"];
        else
            item3 = -1;
    }

    public bool IsGatherNodeGround(int index)
    {
        if ((int)gatherNodeData[index]["Ground"] == 1)
            return true;
        else
            return false;
    }

    public string GetGatherNodeName(int index) => gatherNodeData[index][UserManager.instance.GetDataLanguage().ToString()].ToString();

    public string GetURL(int index) => urlData[index]["URL"].ToString();

    public Sprite GetMapSprite(int index) => mapSprite[index];

    IEnumerator RequestSheetData()
    {
        supplyPattern?.Clear();
        UnityWebRequest data = UnityWebRequest.Get(GetURL(0));
        yield return data.SendWebRequest();
        if (data.result == UnityWebRequest.Result.ConnectionError)
            yield break;
        string file = Regex.Replace(data.downloadHandler.text, "📦", "");
        int removeSize = file.IndexOf("ID,Item");
        string[] value = Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(file.Remove(0, removeSize), @"\bNonexistent\b", "0"), @"\bInsufficient\b", "1"), @"\bSufficient\b", "2"), @"\bUnpredictable\b", "-1"), @"\bPeak\b", "-2"), @"\bHere Or Cycle 4\b", "-3"), @"\bHere Or Cycle 5\b", "-3"), @"\bHere Or Cycle 6\b", "-4"), @"\bHere Or Cycle 7\b", "-4"), @"\bHere Or 6 Or 7\b", "-5"), @"\bHere Or 3 Or 7\b", "-5"), @"\bHere Or 3 Or 6\b", "-5").Split('\n');
        string result = "ID,Item,Supply1,Shift1,Supply2,Shift2,Supply3,Shift3,Supply4,Shift4,Prediction2,Prediction3,Prediction4,Prediction5,Prediction6,Prediction7,ItemP,Pattern,Last Week Pattern,Split Pattern1,Split Pattern2,Popularity,Next Popularity,Patterns identified1,Patterns identified2";
        for (int i = 0; i < GetProductMax(); ++i)
        {
            for (int j = 1; j < value.Length; ++j)
            {
                if (value[j].Substring(0, (i < 9 ? 2 : 3)) == $"{i + 1},")
                {
                    result += $"\n{value[j]}";
                    break;
                }
            }
        }
        supplyPattern = CSVReader.Read(result, false);
        UserManager.instance.UpdateWeek();
        Workshop.instance?.UpdatePeakData();
        WorkshopSchedule.instance?.UpdateSupplyPattern();
    }

    public void UpdateSupplyPattern() => SystemCore.instance.StartCoroutine(RequestSheetData());

    IEnumerator RequestUpdateVersion()
    {
        updateVersionData?.Clear();
        UnityWebRequest data = UnityWebRequest.Get(GetURL(1));
        yield return data.SendWebRequest();
        if (data.result == UnityWebRequest.Result.ConnectionError)
            yield break;
        updateVersionData = CSVReader.Read(data.downloadHandler.text, false);
        int lastIndex = updateVersionData.Count - 1;
        string[] newVersion = updateVersionData[lastIndex]["Version"].ToString().Split('.');
        string[] oldVersion = Application.version.Split('.');
        for (int i = 0; i < newVersion.Length; ++i)
        {
            if (oldVersion.Length > i)
            {
                if (int.Parse(newVersion[i]) < int.Parse(oldVersion[i]))
                    break;
                else if(int.Parse(newVersion[i]) > int.Parse(oldVersion[i]))
                {
                    VersionUpdate.instance.SetData(updateVersionData[lastIndex]["Version"].ToString(), updateVersionData[lastIndex]["URL"].ToString());
                    break;
                }
            }
            else
            {
                VersionUpdate.instance.SetData(updateVersionData[lastIndex]["Version"].ToString(), updateVersionData[lastIndex]["URL"].ToString());
                break;
            }
        }
    }

    public void UpdateVersion() => SystemCore.instance.StartCoroutine(RequestUpdateVersion());

    public string GetPatchNote()
    {
        string textData = "";
        UserManager userManager = UserManager.instance;
        for (int i = updateVersionData.Count - 1; i >= 0; --i)
            textData += $"Version {updateVersionData[i]["Version"]}\n{updateVersionData[i][$"PatchNote {(userManager.GetLanguage() == SystemLanguage.Korean ? "Korean" : "English")}"]}\n\n";
        return textData;
    }

    public List<Dictionary<string, object>> GetSupplyPattern() => supplyPattern;

    public void GetRealDate(byte[] data, Action<byte[], DateTime> action) => SystemCore.instance.StartCoroutine(GetRealDateTimeFromAPI(data, action));

    IEnumerator GetRealDateTimeFromAPI(byte[] form, Action<byte[], DateTime> action)
    {
        UnityWebRequest data = UnityWebRequest.Get(API_URL);
        yield return data.SendWebRequest();
        if (data.result == UnityWebRequest.Result.ConnectionError)
            yield break;
        TimeData timeData = JsonUtility.FromJson<TimeData>(data.downloadHandler.text);
        action.Invoke(form, ParseDateTime(timeData.utc_datetime));
    }

    DateTime ParseDateTime(string datetime)
    {
        //match 0000-00-00
        string date = Regex.Match(datetime, @"^\d{4}-\d{2}-\d{2}").Value;

        //match 00:00:00
        string time = Regex.Match(datetime, @"\d{2}:\d{2}:\d{2}").Value;

        return DateTime.Parse(string.Format("{0} {1}", date, time));
    }
}

public class TimeData
{
    public string abbreviation;
    public string client_ip;
    public string datetime;
    public int day_of_week;
    public int day_of_year;
    public bool dst;
    public string dst_from;
    public int dst_offset;
    public string dst_until;
    public int raw_offset;
    public string timezone;
    public int unixtime;
    public string utc_datetime;
    public string utc_offset;
    public int week_number;
}