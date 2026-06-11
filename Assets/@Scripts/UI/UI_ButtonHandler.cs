using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler
{
    public RectTransform arrow;
    public Image onButtonTextImg;
    public Image offButtonTextImg;
    public Vector3 offset = new Vector3(-30f, 0f, 0f);
    private Transform _originalParent;

    void Awake()
    {
        if (onButtonTextImg != null || offButtonTextImg != null)
        {
            OnClickOn();
        }
    }

    private void Start()
    {
        arrow.gameObject.SetActive(false);
        _originalParent = arrow.parent;
    }

    // public void OnPointerEnter(PointerEventData eventData)
    // {
    //     arrow.gameObject.SetActive(true);
    //     RectTransform buttonRect = eventData.pointerEnter.GetComponent<RectTransform>();
    //     arrow.position = buttonRect.position + offset;
    // }
    public void OnPointerEnter(PointerEventData eventData)
    {
        arrow.gameObject.SetActive(true);
        RectTransform buttonRect = eventData.pointerEnter.GetComponent<RectTransform>();

        arrow.SetParent(buttonRect);
        arrow.anchoredPosition = offset;

        SoundManager.Instance.PlaySfx(SoundManager.Instance.buttonHoverClip);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySfx(SoundManager.Instance.buttonClickClip);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        arrow.gameObject.SetActive(false);
        arrow.SetParent(_originalParent);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        arrow.gameObject.SetActive(false);
        arrow.SetParent(_originalParent);
    }

    public void OnClickOn()
    {
        onButtonTextImg.color = Color.white;
        offButtonTextImg.color = Color.gray;
    }

    public void OnClickOff()
    {
        onButtonTextImg.color = Color.gray;
        offButtonTextImg.color = Color.white;
    }
}
