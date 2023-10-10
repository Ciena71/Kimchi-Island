using System;
using UnityEngine;

[Serializable]
public class UserData
{
    public SystemLanguage language = SystemLanguage.English;
    public SystemLanguage dataLanguage = SystemLanguage.English;
    public bool alwaysOnTop;
    public int alarmVolume;
    public int alarm;

    public int rank = 20;
    public int grooveCurrent = 0;
    public int grooveMax = 45;
    public int workshopRank = 5;
    public int workshopActive = 4;
    public bool netProfit = false;
    public bool[] productBlacklistNew = new bool[81];
    public SalesData[] salesData = new SalesData[7];
    public long dataWeek;
    public int cpuThread = Environment.ProcessorCount >= 8 ? Environment.ProcessorCount - 4 : Environment.ProcessorCount - 1;
    public bool gpuCalculate;
    public int limitEnableCount = 20;

    public int animalAlarmList;
    public int[] inventory = new int[99];

    public bool contribute = false;
    public int packet = 0;
    public Machina.FFXIV.Oodle.OodleImplementation oodle = Machina.FFXIV.Oodle.OodleImplementation.FfxivTcp;
    public bool deucalion = false;
    public string gamePath = @"C:\Program Files (x86)\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\game\ffxiv_dx11.exe";
}