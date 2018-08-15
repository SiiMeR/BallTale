using UnityEngine;

public abstract class Menu<T> : Menu where T : Menu<T>
{
    public static T Instance { get; private set; }

    protected static void Open()
    {
        if (Instance == null) MenuManager.Instance.CreateInstance<T>();
        else Instance.gameObject.SetActive(true);


        MenuManager.Instance.OpenMenu(Instance);
    }

    protected static void Close()
    {
        if (Instance == null)
        {
            Debug.LogError("No instance of menu to close");
            return;
        }

        MenuManager.Instance.CloseMenu(Instance);
    }

    protected virtual void Awake()
    {
        Instance = (T) this;
    }

    protected virtual void OnDestroy()
    {
        Instance = null;
    }

    public override void OnBackPressed()
    {
        Close();
    }
}

public abstract class Menu : MonoBehaviour
{
    public bool DestroyWhenClosed = true;

    public bool DisableMenusUnderneath = true;


    public abstract void OnBackPressed();
}