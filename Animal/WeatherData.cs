using System;
using UnityEngine;
using UnityEngine.UI;

public class WeatherData : MonoBehaviour
{
    public Image imgWeather;
    public Text textWeather;
    public Text textLT;
    public Text textET;

    int weather;

    public void SetData(double time)
    {
        Eorzea eorzea = Eorzea.instance;
        DateTime dt = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(Convert.ToInt64(time * 10000000)).AddYears(1969));
        weather = eorzea.GetWeather(time);
        ResourceManager resourceManager = ResourceManager.instance;
        imgWeather.sprite = Resources.Load<Sprite>($"Sprite/Weather/W{weather}");
        textWeather.text = resourceManager.GetWeather(weather);
        textLT.text = dt.ToString("MM-dd HH:mm:ss");
        textET.text = eorzea.ToEorzeaTime(dt).ToString("HH:mm");
    }

    public void ApplyLanguage()
    {
        textWeather.text = ResourceManager.instance.GetWeather(weather);
    }
}
