﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class PolluterIcon : MonoBehaviour //, IPointerClickHandler
{
    [Serializable]
    class PlacementTypeObject
    {
        public PlaceType placeType;
        public GameObject gameObject;
    }

    public event Action OnDrag;
    public event Action OnRelease;

    [SerializeField]
    private bool isInteractible = true;
    public void SetInteractible(bool v) { isInteractible = v; }

    [SerializeField]
    private Polluter targetPolluter = null;

    [SerializeField]
    private TextMeshProUGUI targetText = null;

    [Header("Types")]
    [SerializeField]
    private List<PlacementTypeObject> placementTypeObjects = null;

    [Header("Audio")]
    [SerializeField]
    private AudioSource audioSource = null;
    [SerializeField]
    private AudioClip startDragClip = null;

    private GameObject spaceForPolluter = null;

    private PolluterAttrib polluterAttrib = null;

    private GameObject gameObjectDragged = null;
    private Polluter polluterDragged = null;

    public int polluterId { get; set; }

    public void SetSpace(GameObject s) { spaceForPolluter = s; }

    public PlayerController playerController { get; set; }

    public void SetPolluterAttributes(PolluterAttrib attrib)
    {
        polluterAttrib = attrib;

        foreach(PlacementTypeObject pto in placementTypeObjects)
        {
            if(attrib.placementAttrib.CanPlaceOn(pto.placeType))
            {
                //pto.gameObject.SetActive(true);
            } else
            {
                pto.gameObject.SetActive(false);
            }
        }
    }
    public PolluterAttrib GetPolluterAttributes() { return polluterAttrib; }
    
    public Polluter GetPolluter() { return targetPolluter; }

    public void SetText(string s) { targetText.text = s; }

    private void Start()
    {
        if(isInteractible)
        {
            OnDrag?.Invoke();

            audioSource.Stop();
            audioSource.clip = startDragClip;
            audioSource.Play();

            playerController.Hold();
        }
    }

    private void OnDestroy()
    {
        OnRelease?.Invoke();
    }

    private void Update()
    {
        if(isInteractible)
        {
            transform.position = Input.mousePosition;

            /*
            if(Input.GetButtonDown("Fire2")) {
                playerController.CancelHold();
                Destroy(gameObject);
            }
            */

            if (Input.GetButtonUp("Fire1"))
            {
                InstantiatePolluter();

                playerController.Hold(polluterDragged);

                bool dropped = playerController.TryDrop();

                if (dropped)
                {
                    Destroy(gameObject);
                }
                else
                {
                    playerController.CancelHold();
                    Destroy(gameObject);
                }
            }
        }
    }

    public GameObject InstantiatePolluter()
    {
        GameObject g = Instantiate(targetPolluter.gameObject, spaceForPolluter.transform);

        Polluter p = g.GetComponentInChildren<Polluter>();
        
        p.SetAttrib(polluterAttrib);
        p.polluterId = polluterId;

        polluterDragged = p;
        gameObjectDragged = g;

        return g;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        InstantiatePolluter();

        playerController.Hold(polluterDragged);

        bool dropped = playerController.TryDrop();

        if(dropped)
        {
            Destroy(gameObject);
        } else
        {
            playerController.CancelHold();
        }
    }
}
