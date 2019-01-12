using UnityEngine;
using UnityEditor;
using ZTools.EditorUtil.Demo;

namespace ZTools.EditorUtil.CustomInspector
{
    //show MyTestClass._selfType, and show diffrent _correspondType according to _selfType's value
    [CustomEditor(typeof(MyTestClass))]
    public class CollisionJudgeAbilityEditor: Editor
    {
        SerializedProperty selfType;
        SerializedProperty correspondTypes;
        SerializedProperty canEdit;

        void OnEnable()
        {
            selfType = serializedObject.FindProperty("_selfType");
            correspondTypes = serializedObject.FindProperty("_correspondType");
            canEdit = serializedObject.FindProperty("canEdit");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update(); //sync from object

            EditorGUILayout.PropertyField(canEdit);//show selfType using default style

            if (!canEdit.boolValue)
            {
                GUI.enabled = false; //disable editing following items, use GUI.enable = true to re-enable.
            }

            EditorGUI.BeginChangeCheck(); // detect whether selftype has changed
            
            EditorGUILayout.PropertyField(selfType, new GUIContent("SelfType =>"));//show selfType using default style, but using another label.

            serializedObject.ApplyModifiedProperties(); //write to object now because object resetCollisionFrom() need to use new value to do sth.
            serializedObject.Update(); //sync from object

            if (EditorGUI.EndChangeCheck())
            {
                (target as MyTestClass)?.resetCorrespondType();//refresh
            }

            show(correspondTypes);

            serializedObject.ApplyModifiedProperties();// write to object
        }


        private void show(SerializedProperty ChooseTypeArray)
        {
            GUILayout.Space(10);
            GUILayout.Label("Can collide with:");
            GUILayout.BeginVertical();
            for (int i = 0; i < ChooseTypeArray.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(ChooseTypeArray.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue, GUILayout.Width(100));

                //show bool value and sync to SerializedProperty (not directly to object)
                ChooseTypeArray.GetArrayElementAtIndex(i).FindPropertyRelative("isChoose").boolValue = EditorGUILayout.Toggle(ChooseTypeArray.GetArrayElementAtIndex(i).FindPropertyRelative("isChoose").boolValue);

                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}