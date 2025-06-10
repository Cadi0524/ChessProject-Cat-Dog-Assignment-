
using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Square : MonoBehaviour
{
    [SerializeField] Image pieceImage;
   [SerializeField] Image hilightImage;
   private Color32 noneColor = new Color32(0, 0, 0, 0);
   private Color32 existColor = new Color32(255, 255, 255, 255);
   public (int row, int col) position;
   public SquareController.PieceType currentState = SquareController.PieceType.None;
   public Button button;
   public Action<int,int,bool> OnButtonClick;

   public bool isHilighted = false;
   
   [SerializeField] private Sprite highlightSprite;

   public void Awake()
   {
      button = GetComponent<Button>();
      button.onClick.AddListener(() => OnButtonClick?.Invoke(position.row, position.col,isHilighted));
   }

   
   //TODO: 하이라이트 버전 따로 만들기 // 적 /우리 구분해야함. 
   public void SetMarker([CanBeNull] Sprite pieceSprite, SquareController.PieceType pieceType)
   {
      if (pieceSprite != null)
      {
         pieceImage.sprite = pieceSprite;
         pieceImage.color = existColor;
         currentState = pieceType;
        
      }
      else
      {
         pieceImage.sprite = null;
         pieceImage.color = noneColor;
         currentState = SquareController.PieceType.None;
      }
   }

   public void isHighlighted()
   {
      if (isHilighted)
      {
         hilightImage.color = existColor;
         hilightImage.sprite = highlightSprite;
      }
      else
      {
         hilightImage.color = noneColor;
         hilightImage.sprite = null;
      }

   }
 
   
   
   
}
