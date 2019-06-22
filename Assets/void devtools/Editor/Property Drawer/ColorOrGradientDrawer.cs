using UnityEngine;
using UnityEditor;

namespace Void.devtools.Editor
{
    [CustomPropertyDrawer(typeof(ColorOrGradient))]
    public class ColorOrGradientDrawer : PropertyDrawer
    {
        private const float DROPDOWN_WIDTH = 30;
        private const float MARGIN = 5;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var modeProp = property.FindPropertyRelative("mode");
            var mainProp = property.FindPropertyRelative(modeProp.intValue == (int)ColorGradientMode.Color ? "color" : "gradient");
            
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width - DROPDOWN_WIDTH - MARGIN, position.height), mainProp, new GUIContent(property.displayName));
            EditorGUI.PropertyField(new Rect(position.xMax - DROPDOWN_WIDTH, position.y, DROPDOWN_WIDTH, position.height), modeProp, new GUIContent());
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}