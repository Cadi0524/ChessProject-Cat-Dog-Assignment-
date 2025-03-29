
    using System.Collections.Generic;
    using UnityEngine;

    public static class PieceMoveCalculator
    {



        public static  List<(int, int)> moveCalculator(int currentRow, int currentCol,SquareController.PieceType[,] board, SquareController.PieceType pieceType)
        {
            List<(int, int)> result = new List<(int, int)>();
            switch (pieceType)
            {
                case SquareController.PieceType.BlackKnight: 
                case SquareController.PieceType.WhiteKnight:
                      result = KnightMove(result, currentRow, currentCol);
                    break;
            }
            
            return result;
        }


//TODO: 각각의 움직일 수 있는 것을 리턴하는 함수들, 특이사항은 폰은 색마다 다르고, 막히면 못 가는 것도 구현해야 함. 
        public static  List<(int, int)> KnightMove(List<(int,int)> result, int currentRow,int currentCol)
        {
            result.Add((currentRow + 2, currentCol + 1));
            result.Add((currentRow + 2, currentCol - 1));   
            result.Add((currentRow - 2, currentCol + 1));
            result.Add((currentRow - 2, currentCol - 1));
            result.Add((currentRow + 1, currentCol -2));
            result.Add((currentRow + 1, currentCol +2));
            result.Add((currentRow - 1, currentCol +2));
            result.Add((currentRow - 1, currentCol -2));
            
            result = outBoard(result);
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
    }
