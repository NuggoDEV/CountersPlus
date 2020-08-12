using IPA.Config.Data;
using IPA.Config.Stores;
using System;
using UnityEngine;

namespace CountersPlus.ConfigModels.Converters
{
    /// <summary>
    /// A converter for any <see cref="Color"/> that converts to it's hex code representation and back.
    /// </summary>
    public sealed class ColorConverter : ValueConverter<Color>
    {
        public override Color FromValue(Value value, object parent)
        {
            if (value is Text t)
            {
                if (ColorUtility.TryParseHtmlString(t.Value, out Color color))
                {
                    return color;
                }
                else throw new ArgumentException("Value cannot be parsed into a Color.", nameof(value));
            }
            else throw new ArgumentException("Value not a string", nameof(value));
        }

        public override Value ToValue(Color obj, object parent) => Value.Text($"#{ColorUtility.ToHtmlStringRGB(obj)}");
    }
}
