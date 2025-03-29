using UnityEngine;

public class GameManager : Singleton<GameManager>
{
  private SquareController squareController;


  public void Start()
  {
    squareController = FindObjectOfType<SquareController>();
    InitChessPieces();
    //squareController.SetMarker(5,5,SquareController.PieceType.CanMove);
    
  }

  void InitChessPieces()
  {
    // 검은색 기물 배치
    squareController.SetMarker(0, 0, SquareController.PieceType.BlackRook);
    squareController.SetMarker(0, 1, SquareController.PieceType.BlackKnight);
    squareController.SetMarker(0, 2, SquareController.PieceType.BlackBishop);
    squareController.SetMarker(0, 3, SquareController.PieceType.BlackQueen);
    squareController.SetMarker(0, 4, SquareController.PieceType.BlackKing);
    squareController.SetMarker(0, 5, SquareController.PieceType.BlackBishop);
    squareController.SetMarker(0, 6, SquareController.PieceType.BlackKnight);
    squareController.SetMarker(0, 7, SquareController.PieceType.BlackRook);
    for (int i = 0; i < 8; i++)
    {
      squareController.SetMarker(1, i, SquareController.PieceType.BlackPawn);
    }

    // 흰색 기물 배치
    for (int i = 0; i < 8; i++)
    {
      squareController.SetMarker(6, i, SquareController.PieceType.WhitePawn);
    }
    squareController.SetMarker(7, 0, SquareController.PieceType.WhiteRook);
    squareController.SetMarker(7, 1, SquareController.PieceType.WhiteKnight);
    squareController.SetMarker(7, 2, SquareController.PieceType.WhiteBishop);
    squareController.SetMarker(7, 3, SquareController.PieceType.WhiteQueen);
    squareController.SetMarker(7, 4, SquareController.PieceType.WhiteKing);
    squareController.SetMarker(7, 5, SquareController.PieceType.WhiteBishop);
    squareController.SetMarker(7, 6, SquareController.PieceType.WhiteKnight);
    squareController.SetMarker(7, 7, SquareController.PieceType.WhiteRook);

    // 빈 칸 초기화 
    for (int row = 2; row < 6; row++)
    {
      for (int col = 0; col < 8; col++)
      {
         //이미 마커가 설정되어 있지 않다면 빈 칸으로 설정
         squareController.SetMarker(row, col, SquareController.PieceType.None);
      }
    }
  }
}
