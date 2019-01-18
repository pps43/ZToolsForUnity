using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ZTools.Game.CollisionUtil;

namespace ZTools.EditorUtil
{
    [CustomEditor(typeof(CollisionConfig))]
    public class CollisonConfigEditor : Editor
    {
        bool valueChange = false;

        public override void OnInspectorGUI()
        {
            CollisionConfig config = target as CollisionConfig;

            GUILayout.Label("Back To Empty state");
            if (GUILayout.Button("init/reset"))
            {
                config.init();
            }

            GUILayout.Space(20);
            GUILayout.Label("Sync if you change ColliderType");
            if (GUILayout.Button("Refresh"))
            {
                config.Refresh();
                valueChange = true;
            }

            GUILayout.Space(20);
            if (valueChange)
            {
                if (GUILayout.Button("Value changed, Click or Press Ctrl + S", GUILayout.Height(50)))
                {
                    valueChange = false;
                    AssetDatabase.SaveAssets();
                }
            }

            show(config);

            EditorUtility.SetDirty(config); //This is very important

        }

        private void show(CollisionConfig config)
        {
            int labelSize = 50;
            // find the longest label
            for (int i = 0; i < config.allTypeNames.Length; i++)
            {
                var textDimensions = GUI.skin.label.CalcSize(new GUIContent(config.allTypeNames[i]));
                if (labelSize < textDimensions.x)
                    labelSize = (int)textDimensions.x;
            }
            labelSize += 30;

            if(true)
            {
                int checkboxSize = 16;
                int indent = 0;
                var topLabelRect = GUILayoutUtility.GetRect(checkboxSize + labelSize, labelSize);
                var topLeft = new Vector2(topLabelRect.x, topLabelRect.y);
                var y = 0;
                for (int i = 0; i < config.allTypeNames.Length; i++)
                {
                    var translate = new Vector3(labelSize + indent + checkboxSize * y + topLeft.x, topLeft.y , 0);
                    //GUI.matrix = Matrix4x4.TRS(translate, Quaternion.Euler(0, 0, 90), Vector3.one);
                    GUI.matrix = Matrix4x4.TRS(new Vector3(180+checkboxSize * y, topLeft.y,0), Quaternion.Euler(0, 0, 90), Vector3.one);
                    GUI.Label(new Rect(0, 0, labelSize, checkboxSize), config.allTypeNames[i], "RightLabel");
                    y++;
                }

                GUI.matrix = Matrix4x4.identity;
                y = 0;
                for (int i = 0; i < config.allTypeNames.Length; i++)
                {
                    int x = 0;
                    var r = GUILayoutUtility.GetRect(indent + checkboxSize * config.allTypeNames.Length + labelSize, checkboxSize);
                    GUI.Label(new Rect(/*r.x + indent*/0, r.y, labelSize, checkboxSize), config.allTypeNames[i], "RightLabel");

                    for (int j = 0; j < config.allTypeNames.Length; j++)
                    {
                        var tooltip = new GUIContent("", config.allTypeNames[i] + " with " + config.allTypeNames[j]);
                        bool val = getValue(config,i, j);
                        bool toggle = GUI.Toggle(new Rect(labelSize + indent + r.x + x * checkboxSize, r.y, checkboxSize, checkboxSize), val, tooltip);
                        if (toggle != val)
                        {
                            setValue(config, i, j, toggle);
                            valueChange = true;
                        }
                        x++;
                    }
                    y++;
                }

            }
            
        }

        private bool getValue(CollisionConfig config, int i, int j)
        {
            return config[i,j];
        }
        private void setValue(CollisionConfig config, int i, int j, bool value)
        {
            config[i, j] = value;
        }

    }
}