using UnityEngine;
using qASIC.Displayer;
using qASIC.Displayer.Internal;
using qASIC.ProjectSettings;

using static qASIC.Internal.qLogger;

namespace qASIC
{
    public static partial class qDebug
    {
        public static void DisplayValue(string tag, object value)
        {
            DisplayerProjectSettings settings = DisplayerProjectSettings.Instance;
            if (settings.CreateDebugDisplayer && !InfoDisplayer.DisplayerExists(settings.debugDisplayerName))
            {
                qASICObjectCreator.CreateDebugDisplyer();
                if (settings.displayDebugGenerationMessage)
                    OnLogColorTag?.Invoke(settings.debugGenerationMessage, settings.debugGenerationMessageColor);
            }

            InfoDisplayer.DisplayValue(tag, value?.ToString() ?? "null", settings.debugDisplayerName);
        }

        public static void ToggleDisplayValue(string tag, bool show)
        {
            DisplayerProjectSettings settings = DisplayerProjectSettings.Instance;
            if (settings.CreateDebugDisplayer && !InfoDisplayer.DisplayerExists(settings.debugDisplayerName))
            {
                qASICObjectCreator.CreateDebugDisplyer();
                if (settings.displayDebugGenerationMessage)
                    OnLogColorTag?.Invoke(settings.debugGenerationMessage, settings.debugGenerationMessageColor);
            }

            InfoDisplayer.ToggleValue(tag, show, settings.debugDisplayerName);
        }
    }
}