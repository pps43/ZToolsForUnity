namespace ZTools.EditorUtil.CustomInspector
{
    // change array value according to Enum
    //[CustomEditor(typeof(CollisionJudgeAbility))]
    //public class CollisionJudgeAbilityEditor: Editor
    //{

    //    SerializedProperty mainEnumType;
    //    SerializedProperty chooseTypes;


    //    void OnEnable()
    //    {
    //        mainEnumType = serializedObject.FindProperty("_type");
    //        chooseTypes = serializedObject.FindProperty("_chooseTypes");
    //    }

    //    public override void OnInspectorGUI()
    //    {
    //        serializedObject.Update();

    //        EditorGUI.BeginChangeCheck();

    //        EditorGUILayout.PropertyField(mainEnumType, new GUIContent("SelfType:"));

    //        serializedObject.ApplyModifiedProperties();

    //        serializedObject.Update();

    //        if (EditorGUI.EndChangeCheck())
    //        {
    //            (target as CollisionJudgeAbility)?.resetCollisionFrom();
    //        }

    //        show(chooseTypes);

    //        serializedObject.ApplyModifiedProperties();
    //    }


    //    private void show(SerializedProperty ChooseTypeArray)
    //    {
    //        GUILayout.Space(10);
    //        GUILayout.Label("Can collide with:");
    //        GUILayout.BeginVertical();
    //        for (int i = 0; i < ChooseTypeArray.arraySize; i++)
    //        {
    //            EditorGUILayout.BeginHorizontal();

    //            EditorGUILayout.LabelField(ChooseTypeArray.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue, GUILayout.Width(100));

    //            ChooseTypeArray.GetArrayElementAtIndex(i).FindPropertyRelative("isChoose").boolValue = EditorGUILayout.Toggle(ChooseTypeArray.GetArrayElementAtIndex(i).FindPropertyRelative("isChoose").boolValue);

    //            EditorGUILayout.EndHorizontal();

    //        }
    //        GUILayout.EndVertical();
    //    }
    //}
}