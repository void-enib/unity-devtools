using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Void.devtools.UI
{
    public class ProgressBar : MaskableGraphic
    {
        private static readonly ColorOrGradient DEFAULT_FOREGROUND = new ColorOrGradient(
            Color.green,
            new Gradient()
            {
                colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(Color.red, 0),
                    new GradientColorKey(Color.green, 1)
                }
            }
        );

        [Header("Appearance")]
        [SerializeField] private bool m_useMaxValueForWidth = false;
        [SerializeField] private float m_widthPerPoint = 1;
        [SerializeField] private Color m_backgroundColor = Color.black;
        [SerializeField] private ColorOrGradient m_foregroundColor = DEFAULT_FOREGROUND;

        [Header("Value")]
        [SerializeField] private float m_value = 70;
        [SerializeField] private float m_maxValue = 100;

        #region Properties

        public bool UseMaxValueForWidth {
            get => m_useMaxValueForWidth;
            set => SetProperty(ref m_useMaxValueForWidth, value);
        }

        public float WidthPerPoint {
            get => m_widthPerPoint;
            set => SetProperty(ref m_widthPerPoint, value);
        }

        public Color BackgroundColor {
            get => m_backgroundColor;
            set => SetProperty(ref m_backgroundColor, value);
        }

        public ColorOrGradient ForegroundColor {
            get => m_foregroundColor;
            set => SetProperty(ref m_foregroundColor, value);
        }

        public float Value {
            get => m_value;
            set => SetProperty(ref m_value, value);
        }

        public float MaxValue {
            get => m_maxValue;
            set => SetProperty(ref m_maxValue, value);
        }
        #endregion

        protected override void OnValidate()
        {
            m_widthPerPoint = Mathf.Max(m_widthPerPoint, 0);
            m_maxValue = Mathf.Max(m_maxValue, 0);
            m_value = Mathf.Clamp(m_value, 0, m_maxValue);
            SetVerticesDirty();
        }

        /// <summary>
        /// Set all setting values then redraw.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxValue"></param>
        /// <param name="foregroundColor"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="useMaxValueForWidth"></param>
        /// <param name="widthPerPoint"></param>
        public void Setup(float value, float maxValue, ColorOrGradient foregroundColor, Color backgroundColor, bool useMaxValueForWidth = false, float widthPerPoint = 1)
        {
            m_value = value;
            m_maxValue = maxValue;
            m_foregroundColor = foregroundColor;
            m_backgroundColor = backgroundColor;
            m_useMaxValueForWidth = useMaxValueForWidth;
            m_widthPerPoint = widthPerPoint;

            OnValidate();
        }

        /// <summary>
        /// Set all setting values then redraw. (foregroundColor = <see cref="DEFAULT_FOREGROUND"/>, backgroundColor = <see cref="Color.black"/>, useMaxValueForWidth = <see cref="false"/>, float widthPerPoint = <see cref="1"/>)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxValue"></param>
        public void Setup(float value, float maxValue) => Setup(value, maxValue, DEFAULT_FOREGROUND, Color.black);

        /// <summary>
        /// Set all setting values then redraw. (backgroundColor = <see cref="Color.black"/>, useMaxValueForWidth = <see cref="false"/>, float widthPerPoint = <see cref="1"/>)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxValue"></param>
        /// <param name="foregroundColor"></param>
        public void Setup(float value, float maxValue, ColorOrGradient foregroundColor) => Setup(value, maxValue, foregroundColor, Color.black);

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            var rt = GetComponent<RectTransform>();
            Vector2 origin = new Vector2(-rt.pivot.x * rt.sizeDelta.x, rt.sizeDelta.y * (1 - rt.pivot.y));
            float refWidth = m_useMaxValueForWidth ? m_widthPerPoint * m_maxValue : rt.sizeDelta.x;
            float normalizedValue = m_value / m_maxValue;

            // x and y are normalized
            void AddVertex(float x, float y, Color color)
            {
                x = Mathf.Clamp01(x) * refWidth;
                y = Mathf.Clamp01(y) * -rt.sizeDelta.y;
                var vert = UIVertex.simpleVert;
                vert.color = color;
                vert.position = origin + new Vector2(x, y);
                vh.AddVert(vert);
            }

            void DrawRect(float from, float to, Color color)
            {
                int vertexId = vh.currentVertCount;

                AddVertex(from, 0, color);
                AddVertex(from, 1, color);
                AddVertex(to, 1, color);
                AddVertex(to, 0, color);
                vh.AddTriangle(vertexId, vertexId + 1, vertexId + 2);
                vh.AddTriangle(vertexId + 2, vertexId + 3, vertexId);
            }

            /* Draw Background */
            DrawRect(0, 1, m_backgroundColor);

            /* Draw Foreground */
            switch (m_foregroundColor.mode)
            {
                case ColorGradientMode.Gradient:
                    switch (m_foregroundColor.gradient.mode)
                    {
                        case GradientMode.Blend:
                            {
                                void AddForegroundVertices(float pos, Color color)
                                {
                                    color = new Color(color.r, color.g, color.b); // ignore alpha component (bad rendering)
                                    AddVertex(pos, 0, color);
                                    AddVertex(pos, 1, color);
                                }

                                AddForegroundVertices(0, m_foregroundColor.Evaluate(0));
                                foreach (var key in m_foregroundColor.gradient.colorKeys.Where(k => k.time < normalizedValue))
                                    AddForegroundVertices(key.time, key.color);
                                AddForegroundVertices(normalizedValue, m_foregroundColor.Evaluate(normalizedValue));

                                for (int i = 4, max = vh.currentVertCount - 2; i < max; i += 2)
                                {
                                    vh.AddTriangle(i, i + 1, i + 2);
                                    vh.AddTriangle(i + 1, i + 3, i + 2);
                                }
                            }
                            break;

                        case GradientMode.Fixed:
                            {
                                float from = 0;
                                var lastKey = m_foregroundColor.gradient.colorKeys.Last();
                                foreach (var key in m_foregroundColor.gradient.colorKeys)
                                {
                                    float time = key.time == lastKey.time ? 1 : key.time;

                                    if (normalizedValue <= time)
                                    {
                                        DrawRect(from, normalizedValue, key.color);
                                        break;
                                    }
                                    DrawRect(from, time, key.color);
                                    from = time;
                                }
                            }
                            break;
                    }
                    break;
                case ColorGradientMode.Color:
                    {
                        DrawRect(0, normalizedValue, m_foregroundColor.color);
                    }
                    break;
            }
        }

        private void SetProperty<T>(ref T currentValue, T newValue)
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return;
            currentValue = newValue;
            OnValidate();
        }
    }
}