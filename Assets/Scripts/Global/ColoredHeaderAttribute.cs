using UnityEngine;

public class ColoredHeaderAttribute : PropertyAttribute
{
    public string Header { get; private set; }
    public Color Color { get; private set; }

    public ColoredHeaderAttribute(string header, string hexColor)
    {
        Header = header;
        ColorUtility.TryParseHtmlString(hexColor, out Color color);
        Color = color;
    }
}