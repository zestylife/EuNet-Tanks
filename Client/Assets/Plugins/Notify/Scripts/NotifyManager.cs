using UnityEngine;

public class NotifyManager : MonoBehaviour
{
    [SerializeField] private UiTipBox _tipBox;

    private static NotifyManager s_instance;
    public static NotifyManager Instance
    {
        get
        {
            if (s_inited == false)
                s_instance = FindObjectOfType<NotifyManager>();

            return s_instance;
        }
    }
    private static bool s_inited = false;

    protected virtual void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        s_inited = true;
    }

    private void Start()
    {
        _tipBox.gameObject.SetActive(false);
    }

    public void ShowTip(string tip)
    {
        _tipBox.ShowTip(tip);
    }
}
