using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

//适用于 [ShowSortingLayer] public string sortingLayer
[CustomPropertyDrawer(typeof(ShowSortingLayerAttribute))]
public class SortingLayerPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int selected = -1;

        var names = new string[SortingLayer.layers.Length];
        for (int i = 0; i < SortingLayer.layers.Length; i++)
        {
            names[i] = SortingLayer.layers[i].name;
            if (property.stringValue == names[i])
                selected = i;
        }

        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        selected = EditorGUI.Popup(position, selected, names);
        EditorGUI.EndProperty();

        if (selected >= 0)
            property.stringValue = names[selected];
    }

    //适用于 int sortingLayer
    //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //{
    //    if (property.propertyType != SerializedPropertyType.Integer)
    //    {
    //        Debug.LogError("SortedLayer property should be an integer ( the layer id )");
    //    }
    //    else
    //    {
    //        SortingLayerField(new GUIContent("Sorting Layer"), property, EditorStyles.popup, EditorStyles.label);
    //    }
    //}

    //public static void SortingLayerField(GUIContent label, SerializedProperty layerID, GUIStyle style, GUIStyle labelStyle)
    //{
    //    MethodInfo methodInfo = typeof(EditorGUILayout).GetMethod("SortingLayerField", BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(GUIContent), typeof(SerializedProperty), typeof(GUIStyle), typeof(GUIStyle) }, null);

    //    if (methodInfo != null)
    //    {
    //        object[] parameters = new object[] { label, layerID, style, labelStyle };
    //        methodInfo.Invoke(null, parameters);
    //    }
    //}


    //适用于 int sortingLayer
    ///**
    // * Is called to draw a property.
    // *
    // * @param position Rectangle on the screen to use for the property GUI.
    // * @param property The SerializedProperty to make the custom GUI for.
    // * @param label The label of the property.
    // */
    //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //{
    //    if (property.propertyType != SerializedPropertyType.Integer)
    //    {
    //        // Integer is expected. Everything else is ignored.
    //        return;
    //    }
    //    EditorGUI.LabelField(position, label);

    //    position.x += EditorGUIUtility.labelWidth;
    //    position.width -= EditorGUIUtility.labelWidth;

    //    string[] sortingLayerNames = GetSortingLayerNames();
    //    int[] sortingLayerIDs = GetSortingLayerIDs();

    //    int sortingLayerIndex = Mathf.Max(0, System.Array.IndexOf(sortingLayerIDs, property.intValue));
    //    sortingLayerIndex = EditorGUI.Popup(position, sortingLayerIndex, sortingLayerNames);
    //    property.intValue = sortingLayerIDs[sortingLayerIndex];
    //}

    ///**
    // * Retrives list of sorting layer names.
    // *
    // * @return List of sorting layer names.
    // */
    //private string[] GetSortingLayerNames()
    //{
    //    System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
    //    PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty(
    //             "sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
    //    return (string[])sortingLayersProperty.GetValue(null, new object[0]);
    //}

    ///**
    // * Retrives list of sorting layer identifiers.
    // *
    // * @return List of sorting layer identifiers.
    // */
    //private int[] GetSortingLayerIDs()
    //{
    //    System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
    //    PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty(
    //            "sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
    //    return (int[])sortingLayersProperty.GetValue(null, new object[0]);
    //}
}
