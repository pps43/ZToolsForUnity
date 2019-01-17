using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ZTools.DebugUtil;

namespace ZTools.Game.CollisionUtil
{
    [CreateAssetMenu(fileName = "NewCollisonConfig", menuName = "ZTools/CollisionConfig")]
    public class CollisionConfig : ScriptableObject
    {
        //Dictionary cannot be serialized in unity, even in your custom editor, it seems to be saved. Use 1-D array instead, cause 2-D array are not supported either.
        //[SerializeField] public Dictionary<int, bool> matrix2D = new Dictionary<int, bool>();
        [SerializeField] private bool[] matrixArray;
        [SerializeField] public string[] allTypeNames;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public void init()
        {
            allTypeNames = Enum.GetNames(typeof(ColliderType));
            Width = allTypeNames.Length;
            Height = allTypeNames.Length;

            //TODO resortName into diffrent layer group

            matrixArray = new bool[Width * Height];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    matrixArray[i * Height + j] = false;
                }
            }
        }

        //refresh while keep old value as much as you can
        public void Refresh()
        {
            var newNames = Enum.GetNames(typeof(ColliderType));

            if (nameHasChanged(allTypeNames, newNames))
            {
                var oldMatrixDic = new Dictionary<ColliderType, HashSet<ColliderType>>();
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        if (matrixArray[i * Height + j])
                        {
                            if (!oldMatrixDic.ContainsKey((ColliderType)i))
                            {
                                oldMatrixDic.Add((ColliderType)i, new HashSet<ColliderType>());
                            }
                            oldMatrixDic[(ColliderType)i].Add((ColliderType)j);
                        }
                    }
                }

                allTypeNames = newNames;
                Width = allTypeNames.Length;
                Height = Width;

                matrixArray = new bool[Width * Height];

                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        if (oldMatrixDic.ContainsKey((ColliderType)i) && oldMatrixDic[(ColliderType)i].Contains((ColliderType)j))
                        {
                            this[i, j] = true;
                        }
                        else
                        {
                            this[i, j] = false;
                        }
                    }
                }
            }
        }

        private bool nameHasChanged(string[] oldName, string[] newName)
        {
            if (oldName == null || newName == null) { return false; }

            return oldName.Length != newName.Length; //TODO more accurate algorithm to detect difference
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (matrixArray[i * Height + j])
                    {
                        sb.Append((ColliderType)i).
                            Append(" <==> ").
                            Append((ColliderType)j).Append(",  ");
                    }
                }
            }

            return sb.ToString();
        }

        public bool this[int i, int j]
        {
            get
            {
                if (i < Width && j < Height && matrixArray != null)
                {
                    return matrixArray[i * Height + j];
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (i < Width && j < Height && matrixArray != null)
                {
                    matrixArray[i * Height + j] = value;
                }
            }
        }

    }
}