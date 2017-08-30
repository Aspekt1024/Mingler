using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuOverlay : MonoBehaviour, IPointerClickHandler
{
    public MenuPanel menuPanel;

    private Coroutine animationRoutine;
    private Coroutine menuPanelRoutine;
    private CanvasGroup canvasGroup;

    private enum States
    {
        None, Hiding, Hidden, Showing, Shown
    }
    private States state;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        state = States.Hidden;
    }

    // Update is called once per frame
    private void Update()
    {
        switch (state)
        {
            case States.None:
                break;
            case States.Hiding:
                HideMenu();
                break;
            case States.Hidden:
                break;
            case States.Showing:
                 ShowMenu();
                break;
            case States.Shown:
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HideMenu();
    }

    public void ShowMenu()
    {
        if (state != States.Hidden) return;
        if (animationRoutine != null) StopCoroutine(animationRoutine);
        animationRoutine = StartCoroutine(ShowMenuRoutine());
    }

    public void HideMenu()
    {
        if (state != States.Shown) return;
        if (animationRoutine != null) StopCoroutine(animationRoutine);
        animationRoutine = StartCoroutine(HideMenuRoutine());
    }
    
    private IEnumerator ShowMenuRoutine()
    {
        canvasGroup.blocksRaycasts = true;
        if (menuPanelRoutine != null) StopCoroutine(menuPanelRoutine);
        menuPanelRoutine = StartCoroutine(menuPanel.Show());
        while (canvasGroup.alpha < 0.97f)
        {
            float alpha = Mathf.Lerp(canvasGroup.alpha, 1f, Time.deltaTime * 5f);
            canvasGroup.alpha = alpha;
            yield return null;
        }
        canvasGroup.alpha = 1;
        state = States.Shown;
    }

    private IEnumerator HideMenuRoutine()
    {
        if (menuPanelRoutine != null) StopCoroutine(menuPanelRoutine);
        menuPanelRoutine = StartCoroutine(menuPanel.Hide());
        while (canvasGroup.alpha > 0.05f)
        {
            float alpha = Mathf.Lerp(canvasGroup.alpha, 0f, Time.deltaTime * 5f);
            canvasGroup.alpha = alpha;
            yield return null;
        }
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;
        state = States.Hidden;
    }
}
