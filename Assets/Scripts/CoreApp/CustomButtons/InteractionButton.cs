using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InteractionButton : MonoBehaviour, IButton {

    private RectTransform rectTf;
    private Vector3 originalScale;
    private Coroutine animationRoutine;
    private Button button;

    private const float shrinkCoefficient = 0.5f;

    private enum States
    {
        None, Pressed, Released, Held, Clicked
    }
    private States state;
    
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        rectTf = GetComponent<RectTransform>();
        button = gameObject.AddComponent<Button>();

        originalScale = rectTf.localScale;
        button.onClick.AddListener(Clicked);
        button.transition = Selectable.Transition.None;
    }

    private void Update()
    {
        switch (state)
        {
            case States.None:
                break;
            case States.Pressed:
                state = States.Held;
                if (animationRoutine != null) StopCoroutine(animationRoutine);
                animationRoutine = StartCoroutine(ShrinkButton());
                break;
            case States.Released:
                state = States.None;
                if (animationRoutine != null) StopCoroutine(animationRoutine);
                animationRoutine = StartCoroutine(UnshrinkButton());
                break;
            case States.Held:
                break;
            case States.Clicked:
                if (animationRoutine != null) StopCoroutine(animationRoutine);
                animationRoutine = StartCoroutine(UnshrinkButton());
                state = States.None;
                break;
        }
    }

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

    private IEnumerator ShrinkButton()
    {
        while (rectTf.localScale.x - originalScale.x * shrinkCoefficient > 0.02f)
        {
            rectTf.localScale = Vector3.Lerp(rectTf.localScale, originalScale * shrinkCoefficient, Time.deltaTime * 8);
            yield return null;
        }
    }

    private IEnumerator UnshrinkButton()
    {
        while (originalScale.x - rectTf.localScale.x > 0.02f)
        {
            rectTf.localScale = Vector3.Lerp(rectTf.localScale, originalScale, Time.deltaTime * 16);
            yield return null;
        }
        rectTf.localScale = originalScale;
    }
}
