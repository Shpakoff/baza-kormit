using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Modal : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private SpriteRenderer modalBackground;

    private MouseActions mouseActions;
    
    private ModalEventBus modalEventBus = ModalEventBus.Instance;
    
    void Awake()
    {
        mouseActions = new MouseActions();
    }

    void OnEnable()
    {
        mouseActions.Enable();
    }

    void OnDisable()
    {
        mouseActions.Disable();
        mouseActions.ActionMap.LMB.performed -= OnLMBClick;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        modalEventBus.NotifyShow += (String message) => ShowMessage(message);
        mouseActions.ActionMap.LMB.performed += OnLMBClick;
        Hide();
    }
    private void Hide()
    {
        mouseActions.Disable();
        descriptionText.alpha = 0.0f;
        titleText.alpha = 0.0f;
        modalBackground.GetComponent<Renderer>().enabled = false;
    }

    private void ShowMessage(String message)
    {
        mouseActions.Enable();
        descriptionText.text = message;
        descriptionText.alpha = 255.0f;
        modalBackground.GetComponent<Renderer>().enabled = true;
        
    }

    private void OnLMBClick(InputAction.CallbackContext ctx)
    {
        Hide();
    }
}
