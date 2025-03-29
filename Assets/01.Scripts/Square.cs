using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Square : MonoBehaviour
{
   private Image image;
   private Color32 noneColor = new Color32(0, 0, 0, 0);
   private Color32 existColor = new Color32(255, 255, 255, 255);
   

   public void Awake()
   {
      image = GetComponent<Image>();
   }

   public void SetMarker([CanBeNull] Sprite pieceSprite)
   {
      if (pieceSprite != null)
      {
         image.sprite = pieceSprite;
         image.color = existColor;
        
      }
      else
      {
         image.color = noneColor;
      }
   }
}
