using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ArusMerah.Data;
using ArusMerah.Managers;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Containers")]
    public CanvasGroup mainMenuGroup;
    public CanvasGroup gameplayGroup;
    public CanvasGroup shopGroup;
    public CanvasGroup resultGroup;
    public CanvasGroup cutsceneGroup;
    public CanvasGroup endingChoiceGroup;

    [Header("Transition Settings")]
    public float fadeDuration = 0.5f;

    private bool isShopOpen = false;
    private int targetRevenue = 0;
    private int grossRevenue = 0;
    private float elapsedTime = 0;
    private float progress = 0;
    private Coroutine currentTransitionCoroutine;


    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void OnEnable()
    {
        FlowManager.OnFlowStateChanged += HandleFlowStateChanged;
        LevelManager.OnUpdateTarget += HandlerTargetUpdated;
        GameData.OnUpdatedGrossEarnings += HandlerGrossRevenue;
        FlowManager.OnLevelInitiated += CloseShop;
    }

    private void OnDisable()
    {
        FlowManager.OnFlowStateChanged -= HandleFlowStateChanged;
        LevelManager.OnUpdateTarget -= HandlerTargetUpdated;
        GameData.OnUpdatedGrossEarnings -= HandlerGrossRevenue;
        FlowManager.OnLevelInitiated -= CloseShop;
    }

    private void Start()
    {
        // Kondisi awal: Main Game tampil, Shop sembunyi
        StartCoroutine(TransitionCanvas(null, gameplayGroup));
    }

    private void HandlerTargetUpdated(int newTarget)
    {
        targetRevenue = newTarget;
    }

    private void HandlerGrossRevenue(int grossEarnings)
    {
        grossRevenue = grossEarnings;
    }

    private void HandleFlowStateChanged(GameFlowState newFlowState)
    {
        // Hentikan transisi sebelumnya jika masih berjalan agar tidak tumpang tindih!
        //if (currentTransitionCoroutine != null)
        //{
        //    StopCoroutine(currentTransitionCoroutine);
        //}

        switch (newFlowState)
        {
            case GameFlowState.MainMenuState:
                StartCoroutine(TransitionCanvas(currentActiveCanvas(), mainMenuGroup));
                break;
            case GameFlowState.GameplayState:
                isShopOpen = false;
                StartCoroutine(TransitionCanvas(currentActiveCanvas(), gameplayGroup));
                break;
            case GameFlowState.ResultState:
                if (resultGroup != null)
                {
                    StartCoroutine(TransitionCanvas(currentActiveCanvas(), resultGroup));
                }
                break;
            case GameFlowState.ShopState:
                isShopOpen = true;
                StartCoroutine(TransitionCanvas(currentActiveCanvas(), shopGroup));
                break;
            case GameFlowState.CutsceneState:
                if (cutsceneGroup != null)
                {
                    StartCoroutine(TransitionCanvas(currentActiveCanvas(), cutsceneGroup));
                }
                break;
            case GameFlowState.EndingChoiceState:
                if (endingChoiceGroup != null)
                {
                    StartCoroutine(TransitionCanvas(currentActiveCanvas(), endingChoiceGroup));
                }
                break;
        }
    }

    private CanvasGroup currentActiveCanvas()
    {
        if (shopGroup != null && shopGroup.alpha > 0.5f) return shopGroup;
        if (resultGroup != null && resultGroup.alpha > 0.5f) return resultGroup;
        if (cutsceneGroup != null && cutsceneGroup.alpha > 0.5f) return cutsceneGroup;
        if (endingChoiceGroup != null && endingChoiceGroup.alpha > 0.5f) return endingChoiceGroup;
        return gameplayGroup;
    }

    #region UI Shop

    // Dipanggil saat pemain klik "Hari Berikutnya / Keluar Toko"
    public void OpenShop()
    {
        StartCoroutine(TransitionCanvas(gameplayGroup, shopGroup));
        isShopOpen = true;
    }
    public void CloseShop()
    {
        StartCoroutine(TransitionCanvas(shopGroup, gameplayGroup));
        isShopOpen = false;
    }
    #endregion

    #region Helper Canvas Crossfade Transition
    private IEnumerator TransitionCanvas(CanvasGroup canvasOut, CanvasGroup canvasIn)
    {
        if (canvasIn == null) yield break;
        // 1. AKTIFKAN GAMEOBJECT PANEL BARU SEBELUM FADE IN
        canvasIn.gameObject.SetActive(true);
        if (canvasOut != null)
        {
            canvasOut.interactable = false;
            canvasOut.blocksRaycasts = false;
        }
        // 2. PROSES CROSSFADE (0.4 Detik)
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            progress = elapsedTime / fadeDuration;
            if (canvasOut != null) canvasOut.alpha = 1f - progress; // Fade Out
            canvasIn.alpha = progress;                             // Fade In
            yield return null;
        }
        // 3. PASTI NILAI AKHIR PRESISI
        if (canvasOut != null) canvasOut.alpha = 0f;
        canvasIn.alpha = 1f;
        canvasIn.interactable = true;
        canvasIn.blocksRaycasts = true;
        // 4. MATIKAN GAMEOBJECT PANEL LAMA AGAR CPU 100% RINGAN!
        if (canvasOut != null)
        {
            canvasOut.gameObject.SetActive(false);
        }
    }
    #endregion
}