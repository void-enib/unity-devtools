using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Void.devtools
{
    [Serializable]
    public struct ColorOrGradient
    {
        public ColorGradientMode mode;
        public Color color;
        public Gradient gradient;

        public ColorOrGradient(Color color, Gradient gradient, ColorGradientMode mode = ColorGradientMode.Color)
        {
            this.mode = mode;
            this.color = color;
            this.gradient = gradient;
        }

        public ColorOrGradient(Color color)
            : this(color, new Gradient(), ColorGradientMode.Color)
        { }

        public ColorOrGradient(Gradient gradient)
            : this(new Color(), gradient, ColorGradientMode.Gradient)
        { }

        public Color Evaluate(float time)
        {
            switch (mode)
            {
                default:
                case ColorGradientMode.Color:
                    return color;
                case ColorGradientMode.Gradient:
                    return gradient?.Evaluate(time) ?? new Color();
            }
        }

        public static implicit operator ColorOrGradient(Color color) => new ColorOrGradient(color);
        public static implicit operator ColorOrGradient(Gradient gradient) => new ColorOrGradient(gradient);
    }

    public enum ColorGradientMode
    {
        Color,
        Gradient
    }
}