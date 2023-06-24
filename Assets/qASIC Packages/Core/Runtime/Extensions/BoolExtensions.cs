using UnityEngine;

namespace qASIC
{
    public static class BoolExtensions
    {
        public static string ToStringFormatted(this bool? value) =>
            ToStringFormatted(value, Color.green, Color.red);

        public static string ToStringFormatted(this bool? value, Color trueColor, Color falseColor) =>
            ToStringFormatted(value, $"<color=#{ColorUtility.ToHtmlStringRGB(trueColor)}>True</color>", $"<color=#{ColorUtility.ToHtmlStringRGB(falseColor)}>False</color>");

        public static string ToStringFormatted(this bool? value, string trueText, string falseText)
        {
            switch (value)
            {
                case true:
                    return trueText;
                case false:
                    return falseText;
                default:
                    return "NULL";
            }
        }

        public static string ToStringFormatted(this bool value) =>
            ToStringFormatted(value as bool?);

        public static string ToStringFormatted(this bool value, Color trueColor, Color falseColor) =>
            ToStringFormatted(value as bool?, trueColor, falseColor);

        public static string ToStringFormatted(this bool value, string trueText, string falseText) =>
            ToStringFormatted(value as bool?, trueText, falseText);
    }
}