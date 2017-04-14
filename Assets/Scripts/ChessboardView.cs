using System;
using System.Collections.Generic;
using UnityEngine;

namespace SCPrototype
{
    public class ChessboardView : MonoBehaviour
    {
        public Func<int, int, bool> TryMoveCallback;
        public Func<List<ChessPiece>> GetChessPiecesCallback;
    }
}