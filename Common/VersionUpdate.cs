using UnityEngine;
using UnityEngine.UI;

public class VersionUpdate : MonoBehaviour
{
    public static VersionUpdate instance;
    public Text textVersion;
    public Text textDes;
    public Button btnDownload;
    public Text textDownload;
    public Button btnCancel;
    public Text textCancel;

    private void Awake()
    {
        instance = this;
        btnCancel.onClick.AddListener(() => gameObject.SetActive(false));
        ApplyLanguage();
    }

    public void SetData(string version,string url)
    {
        textVersion.text = "Version : " + version;
        btnDownload.onClick.RemoveAllListeners();
        btnDownload.onClick.AddListener(() =>
        {
            Application.OpenURL(url);
            Application.Quit();
        });
        gameObject.SetActive(true);
    }

    public void ApplyLanguage()
    {
        ResourceManager resourceManager = ResourceManager.instance;
        textDes.text = resourceManager.GetText(36);
        textDownload.text = resourceManager.GetText(37);
        textCancel.text = resourceManager.GetText(38);
    }
}
