using UnityEngine;
using UnityEngine.UI;

public class GatherNode : MonoBehaviour
{
    public Image imgIcon;

    int[] items = new int[2];

    public void SetDefaultData(Vector2 pos, int _node)
    {
        transform.localPosition = new Vector2(1500.0f * (pos.x - 1) / 41, -1500.0f * (pos.y - 1) / 41);
        ResourceManager.instance.GetGatherNodeItems(_node, out items[0], out items[1]);
    }

    public void SetHighlight(int item)
    {
        if(item == items[0] || (items[1] >= 0 && item == items[1]))
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }
}
