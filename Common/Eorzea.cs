using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Machina.FFXIV;
using Machina.Infrastructure;
using System.Collections.Generic;

public class Eorzea : IDisposable
{
    public static Eorzea instance;

    DateTime eorzeaTime;
    int eorzeaWeather;

    Text textEorzeaTimeHour;
    Text textEorzeaTimeMinute;
    Image imgWeather;
    Text textWeather;

    int[] islandWeather = { 25, 70, 80, 90, 95, 100 };

    private static Thread threadPacket;
    private static Queue<byte[]> threadReceivedQueue = new Queue<byte[]>();
    static FFXIVNetworkMonitor monitor;

    public Eorzea(Text _textHour,Text _textMinute,Image _imgWeather,Text _textWeather)
    {
        instance = this;
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
        imgWeather.sprite = Resources.Load<Sprite>($"Sprite/Weather/{eorzeaWeather}");
        textWeather.text = ResourceManager.instance.GetWeather(eorzeaWeather);
    }

    public void Dispose()
    {
        if (monitor != null)
        {
            if (UserManager.instance.GetPacketType() > 0)
                monitor.Stop();
            monitor.Dispose();
        }
        instance = null;
    }

    public void StopPacketCapture() => monitor.Stop();

    public void SettingPacketCapture()
    {
        if (monitor != null)
        {
            UserManager userManager = UserManager.instance;
            monitor.MonitorType = (userManager.GetPacketType() == 2) ? NetworkMonitorType.WinPCap : NetworkMonitorType.RawSocket;
            if (userManager.GetPacketType() > 0)
                monitor.Start();
        }
    }

    private static void ThreadRun()
    {
        UserManager userManager = UserManager.instance;
        monitor = new FFXIVNetworkMonitor();
        monitor.OodlePath = userManager.GetGamePath();
        monitor.WindowName = "FINAL FANTASY XIV";
        monitor.MonitorType = (userManager.GetPacketType() == 2) ? NetworkMonitorType.WinPCap : NetworkMonitorType.RawSocket;
        monitor.MessageReceivedEventHandler = (TCPConnection connection, long epoch, byte[] message) => MessageReceived(connection, epoch, message);
        if (userManager.GetPacketType() > 0)
            monitor.Start();
    }

    private static void MessageReceived(TCPConnection connection, long epoch, byte[] message)
    {
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
                    if (form.GetFirstSpawnTime().ToUniversalTime().Ticks <= DateTime.UtcNow.Ticks)
                    {
                        if (form.IsActiveAlarm())
                            SystemCore.instance.PlayAlarm();
                        form.SetTime(1);
                    }
                    if (form.GetFirstHideTime().ToUniversalTime().Ticks <= DateTime.UtcNow.Ticks)
                        form.SetTime(2);
                });
                if (newEorzeaTime.Hour % 8 == 0)
                    Animal.instance.ResetWeather();
                eorzeaWeather = _weather;
                imgWeather.sprite = Resources.Load<Sprite>($"Sprite/Weather/{eorzeaWeather}");
                textWeather.text = ResourceManager.instance.GetWeather(eorzeaWeather);
            }
            eorzeaTime = newEorzeaTime;
            textEorzeaTimeHour.text = eorzeaTime.Hour.ToString();
            textEorzeaTimeMinute.text = eorzeaTime.Minute >= 10 ? eorzeaTime.Minute.ToString() : $"0{eorzeaTime.Minute}";
        }
        if (threadReceivedQueue.Count > 0)
        {
            byte[] data = threadReceivedQueue.Dequeue();
            if (data.Length == 112 && data[data.Length - 8] == 50)
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
            }
        }
    }

    public void ApplyLanguage()
    {
        ResourceManager resourceManager = ResourceManager.instance;
        textWeather.text = resourceManager.GetWeather(GetNowWeather());
    }
}
