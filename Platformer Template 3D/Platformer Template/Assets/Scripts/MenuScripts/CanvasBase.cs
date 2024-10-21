using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public abstract class CanvasBase : MonoBehaviour
{
    #region Serialized Fields

    #region Components of the Canvas
    [field:Header("Components of the Canvas")]
    [field:SerializeField, Tooltip("Place Buttons you wish to change in here so the canvas has access to it.")] 
    public Button[] Buttons { get; internal set; }

    [field:SerializeField, Tooltip("Place Toggles you wish to change in here so the canvas has access to it.")] 
    public Toggle[] Toggles { get; internal set; }

    [field: SerializeField, Tooltip("Place Sliders you wish to change in here so the canvas has access to it.")]
    public Slider[] Sliders { get; internal set; }

    [field: SerializeField, Tooltip("Place Dropdowns you wish to change in here so the canvas has access to it.")]
    public TMP_Dropdown[] Dropdowns { get; internal set; }

    [field: SerializeField, Tooltip("Place InputFields you wish to change in here so the canvas has access to it.")]
    public TMP_InputField[] InputFields { get; internal set; }

    [field: SerializeField, Tooltip("Place Images you wish to change in here so the canvas has access to it.")]
    public Image[] Images { get; internal set; }

    [field: SerializeField, Tooltip("Place TextMeshPro objects you wish to change in here so the canvas has access to it.")]
    public TextMeshProUGUI[] Texts { get; internal set; }
    #endregion

    #region Time Settings
    [Header("Time Settings")]
    [SerializeField] internal bool _slowOrPauseTimeWhileActive = false;
    [SerializeField] internal float _timeScale = 0;
    #endregion

    [Header("Event System")]
    [SerializeField] internal EventSystem _eventSystem;

    #endregion

    internal GameObject _lastSelectedObject;
    internal CanvasGroup _canvas;

    internal float _originalTimeScale;

    internal virtual void Awake()
    {
        _canvas = GetComponent<CanvasGroup>();
        if (Buttons.Length > 0)
        {
            _lastSelectedObject = Buttons[0].gameObject;
        }
        else if (Toggles.Length > 0)
        {
            _lastSelectedObject = Toggles[0].gameObject;
        }
        else if (Sliders.Length > 0)
        {
            _lastSelectedObject = Sliders[0].gameObject;
        }
        else if (Dropdowns.Length > 0)
        {
            _lastSelectedObject = Dropdowns[0].gameObject;
        }
        else if (InputFields.Length > 0)
        {
            _lastSelectedObject = InputFields[0].gameObject;
        }
        else
        {
            Debug.LogWarning("A canvas should have something interactable on it.");
        }
    }
    internal virtual void OnEnable()
    {
        if (_slowOrPauseTimeWhileActive)
        {
            _originalTimeScale = Time.timeScale;
            Time.timeScale = _timeScale;
        }
        if (_eventSystem != null)
        {
            _eventSystem.firstSelectedGameObject = _lastSelectedObject;
        }
    }
    internal virtual void OnDisable()
    {
        if (_slowOrPauseTimeWhileActive)
        {
            Time.timeScale = _originalTimeScale;
        }
        if (_eventSystem != null)
        {
            _lastSelectedObject = _eventSystem.currentSelectedGameObject;
        }
    }
}
