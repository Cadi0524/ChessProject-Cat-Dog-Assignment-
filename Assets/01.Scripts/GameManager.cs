using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private SquareController squareController;
    private SquareController.PieceType[,] board;

    private SquareController.PieceType currentPiece = SquareController.PieceType.None;
    private (int,int)? prevPos;

    private List<(int, int)> result = new List<(int, int)>();
    
    


    public void Start()
    {
        squareController = FindObjectOfType<SquareController>();
        board = new SquareController.PieceType[8, 8];
        //로직에 사용할 보드 세팅
        InitChessBoard();
        //체스판 이미지 세팅
        UpdateAllBoardSprites();
        squareController.OnButtonClicked += ClickBoard;
    }


    public void OnClickChessBoard()
    {
    }

    /// <summary>
    /// 피스를 선택했을 때 빛나고, 움직임을 가능하게 함
    /// </summary>
    /// <param name="clickedSquare"></param>
    public void ClickBoard(int row ,int col)
    {
        if (board[row, col] != SquareController.PieceType.None)
        {
            if (board[row, col] == SquareController.PieceType.CanMove)
            {
                board[row,col] = currentPiece;
                board[prevPos.Value.Item1, prevPos.Value.Item2] = SquareController.PieceType.None;
                UpdateBoardSprites(row, col);
                UpdateBoardSprites(prevPos.Value.Item1, prevPos.Value.Item2);
                return;
            }
            ClearCanMove(result);
            //이동가능한 리스트 return
            currentPiece = board[row, col];
            prevPos = (row, col);
            result = PieceMoveCalculator.moveCalculator(row, col, board, currentPiece);

            //이동가능한 모든 리스트들을 마커칠함.
            foreach (var (i, j) in result)
            {
                squareController.SetMarker(i, j, SquareController.PieceType.CanMove);
                board[i, j] = SquareController.PieceType.CanMove;
            }

            
        }
        else
        {
            currentPiece = SquareController.PieceType.None;
            prevPos = null;
        }
       
    }

    /// <summary>
    /// 새롭게 Piece를 선택하기 전, 그 전 선택에서 return한 이동 가능 square들을 지우는 함수
    /// </summary>
    /// <param name="result"></param>
    public void ClearCanMove(List<(int, int)> result)
    {
        foreach (var (i, j) in result)
        {
            squareController.SetMarker(i, j, SquareController.PieceType.None);
        }

        result.Clear();
    }

    public void MovePiece(Square clickedSquare)
    {
        squareController.SetMarker(clickedSquare.position.Item1, clickedSquare.position.Item2, currentPiece);
    }


    /// <summary>
    /// 체스판 보드 세팅
    /// </summary>
    void InitChessBoard()
    {
        // 검은색 기물 배치
        board[0, 0] = SquareController.PieceType.BlackRook;
        board[0, 1] = SquareController.PieceType.BlackKnight;
        board[0, 2] = SquareController.PieceType.BlackBishop;
        board[0, 3] = SquareController.PieceType.BlackQueen;
        board[0, 4] = SquareController.PieceType.BlackKing;
        board[0, 5] = SquareController.PieceType.BlackBishop;
        board[0, 6] = SquareController.PieceType.BlackKnight;
        board[0, 7] = SquareController.PieceType.BlackRook;
        for (int i = 0; i < 8; i++)
        {
            board[1, i] = SquareController.PieceType.BlackPawn;
        }

        // 흰색 기물 배치
        for (int i = 0; i < 8; i++)
        {
            board[6, i] = SquareController.PieceType.WhitePawn;
        }

        board[7, 0] = SquareController.PieceType.WhiteRook;
        board[7, 1] = SquareController.PieceType.WhiteKnight;
        board[7, 2] = SquareController.PieceType.WhiteBishop;
        board[7, 3] = SquareController.PieceType.WhiteQueen;
        board[7, 4] = SquareController.PieceType.WhiteKing;
        board[7, 5] = SquareController.PieceType.WhiteBishop;
        board[7, 6] = SquareController.PieceType.WhiteKnight;
        board[7, 7] = SquareController.PieceType.WhiteRook;

        // 빈 칸 초기화 
        for (int row = 2; row < 6; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                //이미 마커가 설정되어 있지 않다면 빈 칸으로 설정
                board[row, col] = SquareController.PieceType.None;
            }
        }
    }

    /// <summary>
    /// 현재 보드 상황에 맞게 모든 이미지 업데이트
    /// </summary>
    void UpdateAllBoardSprites()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                squareController.SetMarker(row, col, board[row, col]);
            }
        }
    }

    /// <summary>
    /// 특정 칸이 변했을 때 이미지 업데이트
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    void UpdateBoardSprites(int row, int col)
    {
        squareController.SetMarker(row, col, board[row, col]);
    }


    public bool isWhite(SquareController.PieceType piece)
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
}