using CountersPlus.Counters;
using CountersPlus.Custom.Attributes;
using TMPro;
using UnityEngine;
using System;
using System.Reflection;

namespace CountersPlus.Custom
{
    public class CustomCounterTemplate : Counter<CustomConfigModel>
    {
        private PropertyInfo displayName;
        private PropertyInfo formattedText;
        private FieldInfo refreshCounter;

        private MethodInfo counterUpdate;
        private MethodInfo counterStart;
        private MethodInfo counterDestroy;

        private object host;

        private Action counterRefreshedEvent;

        private TMP_Text label;
        private TMP_Text counter;

        internal override void Counter_Start()
        {
            counterRefreshedEvent += RefreshCounter;
            host = settings.CustomCounter.TemplateCounter;
            Type templateType = settings.CustomCounter.TemplateCounter.GetType();
            foreach (PropertyInfo propertyInfo in templateType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                DisplayNameAttribute display = propertyInfo.GetCustomAttribute(typeof(DisplayNameAttribute), true) as DisplayNameAttribute;
                FormattedTextAttribute text = propertyInfo.GetCustomAttribute(typeof(FormattedTextAttribute), true) as FormattedTextAttribute;
                if (display != null) displayName = propertyInfo;
                if (text != null) formattedText = propertyInfo;
            }
            foreach (FieldInfo fieldInfo in templateType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                RefreshCounterAttribute refresh = fieldInfo.GetCustomAttribute(typeof(RefreshCounterAttribute), true) as RefreshCounterAttribute;
                if (refresh != null)
                {
                    if (fieldInfo.FieldType != typeof(Action))
                    {
                        Plugin.Log($"Custom Counter {settings.CustomCounter.Name} provides a RefreshCounterAttribute, however it is not an Action.",
                            LogInfo.Fatal, "Contact the creator of this Custom Counter, rather than to Counters+ itself");
                        Destroy(this);
                        return;
                    }
                    refreshCounter = fieldInfo;
                    fieldInfo.SetValue(host, counterRefreshedEvent);
                }
            }
            foreach (MethodInfo methodInfo in templateType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                CounterUpdateAttribute update = methodInfo.GetCustomAttribute(typeof(CounterUpdateAttribute), true) as CounterUpdateAttribute;
                CounterStartAttribute start = methodInfo.GetCustomAttribute(typeof(CounterStartAttribute), true) as CounterStartAttribute;
                CounterDestroyAttribute destroy = methodInfo.GetCustomAttribute(typeof(CounterDestroyAttribute), true) as CounterDestroyAttribute;
                if (update != null) counterUpdate = methodInfo;
                if (start != null) counterStart = methodInfo;
                if (destroy != null) counterDestroy = methodInfo;
            }
            counterStart?.Invoke(host, new object[] { });
        }

        internal override void Counter_Destroy()
        {
            counterRefreshedEvent -= RefreshCounter;
            counterDestroy?.Invoke(host, new object[] { });
        }

        internal override void Init(CountersData data, Vector3 position)
        {
            TextHelper.CreateText(out counter, position - new Vector3(0, 0.4f, 0));
            counter.text = "Data";
            counter.fontSize = 4;
            counter.color = Color.white;
            counter.alignment = TextAlignmentOptions.Center;

            TextHelper.CreateText(out label, position);
            label.text = $"{settings.CustomCounter.Name}";
            label.fontSize = 3;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.Center;

            RefreshCounter();
        }

        private void Update()
        {
            counterUpdate?.Invoke(host, new object[] { });
        }

        public void RefreshCounter()
        {
            label.text = displayName?.GetGetMethod(false)?.Invoke(host, new object[] { })?.ToString();
            counter.text = formattedText?.GetGetMethod(false)?.Invoke(host, new object[] { })?.ToString();
        }
    }
}
