using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;

public class Option : MonoBehaviour
{
    public static Option instance;

    public Text textLanguage;
    public Dropdown dropdownLanguage;
    public Text textDataLanguage;
    public Dropdown dropdownDataLanguage;
    public Text textAlwaysOnTop;
    public Toggle toggleAlwaysOnTop;
    public Text textAlarmVolume;
    public Slider sliderAlarmVolume;
    public Text textAlarm;
    public Dropdown dropdownAlarm;
    public Text textAlarmVolumeValue;
    public Text textMaker;
    public Text textVersion;
    public Button btnUpdate;
    public Text textUpdate;
    public Button btnPatchNote;
    public Text textPatchNote;
    public GameObject objPatchNote;
    public Text textPatchNoteTop;
    public Button btnPatchNoteClose;
    public Text textPatchNoteText;
    public Button btnManual;
    public Text textManual;
    public Text textKimchiMaker;
    public Button btnKimchiMakerWindowsDownload;
    public Text textKimchiMakerWindowsDownload;
    public Button btnKimchiMakerAndroidDownload;
    public Text textKimchiMakerAndroidDownload;
    public Button btnSupplyCopy;
    public Text textSupplyCopy;
    public Button btnPopularityCopy;
    public Text textPopularityCopy;
    public Dropdown dropdownPacket;
    public Text textPath;
    public Button btnPath;
    public Text textGamePath;

    void Awake()
    {
        instance = this;
        UserManager userManager = UserManager.instance;
        switch (userManager.GetLanguage())
        {
        case SystemLanguage.Japanese:
            {
                dropdownLanguage.value = 1;
                break;
            }
        case SystemLanguage.Korean:
            {
                dropdownLanguage.value = 2;
                break;
            }
        }
        dropdownLanguage.onValueChanged.AddListener((value) =>
        {
            switch (value)
            {
            case 0:
                {
                    userManager.SetLanguage(SystemLanguage.English);
                    break;
                }
            case 1:
                {
                    userManager.SetLanguage(SystemLanguage.Japanese);
                    break;
                }
            case 2:
                {
                    userManager.SetLanguage(SystemLanguage.Korean);
                    break;
                }
            }
        });
        switch (userManager.GetDataLanguage())
        {
        case SystemLanguage.Japanese:
            {
                dropdownDataLanguage.value = 1;
                break;
            }
        case SystemLanguage.Korean:
            {
                dropdownDataLanguage.value = 2;
                break;
            }
        }
        dropdownDataLanguage.onValueChanged.AddListener((value) =>
        {
            switch (value)
            {
            case 0:
                {
                    userManager.SetDataLanguage(SystemLanguage.English);
                    break;
                }
            case 1:
                {
                    userManager.SetDataLanguage(SystemLanguage.Japanese);
                    break;
                }
            case 2:
                {
                    userManager.SetDataLanguage(SystemLanguage.Korean);
                    break;
                }
            }
        });
        dropdownPacket.value = userManager.GetPacketType();
        dropdownPacket.onValueChanged.AddListener((value) =>
        {
            userManager.SetPacketType(value);
        });
        toggleAlwaysOnTop.isOn = userManager.GetAlwaysOnTop();
        toggleAlwaysOnTop.onValueChanged.AddListener((value) =>
        {
            userManager.SetAlwaysOnTop(value);
        });
        sliderAlarmVolume.value = userManager.GetAlarmVolume();
        textAlarmVolumeValue.text = userManager.GetAlarmVolume().ToString();
        sliderAlarmVolume.onValueChanged.AddListener((value) =>
        {
            textAlarmVolumeValue.text = value.ToString();
            userManager.SetAlarmVolume((int)value);
        });
        dropdownAlarm.value = userManager.GetAlarm();
        dropdownAlarm.onValueChanged.AddListener((value) =>
        {
            userManager.SetAlarm(value);
            SystemCore.instance.PlayAlarm();
        });
        textVersion.text = Application.version;
        ResourceManager resourceManager = ResourceManager.instance;
        btnUpdate.onClick.AddListener(() => resourceManager.UpdateVersion());
        btnPatchNote.onClick.AddListener(() =>
        {
            textPatchNoteText.text = resourceManager.GetPatchNote();
            objPatchNote.SetActive(true);
        });
        btnPatchNoteClose.onClick.AddListener(() => objPatchNote.SetActive(false));
        btnManual.onClick.AddListener(() => Application.OpenURL(resourceManager.GetURL(2)));
        btnKimchiMakerWindowsDownload.onClick.AddListener(() => Application.OpenURL(resourceManager.GetURL(3)));
        btnKimchiMakerAndroidDownload.onClick.AddListener(() => Application.OpenURL(resourceManager.GetURL(4)));
        btnSupplyCopy.onClick.AddListener(() => Workshop.instance.SupplyPacketDataCopy());
        btnPopularityCopy.onClick.AddListener(() => Workshop.instance.PopularityPacketDataCopy());
        FileBrowser.SetFilters(true, new FileBrowser.Filter(".exe file", ".exe"));
        btnPath.onClick.AddListener(() => StartCoroutine(FindPath()));
        textGamePath.text = userManager.GetGamePath();
        ApplyLanguage();
    }

    IEnumerator FindPath()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Load Path", "Load");
        if (FileBrowser.Success)
        {
            if (FileBrowser.Result != null)
            {
                string result = FileBrowser.Result[0];
                string[] all = result.Split('\\');
                string last = all[all.Length - 1];
                if(last == "ffxiv_dx11.exe")
                {
                    textGamePath.text = result;
                    UserManager.instance.SetGamePath(result);
                }
            }
        }
    }

    public void ApplyLanguage()
    {
        ResourceManager resourceManager = ResourceManager.instance;
        textLanguage.text = resourceManager.GetText(17);
        textDataLanguage.text = resourceManager.GetText(33);
        textMaker.text = resourceManager.GetText(18);
        textAlwaysOnTop.text = resourceManager.GetText(20);
        textAlarmVolume.text = resourceManager.GetText(24);
        textAlarm.text = resourceManager.GetText(24);
        textUpdate.text = resourceManager.GetText(39);
        textPatchNote.text = resourceManager.GetText(49);
        textPatchNoteTop.text = resourceManager.GetText(49);
        textManual.text = resourceManager.GetText(42);
        textKimchiMaker.text = resourceManager.GetText(43);
        textKimchiMakerWindowsDownload.text = resourceManager.GetText(44);
        textKimchiMakerAndroidDownload.text = resourceManager.GetText(44);
        textSupplyCopy.text = $"{resourceManager.GetText(12)} {resourceManager.GetText(45)}";
        textPopularityCopy.text = $"{resourceManager.GetText(11)} {resourceManager.GetText(45)}";
        textPath.text = resourceManager.GetText(48);
    }
}
