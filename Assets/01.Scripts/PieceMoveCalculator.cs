using System.Collections.Generic;
using UnityEngine;

public static class PieceMoveCalculator
{
    public static List<(int, int)> moveCalculator(int currentRow, int currentCol, SquareController.PieceType[,] board,
        SquareController.PieceType pieceType, CastlingRights castlingRights, (int, int)? lastEnpassant = null )
    {
        List<(int, int)> result = new List<(int, int)>();
        switch (pieceType)
        {
            case SquareController.PieceType.BlackKnight:
            case SquareController.PieceType.WhiteKnight:
                result = KnightMove(result, currentRow, currentCol, board);
                break;
            case SquareController.PieceType.BlackRook:
            case SquareController.PieceType.WhiteRook:
                result = RookMove(result, currentRow, currentCol, board);
                break;
            case SquareController.PieceType.WhiteQueen:
            case SquareController.PieceType.BlackQueen:
                result = QueenMove(result, currentRow, currentCol, board);
                break;
            case SquareController.PieceType.WhiteKing:
            case SquareController.PieceType.BlackKing:
                result = KingMove(result, currentRow, currentCol, board , castlingRights);
                break;
            case SquareController.PieceType.WhiteBishop:
            case SquareController.PieceType.BlackBishop:
                result = BishopMove(result, currentRow, currentCol, board);
                break;
            case SquareController.PieceType.WhitePawn:
                case SquareController.PieceType.BlackPawn:
                result = PawnMove(result, currentRow, currentCol, board,lastEnpassant);
                break;
        }

        return result;
    }


//TODO: 각각의 움직일 수 있는 것을 리턴하는 함수들, 특이사항은 폰은 색마다 다르고, 막히면 못 가는 것도 구현해야 함. 


    public static List<(int, int)> PawnMove(List<(int, int)> result, int currentRow, int currentCol,
        SquareController.PieceType[,] board, (int,int)? lastEnpassant = null)
    {
        bool isWhite = IsWhite(board[currentRow, currentCol]);
        
        int direction = isWhite ? -1 : 1;
        
        bool isFirstMove = true;
        int line = isWhite ? 6 : 1;
        
        isFirstMove = currentRow == line ? true : false; 
        
        int nextRow = currentRow + direction;
        
        // 한 칸 앞이 막혀 있다면 
        if (nextRow < 8 && nextRow >= 0 &&board[nextRow, currentCol] == SquareController.PieceType.None)
        {
            result.Add((nextRow, currentCol));

            if (isFirstMove &&  board[nextRow + direction, currentCol] == SquareController.PieceType.None)
            {
                result.Add((nextRow + direction, currentCol));
            }
        }
        for (int colOffset = -1; colOffset <= 1; colOffset += 2)
        {
            int captureCol = currentCol + colOffset;
            if (captureCol >= 0 && captureCol < 8)
            {
                nextRow = currentRow + direction;
                if (nextRow >= 0 && nextRow < 8)
                {
                    // 대각선에 상대방 말이 있으면 잡을 수 있음
                    if (board[nextRow, captureCol] != SquareController.PieceType.None && 
                        IsWhite(board[nextRow, captureCol]) != isWhite)
                    {
                        result.Add((nextRow, captureCol));
                    }
                }
            }
        }
        if (lastEnpassant.HasValue)
        {
            bool isWhiteEnpassant = IsWhite(board[currentRow, currentCol]);

            int enpassantRow = lastEnpassant.Value.Item1;
            int enpassantCol = lastEnpassant.Value.Item2;

            // Whitepawn일 경우 en passant는 1 + 2 = 3 (0123) 번째 row 에서만 가능
            if (isWhiteEnpassant && currentRow == 3 && enpassantRow == 3 &&
                Mathf.Abs(enpassantCol - currentCol) == 1)
            {
                result.Add((2, enpassantCol));
            }
            // Blackpawn일 경우 6 - 2 = 4 (7654) 번째 row 에서만 가능
            else if (!isWhiteEnpassant && currentRow == 4 && enpassantRow == 4 &&
                     Mathf.Abs(enpassantCol - currentCol) == 1)
            {
                result.Add((5, enpassantCol));
            }
        }
        return result;
        
    }
    public static List<(int, int)> KnightMove(List<(int, int)> result, int currentRow, int currentCol, SquareController.PieceType[,] board)
    {
        // 나이트의 이동 패턴 (L자 모양)
        int[] rowOffsets = { 2, 2, -2, -2, 1, 1, -1, -1 };
        int[] colOffsets = { 1, -1, 1, -1, -2, 2, -2, 2 };
    
        bool isWhite = IsWhite(board[currentRow, currentCol]);
    
        // 나이트는 장애물을 넘어갈 수 있음
        for (int i = 0; i < 8; i++)
        {
            int newRow = currentRow + rowOffsets[i];
            int newCol = currentCol + colOffsets[i];
        
            // 아웃보드인지... 여기서 체크해야 함 아니면 오류뜸
            if (newRow >= 0 && newRow < 8 && newCol >= 0 && newCol < 8)
            {
                // 목적지가 비어있거나 상대 말인 경우에만 이동 가능
                if (board[newRow, newCol] == SquareController.PieceType.None || 
                    IsWhite(board[newRow, newCol]) != isWhite)
                {
                    result.Add((newRow, newCol));
                }
            }
        }
    
        result = outBoard(result);
        return result;
    }
    public static List<(int, int)> RookMove(List<(int, int)> result, int currentRow, int currentCol,
        SquareController.PieceType[,] board)
    {
        // 상, 하, 좌, 우 네 방향
        List<(int, int)> up = new List<(int, int)>();
        List<(int, int)> down = new List<(int, int)>();
        List<(int, int)> left = new List<(int, int)>();
        List<(int, int)> right = new List<(int, int)>();

        bool isWhite = IsWhite(board[currentRow, currentCol]);

        // 위쪽 방향 탐색
        for (int i = currentRow - 1; i >= 0; i--)
            up.Add((i, currentCol));

        // 아래쪽 방향 탐색
        for (int i = currentRow + 1; i < 8; i++)
            down.Add((i, currentCol));

        // 왼쪽 방향 탐색
        for (int i = currentCol - 1; i >= 0; i--)
            left.Add((currentRow, i));

        // 오른쪽 방향 탐색
        for (int i = currentCol + 1; i < 8; i++)
            right.Add((currentRow, i));

        // 각 방향별로 유효한 이동을 필터링
        result.AddRange(validMove(up, board, isWhite));
        result.AddRange(validMove(down, board, isWhite));
        result.AddRange(validMove(left, board, isWhite));
        result.AddRange(validMove(right, board, isWhite));

        result = outBoard(result);

        return result;
    }

    public static List<(int, int)> BishopMove(List<(int, int)> result, int currentRow, int currentCol,
        SquareController.PieceType[,] board)
    {
        // 대각선 네 방향
        List<(int, int)> upRight = new List<(int, int)>();
        List<(int, int)> upLeft = new List<(int, int)>();
        List<(int, int)> downRight = new List<(int, int)>();
        List<(int, int)> downLeft = new List<(int, int)>();

        bool isWhite = IsWhite(board[currentRow, currentCol]);

        // 우상단 대각선 탐색
        for (int i = 1; i < 8; i++)
        {
            if (currentRow - i >= 0 && currentCol + i < 8)
                upRight.Add((currentRow - i, currentCol + i));
        }

        // 좌상단 대각선 탐색
        for (int i = 1; i < 8; i++)
        {
            if (currentRow - i >= 0 && currentCol - i >= 0)
                upLeft.Add((currentRow - i, currentCol - i));
        }

        // 우하단 대각선 탐색
        for (int i = 1; i < 8; i++)
        {
            if (currentRow + i < 8 && currentCol + i < 8)
                downRight.Add((currentRow + i, currentCol + i));
        }

        // 좌하단 대각선 탐색
        for (int i = 1; i < 8; i++)
        {
            if (currentRow + i < 8 && currentCol - i >= 0)
                downLeft.Add((currentRow + i, currentCol - i));
        }

        // 각 방향별로 유효한 이동을 필터링
        result.AddRange(validMove(upRight, board, isWhite));
        result.AddRange(validMove(upLeft, board, isWhite));
        result.AddRange(validMove(downRight, board, isWhite));
        result.AddRange(validMove(downLeft, board, isWhite));

        result = outBoard(result);

        return result;
    }

    public static List<(int, int)> KingMove(List<(int, int)> result, int currentRow, int currentCol,
        SquareController.PieceType[,] board, CastlingRights castlingRights)
    {
        bool isWhite = IsWhite(board[currentRow, currentCol]);

        // 킹의 모든 이동 가능한 8방향 (상, 하, 좌, 우, 대각선 4방향)
        int[] rowOffsets = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] colOffsets = { -1, 0, 1, -1, 1, -1, 0, 1 };

        // 킹은 한 칸씩만 이동 가능
        for (int i = 0; i < 8; i++)
        {
            int newRow = currentRow + rowOffsets[i];
            int newCol = currentCol + colOffsets[i];

            // 보드 경계 체크는 outBoard에서 처리될 것이므로 여기서는 생략
            if (newRow >= 0 && newRow < 8 && newCol >= 0 && newCol < 8)
            {
                // 빈 칸이거나 상대 말이면 이동 가능
                if (board[newRow, newCol] == SquareController.PieceType.None ||
                    IsWhite(board[newRow, newCol]) != isWhite)
                {
                    result.Add((newRow, newCol));
                }
            }
        }
// 캐슬링 조건 체크
        if (isWhite && !castlingRights.WhiteKingMoved)
        {
            
            // 퀸 사이드 (왼쪽)
            if (!castlingRights.WhiteLeftRookMoved &&
                board[7, 1] == SquareController.PieceType.None &&
                board[7, 2] == SquareController.PieceType.None &&
                board[7, 3] == SquareController.PieceType.None )
            {
                if (!GetAllAttackPositions(false, board).Contains((7, 2)) &&
                    !GetAllAttackPositions(false, board).Contains((7, 3)) &&
                    !GetAllAttackPositions(false, board).Contains((7, 4)))
                {
                    result.Add((7, 2)); 

                }
            }

            // 킹 사이드 (오른쪽)
            if (!castlingRights.WhiteRightRookMoved &&
                board[7, 5] == SquareController.PieceType.None &&
                board[7, 6] == SquareController.PieceType.None)
            {
                if (!GetAllAttackPositions(false, board).Contains((7, 4)) &&
                    !GetAllAttackPositions(false, board).Contains((7, 5)) &&
                    !GetAllAttackPositions(false, board).Contains((7, 6)))
                {
                    result.Add((7, 6));

                }
            }
        }
        
        else if (!isWhite && !castlingRights.BlackKingMoved)
        {
            // 퀸 사이드
            if (!castlingRights.BlackLeftRookMoved &&
                board[0, 1] == SquareController.PieceType.None &&
                board[0, 2] == SquareController.PieceType.None &&
                board[0, 3] == SquareController.PieceType.None)
            {
                if (!GetAllAttackPositions(true, board).Contains((0, 2)) &&
                    !GetAllAttackPositions(true, board).Contains((0, 3)) &&
                    !GetAllAttackPositions(true, board).Contains((0, 4)))
                {
                    result.Add((0, 2));

                }
            }

            // 킹 사이드
            if (!castlingRights.BlackRightRookMoved &&
                board[0, 5] == SquareController.PieceType.None &&
                board[0, 6] == SquareController.PieceType.None)
            {
                if (!GetAllAttackPositions(true, board).Contains((0, 4)) &&
                    !GetAllAttackPositions(true, board).Contains((0, 5)) &&
                    !GetAllAttackPositions(true, board).Contains((0, 6)))
                {
                    result.Add((0, 6));
                }
            }
        }

        result = outBoard(result);

        return result;
    }

    public static List<(int, int)> QueenMove(List<(int, int)> result, int currentRow, int currentCol,
        SquareController.PieceType[,] board)
    {
        // 퀸은 룩과 비숍의 이동을 합친 것
        // 룩처럼 상하좌우로 이동
        result = RookMove(result, currentRow, currentCol, board);

        // 비숍처럼 대각선으로 이동
        result = BishopMove(result, currentRow, currentCol, board);

        return result;
    }
    
    // 여기는 어택, 그러니까 공격 위치만 계산하는 함수
    // 다른 애들은 다 상관 없는데, 폰은 공격과 이동이 다르고 ,
    
    public static List<(int, int)> GetAllAttackPositions(bool byWhite, SquareController.PieceType[,] board)
    {
        List<(int, int)> attackPositions = new List<(int, int)>();

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                SquareController.PieceType piece = board[row, col];
                if (piece == SquareController.PieceType.None) continue;
                if (IsWhite(piece) != byWhite) continue;

                List<(int, int)> moves;

                switch (piece)
                {
                    case SquareController.PieceType.WhitePawn:
                    case SquareController.PieceType.BlackPawn:
                        moves = GetPawnAttackPositions(row, col, board);
                        break;

                    case SquareController.PieceType.WhiteKing:
                    case SquareController.PieceType.BlackKing:
                        moves = GetKingAttackPositions(row, col, board);
                        break;

                    default:
                        moves = moveCalculator(row, col, board, piece, new CastlingRights(), null);
                        break;
                }

                attackPositions.AddRange(moves);
            }
        }

        return attackPositions;
    }

    private static List<(int, int)> GetPawnAttackPositions(int row, int col, SquareController.PieceType[,] board)
    {
        List<(int, int)> result = new List<(int, int)>();
        bool isWhite = IsWhite(board[row, col]);

        int direction = isWhite ? -1 : 1;

        for (int colOffset = -1; colOffset <= 1; colOffset += 2)
        {
            int attackRow = row + direction;
            int attackCol = col + colOffset;

            if (attackRow >= 0 && attackRow < 8 && attackCol >= 0 && attackCol < 8)
            {
                result.Add((attackRow, attackCol));
            }
        }

        return result;
    }
    private static List<(int, int)> GetKingAttackPositions(int row, int col, SquareController.PieceType[,] board)
    {
        List<(int, int)> result = new List<(int, int)>();
        int[] rowOffsets = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] colOffsets = { -1, 0, 1, -1, 1, -1, 0, 1 };

        for (int i = 0; i < 8; i++)
        {
            int newRow = row + rowOffsets[i];
            int newCol = col + colOffsets[i];

            if (newRow >= 0 && newRow < 8 && newCol >= 0 && newCol < 8)
            {
                result.Add((newRow, newCol));
            }
        }

        return result;
    }



    //TODO: 리스트 돌면서 배열 밖에 있으면 삭제하는 함수
    public static List<(int, int)> outBoard(List<(int, int)> result)
    {
        for (int i = 0; i < result.Count; i++)
        {
            if (result[i].Item1 < 0 || result[i].Item1 > 7 || result[i].Item2 < 0 || result[i].Item2 > 7)
            {
                result.RemoveAt(i);
                i--;
            }
        }

        return result;
    }

    public static bool IsWhite(SquareController.PieceType piece)
    {
        if (piece == SquareController.PieceType.WhiteBishop ||
            piece == SquareController.PieceType.WhiteKing ||
            piece == SquareController.PieceType.WhiteKnight ||
            piece == SquareController.PieceType.WhiteQueen ||
            piece == SquareController.PieceType.WhiteRook ||
            piece == SquareController.PieceType.WhitePawn
           ) return true;
        else return false;
    }


    /// <summary>
    /// 리스트에 들어 있는 결과값들을 하나씩 꺼내면서, 다른 블럭에 막히면 멈춘 리스트 반환
    /// </summary>
    /// <param name="result"></param>
    /// <param name="board"></param>
    /// <param name="isWhite"></param>
    /// <returns></returns>
    public static List<(int, int)> validMove(List<(int, int)> result, SquareController.PieceType[,] board, bool isWhite)
    {
        List<(int, int)> validMoves = new List<(int, int)>();

        for (int i = 0; i < result.Count; i++)
        {
            if (board[result[i].Item1, result[i].Item2] != SquareController.PieceType.None)
            {
                // 같은 색이라면 
                if (IsWhite(board[result[i].Item1, result[i].Item2]) == isWhite) break;
                validMoves.Add((result[i].Item1, result[i].Item2));
                break;
            }

            //아무 것도 없는 곳이라면
            validMoves.Add((result[i].Item1, result[i].Item2));
        }

        return validMoves;
    }
}