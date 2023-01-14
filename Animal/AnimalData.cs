using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AnimalData : MonoBehaviour
{
    public Toggle toggleAlarm;
    public Image imgOutline;
    public Image imgIcon;
    public ParticleSystem particle;

    bool alarm;
    int index;
    int time;
    int weather;
    int size;

    bool isSpawned;

    AnimalNextSpawn animalNextSpawn;

    public void OnEnable()
    {
        if (isSpawned)
        {
            StartCoroutine(ActiveParticle());
        }
    }

    IEnumerator ActiveParticle()
    {
        yield return new WaitForFixedUpdate();
        if (isSpawned)
        {
            particle.Stop();
            particle.Play();
        }
    }

    public void SetDefaultData(int _index, AnimalNextSpawn _animalNextSpawn)
    {
        index = _index;
        animalNextSpawn = _animalNextSpawn;
        animalNextSpawn.SetAnimal(index);
        animalNextSpawn.SetAnimalName(index);
        animalNextSpawn.SetData(this);
        ResourceManager resourceManager = ResourceManager.instance;
        resourceManager.GetAnimalData(index, out time, out weather, out size, out Vector2 pos);
        imgIcon.sprite = Resources.Load<Sprite>($"Sprite/Animal/{index}");
        transform.localPosition = new Vector2(1500.0f * (pos.x - 1) / 41, -1500.0f * (pos.y - 1) / 41);
        UserManager userManager = UserManager.instance;
        alarm = userManager.GetAnimalAlarmlist(index);
        toggleAlarm.isOn = alarm;
        toggleAlarm.onValueChanged.AddListener((value) =>
        {
            alarm = value;
            userManager.SetAnimalAlarmlist(index, value);
        });
        SetTime(0);
    }

    public bool IsActiveAlarm() => alarm;

    public string GetAnimalName() => ResourceManager.instance.GetAnimalName(index);

    public int GetSpawnTime() => time;

    public int GetSpawnWeather() => weather;

    public void SetPopUp(bool active)
    {
        isSpawned = active;
        if (active)
        {
            particle.Stop();
            particle.Play();
        }
        else
            particle.Stop();
    }

    public void PointerEnter()
    {
        ResourceManager resourceManager = ResourceManager.instance;
        int num = 3 + (time >= 0 ? 1 : 0) + (weather >= 0 ? 1 : 0);
        string[] title = new string[num];
        string[] des = new string[num];
        num = 0;
        title[num] = resourceManager.GetText(23);
        des[num] = resourceManager.GetSize(size);
        ++num;
        if (time >= 0)
        {
            title[num] = resourceManager.GetText(8);
            des[num] = $"{time}:00 ~ {(time + 3 >= 24 ? time - 21 : time + 3)}:00";
            ++num;
        }
        if (weather >= 0)
        {
            title[num] = resourceManager.GetText(22);
            des[num] = resourceManager.GetWeather(weather);
            ++num;
        }
        title[num] = resourceManager.GetText(59);
        des[num] = resourceManager.GetMaterialName(resourceManager.GetAnimalNormalLeaving(index));
        ++num;
        title[num] = resourceManager.GetText(60);
        des[num] = resourceManager.GetMaterialName(resourceManager.GetAnimalRareLeaving(index));
        Tooltip.instance.ShowToolTip(imgIcon.sprite, resourceManager.GetAnimalName(index), title, des);
    }

    public void PointerExit()
    {
        Tooltip.instance.HideToolTip();
    }

    public void ApplyLanguage()
    {
        animalNextSpawn.SetAnimalName(index);
    }

    public DateTime GetFirstSpawnTime() => animalNextSpawn.GetSpawnTime();

    public DateTime GetFirstHideTime() => animalNextSpawn.GetHideTime();

    public void SetTime(int type)
    {
        Eorzea eorzea = Eorzea.instance;
        if (time >= 0 && weather == -1)
        {
            if (type < 2)
            {
                if (eorzea.ToEorzeaTime(DateTime.UtcNow).Hour >= time)
                {
                    if (eorzea.ToEorzeaTime(DateTime.UtcNow).Hour < time + 3)
                    {
                        double startTime = (Math.Truncate((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds / 4200)) * 4200 + 175 * time;
                        DateTime dt = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(Convert.ToInt64(startTime * 10000000)).AddYears(1969));
                        if (type == 0)
                            animalNextSpawn.SetDefaultTime(dt.AddSeconds(4200), dt.AddSeconds(525), dt.AddSeconds(4725));
                        else
                            animalNextSpawn.SetSpawnTime(dt.AddSeconds(4200), dt.AddSeconds(4725));
                    }
                    else
                    {
                        double startTime = (Math.Truncate((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds / 4200) + 1) * 4200 + 175 * time;
                        DateTime dt = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(Convert.ToInt64(startTime * 10000000)).AddYears(1969));
                        if (type == 0)
                            animalNextSpawn.SetDefaultTime(dt, new DateTime(1970, 1, 1), dt.AddSeconds(525));
                        else
                            animalNextSpawn.SetSpawnTime(dt, dt.AddSeconds(525));
                    }
                }
                else
                {
                    double startTime = (Math.Truncate((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds / 4200)) * 4200 + 175 * time;
                    DateTime dt = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(Convert.ToInt64(startTime * 10000000)).AddYears(1969));
                    if (type == 0)
                        animalNextSpawn.SetDefaultTime(dt, dt.AddSeconds(525), new DateTime(1970, 1, 1));
                    else
                        animalNextSpawn.SetSpawnTime(dt, dt.AddSeconds(525));
                }
            }
            else
                animalNextSpawn.SetHideTime();
        }
        else
        {
            if (time == -1)
            {
                if (type < 2)
                {
                    double startTime = (Math.Truncate((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds / 1400) + 1) * 1400;
                    while (true)
                    {
                        DateTime dt = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(Convert.ToInt64(startTime * 10000000)).AddYears(1969));
                        if (eorzea.GetWeather(dt) == weather)
                        {
                            if (type == 0)
                            {
                                if (eorzea.GetNowWeather() == weather)
                                    animalNextSpawn.SetDefaultTime(dt, TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(Convert.ToInt64((Math.Truncate((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds / 1400) + 1) * 1400 * 10000000)).AddYears(1969)), dt.AddSeconds(1400));
                                else
                                    animalNextSpawn.SetDefaultTime(dt, new DateTime(1970, 1, 1), dt.AddSeconds(1400));
                            }
                            else
                                animalNextSpawn.SetSpawnTime(dt, dt.AddSeconds(1400));
                            break;
                        }
                        else
                            startTime += 1400;
                    }
                }
                else
                    animalNextSpawn.SetHideTime();
            }
            else
            {
                if (type < 2)
                {
                    double startTime = (Math.Truncate((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds / 4200)) * 4200;
                    DateTime dt = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(Convert.ToInt64(startTime * 10000000)).AddYears(1969)).AddSeconds(time * 175);
                    DateTime ht = new DateTime(1970, 1, 1);
                    if (eorzea.GetNowTime().Hour < time + 3)
                    {
                        if (eorzea.GetNowWeather() == weather && time <= eorzea.GetNowTime().Hour)
                        {
                            for (int i = 2; i > 0; --i)
                            {
                                if (eorzea.GetWeather(dt.AddSeconds(175 * i)) == weather)
                                {
                                    ht = new DateTime(dt.AddSeconds(175 * (i + 1)).Ticks);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        startTime += 4200;
                        dt = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(Convert.ToInt64(startTime * 10000000)).AddYears(1969)).AddSeconds(time * 175);
                    }
                    while (true)
                    {
                        int start = eorzea.GetWeather(dt) == weather ? 0 : -1;
                        int end = 0;
                        for (int i = 1; i < 3; ++i)
                        {
                            if (start == -1 && eorzea.GetWeather(dt.AddSeconds(175 * i)) == weather)
                                start = i;
                            if (start >= 0 && eorzea.GetWeather(dt.AddSeconds(175 * i)) == weather)
                                end = i + 1;
                        }
                        if (start >= 0)
                        {
                            if (type == 0)
                                animalNextSpawn.SetDefaultTime(dt.AddSeconds(175 * start), ht, dt.AddSeconds(175 * end));
                            else
                                animalNextSpawn.SetSpawnTime(dt.AddSeconds(175 * start), dt.AddSeconds(175 * end));
                            break;
                        }
                        else
                        {
                            startTime += 4200;
                            dt = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(Convert.ToInt64(startTime * 10000000)).AddYears(1969)).AddSeconds(time * 175);
                        }
                    }
                }
                else
                    animalNextSpawn.SetHideTime();
            }
        }
    }
}
