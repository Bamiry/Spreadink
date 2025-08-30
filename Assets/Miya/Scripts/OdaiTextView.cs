using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OdaiTextView : MonoBehaviour
{
    [SerializeField] private Image colorImage;
    [SerializeField] private TextMeshProUGUI ratioLabel;

    public void Set(Color color, float ratio)
    {
        colorImage.color = color;
        ratioLabel.text = $"{ratio.ToString("F0")}%";
    }
}
