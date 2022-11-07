using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Animal : MonoBehaviour
{
    public static Animal instance;

    public Transform transPivot;
    public Text textWeather;
    public Text textWeatherLT;
    public Text textAnimal;
    public Text textAnimalLT;
    public Text textAnimalRT;
    public ScrollRect scrollWeather;
    public ScrollRect scrollAnimal;

    List<AnimalData> animalList = new List<AnimalData>();
    List<WeatherData> weatherList = new List<WeatherData>();

    void Awake()
    {
        instance = this;
        ResourceManager resourceManager = ResourceManager.instance;
        for (int i = 0; i < resourceManager.GetAnimalMax(); ++i)
        {
            GameObject animalObject = Instantiate(Resources.Load("Prefab/Animal/Animal"), transPivot) as GameObject;
            AnimalData animalData = animalObject.GetComponent<AnimalData>();
            GameObject animalNextSpawnObject = Instantiate(Resources.Load("Prefab/Animal/AnimalNextSpawn"), scrollAnimal.content) as GameObject;
            animalData.SetDefaultData(i, animalNextSpawnObject.GetComponent<AnimalNextSpawn>());
            animalList.Add(animalData);
        }
        double startTime = Math.Truncate((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds / 1400) * 1400;
        for (int i = 0; i < 100; ++i)
        {
            GameObject weatherObject = Instantiate(Resources.Load("Prefab/Animal/WeatherItem"), scrollWeather.content) as GameObject;
            WeatherData weatherData = weatherObject.GetComponent<WeatherData>();
            weatherData.SetData(startTime + i * 1400);
            weatherList.Add(weatherData);
        }
        ApplyLanguage();
    }

    private void OnDestroy()
    {
        animalList.Clear();
    }

    public List<AnimalData> GetAnimalData() => animalList;

    public void ResetWeather()
    {
        double startTime = Math.Truncate((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds / 1400) * 1400;
        weatherList[0].transform.SetAsLastSibling();
        weatherList[0].SetData(startTime + (weatherList.Count - 1) * 1400);
        weatherList.Add(weatherList[0]);
        weatherList.Remove(weatherList[0]);
    }

    public void ApplyLanguage()
    {
        ResourceManager resourceManager = ResourceManager.instance;
        weatherList.ForEach((form) =>
        {
            form.ApplyLanguage();
        });
        animalList.ForEach((form) =>
        {
            form.ApplyLanguage();
        });
        textWeather.text = resourceManager.GetText(22);
        textWeatherLT.text = resourceManager.GetText(25);
        textAnimal.text = resourceManager.GetText(21);
        textAnimalLT.text = resourceManager.GetText(25);
        textAnimalRT.text = resourceManager.GetText(26);
    }
}
