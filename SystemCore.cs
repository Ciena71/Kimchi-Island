using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;

public class SystemCore : MonoBehaviour
{
    public static SystemCore instance;

    public Text textWorkshop;
    public Toggle toggleWorkshop;
    public Text textWorkshopSchedule;
    public Toggle toggleWorkshopSchedule;
    public Text textAnimal;
    public Toggle toggleAnimal;
    public Text textGather;
    public Toggle toggleGather;
    public Text textInventory;
    public Toggle toggleInventory;
    public Text textOption;
    public Toggle toggleOption;
    public Text textEorzeaTimeHour;
    public Text textEorzeaTimeMinute;
    public Image imgWeather;
    public Text textWeather;
    public AudioSource audioSource;

    GameObject objWorkshop;
    GameObject objWorkshopSchedule;
    GameObject objAnimal;
    GameObject objGather;
    GameObject objInventory;
    GameObject objOption;

    Eorzea eorzea;

#region WIN32API
    public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    public static readonly IntPtr HWND_NOT_TOPMOST = new IntPtr(-2);
    const UInt32 SWP_SHOWWINDOW = 0x0040;

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left, Top, Right, Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public RECT(System.Drawing.Rectangle r)
            : this(r.Left, r.Top, r.Right, r.Bottom)
        {
        }

        public int X
        {
            get
            {
                return Left;
            }
            set
            {
                Right -= (Left - value);
                Left = value;
            }
        }

        public int Y
        {
            get
            {
                return Top;
            }
            set
            {
                Bottom -= (Top - value);
                Top = value;
            }
        }

        public int Height
        {
            get
            {
                return Bottom - Top;
            }
            set
            {
                Bottom = value + Top;
            }
        }

        public int Width
        {
            get
            {
                return Right - Left;
            }
            set
            {
                Right = value + Left;
            }
        }

        public static implicit operator System.Drawing.Rectangle(RECT r)
        {
            return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
        }

        public static implicit operator RECT(System.Drawing.Rectangle r)
        {
            return new RECT(r);
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

#endregion

    void Start()
    {
        instance = this;
        ResourceManager resourceManager = new ResourceManager();
        UserManager userManager = new UserManager();
        eorzea = new Eorzea(textEorzeaTimeHour, textEorzeaTimeMinute, imgWeather, textWeather);
        objWorkshop = Instantiate(Resources.Load<GameObject>("Prefab/Workshop/WorkshopList"), transform.GetChild(0));
        objWorkshopSchedule = Instantiate(Resources.Load<GameObject>("Prefab/WorkshopSchedule/WorkshopSchedule"), transform.GetChild(1));
        objWorkshopSchedule.SetActive(false);
        objAnimal = Instantiate(Resources.Load<GameObject>("Prefab/Animal/AnimalList"), transform.GetChild(2));
        objAnimal.SetActive(false);
        objGather = Instantiate(Resources.Load<GameObject>("Prefab/Gather/GatherList"), transform.GetChild(3));
        objGather.SetActive(false);
        objInventory = Instantiate(Resources.Load<GameObject>("Prefab/Inventory/InventoryList"), transform.GetChild(4));
        objInventory.SetActive(false);
        objOption = Instantiate(Resources.Load<GameObject>("Prefab/Option/OptionList"), transform.GetChild(5));
        objOption.SetActive(false);
        GameObject objTooltip = Instantiate(Resources.Load<GameObject>("Prefab/Common/ToolTip"), transform);
        objTooltip.SetActive(false);
        GameObject objUpdate = Instantiate(Resources.Load<GameObject>("Prefab/Common/VersionUpdate"), transform);
        objUpdate.SetActive(false);
        toggleWorkshop.onValueChanged.AddListener((trigger) =>
        {
            objWorkshop.SetActive(trigger);
        });
        toggleWorkshopSchedule.onValueChanged.AddListener((trigger) =>
        {
            objWorkshopSchedule.SetActive(trigger);
        });
        toggleAnimal.onValueChanged.AddListener((trigger) =>
        {
            objAnimal.SetActive(trigger);
        });
        toggleGather.onValueChanged.AddListener((trigger) =>
        {
            objGather.SetActive(trigger);
        });
        toggleInventory.onValueChanged.AddListener((trigger) =>
        {
            objInventory.SetActive(trigger);
        });
        toggleOption.onValueChanged.AddListener((trigger) =>
        {
            objOption.SetActive(trigger);
        });
        SetAlarmVolume(userManager.GetAlarmVolume());
        SetAlarm(userManager.GetAlarm());
    }

    private void OnDestroy()
    {
        eorzea.Dispose();
    }

    public void SetAlarmVolume(int volume) => audioSource.volume = volume / 100.0f;

    public void SetAlarm(int value)
    {
        switch(value)
        {
        case 0:
            {
                audioSource.clip = Resources.Load<AudioClip>("Audio/FFXIV");
                break;
            }
        case 1:
            {
                audioSource.clip = Resources.Load<AudioClip>("Audio/FFVII");
                break;
            }
        case 2:
            {
                audioSource.clip = Resources.Load<AudioClip>("Audio/Pingu");
                break;
            }
        case 3:
            {
                audioSource.clip = Resources.Load<AudioClip>("Audio/Zelda");
                break;
            }
        case 4:
            {
                audioSource.clip = Resources.Load<AudioClip>("Audio/Metal Gear");
                break;
            }
        case 5:
            {
                audioSource.clip = Resources.Load<AudioClip>("Audio/FBI");
                break;
            }
        }
    }

    public void PlayAlarm() => audioSource.Play();

    private void Update()
    {
        eorzea.Update();
    }

    public void SendDebugLog(string data) => Debug.Log(data);

    public void ApplyLanguage()
    {
        ResourceManager resourceManager = ResourceManager.instance;
        textWorkshop.text = resourceManager.GetText(0);
        textWorkshopSchedule.text = resourceManager.GetText(34);
        textAnimal.text = resourceManager.GetText(21);
        textGather.text = resourceManager.GetText(40);
        textInventory.text = resourceManager.GetText(46);
        textOption.text = resourceManager.GetText(16);
    }

    public string GetVersion() => Application.version;

    public bool AssignTopmostWindow(string WindowTitle, bool MakeTopmost)
    {
        IntPtr hWnd = FindWindow(null, WindowTitle);

        GetWindowRect(new HandleRef(this, hWnd), out RECT rect);

        return SetWindowPos(hWnd, MakeTopmost ? HWND_TOPMOST : HWND_NOT_TOPMOST, rect.X, rect.Y, rect.Width, rect.Height, SWP_SHOWWINDOW);
    }
}
