using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private Selectable[] selectables;
    protected Selectable[] Selectables
    {
        get
        {
            if (selectables == null || selectables.Length == 0 || selectables.Contains(null))
            {
                selectables = GetComponentsInChildren<Selectable>();
            }
            return selectables;
        }

        private set => selectables = value;
    }

    [Tooltip("If a parent menu is set, this menu will be closed when the parent is.")]
    [SerializeField] Menu parent;
    public bool isOpen = true;

    [Header("Events")]
    [Tooltip("This event is called when the menu is opened.")]
    public UnityEvent onMenuOpened;
    [Tooltip("This event is called when the menu is closed.")]
    public UnityEvent onMenuClosed;
    [Space]
    [Tooltip("This event is called when the menu is shown or opened.")]
    public UnityEvent onMenuShown;
    [Tooltip("This event is called when the menu is hidden or closed.")]
    public UnityEvent onMenuHidden;

    protected virtual void Start()
    {
        Selectables = GetComponentsInChildren<Selectable>();

        if (isOpen)
        {
            ForceOpenMenu(force: true);
        }
        else
        {
            ForceCloseMenu(force: true);
        }
    }

    // ContextMenu means you can run this method in the inspector by right-clicking the component
    [ContextMenu("Open Menu")]
    public void OpenMenu() => ForceOpenMenu(force: false);

    private void ForceOpenMenu(bool force = false)
    {
        if (!isOpen || force) // Only open the menu if we are closed or if we are force it
        {
            isOpen = true;
            ShowMenu();
            onMenuOpened.Invoke();
        }
    }

    [ContextMenu("Close Menu")]
    public void CloseMenu() => ForceCloseMenu(force: false);

    public void ForceCloseMenu(bool force = false)
    {
        if (isOpen || force) // Only close the menu if we are open or if we force it
        {
            isOpen = false;
            HideMenu();
            onMenuClosed.Invoke();

            // Close all menus with this as its parent
            foreach (var child in FindObjectsOfType<Menu>())
            {
                if (child.gameObject.activeInHierarchy && child.parent == this)
                {
                    child.CloseMenu();
                }
            }
        }
    }

    [ContextMenu("Show Menu")]
    public void ShowMenu()
    {
        SetSelectables(true);
        onMenuShown.Invoke();
    }

    [ContextMenu("Hide Menu")]
    public void HideMenu()
    {
        SetSelectables(false);
        onMenuHidden.Invoke();
    }

    public void ToggleMenu()
    {
        if (isOpen)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    // Set all ui elements in the menu be interactable or not
    protected virtual void SetSelectables(bool state)
    {
        if (Selectables.Length > 0)
        {
            foreach (var selectable in Selectables)
            {
                selectable.interactable = state;
            }
        }
    }
}
