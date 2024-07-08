using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplySafeArea : MonoBehaviour
{
    void Start()
    {
        RectTransform uiElement = gameObject.GetComponent<RectTransform>();
        if (uiElement == null)
        {
            return;
        }
        Rect safeArea = Screen.safeArea;
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        uiElement.anchorMin = anchorMin;
        uiElement.anchorMax = anchorMax;
    }

}
