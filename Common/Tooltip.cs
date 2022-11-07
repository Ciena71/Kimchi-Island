using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public static Tooltip instance;

    public RectTransform rectTooltip;
    public GameObject objTitle;
    public Image imgIcon;
    public Text textTitle;
    public GameObject objDescription;
    public TextMeshProUGUI textDescription;
    public GameObject[] objTag = new GameObject[6];
    public Text[] textTagTitle = new Text[6];
    public Text[] textTagDescription = new Text[6];

    RectTransform toolTipPivot;

    void Awake()
    {
        instance = this;
        toolTipPivot = GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        Vector2 Res;
        Res.x = Screen.width;
        Res.y = Screen.height;
        Vector3 dummyRes = Input.mousePosition;
        float x = (dummyRes.x > Res.x - rectTooltip.sizeDelta.x) ? 1 : 0;
        float y = (dummyRes.y > Res.y - rectTooltip.sizeDelta.y) ? 1 : 0;
        toolTipPivot.pivot = new Vector2(x, y);
        transform.position = dummyRes;
    }

    public void ShowToolTip(Sprite _icon, string _title, string[] _tagTitle, string[] _tagDes)
    {
        objDescription.SetActive(false);
        imgIcon.sprite = _icon;
        textTitle.text = _title;
        objTitle.SetActive(true);
        rectTooltip.sizeDelta = new Vector2(300, 105 + 49 * _tagTitle.Length);
        for (int i = 0; i < _tagTitle.Length; ++i)
        {
            textTagTitle[i].text = _tagTitle[i];
            textTagDescription[i].text = _tagDes[i];
            objTag[i].SetActive(true);
        }
        for (int i = _tagTitle.Length; i < 6; ++i)
            objTag[i].SetActive(false);
        gameObject.SetActive(true);
    }

    public void ShowToolTip(string _text)
    {
        objTitle.SetActive(false);
        textDescription.text = _text;
        objDescription.SetActive(true);
        for (int i = 0; i < textTagTitle.Length; ++i)
            objTag[i].SetActive(false);
        rectTooltip.sizeDelta = new Vector2(350, 400);
        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
    }
}
