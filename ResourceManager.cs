using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;


public class ResourceManager
{
    public static ResourceManager instance;

    List<Dictionary<string, object>> productData;
    List<Dictionary<string, object>> categoryData;
    List<Dictionary<string, object>> supplyData;
    List<Dictionary<string, object>> demandShiftData;
    List<Dictionary<string, object>> statusData;
    List<Dictionary<string, object>> animalData;
    List<Dictionary<string, object>> sizeData;
    List<Dictionary<string, object>> weatherData;
    List<Dictionary<string, object>> textData;
    List<Dictionary<string, object>> supplyPattern;
    List<Dictionary<string, object>> updateVersionData;
    List<Dictionary<string, object>> itemData;
    List<Dictionary<string, object>> gatherNodeData;
    List<Dictionary<string, object>> urlData;

    byte[,] popularityData =
    {
        {0,3,2,1,1,2,2,2,3,3,3,1,4,4,1,2,4,3,2,3,4,4,3,4,3,1,2,3,3,1,2,4,1,2,1,4,1,2,3,2,1,3,2,1,3,2,3,2,3,1,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,1,2,2,3,3,3,2,4,1,1,4,1,2,2,4,3,2,2,1,1,2,3,1,2,2,3,3,2,1,3,3,4,4,2,4,2,1,4,1,3,4,3,1,2,3,3,1,3,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,1,3,3,2,1,1,3,2,4,1,4,3,2,2,4,3,3,1,2,2,3,2,2,4,3,2,2,4,1,3,2,4,3,3,1,1,4,2,2,3,1,1,1,3,4,3,2,1,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,3,2,4,3,2,4,2,4,1,3,1,2,1,2,3,3,4,3,3,3,4,2,2,1,1,4,1,2,2,3,1,3,2,1,4,2,2,3,1,3,1,1,3,2,3,4,3,2,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,1,3,3,2,2,1,1,4,4,2,3,4,2,2,3,3,3,3,4,2,3,1,1,1,3,4,2,3,2,2,4,1,3,3,2,1,2,4,4,2,3,1,2,2,1,3,2,3,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,3,4,3,2,3,2,1,2,2,3,2,4,3,4,1,1,3,1,2,1,4,3,3,4,1,4,3,4,3,2,3,1,2,4,3,2,3,1,1,3,1,2,2,2,1,2,2,2,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,2,1,3,2,2,3,1,2,3,3,4,1,4,1,4,3,1,1,3,3,2,1,2,3,1,3,3,1,2,2,2,4,2,4,2,4,1,2,4,3,1,2,2,4,3,1,3,3,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,2,4,1,1,3,3,2,3,1,3,3,1,2,2,4,4,3,3,3,1,2,2,3,2,2,3,2,2,1,2,3,1,3,1,1,1,4,4,4,4,4,1,2,3,1,3,2,3,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,1,2,3,1,4,3,3,2,4,3,2,1,1,2,4,3,2,4,2,3,4,2,3,3,3,1,1,2,4,1,3,1,4,3,2,2,2,3,1,1,3,2,2,3,1,3,4,1,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,1,1,4,1,2,4,2,3,3,3,3,2,1,4,2,2,1,2,2,3,3,4,2,2,4,1,4,1,2,3,4,2,3,1,3,3,3,1,4,2,2,1,3,1,2,3,2,3,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,4,1,2,3,4,1,1,2,1,2,3,3,3,4,3,2,3,3,2,4,2,2,3,2,4,1,4,1,2,2,3,1,4,4,3,1,1,2,1,3,3,3,2,3,1,2,1,2,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,1,2,1,3,3,4,2,2,1,4,3,3,3,4,2,2,3,3,3,4,1,2,3,2,1,2,1,3,2,2,2,3,1,4,2,1,1,2,2,4,3,1,2,4,4,3,3,1,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,4,3,3,1,2,1,2,2,3,4,4,2,3,3,1,1,3,2,3,1,2,1,2,2,4,2,1,3,1,3,2,1,3,3,4,2,4,3,4,3,1,2,4,1,2,3,1,3,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,1,2,4,4,2,4,2,3,2,3,1,1,3,1,3,3,2,3,3,4,4,1,2,3,1,1,4,1,1,3,3,2,1,2,4,3,3,4,1,2,2,2,3,2,2,3,3,1,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,1,2,2,3,1,1,3,2,3,4,4,2,2,4,1,3,1,1,1,2,3,3,2,4,3,4,1,3,4,4,3,1,2,3,2,2,2,2,3,1,3,3,3,2,2,1,4,2,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,2,3,1,4,2,3,2,2,4,4,1,1,3,2,3,3,2,2,1,3,2,1,2,3,3,3,2,1,4,2,2,2,1,4,4,3,1,2,3,3,3,3,1,2,4,3,4,1,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,4,2,1,2,3,2,3,4,3,2,1,2,1,3,3,1,4,4,3,3,3,4,1,3,2,1,3,2,1,4,3,4,1,2,2,2,3,2,1,2,3,2,3,2,1,2,1,4,3,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,4,1,1,4,2,3,2,3,4,3,2,3,2,3,1,2,1,2,3,1,3,1,3,1,2,2,1,1,3,4,2,3,4,3,4,2,3,1,3,4,2,3,2,2,1,2,3,2,4,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,2,3,4,4,2,3,3,2,1,1,4,3,1,2,2,1,3,1,4,2,3,1,2,3,3,1,2,2,2,3,4,4,2,3,1,3,1,1,2,4,3,3,1,2,2,4,1,3,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,2,1,3,2,3,1,4,4,3,4,1,1,2,3,2,2,3,3,1,2,1,3,3,3,4,1,2,4,4,1,1,3,3,2,1,2,3,4,1,2,3,2,4,2,1,3,2,2,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,4,4,3,2,1,2,1,4,1,2,3,2,3,2,3,3,1,4,2,1,1,3,2,3,3,3,2,4,3,4,3,3,1,2,2,1,2,3,3,2,4,1,4,2,2,1,1,2,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,2,1,3,3,4,3,2,1,2,4,2,1,3,4,1,2,3,2,2,1,3,4,4,3,2,3,1,2,4,3,3,2,2,1,4,2,1,1,3,3,3,1,1,2,2,2,4,3,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,4,2,2,2,1,3,2,2,3,3,3,4,1,1,4,3,1,3,3,1,3,2,3,1,3,1,2,4,3,4,2,4,4,2,3,2,2,3,3,2,2,1,1,1,2,3,1,4,2,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,2,1,1,1,2,3,3,3,2,3,2,4,2,3,4,4,3,2,3,1,3,3,3,1,4,2,2,2,1,1,4,1,1,1,3,2,4,2,3,2,2,3,2,3,4,1,4,3,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,1,2,4,2,3,1,4,3,4,2,3,3,3,1,2,1,3,3,2,4,3,4,1,1,1,3,3,4,1,2,2,3,2,1,2,2,1,4,2,3,4,1,3,1,2,3,2,2,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,4,3,2,3,1,4,2,1,2,3,4,2,3,1,1,3,2,1,1,3,3,2,1,3,3,3,2,4,1,2,1,2,1,4,2,3,4,1,2,3,1,2,4,2,3,3,2,3,2,4,0,0,0,0,0,0,0,0,0,0,0},
        {0,4,4,3,2,1,3,1,2,2,2,2,3,4,3,1,3,1,2,1,4,3,2,4,3,3,2,3,2,1,2,2,2,3,3,4,3,4,3,1,1,2,1,2,4,3,3,1,2,1,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,4,2,3,3,3,2,2,2,1,1,1,3,4,3,2,4,3,2,3,3,1,4,2,2,2,2,1,1,2,3,1,4,3,2,4,1,1,3,3,2,3,1,4,3,1,2,3,4,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,3,2,1,3,3,1,4,4,3,2,4,2,3,2,2,1,2,1,4,3,3,2,3,1,2,1,3,2,1,4,4,3,1,4,2,1,4,3,1,3,1,2,3,2,2,2,3,2,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,3,2,1,2,1,3,3,3,4,4,4,3,2,2,1,2,1,3,2,4,3,1,2,3,4,1,3,1,3,2,2,2,3,2,3,2,1,3,4,2,3,4,2,3,1,1,1,4,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,3,4,3,1,3,1,3,1,2,2,3,2,4,1,2,4,2,1,2,4,3,2,3,3,3,3,2,4,3,4,1,4,3,2,3,2,1,1,2,2,1,2,1,1,3,1,3,4,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,4,1,3,2,4,2,4,3,2,3,1,3,1,3,2,2,1,1,1,3,1,2,4,3,2,2,1,1,2,2,3,1,2,2,3,3,4,2,3,1,4,2,3,4,1,3,2,4,3,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,1,3,3,1,4,3,2,4,3,2,3,1,1,2,2,4,3,3,1,4,2,2,4,2,4,3,1,3,3,3,3,1,4,3,2,3,1,1,1,1,2,2,3,2,2,4,1,2,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,4,1,1,3,1,3,4,2,3,2,3,2,4,1,2,2,3,1,3,3,3,1,2,4,4,2,3,2,3,2,1,1,3,2,4,2,2,1,4,1,1,3,2,2,1,4,3,2,3,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,2,1,2,1,3,2,3,4,1,1,4,2,4,3,3,2,2,4,3,4,3,3,2,1,4,2,2,1,2,1,1,3,3,4,1,3,4,1,3,2,3,2,2,2,1,1,2,3,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,3,1,3,2,2,1,3,2,1,1,4,4,3,4,2,3,1,2,3,1,3,4,3,3,2,1,1,3,1,4,1,2,2,3,4,4,3,2,1,2,2,3,2,4,1,2,3,2,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,1,2,3,2,4,3,1,2,2,3,1,4,3,3,4,1,1,2,3,2,3,2,3,2,1,3,3,2,1,2,3,3,2,1,4,4,1,3,3,3,2,4,4,1,2,1,4,1,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,2,4,3,2,2,3,1,4,3,3,3,2,2,1,1,4,4,2,1,1,3,4,2,2,3,3,2,1,3,1,4,1,4,1,3,3,2,1,2,3,2,3,2,4,1,2,3,2,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,4,2,3,4,3,2,1,3,1,2,1,2,2,3,1,3,4,2,3,3,2,3,1,3,1,4,2,1,3,4,2,3,2,4,1,2,2,2,4,3,4,2,3,1,3,1,2,1,1,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,4,3,1,1,3,2,2,3,2,4,3,3,1,4,1,2,3,1,1,2,2,1,2,3,3,2,1,2,2,4,3,3,3,1,1,4,1,3,2,3,1,2,2,4,4,2,3,4,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,3,1,2,1,1,2,4,2,1,2,3,4,4,3,2,3,2,2,3,2,3,2,1,4,4,1,1,3,2,3,1,1,4,2,1,1,2,2,4,2,3,1,3,2,3,4,3,3,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,2,4,2,3,2,1,4,2,1,4,2,1,3,1,3,3,4,1,4,2,1,2,3,4,3,1,4,3,1,2,1,2,1,2,4,2,3,1,3,3,3,2,2,2,3,3,3,1,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,2,3,4,4,2,2,2,2,1,3,3,1,1,4,1,3,3,3,2,2,2,3,3,1,2,1,1,4,2,3,1,3,4,3,3,4,2,4,2,2,1,1,1,3,1,2,2,4,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,3,3,2,1,1,3,4,2,2,1,2,4,3,4,1,2,3,3,4,2,2,1,1,2,2,3,1,3,3,2,2,2,2,4,1,3,2,1,3,3,3,4,4,2,3,1,4,1,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,3,4,1,1,2,2,3,3,3,1,3,1,4,2,4,2,3,1,3,3,1,3,2,1,2,3,4,3,1,2,3,3,4,1,3,3,2,2,4,4,4,2,2,1,2,2,2,1,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,2,3,4,3,4,2,3,2,4,2,1,3,2,1,1,3,4,3,2,2,3,3,1,1,1,2,3,1,1,3,1,2,4,2,3,3,1,2,2,3,2,1,2,2,3,4,4,3,4,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,3,2,2,1,3,2,2,3,4,4,4,1,1,1,3,3,3,2,1,4,3,2,3,2,4,1,3,2,3,4,1,1,3,3,1,1,4,3,1,2,4,3,2,1,2,3,2,2,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,4,2,1,2,4,1,2,3,3,4,2,2,3,1,3,3,3,1,2,2,1,3,3,3,2,4,3,3,1,2,1,2,2,2,4,2,4,1,2,3,4,4,3,3,1,2,3,1,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,2,1,3,1,3,4,2,2,2,1,4,1,3,4,3,2,2,2,1,3,2,1,2,1,2,4,1,3,2,2,3,3,2,1,2,3,1,4,4,3,3,4,2,4,1,3,3,1,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,2,2,4,3,1,4,2,1,1,3,4,1,3,3,3,2,2,2,3,3,3,3,1,4,4,3,1,1,1,2,4,3,3,2,2,3,2,3,2,3,4,2,1,2,1,2,1,4,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,4,3,3,1,3,1,1,4,2,2,4,2,2,3,1,3,1,3,3,1,3,3,1,3,3,2,3,2,3,2,2,4,2,4,3,1,2,3,1,2,1,1,4,4,2,2,1,2,4,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,1,1,3,4,3,1,1,3,4,3,3,4,2,2,2,2,2,1,3,4,4,3,2,1,1,3,2,3,3,4,2,1,1,2,3,3,3,2,2,1,3,4,3,2,2,2,4,1,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,3,3,4,2,1,2,2,2,1,3,2,1,3,4,4,3,1,2,2,1,4,2,2,4,4,1,3,1,1,3,3,3,4,3,3,3,2,1,2,1,1,3,3,2,2,2,4,2,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,2,1,4,3,1,2,3,4,3,3,3,2,1,4,2,2,2,1,2,2,3,3,2,4,2,2,4,3,3,3,1,3,2,1,4,1,3,1,3,3,2,4,2,4,2,3,1,1,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,1,4,3,1,1,4,3,2,3,3,3,2,4,2,1,2,3,3,2,3,1,4,3,2,2,3,1,2,2,1,4,4,3,2,3,2,1,1,1,2,3,3,2,3,2,4,1,1,4,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,1,2,3,1,3,2,2,1,4,3,3,2,2,4,4,3,4,1,3,2,1,3,2,4,2,1,4,3,2,3,1,2,3,1,2,1,2,3,3,4,3,3,4,2,1,2,3,1,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,4,3,3,2,2,2,1,2,3,1,1,3,3,4,2,4,1,2,1,2,3,1,2,2,3,4,2,3,3,4,3,4,2,4,2,3,3,2,2,1,1,3,1,3,1,2,3,1,4,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,3,2,4,1,3,4,2,1,3,4,3,2,1,3,2,1,1,1,2,3,2,2,1,3,3,1,2,1,3,3,2,4,1,2,3,3,1,2,3,4,4,3,3,2,2,1,2,4,4,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,2,4,1,3,3,1,2,1,2,3,3,2,2,4,3,4,4,3,1,2,2,4,1,1,4,1,2,2,2,1,3,1,2,3,3,2,3,2,1,3,4,3,2,1,4,3,2,3,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,3,3,2,4,1,3,4,1,1,2,3,3,1,2,4,2,4,1,2,4,4,2,1,3,3,1,3,2,2,1,2,4,2,3,4,3,1,3,2,3,2,3,1,1,3,3,1,2,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,1,3,2,1,4,2,3,3,1,4,3,2,4,3,2,1,2,1,1,3,4,3,2,2,1,1,2,1,1,2,4,3,2,3,3,1,4,2,3,3,3,4,4,2,3,1,2,2,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,4,4,1,3,2,2,2,2,1,3,3,1,4,3,3,2,1,3,2,1,3,2,2,4,2,2,1,3,1,4,3,2,2,1,3,1,4,2,2,3,1,4,3,1,3,3,3,4,1,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,4,3,1,2,1,4,2,1,2,4,2,1,2,3,3,3,2,3,3,3,3,2,3,2,2,4,2,3,4,1,1,4,2,3,1,2,3,4,1,1,1,1,1,3,2,3,2,2,4,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,1,3,2,3,4,1,1,4,2,3,3,2,2,2,1,4,2,2,4,2,3,3,2,4,3,4,3,2,1,1,2,1,4,3,3,2,3,2,1,3,4,1,1,2,1,3,2,1,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,3,3,1,2,2,2,4,1,3,2,1,2,3,4,1,4,2,1,4,3,2,2,3,2,2,1,2,2,1,1,1,1,1,3,3,2,2,4,4,4,3,3,2,4,3,3,3,1,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,3,4,2,1,1,2,4,2,1,3,4,3,3,2,3,2,2,2,3,4,3,2,2,1,2,3,3,4,3,2,3,4,1,3,4,1,3,1,2,2,3,1,3,2,1,1,2,1,4,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,3,4,4,4,2,1,2,3,2,2,3,1,3,1,2,1,4,4,1,3,2,2,2,3,3,3,3,2,4,1,3,1,3,2,3,2,1,1,1,1,2,3,4,2,4,2,1,3,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,2,3,2,1,3,4,3,2,2,4,4,1,3,3,2,1,1,2,2,1,3,3,4,4,1,3,3,2,2,4,1,3,2,2,3,4,1,2,2,3,3,3,2,1,1,2,3,4,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,3,1,2,4,2,2,4,3,4,3,2,2,3,1,1,3,3,2,1,3,4,3,4,4,2,3,2,3,2,1,1,4,1,3,4,1,3,2,2,1,2,3,2,3,2,1,2,1,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,4,2,1,1,4,3,3,3,2,3,4,3,2,1,1,2,1,2,2,4,2,2,2,4,1,2,3,4,1,1,3,1,3,3,4,3,1,4,1,3,2,3,3,1,2,2,3,3,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,2,3,4,2,3,2,1,1,3,3,2,4,1,3,4,1,2,2,3,4,4,2,3,3,1,3,4,1,1,4,2,2,3,2,1,3,4,1,2,2,3,2,1,3,3,1,2,3,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,3,4,2,3,2,2,4,3,1,2,1,2,4,1,3,3,4,1,1,3,3,3,3,2,1,1,1,4,2,3,4,2,2,2,3,1,3,2,4,2,4,2,3,2,1,1,2,3,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,1,2,2,3,2,3,3,3,4,2,4,2,1,4,1,1,4,2,3,1,4,3,3,3,3,1,3,2,1,4,1,2,1,2,2,3,4,1,3,2,2,2,3,1,4,1,2,3,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,1,3,3,3,1,2,4,2,3,2,2,3,2,1,4,4,2,4,3,3,4,2,2,3,1,2,1,2,1,3,3,3,4,2,2,4,2,1,1,3,4,1,3,1,2,2,1,3,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,4,4,2,2,2,1,3,2,4,3,3,3,1,1,3,1,2,2,1,3,3,2,3,3,3,2,4,4,1,4,2,2,2,2,3,1,1,3,3,1,4,3,1,1,1,2,4,3,2,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,4,1,3,2,4,2,2,2,4,1,3,3,1,2,1,3,3,1,3,4,1,2,2,3,3,3,3,1,4,4,3,1,1,3,2,3,2,2,2,1,2,2,4,1,2,3,2,3,1,4,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,1,1,3,4,3,2,1,3,3,2,2,2,3,4,1,4,3,2,1,2,1,3,1,4,4,3,3,2,2,1,1,2,2,4,3,3,1,4,3,1,2,2,2,3,3,2,1,4,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,1,3,4,2,2,2,3,2,3,4,4,3,3,1,2,1,3,2,2,3,3,2,4,2,4,1,3,2,2,1,2,3,3,3,4,1,1,4,3,1,3,1,2,4,2,2,1,1,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,2,3,4,1,3,3,4,2,3,1,1,3,2,4,1,2,3,4,4,2,1,1,4,1,3,1,2,3,3,2,1,3,1,2,2,2,2,3,3,3,3,2,1,4,4,2,3,2,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,3,2,2,2,3,1,3,4,2,1,3,1,4,4,2,1,1,3,1,2,2,2,3,4,2,3,3,1,3,1,2,3,4,4,2,4,3,2,4,3,2,2,1,3,1,1,1,3,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,2,2,3,3,2,3,3,2,4,1,4,4,1,2,3,1,1,1,2,2,2,2,2,4,3,2,3,3,3,4,4,1,3,4,3,3,3,1,2,2,4,1,1,2,3,2,3,1,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,2,3,3,4,4,2,1,3,3,2,1,1,2,2,1,4,2,4,2,2,1,1,3,2,3,1,2,4,2,3,2,3,3,2,1,2,4,1,3,3,1,3,3,1,2,4,4,3,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,2,3,2,3,2,2,4,3,1,3,1,2,3,4,4,1,2,2,1,2,1,4,4,3,3,2,1,2,2,3,3,3,4,2,1,4,2,3,4,3,2,1,2,3,1,1,3,3,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,3,3,1,2,4,1,3,3,3,4,1,1,2,4,2,2,1,2,1,1,3,1,1,2,2,3,4,1,1,3,3,3,2,3,4,2,4,3,1,3,2,2,2,4,3,4,2,2,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,4,2,1,1,3,1,3,2,2,4,3,2,2,3,1,3,4,3,1,4,2,2,4,2,3,2,1,2,3,2,3,1,1,1,3,4,2,2,1,3,1,2,3,3,4,4,1,3,2,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,4,1,2,2,1,2,3,3,4,4,1,1,3,2,3,2,3,4,3,1,2,2,3,4,1,4,2,2,4,4,2,3,3,3,3,3,3,1,1,2,1,2,2,3,1,2,1,1,3,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,1,3,2,3,4,2,3,4,2,2,1,2,3,1,4,3,1,4,2,2,3,4,1,1,1,2,3,1,3,3,2,2,4,4,3,2,1,1,3,3,1,3,3,2,2,2,3,1,2,4,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,1,2,4,3,2,3,1,3,3,1,2,3,4,4,1,2,2,2,1,1,3,2,2,4,3,4,4,1,3,2,3,3,4,4,2,3,2,2,3,1,3,3,1,1,1,1,2,2,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,1,2,3,3,3,3,4,4,4,2,1,2,1,2,1,2,3,4,2,3,3,1,1,1,2,1,3,1,3,3,1,2,1,3,3,2,2,4,2,2,3,4,2,1,2,4,4,2,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,1,1,3,3,2,4,3,3,4,1,4,2,3,2,2,1,1,2,2,4,1,2,3,2,1,1,2,2,1,3,4,3,3,3,2,1,3,3,2,1,4,3,4,1,3,2,3,2,4,0,0,0,0,0,0,0,0,0,0,0},
        {0,4,1,1,3,1,2,3,3,3,1,2,2,2,4,2,3,4,3,3,3,4,1,1,2,4,1,3,2,2,1,2,3,1,4,1,2,3,4,3,2,4,2,1,2,2,1,3,3,2,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,2,2,1,3,3,2,3,4,1,4,2,3,1,2,1,4,2,3,3,4,1,2,1,1,1,2,4,1,3,2,3,3,1,4,2,2,3,2,3,1,2,3,3,4,4,2,2,1,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,1,4,3,1,3,2,2,1,1,3,4,4,2,3,2,2,1,3,3,1,2,4,1,4,3,2,1,2,2,2,3,2,3,4,4,2,3,3,1,3,4,1,1,2,3,2,1,2,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,2,3,3,2,3,1,3,4,1,2,1,2,4,4,1,2,3,2,4,3,3,3,3,2,1,1,4,1,3,2,3,4,3,2,3,2,2,4,1,1,2,4,2,3,1,2,1,3,2,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,4,1,4,2,4,1,2,3,3,1,3,3,1,2,2,2,3,2,3,1,2,4,2,2,3,1,1,3,2,2,3,4,1,2,3,4,4,3,1,1,2,1,2,4,1,3,3,2,3,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,4,2,1,4,2,3,1,1,2,2,4,3,1,3,3,2,4,3,1,4,2,3,1,2,3,2,3,1,2,3,2,2,3,3,2,2,4,3,2,1,1,4,1,1,3,2,4,3,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,2,2,3,2,3,1,4,4,1,3,2,1,4,3,2,1,4,2,3,2,3,1,1,3,2,2,4,3,2,3,1,1,1,1,2,4,3,3,1,2,4,3,3,4,2,2,1,3,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,4,3,1,2,1,1,4,3,2,3,3,1,4,3,2,2,2,3,1,4,1,3,3,2,2,4,1,1,1,4,3,3,3,3,2,1,4,2,2,2,1,2,3,2,2,3,1,4,3,2,0,0,0,0,0,0,0,0,0,0,0},
        {0,4,2,2,2,1,2,1,3,4,3,3,1,4,3,3,2,1,1,4,1,2,2,3,3,1,3,1,3,4,2,2,2,2,4,2,3,1,1,2,3,4,1,3,1,3,2,4,2,3,3,0,0,0,0,0,0,0,0,0,0,0},
        {0,3,2,2,3,4,3,2,1,1,4,1,3,3,2,1,4,2,3,2,2,4,3,2,4,1,2,2,2,3,1,3,4,3,3,1,1,4,2,3,3,2,3,2,1,2,1,1,3,4,1,0,0,0,0,0,0,0,0,0,0,0}
    };

    public ResourceManager()
    {
        instance = this;
        productData = CSVReader.Read("CSV/Product", true);
        categoryData = CSVReader.Read("CSV/Category", true);
        supplyData = CSVReader.Read("CSV/Supply", true);
        demandShiftData = CSVReader.Read("CSV/Demand Shift", true);
        statusData = CSVReader.Read("CSV/Status", true);
        animalData = CSVReader.Read("CSV/Animal", true);
        sizeData = CSVReader.Read("CSV/Size", true);
        weatherData = CSVReader.Read("CSV/Weather", true);
        textData = CSVReader.Read("CSV/Text", true);
        itemData = CSVReader.Read("CSV/Item", true);
        gatherNodeData = CSVReader.Read("CSV/GatherNode", true);
        urlData = CSVReader.Read("CSV/URL", true);
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
        itemData.Clear();
        itemData = null;
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
            mat.index = new int[3];
            mat.quantity = new int[3];
            mat.index[2] = index;
            mat.quantity[2] = (int)productData[product]["Material Required 3"];
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
            pos = new Vector2(float.Parse(animalData[index]["X"].ToString()), float.Parse(animalData[index]["Y"].ToString()));
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

    public string GetSize(int index) => sizeData[index][UserManager.instance.GetLanguage().ToString()].ToString();

    public string GetWeather(int index) => weatherData[index][UserManager.instance.GetDataLanguage().ToString()].ToString();

    public string GetText(int index) => textData[index][UserManager.instance.GetLanguage().ToString()].ToString();

    public byte GetStatusData(int line, int index) => popularityData[line, index + 1];

    public string GetItemName(int index) => itemData[index][UserManager.instance.GetDataLanguage().ToString()].ToString();

    public int GetItemMax() => itemData.Count;

    public void GetGatherNodeItems(int index, out int item1, out int item2)
    {
        item1 = (int)gatherNodeData[index]["Item1"];
        if (gatherNodeData[index]["Item2"].ToString().Length > 0)
            item2 = (int)gatherNodeData[index]["Item2"];
        else
            item2 = -1;
    }

    public string GetGatherNodeName(int index) => gatherNodeData[index][UserManager.instance.GetDataLanguage().ToString()].ToString();

    public string GetURL(int index) => urlData[index]["URL"].ToString();

    IEnumerator RequestSheetData()
    {
        supplyPattern?.Clear();
        UnityWebRequest data = UnityWebRequest.Get(GetURL(0));
        yield return data.SendWebRequest();
        if (data.result == UnityWebRequest.Result.ConnectionError)
            yield break;
        string file = Regex.Replace(data.downloadHandler.text, "📦", "");
        int removeSize = file.IndexOf("ID,Item");
        string[] value = Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(file.Remove(0, removeSize), @"\bNonexistent\b", "0"), @"\bInsufficient\b", "1"), @"\bSufficient\b", "2"), @"\bUnpredictable\b", "-1"), @"\bPeak\b", "-2"), @"\bHere Or Cycle 4\b", "-3"), @"\bHere Or Cycle 5\b", "-3"), @"\bHere Or Cycle 6\b", "-4"), @"\bHere Or Cycle 7\b", "-4").Split('\n');
        string result = "ID,Item,Supply1,Shift1,Supply2,Shift2,Supply3,Shift3,Supply4,Shift4,Prediction2,Prediction3,Prediction4,Prediction5,Prediction6,Prediction7,ItemP,Pattern,Popularity,Next Popularity,Patterns identified";
        for (int i = 0; i < 50; ++i)
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

    public void UpdateSupplyPattern()
    {
        SystemCore.instance.StartCoroutine(RequestSheetData());
    }

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

    public void UpdateVersion()
    {
        SystemCore.instance.StartCoroutine(RequestUpdateVersion());
    }

    public List<Dictionary<string, object>> GetSupplyPattern() => supplyPattern;
}
