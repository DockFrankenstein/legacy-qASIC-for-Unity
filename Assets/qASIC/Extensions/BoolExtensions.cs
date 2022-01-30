using UnityEngine;

namespace qASIC
{
    public static class BoolExtensions
    {
        public static string ToStringFormated(this bool? value) =>
            ToStringFormated(value, Color.green, Color.red);

        public static string ToStringFormated(this bool? value, Color trueColor, Color falseColor) =>
            ToStringFormated(value, $"<color=#{ColorUtility.ToHtmlStringRGB(trueColor)}>True</color>", $"<color=#{ColorUtility.ToHtmlStringRGB(falseColor)}>False</color>");

        public static string ToStringFormated(this bool? value, string trueText, string falseText)
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

        public static string ToStringFormated(this bool value) =>
            ToStringFormated(value as bool?);

        public static string ToStringFormated(this bool value, Color trueColor, Color falseColor) =>
            ToStringFormated(value as bool?, trueColor, falseColor);

        public static string ToStringFormated(this bool value, string trueText, string falseText) =>
            ToStringFormated(value as bool?, trueText, falseText);
    }
}