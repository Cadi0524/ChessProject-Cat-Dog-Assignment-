using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class CastlingRights
{
    public bool WhiteKingMoved = false;
    public bool WhiteLeftRookMoved = false;
    public bool WhiteRightRookMoved = false;

    public bool BlackKingMoved = false;
    public bool BlackLeftRookMoved = false;
    public bool BlackRightRookMoved = false;
}

public class GameManager : MonoBehaviour
{
    private SquareController squareController;
    private SquareController.PieceType[,] board;
    public Canvas canvas;

    private SquareController.PieceType currentPiece = SquareController.PieceType.None;
    private (int, int)? prevPos;

    private List<(int, int)> result = new List<(int, int)>();

    public GameObject PanelPrefab;

    //앙파상 체크용 변수
    public (int, int)? lastEnPassant = null;

    
    // 캐슬링 확인용 변수
    public CastlingRights castlingRights;


    // false면 흑 차례 , true면 백 차례 
    private bool currentTurn = true;

    public enum ResultType
    {
        None,
        WhiteWin,
        BlackWin,
        Draw,
    }

    private bool WhiteDraw = false;
    private bool BlackDraw = false;


    public void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        squareController = FindObjectOfType<SquareController>();
        board = new SquareController.PieceType[8, 8];
        //로직에 사용할 보드 세팅
        InitChessBoard();
        //체스판 이미지 세팅
        UpdateAllBoardSprites();
        squareController.OnButtonClicked += ClickBoard;
    }


    /// <summary>
    /// 피스를 선택했을 때 빛나고, 움직임을 가능하게 함
    /// </summary>
    /// <param name="clickedSquare"></param>
    public void ClickBoard(int row, int col, bool isHighlighted)
    {
        //TODO: 일단 검사를 해야 함, 클릭한 곳이 highlighted인지 아닌지
        // highlighted면 선택한 말을 이동시킴, 아니라면 새로운 말을 선택했을 때와 빈 공간을 선택했을 때로나뉨


        if (isHighlighted)
        {
            board[row, col] = currentPiece;
            board[prevPos.Value.Item1, prevPos.Value.Item2] = SquareController.PieceType.None;
            UpdateBoardSprites(row, col);
            UpdateBoardSprites(prevPos.Value.Item1, prevPos.Value.Item2);
            //TODO: 이전에 하이라이트 된 부분 다 취소시켜야 함.
            foreach ((int, int) p in result)
            {
                squareController.SetHighlight(p.Item1, p.Item2, false);
            }

            if (currentPiece == SquareController.PieceType.WhitePawn &&
                prevPos.Value.Item1 == 3 && row == 2 &&
                Mathf.Abs(col - prevPos.Value.Item2) == 1 &&
                lastEnPassant.HasValue &&
                lastEnPassant.Value.Item1 == 3 && lastEnPassant.Value.Item2 == col)
            {
                // 앙파상 - 흑 폰 제거
                board[3, col] = SquareController.PieceType.None;
                UpdateBoardSprites(3, col);
            }
            else if (currentPiece == SquareController.PieceType.BlackPawn &&
                     prevPos.Value.Item1 == 4 && row == 5 &&
                     Mathf.Abs(col - prevPos.Value.Item2) == 1 &&
                     lastEnPassant.HasValue &&
                     lastEnPassant.Value.Item1 == 4 && lastEnPassant.Value.Item2 == col)
            {
                // 앙파상 - 백 폰 제거
                board[4, col] = SquareController.PieceType.None;
                UpdateBoardSprites(4, col);
            }

            // 두 칸 움직였을 때만 변수에다가 움직인 위치 추가
            if (currentPiece == SquareController.PieceType.WhitePawn && prevPos.Value.Item1 - row == 2)
            {
                lastEnPassant = (row, col);
            }
            // 여긴 row가 더 크니까
            else if (currentPiece == SquareController.PieceType.BlackPawn && row - prevPos.Value.Item1 == 2)
            {
                lastEnPassant = (row, col);
            }
            else lastEnPassant = null;


            //프로모션 기능 구현 -> 추가적으로 선택하는 것까지 구현해야함. 
            if (currentPiece == SquareController.PieceType.WhitePawn && row == 0)
            {
                board[row, col] = SquareController.PieceType.WhiteQueen;
                UpdateBoardSprites(row, col);
            }

            if (currentPiece == SquareController.PieceType.BlackPawn && row == 7)
            {
                board[row, col] = SquareController.PieceType.BlackQueen;
                UpdateBoardSprites(row, col);
            }
            // 캐슬링 시 이동해야함
            if (currentPiece == SquareController.PieceType.WhiteKing)
            {
                // 킹사이드 캐슬링(화이트 version)
                if (prevPos == (7, 4) && (row, col) == (7, 6))
                {
                    board[7, 5] = board[7, 7]; // 룩 이동
                    board[7, 7] = SquareController.PieceType.None;
                    UpdateBoardSprites(7, 5);
                    UpdateBoardSprites(7, 7);
                    castlingRights.WhiteKingMoved = true;
                    castlingRights.WhiteRightRookMoved = true;
                }
                // 퀸사이드 (화이트 version)
                else if (prevPos == (7, 4) && (row, col) == (7, 2))
                {
                    board[7, 3] = board[7, 0];
                    board[7, 0] = SquareController.PieceType.None;
                    UpdateBoardSprites(7, 3);
                    UpdateBoardSprites(7, 0);
                    castlingRights.WhiteKingMoved = true;
                    castlingRights.WhiteLeftRookMoved = true;
                }
            }
            else if (currentPiece == SquareController.PieceType.BlackKing)
            {
                // 킹사이드 캐슬링 (다크 version)
                if (prevPos == (0, 4) && (row, col) == (0, 6))
                {
                    board[0, 5] = board[0, 7];
                    board[0, 7] = SquareController.PieceType.None;
                    UpdateBoardSprites(0, 5);
                    UpdateBoardSprites(0, 7);
                    castlingRights.BlackKingMoved = true;
                    castlingRights.BlackRightRookMoved = true;
                }
                // 퀸사이드 캐슬링 (다크 version)
                else if (prevPos == (0, 4) && (row, col) == (0, 2))
                {
                    board[0, 3] = board[0, 0];
                    board[0, 0] = SquareController.PieceType.None;
                    UpdateBoardSprites(0, 3);
                    UpdateBoardSprites(0, 0);
                    castlingRights.BlackKingMoved = true;
                    castlingRights.BlackLeftRookMoved = true;
                }
            }


            ResultType Gameresult = CheckGameOver();
            if (Gameresult == ResultType.None) currentTurn = !currentTurn;
            else if (Gameresult == ResultType.WhiteWin)
            {
                ShowPanel("White win", () => { }, () =>
                {
                    InitChessBoard();
                    UpdateAllBoardSprites();
                    currentPiece = SquareController.PieceType.None;
                    currentTurn = true;
                });
            }
            else if (Gameresult == ResultType.BlackWin)
            {
                ShowPanel("Black win", () => { }, () =>
                {
                    InitChessBoard();
                    UpdateAllBoardSprites();
                    currentPiece = SquareController.PieceType.None;
                    currentTurn = true;
                });
            }

            return;
        }
        else
        {
            // 만약 빈 땅, 하이라이트도 되지 않은 공간을 클릭했다면 하이라이트 된 부분을 삭제함
            if (board[row, col] == SquareController.PieceType.None)
            {
                currentPiece = SquareController.PieceType.None;
                prevPos = null;
                foreach ((int, int) p in result)
                {
                    squareController.SetHighlight(p.Item1, p.Item2, false);
                }
            }
            // 체스 피스를 선택했다면 이동 가능한 공간에 하이라이트를 칠함.
            else
            {
                //현재 턴이 아니라면 리턴 ~
                if (PieceMoveCalculator.IsWhite(board[row, col]) != currentTurn) return;
                // 전에 선택했던 말 못가게
                foreach ((int, int) p in result)
                {
                    squareController.SetHighlight(p.Item1, p.Item2, false);
                }

                currentPiece = board[row, col];
                prevPos = (row, col);
                result = PieceMoveCalculator.moveCalculator(row, col, board, currentPiece, castlingRights, lastEnPassant);
                foreach ((int, int) p in result)
                {
                    squareController.SetHighlight(p.Item1, p.Item2, true);
                }
            }
        }
    }

    /// <summary>
    /// 새롭게 Piece를 선택하기 전, 그 전 선택에서 return한 이동 가능 square들을 지우는 함수
    /// </summary>
    /// <param name="result"></param>
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

        castlingRights = new CastlingRights();
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


    private ResultType CheckGameOver()
    {
        bool White = false;
        bool Black = false;
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] == SquareController.PieceType.WhiteKing) White = true;
                if (board[i, j] == SquareController.PieceType.BlackKing) Black = true;
            }
        }

        if (!White && Black) return ResultType.BlackWin;
        if (White && !Black) return ResultType.WhiteWin;

        else return ResultType.None;
    }


    public void ShowPanel(string text, Action OnClickConfirmButton, Action OnClickRetryButton)
    {
        GameObject panel = Instantiate(PanelPrefab, canvas.transform);
        PanelController panelController = panel.GetComponent<PanelController>();
        panelController.ShowPanel(text);
        panelController.OnClickConfirmButton = OnClickConfirmButton;
        panelController.OnClickRetryButton = OnClickRetryButton;
    }
}