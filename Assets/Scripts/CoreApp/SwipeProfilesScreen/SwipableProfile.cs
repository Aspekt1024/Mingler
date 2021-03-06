﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SwipableProfile : MonoBehaviour, ISwipable {

    public Text NameText;
    public Text AgeText;
    public Text TaglineText;
    public CanvasGroup YesOverlay;
    public CanvasGroup NoOverlay;
    public Image Photo;
    
    private enum States
    {
        None, Pressed, Released, Held, Clicked, SwipedLeft, SwipedRight, Disabled
    }
    private States state;

    private RectTransform rectTf;
    private Coroutine animationRoutine;

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Vector2 pointerStartPosition;
    private Vector2 pointerPosition;
    private bool isDragged;

    private const float shrinkCoefficient = 0.95f;

    private void Start()
    {
        rectTf = GetComponent<RectTransform>();
        rectTf.position -= rectTf.position.x * Vector3.right;

        originalScale = rectTf.localScale;
        originalPosition = rectTf.position;

        YesOverlay.alpha = 0;
        NoOverlay.alpha = 0;
    }

	private void Update ()
    {
        switch (state)
        {
            case States.None:
                break;
            case States.Pressed:
                state = States.Held;
                isDragged = false;
                if (animationRoutine != null) StopCoroutine(animationRoutine);
                animationRoutine = StartCoroutine(Shrink());
                break;
            case States.Released:
                state = States.None;
                if (animationRoutine != null) StopCoroutine(animationRoutine);
                animationRoutine = StartCoroutine(ReleaseRoutine());
                break;
            case States.Held:
                ProcessSwiping();
                break;
            case States.Clicked:
                state = States.None;
                if (animationRoutine != null) StopCoroutine(animationRoutine);
                animationRoutine = StartCoroutine(ReleaseRoutine());
                break;
            case States.SwipedLeft:
                StartCoroutine(SwipeLeftRoutine());
                break;
            case States.SwipedRight:
                StartCoroutine(SwipeRightRoutine());
                break;
            case States.Disabled:
                break;
        }
	}

    public void SetProfile(Profile profile)
    {
        NameText.text = profile.Name;
        AgeText.text = profile.Age;
        TaglineText.text = profile.Tagline;
        
        Photo.sprite = Resources.Load<Sprite>("Portraits/" + profile.PhotoPath);
        Photo.transform.position = profile.PhotoPosition;
        Photo.rectTransform.anchoredPosition = profile.PhotoAnchor;
        Photo.rectTransform.sizeDelta = profile.PhotoSize;
        Photo.transform.localScale = profile.PhotoScale;
    }
    
    public Profile GetProfile()
    {
        return new Profile()
        {
            Name = NameText.text,
            Age = AgeText.text,
            Tagline = TaglineText.text,
#if UNITY_EDITOR
            PhotoPath = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(Photo.sprite)),
#endif
            PhotoSprite = Photo.sprite,
            PhotoAnchor = Photo.rectTransform.anchoredPosition,
            PhotoSize = Photo.rectTransform.sizeDelta,
            PhotoPosition = Photo.transform.position,
            PhotoScale = Photo.transform.localScale
        };
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (state == States.Held || state == States.Disabled) return;
        state = States.Pressed;
        pointerStartPosition = InputManager.GetPointerPosition();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (state == States.Held)
        {
            if (CheckForSwipe())
            {
                ProfilesManager.Instance.ProfileSwiped();
                return;
            }
            state = States.Released;
        }
    }

    private bool CheckForSwipe()
    {
        float positionOnScreen = Camera.main.WorldToScreenPoint(transform.position).x;
        if (positionOnScreen < Screen.width * 0.1f)
        {
            state = States.SwipedLeft;
            return true;
        }
        else if (positionOnScreen > Screen.width * 0.9f)
        {
            state = States.SwipedRight;
            return true;
        }
        return false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDragged || state == States.Disabled) return;
        state = States.Clicked;
    }

    private void ProcessSwiping()
    {
        pointerPosition = InputManager.GetPointerPosition();
        if (Vector2.Distance(pointerPosition, pointerStartPosition) > 0.3f)
        {
            isDragged = true;
        }
        float xPos = originalPosition.x + pointerPosition.x - pointerStartPosition.x;
        float yPos = originalPosition.y + pointerPosition.y - pointerStartPosition.y;
        rectTf.position = new Vector3(xPos, yPos, rectTf.position.z);
        ProcessSwipePosition();
    }

    private void ProcessSwipePosition()
    {
        float positionOnScreen = Camera.main.WorldToScreenPoint(transform.position).x;
        float screenHalfWidth = Screen.width / 2f;

        if (positionOnScreen < screenHalfWidth * 0.7f)
        {
            NoOverlay.alpha = (Screen.width / 2 - positionOnScreen) / (Screen.width / 2f);
        }
        else
        {
            NoOverlay.alpha = 0;
        }

        if (positionOnScreen > screenHalfWidth * 1.2f)
        {
            YesOverlay.alpha = (positionOnScreen - (Screen.width / 2)) / (Screen.width / 2f);
        }
        else
        {
            YesOverlay.alpha = 0;
        }
    }

    private IEnumerator ReleaseRoutine()
    {
        StartCoroutine(Unshrink());
        while (Vector3.Distance(rectTf.position, originalPosition) > 0.02f)
        {
            rectTf.position = Vector3.Lerp(rectTf.position, originalPosition, Time.deltaTime * 16f);
            ProcessSwipePosition();
            yield return null;
        }
    }

    private IEnumerator Shrink()
    {
        while (rectTf.localScale.x - originalScale.x * shrinkCoefficient > 0.02f)
        {
            rectTf.localScale = Vector3.Lerp(rectTf.localScale, originalScale * shrinkCoefficient, Time.deltaTime * 8);
            yield return null;
        }
    }

    private IEnumerator Unshrink()
    {
        while (originalScale.x - rectTf.localScale.x > 0.02f)
        {
            rectTf.localScale = Vector3.Lerp(rectTf.localScale, originalScale, Time.deltaTime * 16);
            yield return null;
        }
        rectTf.localScale = originalScale;
    }

    private IEnumerator SwipeLeftRoutine()
    {
        Vector3 endPoint = Camera.main.ScreenToWorldPoint(new Vector3(-Screen.width, 0f, transform.position.z));

        while (Vector3.Distance(transform.position, endPoint) > 0.02f)
        {
            transform.position = Vector3.Lerp(transform.position, endPoint, Time.deltaTime * 3f);
            yield return null;
        }
    }

    private IEnumerator SwipeRightRoutine()
    {
        Vector3 endPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 2, Screen.height, transform.position.z));

        while (Vector3.Distance(transform.position, endPoint) > 0.02f)
        {
            transform.position = Vector3.Lerp(transform.position, endPoint, Time.deltaTime * 3f);
            yield return null;
        }
    }

    private Vector3 V2ToV3(Vector2 v2)
    {
        return new Vector3(v2.x, v2.y, 0);
    }

}
