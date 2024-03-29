using System;
using System.IO;
using UnityEngine;

public class UserManager
{
    public static UserManager instance;

    string dataPath = Application.persistentDataPath;

    public string GetFilePath() => dataPath;

    string GetSaveFilePath() => Path.Combine(dataPath, "UserSave.hxd");

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
        Workshop.instance.UpdateEnableList();
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

    public int GetWorkshopRank() => userData.workshopRank;
    public void SetWorkshopRank(int value)
    {
        userData.workshopRank = value;
        SaveData();
    }

    public int GetWorkshopActive() => userData.workshopActive;
    public void SetWorkshopActive(int value)
    {
        userData.workshopActive = value;
        SaveData();
    }

    public bool GetNetProfit() => userData.netProfit;
    public void SetNetProfit(bool value)
    {
        userData.netProfit = value;
        SaveData();
    }

    public bool GetProductBlacklist(int index) => !userData.productBlacklistNew[index];
    public void SetProductBlacklist(int index, bool value)
    {
        if (value != GetProductBlacklist(index))
        {
            userData.productBlacklistNew[index] = !value;
            SaveData();
            Workshop.instance.UpdateEnableList();
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

    public Machina.FFXIV.Oodle.OodleImplementation GetOodle() => userData.oodle;
    public void SetOodle(Machina.FFXIV.Oodle.OodleImplementation oodle)
    {
        Eorzea eorzea = Eorzea.instance;
        eorzea.StopPacketCapture();
        userData.oodle = oodle;
        eorzea.SettingPacketCapture();
        SaveData();
    }

    public bool GetDeucalion() => userData.deucalion;
    public void SetDeucalion(bool value)
    {
        Eorzea eorzea = Eorzea.instance;
        eorzea.StopPacketCapture();
        userData.deucalion = value;
        eorzea.SettingPacketCapture();
        SaveData();
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

    public int GetCPUThread() => (userData.cpuThread < Environment.ProcessorCount ? (userData.cpuThread > 0 ? userData.cpuThread : 1) : Environment.ProcessorCount - 1);
    public void SetCPUThread(int value)
    {
        if (value <= 0 || value >= Environment.ProcessorCount)
            return;
        userData.cpuThread = value;
        SaveData();
    }

    public bool IsGPUCalculate() => userData.gpuCalculate;
    public void SetGPUCalculate(bool value)
    {
        userData.gpuCalculate = value;
        SaveData();
    }

    public int GetLimitCount() => userData.limitEnableCount;
    public void SetLimitCount(int value)
    {
        if (value < 0)
            value = 0;
        userData.limitEnableCount = value;
        SaveData();
    }

    public bool GetContribute() => userData.contribute;
    public void SetContribute(bool _value)
    {
        userData.contribute = _value;
        SaveData();
    }

    void LoadData()
    {
        if (IsFileExist())
        {
            userData = JsonUtility.FromJson<UserData>(File.ReadAllText(GetSaveFilePath()));
            bool changed = false;
            if (userData.productBlacklistNew.Length != 81)
            {
                Array.Resize(ref userData.productBlacklistNew, 81);
                changed = true;
            }
            if (userData.inventory.Length != 99)
            {
                Array.Resize(ref userData.inventory, 99);
                changed = true;
            }
            if (userData.oodle == 0)
            {
                userData.oodle = Machina.FFXIV.Oodle.OodleImplementation.FfxivTcp;
                changed = true;
            }
            if (changed)
                SaveData();
        }
        else
        {
            userData = new UserData();
            userData.language = Application.systemLanguage;
            if (userData.language != SystemLanguage.Korean && userData.language != SystemLanguage.English && userData.language != SystemLanguage.Japanese)
                userData.language = SystemLanguage.English;
            userData.dataLanguage = Application.systemLanguage;
            if (userData.dataLanguage != SystemLanguage.Korean && userData.dataLanguage != SystemLanguage.English && userData.dataLanguage != SystemLanguage.Japanese)
                userData.dataLanguage = SystemLanguage.English;
        }
    }

    void SaveData() => File.WriteAllText(GetSaveFilePath(), JsonUtility.ToJson(userData));

    public bool IsFileExist() => File.Exists(GetSaveFilePath());

    public void UpdateWeek()
    {
        long week = DateTime.UtcNow.AddDays(-1).AddHours(-8).Ticks / 6048000000000 - 105483;
        if (userData.dataWeek != week)
        {
            userData.salesData = new SalesData[7];
            userData.dataWeek = week;
            SaveData();
        }
    }
}
