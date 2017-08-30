using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IButton
{
    protected Image flashImage;
    private Coroutine animationRoutine;
    private MenuOverlay menuOverlay;

    private enum States
    {
        None, Held, Pressed, Released, Clicked
    }
    private States state;
    
    private void Start()
    {
        menuOverlay = GetComponentInParent<MenuOverlay>();
        flashImage = GetComponent<Image>();
        flashImage.color = Color.clear;
    }

    // Update is called once per frame
    private void Update()
    {
        switch (state)
        {
            case States.None:
                break;
            case States.Pressed:
                state = States.Held;
                if (animationRoutine != null) StopCoroutine(animationRoutine);
                animationRoutine = StartCoroutine(ShowFlash());
                break;
            case States.Released:
                state = States.None;
                if (animationRoutine != null) StopCoroutine(animationRoutine);
                animationRoutine = StartCoroutine(FadeFlash());
                break;
            case States.Held:
                break;
            case States.Clicked:
                ButtonClicked();
                if (animationRoutine != null) StopCoroutine(animationRoutine);
                animationRoutine = StartCoroutine(FadeFlash());
                state = States.None;
                break;
        }
    }

    protected virtual void ButtonClicked() { }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (state == States.Held || state == States.Clicked) return;
        state = States.Pressed;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (state == States.Held)
        {
            state = States.Released;
        }
    }

    public virtual void Clicked()
    {
        state = States.Clicked;
    }

    private IEnumerator ShowFlash()
    {
        float targetAlpha = 0.45f;
        while (flashImage.color.a < targetAlpha)
        {
            float alpha = Mathf.Lerp(flashImage.color.a, targetAlpha, Time.deltaTime * 10f);
            flashImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
    }

    private IEnumerator FadeFlash()
    {
        while (flashImage.color.a > 0.02f)
        {
            float alpha = Mathf.Lerp(flashImage.color.a, 0f, Time.deltaTime * 5f);
            flashImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
        flashImage.color = Color.clear;
    }
}
