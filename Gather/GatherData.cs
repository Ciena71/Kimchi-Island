using UnityEngine;
using UnityEngine.UI;

public class GatherData : MonoBehaviour
{
    public Image imgBG;
    public Button btnBG;
    public Image imgItem;
    public Text textItem;

    int item;

    public void SetDefaultData(int _item)
    {
        item = _item;
        imgItem.sprite = Resources.Load<Sprite>($"Sprite/Material/M{_item}");
        ApplyLanguage();
        btnBG.onClick.AddListener(() =>
        {
            if (Gather.instance.ShowNode(item))
                imgBG.color = Color.yellow;
            else
                imgBG.color = Color.white;
        });
    }

    public void SetNormalColor(int _item)
    {
        if (item == _item)
            imgBG.color = Color.white;
    }

    public void ApplyLanguage() => textItem.text = ResourceManager.instance.GetMaterialName(item);
}
