﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuInGameController : MonoBehaviour
{
    private SingletonLevelManager levelController = null;

    [SerializeField]
    private Button btnOpen = null;
    [SerializeField]
    private Button btnRestart = null;
    [SerializeField]
    private Button btnNewGame = null;
    [SerializeField]
    private Button btnQuit = null;

    [SerializeField]
    private CanvasGroup menuContent = null;
    private RectTransform menuTransform = null;
    [SerializeField]
    private Vector2 menuTargetPosition = Vector2.zero;
    private Vector2 menuDefaultPosition = Vector2.zero;

    [SerializeField]
    private float tweenDuration = 0.25f;
    [SerializeField]
    private Ease tweenEase = Ease.Linear;

    private bool isShown = false;

    private void Start()
    {
        levelController = UIManager.Instance.levelController;

        menuTransform = menuContent.GetComponent<RectTransform>();
        menuDefaultPosition = menuTransform.anchoredPosition;

        HideDirectMenu();

        btnOpen.onClick.AddListener(OpenOnClick);
        btnRestart.onClick.AddListener(RestartOnClick);
        btnNewGame.onClick.AddListener(NewGameOnClick);
        btnQuit.onClick.AddListener(QuitOnClick);
    }

    private void OnDestroy()
    {
        btnOpen.onClick.RemoveListener(OpenOnClick);
        btnRestart.onClick.RemoveListener(RestartOnClick);
        btnNewGame.onClick.RemoveListener(NewGameOnClick);
        btnQuit.onClick.RemoveListener(QuitOnClick);
    }

    void OpenOnClick()
    {
        if(!isShown)
        {
            ShowMenu();
        } else
        {
            HideMenu();
        }
    }

    void RestartOnClick()
    {
        levelController.LoadCurrentLevel();
    }

    void NewGameOnClick()
    {
        levelController.LoadRandomLevel();
    }

    void QuitOnClick()
    {
        //levelController.LoadHomeLevel();
        Application.Quit();
    }

    void ShowMenu()
    {
        //menuContent.DOFade(1f, tweenDuration).SetEase(tweenEase);
        menuTransform.DOKill();
        menuTransform.DOAnchorPos(menuTargetPosition, tweenDuration).SetEase(tweenEase);
        menuContent.interactable = true;

        isShown = true;
    }

    void HideMenu()
    {
        //menuContent.DOFade(0f, tweenDuration).SetEase(tweenEase);
        menuTransform.DOKill();
        menuTransform.DOAnchorPos(menuDefaultPosition, tweenDuration).SetEase(tweenEase);
        menuContent.interactable = false;

        isShown = false;
    }

    void HideDirectMenu()
    {
        //menuContent.DOFade(0f, 0f);
        menuTransform.DOKill();
        menuTransform.DOAnchorPos(menuDefaultPosition, 0f).SetEase(tweenEase);
        menuContent.interactable = false;

        isShown = false;
    }
}