using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PalatteInk : MonoBehaviour, IPointerClickHandler
{
    #region 参照
    [SerializeField] private GameObject frame;
    [SerializeField] private Image colorImage;
    #endregion

    #region コールバック
    public event System.Action<PalatteInk> OnTouch;
    #endregion

    #region プロパティ
    public Color Color => colorImage.color;
    public bool IsUsed => colorImage.color == Color.grey;
    #endregion

    #region 状態変数
    private bool isSelected = false;
    #endregion

    public void Set(Color color)
    {
        colorImage.color = color;
        frame.SetActive(false);
    }

    public void TurnSelected()
    {
        isSelected = !isSelected;
        frame.SetActive(isSelected);
    }

    public void OnUse()
    {
        frame.SetActive(false);
        colorImage.color = Color.grey;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TurnSelected();
        OnTouch?.Invoke(this);
    }
}
