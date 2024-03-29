using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class Gather : MonoBehaviour
{
    public static Gather instance;

    public Transform transPivot;
    public ScrollRect scrollGather;
    public Text textItem;

    List<GatherData> gatherDataList = new List<GatherData>();
    List<GatherNode> gatherNodeList = new List<GatherNode>();

    int item = -1;

    private void Awake()
    {
        instance = this;
        List<Dictionary<string, object>> gatherList = CSVReader.Read("CSV/Gather", true);
        gatherList.ForEach((form) =>
        {
            GatherData data = Instantiate(Resources.Load<GatherData>("Prefab/Gather/GatherData"), scrollGather.content);
            data.SetDefaultData((int)form["Item"]);
            gatherDataList.Add(data);
        });
        gatherList.Clear();
        gatherList = null;
        List<Dictionary<string, object>> nodeList = CSVReader.Read("CSV/GatherNodeList", true);
        nodeList.ForEach((form) =>
        {
            GatherNode node = Instantiate(Resources.Load<GatherNode>("Prefab/Gather/GatherNode"), transPivot);
            node.SetDefaultData(new Vector2(float.Parse(form["X"].ToString(), CultureInfo.InvariantCulture), float.Parse(form["Y"].ToString(), CultureInfo.InvariantCulture)), (int)form["Node"]);
            gatherNodeList.Add(node);
        });
        nodeList.Clear();
        nodeList = null;
        ApplyLanguage();
    }

    public bool ShowNode(int _item)
    {
        if (item != _item)
        {
            if (item != -1)
            {
                gatherDataList.ForEach((form) =>
                {
                    form.SetNormalColor(item);
                });
            }
            item = _item;
        }
        else
            item = -1;
        gatherNodeList.ForEach((form) =>
        {
            form.SetHighlight(item);
        });
        return item == -1 ? false : true;
    }

    public void ApplyLanguage()
    {
        textItem.text = ResourceManager.instance.GetText(41);
        gatherDataList.ForEach((form) =>
        {
            form.ApplyLanguage();
        });
    }
}
