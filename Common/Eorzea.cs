using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Machina.FFXIV;
using Machina.Infrastructure;
using Debug = UnityEngine.Debug;

public class Eorzea : IDisposable
{
    public static Eorzea instance;

    DateTime eorzeaTime;
    int eorzeaWeather;

    Text textEorzeaTimeHour;
    Text textEorzeaTimeMinute;
    Image imgWeather;
    Text textWeather;

    static UserManager userManager;
    ResourceManager resourceManager;

    int[] islandWeather = { 25, 70, 80, 90, 95, 100 };

    private static Thread threadPacket;
    private static Queue<byte[]> threadReceivedQueue = new Queue<byte[]>();
    static FFXIVNetworkMonitor monitor;
    static List<int> processIdList = new List<int>();

    public Eorzea(Text _textHour,Text _textMinute,Image _imgWeather,Text _textWeather)
    {
        instance = this;
        userManager = UserManager.instance;
        resourceManager = ResourceManager.instance;
#if UNITY_EDITOR
        AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        try
        {
            StateData stateData = JsonUtility.FromJson<StateData>(GUIUtility.systemCopyBuffer);
            Debug.Log(JsonUtility.ToJson(stateData));
            string a = "";
            for (int i = 1; i <= 72; ++i)
            {
                a += $"{stateData.supplyDemand[i].supply}\t{2 - stateData.supplyDemand[i].demand}";
                if (i <= 71)
                    a += "\n";
            }
            Debug.Log(a);
            GUIUtility.systemCopyBuffer = a;
            Debug.Log($"{stateData.popularity}\t{stateData.predictedPopularity}");
        }
        catch
        {
        }
#endif
        threadPacket = new Thread(ThreadRun);
        threadPacket.Start();
        textEorzeaTimeHour = _textHour;
        textEorzeaTimeMinute = _textMinute;
        imgWeather = _imgWeather;
        textWeather = _textWeather;
        eorzeaTime = ToEorzeaTime(DateTime.UtcNow);
        textEorzeaTimeHour.text = eorzeaTime.Hour.ToString();
        textEorzeaTimeMinute.text = eorzeaTime.Minute >= 10 ? eorzeaTime.Minute.ToString() : $"0{eorzeaTime.Minute}";
        eorzeaWeather = GetWeather(DateTime.UtcNow);
        imgWeather.sprite = Resources.Load<Sprite>($"Sprite/Weather/W{eorzeaWeather}");
        textWeather.text = resourceManager.GetWeather(eorzeaWeather);
    }

    public void Dispose()
    {
        if (monitor != null)
        {
            if (userManager.GetPacketType() > 0)
                monitor.Stop();
            monitor.Dispose();
        }
        instance = null;
    }

    public List<int> GetProcessIdList() => processIdList;

    public void StopPacketCapture() => monitor.Stop();

    public void SettingPacketCapture()
    {
        if (monitor != null)
        {
            monitor.OodleImplementation = userManager.GetOodle();
            monitor.MonitorType = (userManager.GetPacketType() == 2) ? NetworkMonitorType.WinPCap : NetworkMonitorType.RawSocket;
            Process[] process = Process.GetProcessesByName("ffxiv_dx11");
            for (int i = processIdList.Count - 1; i >= 0; --i)
            {
                bool checker = false;
                for (int j = 0; j < process.Length; ++j)
                {
                    if (process[j].HasExited && process[j].Id == processIdList[i])
                        checker = true;
                }
                if (!checker)
                    processIdList.RemoveAt(i);
            }
            if (processIdList.Count > 0)
                monitor.ProcessID = (uint)processIdList[0];
            Option.instance.UpdateProcessID(processIdList);
            if (userManager.GetDeucalion())
            {
                if (processIdList.Count > 0)
                    monitor.UseDeucalion = userManager.GetDeucalion();
                else
                    monitor.UseDeucalion = false;
            }
            else
                monitor.UseDeucalion = false;
            if (userManager.GetPacketType() > 0)
                monitor.Start();
        }
    }

    private static void ThreadRun()
    {
        monitor = new FFXIVNetworkMonitor();
        monitor.OodlePath = userManager.GetGamePath();
        monitor.WindowName = "FINAL FANTASY XIV";
        monitor.MessageReceivedEventHandler = (TCPConnection connection, long epoch, byte[] message) => MessageReceived(connection, epoch, message);
        monitor.OodleImplementation = userManager.GetOodle();
        monitor.MonitorType = (userManager.GetPacketType() == 2) ? NetworkMonitorType.WinPCap : NetworkMonitorType.RawSocket;
        monitor.LocalIP = GetLocalIPAddress();
        Process[] process = Process.GetProcessesByName("ffxiv_dx11");
        for (int i = 0; i < process.Length; ++i)
            processIdList.Add(process[i].Id);
        if (process.Length > 0)
        {
            monitor.ProcessID = (uint)process[0].Id;
            monitor.UseDeucalion = userManager.GetDeucalion();
        }
        if (userManager.GetPacketType() > 0)
            monitor.Start();
    }

    public static IPAddress GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip;
            }
        }
        return null;
    }

    private static void MessageReceived(TCPConnection connection, long epoch, byte[] message)
    {
        if (userManager.GetDeucalion())
        {
            if (!monitor.UseDeucalion)
            {
                monitor.Stop();
                monitor.ProcessID = connection.ProcessId;
                monitor.UseDeucalion = true;
                monitor.Start();
                if (!processIdList.Exists(a => a == connection.ProcessId))
                    processIdList.Add((int)connection.ProcessId);
                Option.instance.UpdateProcessID(processIdList);
            }
        }
        threadReceivedQueue.Enqueue(message);
    }

    public DateTime ToEorzeaTime(DateTime date)
    {
        const double EORZEA_MULTIPLIER = 3600d / 175d;
        long epochTicks = date.ToUniversalTime().Ticks - (new DateTime(1970, 1, 1).Ticks);
        long eorzeaTicks = (long)Math.Round(epochTicks * EORZEA_MULTIPLIER);
        return new DateTime(eorzeaTicks);
    }

    private int CalculateWeather(double time)
    {
        var bell = time / 175;
        var increment = ((uint)(bell + 8 - (bell % 8))) % 24;
        var totalDays = (uint)(time / 4200);
        var calcBase = (totalDays * 100) + increment;
        var step1 = (calcBase << 11) ^ calcBase;
        var step2 = (step1 >> 8) ^ step1;
        return (int)(step2 % 100);
    }

    public int GetWeather(double time)
    {
        int target = CalculateWeather(time);
        for (int i = 0; i < islandWeather.Length; ++i)
        {
            if (i == 0)
            {
                if (target < islandWeather[i])
                {
                    return i;
                }
            }
            else if (islandWeather[i - 1] <= target && target < islandWeather[i])
                return i;
        }
        return -1;
    }

    public int GetWeather(DateTime time)
    {
        var unix = (time.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        int target = CalculateWeather(unix);
        for (int i = 0; i < islandWeather.Length; ++i)
        {
            if (i == 0)
            {
                if (target < islandWeather[i])
                {
                    return i;
                }
            }
            else if (islandWeather[i - 1] <= target && target < islandWeather[i])
                return i;
        }
        return -1;
    }

    public DateTime GetNowTime() => eorzeaTime;

    public int GetNowWeather() => eorzeaWeather;

    public void Update()
    {
        DateTime newEorzeaTime = ToEorzeaTime(DateTime.UtcNow);
        if (eorzeaTime.Hour != newEorzeaTime.Hour || eorzeaTime.Minute != newEorzeaTime.Minute)
        {
            if (eorzeaTime.Hour != newEorzeaTime.Hour)
            {
                int _weather = GetWeather(DateTime.UtcNow);
                Animal.instance.GetAnimalData().ForEach((form) =>
                {
                    if (!form.IsSpawned() && form.GetFirstSpawnTime().ToUniversalTime().Ticks <= DateTime.UtcNow.Ticks)
                    {
                        if (form.IsActiveAlarm())
                            SystemCore.instance.PlayAlarm();
                        form.SetTime(1);
                    }
                    else if (form.IsSpawned() && form.GetFirstHideTime().ToUniversalTime().Ticks <= DateTime.UtcNow.Ticks)
                        form.SetTime(2);
                });
                if (newEorzeaTime.Hour % 8 == 0)
                    Animal.instance.ResetWeather();
                eorzeaWeather = _weather;
                imgWeather.sprite = Resources.Load<Sprite>($"Sprite/Weather/W{eorzeaWeather}");
                textWeather.text = resourceManager.GetWeather(eorzeaWeather);
            }
            eorzeaTime = newEorzeaTime;
            textEorzeaTimeHour.text = eorzeaTime.Hour.ToString();
            textEorzeaTimeMinute.text = eorzeaTime.Minute >= 10 ? eorzeaTime.Minute.ToString() : $"0{eorzeaTime.Minute}";
        }
        if (threadReceivedQueue.Count > 0)
        {
            byte[] data = threadReceivedQueue.Dequeue();
            /*if (data.Length == 128)
                Debug.Log($"{data.Length} : {string.Join(", ", data)}");*/
            //int opcode = data[18] + (data[19] * 0x100);
            if ((data.Length == 112 && data[data.Length - 8] == 50) || (data.Length == 120 && data[data.Length - 6] == 50) || (data.Length == 128 && data[data.Length - 7] == 50))
            {
                Workshop.instance.SetPacketData(data);
                return;
            }
            else if (data.Length == 64)
            {
                int item = data[40] + (data[41] * 0x100);
                if (37551 <= item && item <= 37611)
                    Inventory.instance.SetItemQuantity(item - 37551, data[48] + (data[49] * 0x100));
                else if (39224 <= item && item <= 39232)
                    Inventory.instance.SetItemQuantity(item - 39163, data[48] + (data[49] * 0x100));
                else if (39887 <= item && item <= 39902)
                    Inventory.instance.SetItemQuantity(item - 39817, data[48] + (data[49] * 0x100));
                else if (41630 <= item && item <= 41634)
                    Inventory.instance.SetItemQuantity(item - 41544, data[48] + (data[49] * 0x100));
                else if (41635 <= item && item <= 41638)
                    Inventory.instance.SetItemQuantity(item - 41540, data[48] + (data[49] * 0x100));
                else if (41639 <= item && item <= 41642)
                    Inventory.instance.SetItemQuantity(item - 41548, data[48] + (data[49] * 0x100));
            }
            /*
            else if (opcode != 358 && opcode != 387 && opcode != 520 && opcode != 4134 && opcode != 852)
            {
                if (data.Length - 20 >= 60)
                {
                    string value = "";
                    for (int i = 20; i < data.Length; ++i)
                    {
                        value += data[i];
                        if (i < data.Length - 1)
                            value += ", ";
                    }
                    Debug.Log($"{opcode} : {data.Length - 20}\n{value}");
                    //Debug.Log($"{opcode} {data.Length} : {string.Join(", ", data)}");
                }
            }*/
        }
    }

    public void ApplyLanguage()
    {
        textWeather.text = resourceManager.GetWeather(GetNowWeather());
    }

    public void OnBeforeAssemblyReload()
    {
        StopPacketCapture();
    }

    public void OnAfterAssemblyReload()
    {
        SettingPacketCapture();
    }
}
