﻿#define CONTROLLER_HANDLES_DROP
#define CONTROLLER_HANDLES_CANCEL_HOLD
#define CONTROLLER_HANDLES_REMOVE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum State
    {
        EMPTY,
        HOLDING,
    }

    [SerializeField] private float boardPlaneY = 0;

    [SerializeField] private bool handlingDrop = false;
    [SerializeField] private bool handlingCancelHold = false;
    [SerializeField] private bool handlingRemove = true;

    State state = State.EMPTY;
    Polluter holdingPolluter = null;
    WorldStateManager stateManager = null;

    [Header("Audio")]
    [SerializeField]
    private AudioSource audioSource = null;
    [SerializeField]
    private AudioClip purchaseClip = null;
    [SerializeField]
    private AudioClip errorClip = null;
    [SerializeField]
    private AudioClip componentSoldClip = null;

    public State GetState() { return state; }

    public void Hold(Polluter polluter = null)
    {
        state = State.HOLDING;
        holdingPolluter = polluter;
    }

    public void CancelHold()
    {
        state = State.EMPTY;
        if (holdingPolluter != null)
            Destroy(holdingPolluter.gameObject);
    }

    public bool TryDrop()
    {
        Space hitSpace = GetMouseHitComp<Space>();
        if (hitSpace && hitSpace.CanPlacePolluter(stateManager.GetCurrentPlayerID(), holdingPolluter.GetAttrib()))
        {
            Purchase();
            hitSpace.SetPolluter(holdingPolluter);
            TransformPolluter(hitSpace);
            SetPolluterText();
            state = State.EMPTY;
            return true;
        }

        audioSource.PlayOneShot(errorClip);

        return false;
    }

    public bool TryRemove(Space space)
    {
        var polluter = space.GetPolluter();
        if (polluter && CanRemove(polluter))
        {
            space.SetPolluter(null);
            var playerState = stateManager.GetCurrentPlayerState();
            playerState.AddMoney(-polluter.GetAttrib().economicAttrib.removalCost);
            playerState.RemovePolluter(polluter);
            Destroy(polluter.gameObject);

            audioSource.PlayOneShot(componentSoldClip);

            return true;
        }

        Feedback.Instance.FeedbackInsufficientRemovalCoins();
        return false;
    }

    public bool TryRepair(Polluter polluter)
    {
        if (CanRepair(polluter))
        {
            var playerState = stateManager.GetCurrentPlayerState();
            playerState.AddMoney(-GetRepairCost(polluter));
            var healthComp = polluter.GetComponentInChildren<Health>();
            healthComp.Recover();
            return true;
        }
        return false;
    }

    private void Start()
    {
        stateManager = FindObjectOfType<WorldStateManager>();
    }

    private void SetPolluterText()
    {
        var textMesh = holdingPolluter.GetIdTextMesh();
        textMesh.gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);
        holdingPolluter.SetIdText(holdingPolluter.polluterId.ToString());
    }

    private void TransformPolluter(Space validSpace)
    {
        var targetPos = validSpace.transform.position;
        targetPos.y = holdingPolluter.transform.position.y;

        holdingPolluter.transform.parent = validSpace.transform;

        holdingPolluter.transform.position = targetPos;
        holdingPolluter.transform.rotation = Quaternion.Euler(0, 30, 0);
        holdingPolluter.transform.localScale = Vector3.one;
    }

    private void Update()
    {
        switch (state)
        {
            case State.EMPTY:
                {
                    if (handlingRemove)
                    {
                        if (Input.GetButtonDown("Fire2"))
                        {
                            var hitSpace = GetMouseHitComp<Space>();
                            if (hitSpace) { TryRepair(hitSpace.GetPolluter()); }
                        }
                    }
                }
                break;
            case State.HOLDING:
                {
                    //FollowMouse();

                    if (handlingCancelHold)
                    {
                        if (Input.GetButtonDown("Fire2"))
                        {
                            CancelHold();
                        }
                    }

                    if (handlingDrop)
                    {
                        if (Input.GetButtonDown("Fire1"))
                        {
                            TryDrop();
                        }
                    }

                }
                break;
        }
    }

    private bool CanRemove(Polluter polluter)
    {
        if (polluter.GetOwnerID() == stateManager.GetCurrentPlayerID()
            && polluter.GetAttrib().economicAttrib.removalCost <= stateManager.GetCurrentPlayerState().GetMoney())
        {
            return true;
        }
        return false;
    }

    public float GetRepairCost(Polluter polluter)
    {
        var healthComp = polluter.GetComponentInChildren<Health>();
        var economicAttrib = polluter.GetAttrib().economicAttrib;
        var extraRatio = economicAttrib.repairExtraRatio;
        var lostHealthRatio = 1 - healthComp.GetHealthPercent();
        var repairCost = (lostHealthRatio + extraRatio) * economicAttrib.price;
        return repairCost;
    }

    public bool CanRepair(Polluter polluter)
    {
        if (polluter.GetOwnerID() == stateManager.GetCurrentPlayerID())
        {
            var healthComp = polluter.GetComponentInChildren<Health>();
            if (healthComp.GetHealthPercent() < 1 && healthComp.GetHealthPercent() > 0)
            {
                if (stateManager.GetCurrentPlayerState().GetMoney() >= GetRepairCost(polluter))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void FollowMouse()
    {
        holdingPolluter.transform.position = GetWorldMousePos();
    }

    private Vector3 GetWorldMousePos()
    {
        var camera = Camera.main;
        var mousePos = Input.mousePosition;
        mousePos.z = camera.transform.position.y - boardPlaneY;
        return camera.ScreenToWorldPoint(mousePos);
    }

    private RaycastHit[] MouseRaycastDownAll()
    {
        var raycastOrigin = GetWorldMousePos();
        raycastOrigin.y += 100;
        return Physics.RaycastAll(raycastOrigin, Vector3.down, 9999);
    }

    public T GetMouseHitComp<T>() where T : class
    {
        foreach (var hit in MouseRaycastDownAll())
        {
            var comp = hit.transform.gameObject.GetComponent<T>();
            if (comp != null)
            {
                return comp;
            }
        }
        return null;
    }

    private void Purchase()
    {
        int currentPlayerID = stateManager.GetCurrentPlayerID();
        holdingPolluter.SetOwnerID(currentPlayerID);

        var playerState = stateManager.GetPlayerState(currentPlayerID);

        float price = holdingPolluter.GetAttrib().economicAttrib.price;
        playerState.AddMoney(-price);
        playerState.AddPolluter(holdingPolluter);

        audioSource.PlayOneShot(purchaseClip);
    }
}
