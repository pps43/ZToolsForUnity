using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ZTools.EditorUtil.CustomAttribute
{

    [CustomPropertyDrawer(typeof(ShowLayerAttribute))]
    public class LayerPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int selected = -1;
            //show dropdown menu
            List<string> layerNames = new List<string>();
            for (int i = 8; i <= 31; i++) // custom layer always starts from 8
            {
                string layerName = LayerMask.LayerToName(i);
                if (layerName.Length > 0)
                {
                    layerNames.Add(layerName);
                }
            }

            selected = property.intValue - 8;

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            selected = EditorGUI.Popup(position, selected, layerNames.ToArray());
            EditorGUI.EndProperty();

            if (selected >= 0)
            {
                property.intValue = selected + 8;
            }
        }
    }
}