using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Square : MonoBehaviour
{
   private Image image;

   public void Start()
   {
      image = GetComponent<Image>();
   }
   
   public void SetMarker(Image PieceImage)
   {
      image.sprite = PieceImage.sprite;
   }
   
   
}
