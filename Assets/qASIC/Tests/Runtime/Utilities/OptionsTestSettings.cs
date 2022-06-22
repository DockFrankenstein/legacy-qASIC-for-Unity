using qASIC.Options;
using UnityEngine;

namespace qASIC.Tests.Runtime.Utilities
{
    internal static class OptionsTestSettings
    {
        public const string IntSettingName = "test_intsetting";
        public const string FloatSettingName = "test_floatsetting";
        public const string BoolSettingName = "test_boolsetting";
        public const string DoubleSettingName = "test_doublesetting";
        public const string DisabledSettingName = "test_disabledsetting";


        [OptionsSetting(IntSettingName, enableMethodName = nameof(IsEnabled))]
        private static void IntSetting(int value) =>
            LogValue(value);

        [OptionsSetting(FloatSettingName, enableMethodName = nameof(IsEnabled))]
        private static void FloatSetting(float value) =>
            LogValue(value);

        [OptionsSetting(BoolSettingName, enableMethodName = nameof(IsEnabled))]
        private static void BoolSetting(bool value) =>
            LogValue(value);

        [OptionsSetting(DoubleSettingName, enableMethodName = nameof(IsEnabled))]
        private static void DoubleSetting(double value) =>
            LogValue(value);

        [OptionsSetting(DisabledSettingName, enableMethodName = nameof(IsEnabled))]
        private static void DisabledSetting(double value) =>
            LogValue(value);

        static void LogValue(object value) =>
            Debug.Log($"Setting's value changed to {value}");

        static bool IsEnabled() =>
            false;
    }
}