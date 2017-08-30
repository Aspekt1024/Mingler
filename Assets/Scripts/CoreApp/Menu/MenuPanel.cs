using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanel : MonoBehaviour {

    private RectTransform rectTf;

    private void Start()
    {
        rectTf = GetComponent<RectTransform>();
        rectTf.position = new Vector3()
        {
            x = Camera.main.ScreenToWorldPoint(new Vector3(-Screen.width - 10f, 0, 0)).x,
            y = 0f,
            z = rectTf.position.z
        };
    }

    public IEnumerator Show()
    {
        while (rectTf.position.x < -0.04f)
        {
            float xPos = Mathf.Lerp(rectTf.position.x, 0, Time.deltaTime * 7f);
            rectTf.position = new Vector3(xPos, 0, rectTf.position.y);
            yield return null;
        }
    }

    public IEnumerator Hide()
    {
        float endPosX = Camera.main.ScreenToWorldPoint(new Vector3(-Screen.width - 10f, 0, 0)).x;
        while (Mathf.Abs(rectTf.position.x - endPosX) > 0.04f)
        {
            float xPos = Mathf.Lerp(rectTf.position.x, endPosX, Time.deltaTime * 3f);
            rectTf.position = new Vector3(xPos, 0, rectTf.position.y);
            yield return null;
        }
    }
}
