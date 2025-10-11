using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TouchDetector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // PointerDown
    public event System.Action<Vector2> OnTouch;

    // PointerUp
    public event System.Action<Vector2> OnTap;

    void Awake()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 1f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnTouch?.Invoke(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnTap?.Invoke(eventData.position);
    }
}
