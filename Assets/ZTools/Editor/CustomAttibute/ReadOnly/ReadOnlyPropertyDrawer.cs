using UnityEngine;
using UnityEditor;
namespace ZTools.EditorUtil.CustomAttribute
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyPropertyDrawer : PropertyDrawer
    {
        const float _InfoHeightScaler = 1.5f;

        private float _PropertyHeight = 0;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var roattrib = attribute as ReadOnlyAttribute;

            _PropertyHeight = EditorGUI.GetPropertyHeight(property, label, true);

            if (!string.IsNullOrEmpty(roattrib.Info))
            {
                return _PropertyHeight * (1.0f + _InfoHeightScaler);
            }

            return _PropertyHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var roattrib = attribute as ReadOnlyAttribute;

            if (!string.IsNullOrEmpty(roattrib.Info))
            {
                position.height = _PropertyHeight * _InfoHeightScaler;

                EditorGUI.HelpBox(position, roattrib.Info, MessageType.Info);

                position.y += _PropertyHeight * _InfoHeightScaler;
            }

            GUI.enabled = false;

            EditorGUI.PropertyField(position, property, label, true);

            GUI.enabled = true;
        }
    }
}