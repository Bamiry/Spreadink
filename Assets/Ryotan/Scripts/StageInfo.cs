using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StageInfo
{
    [SerializeField] private int _id;
    public int Id => _id;

    [SerializeField] private Image _image;
    public Image Image => _image;

    [SerializeField] private List<ColorType> _odaiColors;
    [SerializeField] private List<float> _odaiRatios;

    /// <summary>
    /// ColorType -> Color の変換は InkColorUtil.GetColor を使用すること
    /// </summary>
    public Dictionary<ColorType, float> Odai => _odaiColors
        .Zip(_odaiRatios, (color, ratio) => (color, ratio))
        .ToDictionary(x => x.color, x => x.ratio);

    public List<Color> AvailableColors => _odaiColors.Select(InkColorUtil.GetColor).ToList();

    [SerializeField] private float _spreadSpeedRate;
    public float SpreadSpeedRate => _spreadSpeedRate;
}