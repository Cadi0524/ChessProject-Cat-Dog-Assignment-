using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class SquareController : MonoBehaviour
{
   [SerializeField] private Sprite[] sprites;
   public Square[,] squares = new Square[8, 8];
   [SerializeField] GameObject squarePrefab;

   private Dictionary<PieceType, Sprite> pieceSprites;
   
   public Action<int,int> OnButtonClicked;


   public enum PieceType
   {
      WhiteKing,
      WhiteQueen,
      WhiteRook,
      WhiteBishop,
      WhiteKnight,
      WhitePawn,
      BlackKing,
      BlackQueen,
      BlackRook,
      BlackBishop,
      BlackKnight,
      BlackPawn,
      CanMove,
      None,
   }

   public void Awake()
   {
      // Dictionary에 Sprite 넣기. ( 체스 기물 별 ) 
      pieceSprites = new Dictionary<PieceType, Sprite>
      {
         { PieceType.WhiteKing, sprites[0] },
         { PieceType.WhiteQueen, sprites[1] },
         { PieceType.WhiteRook, sprites[2] },
         { PieceType.WhiteBishop, sprites[3] },
         { PieceType.WhiteKnight, sprites[4] },
         { PieceType.WhitePawn, sprites[5] },
         { PieceType.BlackKing, sprites[6] },
         { PieceType.BlackQueen, sprites[7] },
         { PieceType.BlackRook, sprites[8] },
         { PieceType.BlackBishop, sprites[9] },
         { PieceType.BlackKnight, sprites[10] },
         { PieceType.BlackPawn, sprites[11] },
         {PieceType.CanMove, sprites[12]},
      };
      // 보드 생성
      InitBoard();
      
   }
/// <summary>
/// 보드 생성 함수, 새롭게 8X8 보드를 만듬, 새로 시작할 때는 다 삭제해 주어야 함
/// </summary>
   public void InitBoard()
   {
      for (int i = 0; i < squares.GetLength(0); i++)
      {
         for (int j = 0; j < squares.GetLength(1); j++)
         {
            Square temp = Instantiate(squarePrefab, this.transform).GetComponent<Square>();
            squares[i, j] = temp;
            temp.OnButtonClick += HandleSquareClicked;
            temp.position = (i, j);
         }
      }
   }

   public void HandleSquareClicked(int row, int col)
   {
      OnButtonClicked?.Invoke(row, col);
   }
 

/// <summary>
/// 들어온 row, col, PieceType을 사용해 그 칸의 마커를 설정 / 현재 상태까지도 설정
/// </summary>
/// <param name="row"></param>
/// <param name="col"></param>
/// <param name="piece"></param>
   public void SetMarker(int row, int col, PieceType piece)
   {
     Square target = squares[row, col];
     if (piece != PieceType.None)
     {
        target.SetMarker(pieceSprites[piece], piece);
     }
     else
     {
        target.SetMarker(null, piece);
     }
   }
   
}
