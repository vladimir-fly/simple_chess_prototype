using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SCPrototype
{
    public class ChessboardView : MonoBehaviour
    {
        public Func<int, int, bool> TryMoveCallback;

        public Func<List<ChessPiece>> GetChessPiecesCallback;
        public Text ChessBoardData;

        public void ShowChessboard()
        {
            var result = string.Empty;

            ChessBoardData.text = result;

            var chessPieces = GetChessPiecesCallback().Select(p => p.CellId).ToList();
            for (var i = 0; i < 64; i++)
            {
                if (i % 8 == 0) result += "\n";
                result += chessPieces.Contains(i) ? " x" : " o" ;

            }

            ChessBoardData.text = result;
        }
    }
}