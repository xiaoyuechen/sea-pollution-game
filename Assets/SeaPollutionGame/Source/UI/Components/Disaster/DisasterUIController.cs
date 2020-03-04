﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DisasterUIController : MonoBehaviour
{
    /*
     * DisasterUIController: based on DisasterManager events, shows or hides the corresponding icon (if there is a disaster or not)
     *  - also loads corresponding VideoClip in VideoLoader when shown, and shows and hides the video when the icon is clicked
     * 
     * debug: default video clip for each disaster and default
     */

    [System.Serializable]
    class DisasterContent
    {
        public CanvasGroup canvasGroup = null;
        public DisasterIcon disasterIcon = null;
    }

    [System.Serializable]
    class VideoContent
    {
        public CanvasGroup canvasGroup = null;
        public VideoLoader videoLoader = null;
        public bool isShown { get; set; }
    }

    [SerializeField]
    private DisasterManager disasterManager = null;

    [SerializeField]
    private DisasterContent defaultContent = null;

    [SerializeField]
    private DisasterContent disasterContent = null;

    [SerializeField]
    private VideoContent videoContent = null;

    [SerializeField]
    private float tweenDuration = 0.25f;
    [SerializeField]
    private Ease tweenEase = Ease.Flash;

    [Header("Debug")]
    [SerializeField]
    private bool isDebug = false;
    [SerializeField]
    private string defaultVideoClip = "World_Farm_Fine_Rabbit";

    private DisasterContent currentContentShown = null;

    private void Start()
    {
        if (isDebug)
        {
            Disaster tempDefaultDisaster = new Disaster();
            tempDefaultDisaster.clipTitle = defaultVideoClip;
            defaultContent.disasterIcon.SetDisaster(tempDefaultDisaster);
        }

        defaultContent.disasterIcon.OnClick += DisasterIcon_OnClick;
        disasterContent.disasterIcon.OnClick += DisasterIcon_OnClick;

        videoContent.videoLoader.OnClipFinish += VideoLoader_OnClipFinish;

        HideDirectContent(disasterContent);
        HideVideo(videoContent);

        ShowContent(defaultContent);

        disasterManager.AddDisasterEventListener(OnDisaster);
        disasterManager.AddNoDisasterEventListener(OnNoDisaster);
    }

    private void OnDestroy()
    {
        defaultContent.disasterIcon.OnClick -= DisasterIcon_OnClick;
        disasterContent.disasterIcon.OnClick -= DisasterIcon_OnClick;

        videoContent.videoLoader.OnClipFinish -= VideoLoader_OnClipFinish;
    }


    private void ShowContent(DisasterContent content)
    {
        content.canvasGroup.DOFade(1f, tweenDuration).SetEase(tweenEase);
        content.canvasGroup.blocksRaycasts = true;

        currentContentShown = content;
        
        videoContent.videoLoader.LoadVideo(content.disasterIcon.GetDisaster().clipTitle);
    }

    private void HideContent(DisasterContent content)
    {
        content.canvasGroup.DOFade(0f, tweenDuration).SetEase(tweenEase);
        content.canvasGroup.blocksRaycasts = false;
    }

    private void HideDirectContent(DisasterContent content)
    {
        content.canvasGroup.DOFade(0f, 0f);
        content.canvasGroup.blocksRaycasts = false;
    }

    private void ShowVideo(VideoContent content)
    {
        content.videoLoader.StopVideo();

        content.canvasGroup.DOFade(1f, tweenDuration).SetEase(tweenEase);
        content.isShown = true;

        content.videoLoader.PlayVideo();
    }

    private void HideVideo(VideoContent content)
    {
        content.canvasGroup.DOFade(0f, tweenDuration).SetEase(tweenEase);
        content.isShown = false;

        content.videoLoader.StopVideo();
    }

    // callbacks

    private void OnDisaster(Disaster disaster)
    {
        if (isDebug)
        {
            disaster.clipTitle = defaultVideoClip;
        }

        disasterContent.disasterIcon.SetDisaster(disaster);

        HideContent(defaultContent);
        ShowContent(disasterContent);
    }

    private void OnNoDisaster()
    {
        if (currentContentShown == disasterContent)
        {
            HideContent(disasterContent);
            ShowContent(defaultContent);

        }

        disasterContent.disasterIcon.SetDisaster(null);
    }

    private void DisasterIcon_OnClick(DisasterIcon obj)
    {
        if (videoContent.isShown)
        {
            HideVideo(videoContent);
        }
        else
        {
            ShowVideo(videoContent);
        }
    }

    private void VideoLoader_OnClipFinish()
    {
        if(videoContent.isShown)
        {
            HideVideo(videoContent);
        } else
        {

        }
    }
}
