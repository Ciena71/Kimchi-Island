using System;
using UnityEngine;

[Serializable]
public class UserData
{
    public SystemLanguage language = SystemLanguage.English;
    public SystemLanguage dataLanguage = SystemLanguage.English;
    public int packet = 0;
    public bool alwaysOnTop;
    public int alarmVolume;
    public int alarm;

    public int rank = 10;
    public int grooveCurrent = 0;
    public int grooveMax = 35;
    public int workshopRank = 3;
    public int workshopActive = 3;
    public bool netProfit = false;
    public bool groovePriority = false;
    public bool[] productBlacklistNew = new bool[50];
    public SalesData[] salesData = new SalesData[7];
    public long dataWeek;
    public int cpuThread = Environment.ProcessorCount >= 8 ? Environment.ProcessorCount - 4 : Environment.ProcessorCount - 1;
    public bool gpuCalculate;
    public int limitEnableCount = 100;

    public int animalAlarmList;
    public int[] inventory = new int[61];

    public string gamePath = @"C:\Program Files (x86)\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\game\ffxiv_dx11.exe";
}
