﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class PurchasableIcon : MonoBehaviour, IPointerDownHandler, IPointerUpHandler //, IPointerClickHandler
{
    /*
     * PurchasableIcon:
     *  OnClick: Creates the corresponding PolluterIcon with attributes
     */

    [SerializeField]
    private TextMeshProUGUI targetText = null;

    [SerializeField]
    private PolluterIcon targetPolluterIcon = null;

    private GameObject spaceForPolluter = null;

    private PolluterAttrib polluterAttrib = null;

    WorldStateManager worldStateManager = null;

    public PlayerController playerController { get; set; }

    public Transform shopTransform { get; set; }

    public int polluterId { get; set; }

    private bool isDragging = false;

    private void Start()
    {
        worldStateManager = FindObjectOfType<WorldStateManager>();
    }

    private void InstantiatePolluterIcon()
    {
        if (!gameObject.activeInHierarchy) return;

        if (worldStateManager.GetCurrentPlayerState().GetMoney() < polluterAttrib.economicAttrib.price)
        {
            return;
        }

        PolluterIcon newIcon = Instantiate(targetPolluterIcon, shopTransform);
        newIcon.transform.position = Input.mousePosition;

        newIcon.SetPolluterAttributes(polluterAttrib);
        newIcon.SetSpace(spaceForPolluter);
        newIcon.SetText(targetText.text);
        newIcon.playerController = playerController;
        newIcon.polluterId = polluterId;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        InstantiatePolluterIcon();
    }


    public void SetSpace(GameObject s) { spaceForPolluter = s; }

    public void SetPolluterAttributes(PolluterAttrib attrib) { polluterAttrib = attrib; }

    public PolluterAttrib GetPolluterAttributes() { return polluterAttrib; }

    public void SetText(string s) { targetText.text = s; }

    public PolluterIcon GetPolluterIcon() { return targetPolluterIcon; }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isDragging) return;
        isDragging = true;

        InstantiatePolluterIcon();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;
    }
}
