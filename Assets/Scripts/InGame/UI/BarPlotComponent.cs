using TMPro;
using UnityEngine;

public enum TextPositionType
{
    Top,
    Bottom
}

public class BarPlotComponent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _percentageText;
    public TextMeshProUGUI PercentageText => _percentageText;

    private TextPositionType _textPosition = TextPositionType.Top;

    public TextPositionType TextPosition
    {
        get => _textPosition;
        set
        {
            _textPosition = value;
            switch (value)
            {
                case TextPositionType.Top:
                    _percentageText.rectTransform.pivot = new Vector2(0.5f, 0);
                    _percentageText.rectTransform.anchorMin = new Vector2(0.5f, 1);
                    _percentageText.rectTransform.anchorMax = new Vector2(0.5f, 1);
                    _percentageText.rectTransform.anchoredPosition = new Vector2(0, 10);
                    break;
                case TextPositionType.Bottom:
                    _percentageText.rectTransform.pivot = new Vector2(0.5f, 1);
                    _percentageText.rectTransform.anchorMin = new Vector2(0.5f, 0);
                    _percentageText.rectTransform.anchorMax = new Vector2(0.5f, 0);
                    _percentageText.rectTransform.anchoredPosition = new Vector2(0, -10);
                    break;
                default:
                    break;
            }
        }
    }
}