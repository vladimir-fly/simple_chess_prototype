using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SCPrototype
{
    public class Chessboard
    {
        private List<ChessPiece> _availableChessPieces;
        private bool CurrentSideTurn = true;

        public event Action SideChanged;

        public event Action GameEnded;

        public Chessboard(List<ChessPiece> chessPieces)
        {
            _availableChessPieces = chessPieces;
        }

        public List<ChessPiece> GetChessPieces()
        {
            return _availableChessPieces;
        }

        protected virtual void OnSideChanged()
        {
            if (SideChanged != null) SideChanged();
        }

        private void EndGameCheck()
        {
            var endGame = false;

            if (endGame && GameEnded != null)
                GameEnded();
        }

        public bool TryMove(int from, int to)
        {
            Debug.Log(string.Format("[Chessboard.TryMove] From: {0}; to: {1}.", from, to));

            var result = CheckMove(from, to);
            if (result)
            {
                Move(from, to);
                ChangeSide();
                EndGameCheck();
            }

            Debug.Log(string.Format("[Chessboard.TryMove] result = {0}", result));
            return result;
        }

        private void ChangeSide()
        {
            CurrentSideTurn = !CurrentSideTurn;
            OnSideChanged();
        }

        private bool CheckMove(int from, int to)
        {
            var chessPiece = _availableChessPieces.FirstOrDefault(p => p.CellId == from && p.Side == CurrentSideTurn);

            if (chessPiece == null)
            {
                Debug.Log("[Chessboard.CheckMove] Invalid move!");
                return false;
            }

            var reachableCells = GetReachableCells(chessPiece);
            return reachableCells.Contains(to);
        }

        private List<int> GetReachableCells(ChessPiece chessPiece)
        {
            Debug.Log(string.Format("[Chessboard.GetReachableCells] Type: {0}; Side: {1}; CellId: {2}.",
                chessPiece.Type, chessPiece.Side ? "white" : "black", chessPiece.CellId));

            var result = new List<int>();

            switch (chessPiece.Type)
            {
                case EChessPieceType.Pawn:
                    result = PawnAvailableCells(chessPiece);
                    break;
                case EChessPieceType.Knight:
                    result = KnightAvailableCells(chessPiece);
                    break;
                case EChessPieceType.Bishop:
                    result = BishopAvailableCells(chessPiece);
                    break;
                case EChessPieceType.Rook:
                    result = RookAvailableCells(chessPiece);
                    break;
                case EChessPieceType.Queen:
                    result = QueenAvailableCells(chessPiece);
                    break;
                case EChessPieceType.King:
                    result = KingAvailableCells(chessPiece);
                    break;
            }

            Debug.Log(string.Format("[Chessboard.GetReachableCells] {0} available cells: {1}.",
                chessPiece.Type, string.Join(", ", result.Select(p => p.ToString()).ToArray())));

            return result;
        }

        private List<int> PawnAvailableCells(ChessPiece pawn)
        {
            var result = new List<int>();
            switch (pawn.Side)
            {
                case true:
                {
                    if (_availableChessPieces.All(p => pawn.Side && p.CellId != pawn.CellId - 8))
                        result.Add(pawn.CellId - 8);
                    if (_availableChessPieces.All(p => pawn.Side && pawn.CellId / 8 == 6 &&
                            p.CellId != pawn.CellId - 8 && p.CellId != pawn.CellId - 16)) result.Add(pawn.CellId - 16);

                    result.AddRange(_availableChessPieces.Where(p => !p.Side &&
                            p.CellId == pawn.CellId - 7 || p.CellId == pawn.CellId - 9) .Select(p => p.CellId));
                    break;
                }
                case false:
                {
                    if (_availableChessPieces.All(p => !pawn.Side && p.CellId != pawn.CellId + 8))
                        result.Add(pawn.CellId + 8);

                    if (_availableChessPieces.All(p => !pawn.Side && pawn.CellId / 8 == 1 &&
                        p.CellId != pawn.CellId + 8 && p.CellId != pawn.CellId + 16)) result.Add(pawn.CellId + 16);

                    result.AddRange(_availableChessPieces.Where(p => p.Side &&
                        p.CellId == pawn.CellId + 7 || p.CellId == pawn.CellId + 9).Select(p => p.CellId));
                    break;
                }
            }
            return result;
        }

        private List<int> RookAvailableCells(ChessPiece rook)
        {
            var result = new List<int>();

            var leftChecked = false;
            var rightChecked = false;
            var upChecked = false;
            var downChecked = false;

            for (var i = 1; i < 8; i++)
            {
                if (!leftChecked)
                {
                    var left = rook.CellId - 1 * i;
                    var tmp = rook.CellId / 8;

                    if (left < tmp * 8) leftChecked = true;
                    else
                    {
                        if (_availableChessPieces.Any(p => p.Side == rook.Side && p.CellId == left)) leftChecked = true;
                        if (_availableChessPieces.All(p => p.CellId != left)) result.Add(left);
                        if (_availableChessPieces.Any(p => p.Side != rook.Side && p.CellId == left))
                        {
                            result.Add(left);
                            leftChecked = true;
                        }
                    }
                }

                if (!rightChecked)
                {
                    var right = rook.CellId + 1 * i;
                    var tmp = rook.CellId / 8;

                    if (right > tmp * 8) rightChecked = true;
                    else
                    {
                        if (_availableChessPieces.Any(p => p.Side == rook.Side && p.CellId == right)) rightChecked = true;
                        if (_availableChessPieces.All(p => p.CellId != right)) result.Add(right);
                        if (_availableChessPieces.Any(p => p.Side != rook.Side && p.CellId == right))
                        {
                            result.Add(right);
                            rightChecked = true;
                        }
                    }
                }

                if (!upChecked)
                {
                    var up = rook.CellId - 8 * i;

                    if (up < 0) upChecked = true;
                    else
                    {
                        if (_availableChessPieces.Any(p => p.Side == rook.Side && p.CellId == up)) upChecked = true;
                        if (_availableChessPieces.All(p => p.CellId != up)) result.Add(up);
                        if (_availableChessPieces.Any(p => p.Side != rook.Side && p.CellId == up))
                        {
                            result.Add(up);
                            upChecked = true;
                        }
                    }
                }

                if (!downChecked)
                {
                    var down = rook.CellId + 8 * i;

                    if (down > 63) downChecked = true;
                    else
                    {
                        if (_availableChessPieces.Any(p => p.Side == rook.Side && p.CellId == down)) downChecked = true;
                        if (_availableChessPieces.All(p => p.CellId != down)) result.Add(down);
                        if (_availableChessPieces.Any(p => p.Side != rook.Side && p.CellId == down))
                        {
                            result.Add(down);
                            downChecked = true;
                        }
                    }
                }
            }

            return result;
        }

        private List<int> KnightAvailableCells(ChessPiece knight)
        {
            var result = new List<int>();

            switch (knight.Side)
            {
                case true:



                    break;
                case false:

                    break;
            }

            return result;
        }

        private List<int> BishopAvailableCells(ChessPiece bishop)
        {
            var result = new List<int>();

            switch (bishop.Side)
            {
                case true:



                    break;
                case false:

                    break;
            }


            return result;
        }

        private List<int> QueenAvailableCells(ChessPiece queen)
        {
            var result = new List<int>();

            switch (queen.Side)
            {
                case true:

                    break;
                case false:

                    break;
            }


            return result;
        }

        private List<int> KingAvailableCells(ChessPiece king)
        {
            var result = new List<int>();

            switch (king.Side)
            {
                case true:

                    break;
                case false:

                    break;
            }

            return result;
        }

        private void Move(int from, int to)
        {
            if (_availableChessPieces.Any(p => p.CellId == to))
                _availableChessPieces.RemoveAll(p => p.CellId == to);

            var chessPiece = _availableChessPieces.Find(p => p.CellId == from);
            chessPiece.CellId = to;
        }
    }
}