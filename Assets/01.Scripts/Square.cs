
using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class Square : MonoBehaviour
{
   private Image image;
   private Color32 noneColor = new Color32(0, 0, 0, 0);
   private Color32 existColor = new Color32(255, 255, 255, 255);
   public (int row, int col) position;
   public SquareController.PieceType currentState;
   public Button button;
   public Action<int,int> OnButtonClick;

   public void Awake()
   {
      image = GetComponent<Image>();
      button = GetComponent<Button>();
      button.onClick.AddListener(() => OnButtonClick?.Invoke(position.row, position.col));
   }

   
   //TODO: 하이라이트 버전 따로 만들기 // 적 /우리 구분해야함. 
   public void SetMarker([CanBeNull] Sprite pieceSprite, SquareController.PieceType pieceType)
   {
      if (pieceSprite != null)
      {
         image.sprite = pieceSprite;
         image.color = existColor;
         currentState = pieceType;
        
      }
      else
      {
         image.color = noneColor;
         currentState = SquareController.PieceType.None;
      }
   }
   
   
   
}
