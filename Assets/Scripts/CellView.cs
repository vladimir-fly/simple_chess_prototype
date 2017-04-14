using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SCPrototype
{
    public class CellView : MonoBehaviour, IDropHandler
    {
        private Func<int, int, bool> _tryMoveCallback;
        public int Id;

        void Start()
        {
            _tryMoveCallback = transform.parent.GetComponent<ChessboardView>().TryMoveCallback;
        }

        public void OnDrop(PointerEventData eventData)
        {
            var chessPiece = eventData.pointerDrag.GetComponent<ChessPieceView>();
            Debug.Log(string.Format("[CellView.OnDrop] CellId from: {0}; CellId to: {1}.", chessPiece.CurrentCellId, Id));

            var moveResult = false;

            if (_tryMoveCallback != null)
               moveResult = _tryMoveCallback(chessPiece.CurrentCellId, Id);

            if (moveResult)
            {
                Debug.Log("[CellView.OnDrop] ChildCount: " + transform.childCount);

                if (transform.childCount > 0)
                    foreach (Transform child in transform) Destroy(child.gameObject);

                chessPiece.transform.SetParent(transform);
                chessPiece.transform.localPosition = Vector3.zero;
                chessPiece.CurrentCellId = Id;
            }

            chessPiece.HasMoved = moveResult;
        }
    }
}