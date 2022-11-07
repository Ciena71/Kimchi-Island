using System;
using System.IO;
using UnityEngine;

public class UserManager
{
    public static UserManager instance;

    string GetSaveFilePath() => Path.Combine(Application.persistentDataPath, "UserSave.hxd");

    UserData userData;

    public UserManager()
    {
        instance = this;
        LoadData();
        SystemCore systemCore = SystemCore.instance;
        systemCore.ApplyLanguage();
        systemCore.AssignTopmostWindow(Application.productName, userData.alwaysOnTop);
        UpdateWeek();
    }

    ~UserManager()
    {
        instance = null;
    }

    public SystemLanguage GetLanguage() => userData.language;
    public void SetLanguage(SystemLanguage _language)
    {
        userData.language = _language;
        SystemCore.instance.ApplyLanguage();
        Workshop.instance.ApplyLanguage();
        WorkshopSchedule.instance.ApplyLanguage();
        Animal.instance.ApplyLanguage();
        Gather.instance.ApplyLanguage();
        Inventory.instance.ApplyLanguage();
        Option.instance.ApplyLanguage();
        Eorzea.instance.ApplyLanguage();
        VersionUpdate.instance.ApplyLanguage();
        SaveData();
    }

    public SystemLanguage GetDataLanguage() => userData.dataLanguage;
    public void SetDataLanguage(SystemLanguage _language)
    {
        userData.dataLanguage = _language;
        Workshop.instance.ApplyLanguage();
        Animal.instance.ApplyLanguage();
        Eorzea.instance.ApplyLanguage();
        Gather.instance.ApplyLanguage();
        SaveData();
    }

    public int GetPacketType() => userData.packet;
    public void SetPacketType(int type)
    {
        Eorzea eorzea = Eorzea.instance;
        if (userData.packet > 0)
            eorzea.StopPacketCapture();
        userData.packet = type;
        eorzea.SettingPacketCapture();
        SaveData();
    }

    public bool GetAlwaysOnTop() => userData.alwaysOnTop;
    public void SetAlwaysOnTop(bool type)
    {
        userData.alwaysOnTop = type;
        SystemCore.instance.AssignTopmostWindow(Application.productName, type);
        SaveData();
    }

    public int GetAlarmVolume() => 100 - userData.alarmVolume;
    public void SetAlarmVolume(int value)
    {
        userData.alarmVolume = 100 - value;
        SystemCore.instance.SetAlarmVolume(value);
        SaveData();
    }

    public int GetAlarm() => userData.alarm;
    public void SetAlarm(int value)
    {
        userData.alarm = value;
        SystemCore.instance.SetAlarm(value);
        SaveData();
    }

    public int GetPlayerRank() => userData.rank;
    public void SetPlayerRank(int rank)
    {
        userData.rank = rank;
        SaveData();
    }

    public int GetCurrentGroove() => userData.grooveCurrent;
    public void SetCurrentGroove(int groove)
    {
        userData.grooveCurrent = groove;
        SaveData();
    }

    public int GetMaxGroove() => userData.grooveMax;
    public void SetMaxGroove(int groove)
    {
        userData.grooveMax = groove;
        SaveData();
    }

    public int GetWorkshopTier(int index) => userData.workshopTier[index];
    public void SetWorkshopTier(int index, int value)
    {
        userData.workshopTier[index] = value;
        SaveData();
    }

    public bool GetGroovePriority() => userData.groovePriority;
    public void SetGroovePriority(bool value)
    {
        userData.groovePriority = value;
        SaveData();
    }

    public bool GetProductBlacklist(int index) => ((userData.productBlacklist & (1 << index)) == 0 ? true : false);
    public void SetProductBlacklist(int index, bool value)
    {
        if(value != GetProductBlacklist(index))
        {
            if (value)
                userData.productBlacklist -= (1 << index);
            else
                userData.productBlacklist += (1 << index);
            SaveData();
        }
    }

    public SalesData GetSalesData(int cycle) => userData.salesData[cycle];
    public void SetSalesData(int cycle, SalesData _salesData)
    {
        userData.salesData[cycle] = _salesData;
        SaveData();
    }

    public bool GetAnimalAlarmlist(int index) => ((userData.animalAlarmList & (1 << index)) == 0 ? false : true);
    public void SetAnimalAlarmlist(int index, bool value)
    {
        if (value != GetAnimalAlarmlist(index))
        {
            if (value)
                userData.animalAlarmList += (1 << index);
            else
                userData.animalAlarmList -= (1 << index);
            SaveData();
        }
    }

    public int GetInventory(int index) => userData.inventory[index];
    public void SetInventory(int index, int value)
    {
        if(userData.inventory[index] != value)
        {
            userData.inventory[index] = value;
            SaveData();
        }
    }

    public string GetGamePath() => userData.gamePath;
    public void SetGamePath(string path)
    {
        if (path.Length == 0)
            return;
        Eorzea eorzea = Eorzea.instance;
        if (userData.packet > 0)
            eorzea.StopPacketCapture();
        userData.gamePath = path;
        eorzea.SettingPacketCapture();
        SaveData();
    }

    void LoadData()
    {
        if (IsFileExist())
        {
            userData = JsonUtility.FromJson<UserData>(File.ReadAllText(GetSaveFilePath()));
        }
        else
        {
            userData = new UserData();
            userData.language = Application.systemLanguage;
            if (userData.language != SystemLanguage.Korean && userData.language != SystemLanguage.English && userData.language != SystemLanguage.Japanese)
                userData.language = SystemLanguage.English;
            userData.dataLanguage = Application.systemLanguage;
            if (userData.dataLanguage != SystemLanguage.English && userData.dataLanguage != SystemLanguage.Japanese)
                userData.dataLanguage = SystemLanguage.English;
        }
    }

    void SaveData() => File.WriteAllText(GetSaveFilePath(), JsonUtility.ToJson(userData));

    public bool IsFileExist() => File.Exists(GetSaveFilePath());

    public void UpdateWeek()
    {
        long week = DateTime.UtcNow.AddDays(-1).AddHours(-9).Ticks / 6048000000000;
        if (userData.dataWeek > week)
        {
            userData.salesData = new SalesData[7];
            userData.dataWeek = week;
            SaveData();
        }
    }
}
