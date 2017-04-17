﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SCPrototype
{
    public class ChessPieceView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Vector3 _startPosition;
        private Transform _parentTransform;
        private Transform _canvasTransform;
        public int CurrentCellId;

        [HideInInspector] public Animation moveAnimation;
        [HideInInspector] public bool HasMoved;

        void Start()
        {
            _canvasTransform = transform.parent.parent.parent;
            CurrentCellId = transform.parent.GetComponent<CellView>().Id;
            moveAnimation = GetComponent<Animation>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPosition = transform.position;
            _parentTransform = transform.parent.transform;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            transform.SetParent(_canvasTransform);

            if (moveAnimation != null)
            {
                moveAnimation.wrapMode = WrapMode.Loop;
                moveAnimation.Play();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log(string.Format("[ChessPieceView.OnEndDrag] HasMoved: {0}", HasMoved));

            if (!HasMoved)
            {
                transform.SetParent(_parentTransform);
                transform.position = _startPosition;
            }

            GetComponent<CanvasGroup>().blocksRaycasts = true;

            if (moveAnimation != null)
                moveAnimation.wrapMode = WrapMode.Once;

        }
    }
}