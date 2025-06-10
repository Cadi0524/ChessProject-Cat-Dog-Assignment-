using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
 [SerializeField] private TMP_Text text;
 [SerializeField] private Button confirmButton;
 [SerializeField] private Button retryButton;
 
 public Action OnClickConfirmButton;
 public Action OnClickRetryButton;


 public void ShowPanel(string input)
 {
  text.text = input;
 }

 public void OnClickConfirm()
 {
OnClickConfirmButton?.Invoke();
Destroy(this.gameObject);
 }

 public void OnClickRetry()
 {
OnClickRetryButton?.Invoke();
  Destroy(this.gameObject);
 }
 
 
 
 
}
