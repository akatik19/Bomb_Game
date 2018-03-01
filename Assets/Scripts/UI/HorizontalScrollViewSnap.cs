using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ScrollRect))]
public class HorizontalScrollViewSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{

    public float SmoothTime = 1f / 60f;

    private ScrollRect _scrollRect;

    private float? _moveTo;
    private bool _dragging = false;

    void Start()
    {
        _scrollRect = GetComponent<ScrollRect>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            switch (Input.GetTouch(0).phase)
            {
                case TouchPhase.Began:
                    _moveTo = null;
                    break;
                case TouchPhase.Moved:
                    break;
                case TouchPhase.Ended:
                    SetNearestElement();
                    break;
                default:
                    //Debug.Log(Input.GetTouch(0).phase);
                    break;
            }
        }
        else if (!_dragging)
        {
            if (_moveTo.HasValue)
            {
                float velocity = 0;
                _scrollRect.horizontalNormalizedPosition = Mathf.SmoothDamp(_scrollRect.horizontalNormalizedPosition, _moveTo.Value, ref velocity, SmoothTime);

                if (Mathf.Approximately(_scrollRect.horizontalNormalizedPosition, _moveTo.Value))
                    _moveTo = null;
            }
        }
    }

    private void SetNearestElement()
    {
        int normalizeFactor = (_scrollRect.content.childCount - 1);
        _moveTo = Mathf.Round(_scrollRect.horizontalNormalizedPosition * normalizeFactor) / normalizeFactor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _moveTo = null;
        _dragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_dragging)
        {
            SetNearestElement();
            _dragging = false;
        }
    }
}
