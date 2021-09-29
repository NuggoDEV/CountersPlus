using Heck.SettingsSetter;
using System;
using System.Reflection;
using UnityEngine;

namespace CountersPlus.ConfigModels.SettableSettings
{
    public class CountersPlusWrapperSetting : ISettableSetting
    {
        private readonly PropertyInfo settingsProperty;
        private readonly object settingsInstance;

        private object originalValue;

        public CountersPlusWrapperSetting(string groupName, string fieldName,
            PropertyInfo settingsProperty, object settingsInstance)
        {
            GroupName = groupName;
            FieldName = fieldName;

            this.settingsProperty = settingsProperty;
            this.settingsInstance = settingsInstance;
        }

        public string GroupName { get; }

        public string FieldName { get; }

        public object TrueValue => settingsProperty.GetValue(settingsInstance);

        public void SetTemporary(object tempValue)
        {
            if (tempValue != null)
            {
                originalValue = settingsProperty.GetValue(settingsInstance);

                if (settingsProperty.PropertyType.IsEnum)
                {
                    tempValue = Enum.Parse(settingsProperty.PropertyType, tempValue.ToString());
                }
                else if (settingsProperty.PropertyType == typeof(Color))
                {
                    ColorUtility.TryParseHtmlString(tempValue.ToString(), out var tempColorValue);
                    tempValue = tempColorValue;
                }
                else if (settingsProperty.PropertyType == typeof(int))
                {
                    tempValue = Convert.ToInt32(tempValue);
                }

                settingsProperty.SetValue(settingsInstance, tempValue);
            }
            else
            {
                settingsProperty.SetValue(settingsInstance, originalValue);
                originalValue = null;
            }
        }
    }
}
