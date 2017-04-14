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
            var endGame =
                !KingAvailableCells(_availableChessPieces.FirstOrDefault(p => p.Type == EChessPieceType.King && p.Side == CurrentSideTurn)).Any();

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
                //EndGameChseck();
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
                    if (left < 0 || left > 63) leftChecked = true;
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
                    if (right < 0 || right > 63) rightChecked = true;
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
                    if (up < 0 || up > 63) upChecked = true;
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
                    if (down < 0 || down > 63) downChecked = true;
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
            var offsets = new[] {6, 10, 15, 17};

            foreach (var offset in offsets)
            {
                var targetCellId = knight.CellId + offset;

                if ((targetCellId >= 0 && targetCellId < 64) &&
                    (_availableChessPieces.Any(p => p.Side != knight.Side && p.CellId == targetCellId) ||
                    (_availableChessPieces.All(p => p.CellId != targetCellId)))) result.Add(targetCellId);

                targetCellId = knight.CellId - offset;
                if ((targetCellId >= 0 && targetCellId < 64) &&
                    (_availableChessPieces.Any(p => p.Side != knight.Side && p.CellId == targetCellId) ||
                    (_availableChessPieces.All(p => p.CellId != targetCellId)))) result.Add(targetCellId);
            }
            return result;
        }

        private List<int> BishopAvailableCells(ChessPiece bishop)
        {
            var result = new List<int>();

            var northWestChecked = false;
            var northEastChecked = false;
            var southWestChecked = false;
            var southEastChecked = false;

            for (var i = 1; i < 8; i++)
            {
                if (!northWestChecked)
                {
                    var northWest = bishop.CellId - 9 * i;
                    if (northWest < 0 || northWest > 63 || bishop.CellId % 8 == 0) northWestChecked = true;
                    else
                    {
                        if (_availableChessPieces.Any(p => p.Side == bishop.Side && p.CellId == northWest)) northWestChecked = true;
                        if (_availableChessPieces.All(p => p.CellId != northWest)) result.Add(northWest);
                        if (_availableChessPieces.Any(p => p.Side != bishop.Side && p.CellId == northWest))
                        {
                            result.Add(northWest);
                            northWestChecked = true;
                        }
                    }
                    if (northWest % 8 == 0) northWestChecked = true;
                }

                if (!northEastChecked)
                {
                    var northEast = bishop.CellId - 7 * i;
                    if (northEast < 0 || northEast > 63 || (bishop.CellId + 1) % 8 == 0) northEastChecked = true;
                    else
                    {
                        if (_availableChessPieces.Any(p => p.Side == bishop.Side && p.CellId == northEast)) northEastChecked = true;
                        if (_availableChessPieces.All(p => p.CellId != northEast)) result.Add(northEast);
                        if (_availableChessPieces.Any(p => p.Side != bishop.Side && p.CellId == northEast))
                        {
                            result.Add(northEast);
                            northEastChecked = true;
                        }
                    }
                    if ((northEast + 1) % 8 == 0) northEastChecked = true;
                }

                if (!southWestChecked)
                {
                    var southWest = bishop.CellId + 7 * i;
                    if (southWest < 0 || southWest > 63 || bishop.CellId % 8 == 0) southWestChecked = true;
                    else
                    {
                        if (_availableChessPieces.Any(p => p.Side == bishop.Side && p.CellId == southWest)) southWestChecked = true;
                        if (_availableChessPieces.All(p => p.CellId != southWest)) result.Add(southWest);
                        if (_availableChessPieces.Any(p => p.Side != bishop.Side && p.CellId == southWest))
                        {
                            result.Add(southWest);
                            southWestChecked = true;
                        }
                    }
                    if (southWest % 8 == 0) southWestChecked = true;
                }

                if (!southEastChecked)
                {
                    var southEast = bishop.CellId + 9 * i;
                    if (southEast < 0 || southEast > 63 || (bishop.CellId + 1) % 8 == 0) southEastChecked = true;
                    else
                    {
                        if (_availableChessPieces.Any(p => p.Side == bishop.Side && p.CellId == southEast)) southEastChecked = true;
                        if (_availableChessPieces.All(p => p.CellId != southEast)) result.Add(southEast);
                        if (_availableChessPieces.Any(p => p.Side != bishop.Side && p.CellId == southEast))
                        {
                            result.Add(southEast);
                            southEastChecked = true;
                        }
                    }
                    if ((southEast + 1) % 8 == 0) southEastChecked = true;
                }
            }

            return result;
        }

        private List<int> QueenAvailableCells(ChessPiece queen)
        {
            var result = new List<int>();

            var northChecked = false;
            var southChecked = false;
            var westChecked = false;
            var eastChecked = false;

            var northWestChecked = false;
            var northEastChecked = false;
            var southWestChecked = false;
            var southEastChecked = false;

            for (var i = 1; i < 8; i++)
            {
                if (!westChecked)
                {
                    var left = queen.CellId - 1 * i;
                    if (left < 0 || left > 63) westChecked = true;
                    else
                    {
                        if (_availableChessPieces.Any(p => p.Side == queen.Side && p.CellId == left)) westChecked = true;
                        if (_availableChessPieces.All(p => p.CellId != left)) result.Add(left);
                        if (_availableChessPieces.Any(p => p.Side != queen.Side && p.CellId == left))
                        {
                            result.Add(left);
                            westChecked = true;
                        }
                    }
                }

                if (!eastChecked)
                {
                    var right = queen.CellId + 1 * i;
                    if (right < 0 || right > 63) eastChecked = true;
                    else
                    {
                        if (_availableChessPieces.Any(p => p.Side == queen.Side && p.CellId == right)) eastChecked = true;
                        if (_availableChessPieces.All(p => p.CellId != right)) result.Add(right);
                        if (_availableChessPieces.Any(p => p.Side != queen.Side && p.CellId == right))
                        {
                            result.Add(right);
                            eastChecked = true;
                        }
                    }
                }

                if (!northChecked)
                {
                    var up = queen.CellId - 8 * i;
                    if (up < 0 || up > 63) northChecked = true;
                    else
                    {
                        if (_availableChessPieces.Any(p => p.Side == queen.Side && p.CellId == up)) northChecked = true;
                        if (_availableChessPieces.All(p => p.CellId != up)) result.Add(up);
                        if (_availableChessPieces.Any(p => p.Side != queen.Side && p.CellId == up))
                        {
                            result.Add(up);
                            northChecked = true;
                        }
                    }
                }

                if (!southChecked)
                {
                    var down = queen.CellId + 8 * i;
                    if (down < 0 || down > 63) southChecked = true;
                    else
                    {
                        if (_availableChessPieces.Any(p => p.Side == queen.Side && p.CellId == down)) southChecked = true;
                        if (_availableChessPieces.All(p => p.CellId != down)) result.Add(down);
                        if (_availableChessPieces.Any(p => p.Side != queen.Side && p.CellId == down))
                        {
                            result.Add(down);
                            southChecked = true;
                        }
                    }
                }

                if (!northWestChecked)
                {
                    var northWest = queen.CellId - 9 * i;
                    if (northWest < 0 || northWest > 63 || queen.CellId % 8 == 0) northWestChecked = true;
                    else
                    {
                        if (_availableChessPieces.Any(p => p.Side == queen.Side && p.CellId == northWest)) northWestChecked = true;
                        if (_availableChessPieces.All(p => p.CellId != northWest)) result.Add(northWest);
                        if (_availableChessPieces.Any(p => p.Side != queen.Side && p.CellId == northWest))
                        {
                            result.Add(northWest);
                            northWestChecked = true;
                        }
                    }
                    if (northWest % 8 == 0) northWestChecked = true;
                }

                if (!northEastChecked)
                {
                    var northEast = queen.CellId - 7 * i;
                    if (northEast < 0 || northEast > 63 || (queen.CellId + 1) % 8 == 0) northEastChecked = true;
                    else
                    {
                        if (_availableChessPieces.Any(p => p.Side == queen.Side && p.CellId == northEast)) northEastChecked = true;
                        if (_availableChessPieces.All(p => p.CellId != northEast)) result.Add(northEast);
                        if (_availableChessPieces.Any(p => p.Side != queen.Side && p.CellId == northEast))
                        {
                            result.Add(northEast);
                            northEastChecked = true;
                        }
                    }
                    if ((northEast + 1) % 8 == 0) northEastChecked = true;
                }

                if (!southWestChecked)
                {
                    var southWest = queen.CellId + 7 * i;
                    if (southWest < 0 || southWest > 63 || queen.CellId % 8 == 0) southWestChecked = true;
                    else
                    {
                        if (_availableChessPieces.Any(p => p.Side == queen.Side && p.CellId == southWest)) southWestChecked = true;
                        if (_availableChessPieces.All(p => p.CellId != southWest)) result.Add(southWest);
                        if (_availableChessPieces.Any(p => p.Side != queen.Side && p.CellId == southWest))
                        {
                            result.Add(southWest);
                            southWestChecked = true;
                        }
                    }
                    if (southWest % 8 == 0) southWestChecked = true;
                }

                if (!southEastChecked)
                {
                    var southEast = queen.CellId + 9 * i;
                    if (southEast < 0 || southEast > 63 || (queen.CellId + 1) % 8 == 0) southEastChecked = true;
                    else
                    {
                        if (_availableChessPieces.Any(p => p.Side == queen.Side && p.CellId == southEast)) southEastChecked = true;
                        if (_availableChessPieces.All(p => p.CellId != southEast)) result.Add(southEast);
                        if (_availableChessPieces.Any(p => p.Side != queen.Side && p.CellId == southEast))
                        {
                            result.Add(southEast);
                            southEastChecked = true;
                        }
                    }
                    if ((southEast + 1) % 8 == 0) southEastChecked = true;
                }
            }
            return result;
        }

        private List<int> KingAvailableCells(ChessPiece king)
        {
            var tmpResult = new List<int>();
            var offsets = new[] {1, 7, 8, 9};
            foreach (var offset in offsets)
            {
                var targetCellId = king.CellId + offset;
                if ((targetCellId >= 0 && targetCellId < 64) &&
                    (_availableChessPieces.Any(p => p.Side != king.Side && p.CellId == targetCellId) ||
                     (_availableChessPieces.All(p => p.CellId != targetCellId)))) tmpResult.Add(targetCellId);

                targetCellId = king.CellId - offset;
                if ((targetCellId >= 0 && targetCellId < 64) &&
                    (_availableChessPieces.Any(p => p.Side != king.Side && p.CellId == targetCellId) ||
                     (_availableChessPieces.All(p => p.CellId != targetCellId)))) tmpResult.Add(targetCellId);
            }

            var opponentChessPieces = _availableChessPieces.Where(p => p.Side != king.Side);
            var result = tmpResult;

//            foreach (var chessPiece in opponentChessPieces)
//                tmpResult.AddRange(GetReachableCells(chessPiece));

            return result; //.Except(tmpResult).ToList();
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