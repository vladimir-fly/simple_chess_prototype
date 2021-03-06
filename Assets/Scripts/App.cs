using System.Collections.Generic;
using UnityEngine;

namespace SCPrototype
{
    public class App : MonoBehaviour
    {
        public ChessboardView ChessboardView;
        public GameScreen GameScreen;
        private Chessboard _chessboard;

        void Awake()
        {
            var chessPiecesSetting = GetBaseChessPiecesSetting();
            _chessboard = new Chessboard(chessPiecesSetting);

            ChessboardView.TryMoveCallback = _chessboard.TryMove;
            _chessboard.SideChanged += GameScreen.SwitchTurnView;
            _chessboard.OnGameOver += GameScreen.ShowCaption;

            ChessboardView.GetChessPiecesCallback = _chessboard.GetChessPieces;
        }

        private List<ChessPiece> GetBaseChessPiecesSetting()
        {
            return new List<ChessPiece>
            {
                new ChessPiece { CellId = 0, Side = false, Type = EChessPieceType.Rook },
                new ChessPiece { CellId = 1, Side = false, Type = EChessPieceType.Knight },
                new ChessPiece { CellId = 2, Side = false, Type = EChessPieceType.Bishop },
                new ChessPiece { CellId = 3, Side = false, Type = EChessPieceType.Queen },
                new ChessPiece { CellId = 4, Side = false, Type = EChessPieceType.King },
                new ChessPiece { CellId = 5, Side = false, Type = EChessPieceType.Bishop },
                new ChessPiece { CellId = 6, Side = false, Type = EChessPieceType.Knight },
                new ChessPiece { CellId = 7, Side = false, Type = EChessPieceType.Rook },

                new ChessPiece { CellId = 8, Side = false, Type = EChessPieceType.Pawn },
                new ChessPiece { CellId = 9, Side = false, Type = EChessPieceType.Pawn },
                new ChessPiece { CellId = 10, Side = false, Type = EChessPieceType.Pawn },
                new ChessPiece { CellId = 11, Side = false, Type = EChessPieceType.Pawn },
                new ChessPiece { CellId = 12, Side = false, Type = EChessPieceType.Pawn },
                new ChessPiece { CellId = 13, Side = false, Type = EChessPieceType.Pawn },
                new ChessPiece { CellId = 14, Side = false, Type = EChessPieceType.Pawn },
                new ChessPiece { CellId = 15, Side = false, Type = EChessPieceType.Pawn },

                new ChessPiece { CellId = 48, Side = true, Type = EChessPieceType.Pawn },
                new ChessPiece { CellId = 49, Side = true, Type = EChessPieceType.Pawn },
                new ChessPiece { CellId = 50, Side = true, Type = EChessPieceType.Pawn },
                new ChessPiece { CellId = 51, Side = true, Type = EChessPieceType.Pawn },
                new ChessPiece { CellId = 52, Side = true, Type = EChessPieceType.Pawn },
                new ChessPiece { CellId = 53, Side = true, Type = EChessPieceType.Pawn },
                new ChessPiece { CellId = 54, Side = true, Type = EChessPieceType.Pawn },
                new ChessPiece { CellId = 55, Side = true, Type = EChessPieceType.Pawn },

                new ChessPiece { CellId = 56, Side = true, Type = EChessPieceType.Rook },
                new ChessPiece { CellId = 57, Side = true, Type = EChessPieceType.Knight },
                new ChessPiece { CellId = 58, Side = true, Type = EChessPieceType.Bishop },
                new ChessPiece { CellId = 59, Side = true, Type = EChessPieceType.King },
                new ChessPiece { CellId = 60, Side = true, Type = EChessPieceType.Queen },
                new ChessPiece { CellId = 61, Side = true, Type = EChessPieceType.Bishop },
                new ChessPiece { CellId = 62, Side = true, Type = EChessPieceType.Knight },
                new ChessPiece { CellId = 63, Side = true, Type = EChessPieceType.Rook },
            };
        }
    }
}