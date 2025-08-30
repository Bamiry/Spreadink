using UnityEngine;

public static class InkColorUtil
{
    public static Color GetColor(ColorType colorType)
    {
        return colorType switch
        {
            ColorType.Red => Red,
            ColorType.Green => Green,
            ColorType.Blue => Blue,
            ColorType.Yellow => Yellow,
            ColorType.Orange => Orange,
            ColorType.Purple => Purple,
            ColorType.SkyBlue => SkyBlue,
            ColorType.Pink => Pink,
            _ => Color.white
        };
    }

    public static readonly Color Red = new Color32(255, 0, 0, 255);
    public static readonly Color Green = new Color32(0, 255, 0, 255);
    public static readonly Color Blue = new Color32(0, 0, 255, 255);

    public static readonly Color Yellow = ColorUtility.TryParseHtmlString("#F4DE36", out var yellow)
        ? yellow
        : new Color32(244, 222, 54, 255);

    public static readonly Color Orange = ColorUtility.TryParseHtmlString("#FFA500", out var orange)
        ? orange
        : new Color32(255, 165, 0, 255);

    public static readonly Color Purple = ColorUtility.TryParseHtmlString("A100FF", out var purple)
        ? purple
        : new Color32(161, 0, 255, 255);

    public static readonly Color SkyBlue = new Color32(0, 255, 255, 255);

    public static readonly Color Pink = ColorUtility.TryParseHtmlString("#FF00DD", out var pink)
        ? pink
        : new Color32(255, 0, 221, 255);
}