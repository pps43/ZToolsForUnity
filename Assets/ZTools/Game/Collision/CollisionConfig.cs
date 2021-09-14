using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ZTools.Game.Collision
{
    /// <summary>
    /// customized collision matrix.
    /// Better not use this because its redundancy will lead to endless confusion.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCollisonConfig", menuName = "ZTools/CollisionConfig")]
    public class CollisionConfig : ScriptableObject
    {
        //Dictionary cannot be serialized in unity, even in your custom editor, it seems to be saved. Use 1-D array instead, cause 2-D array are not supported either.
        //[SerializeField] public Dictionary<int, bool> matrix2D = new Dictionary<int, bool>();
        [SerializeField] private bool[] matrixArray;
        [SerializeField] public string[] allTypeNames;

        //public int width { get; private set; }  // property cannot be serialized in unity.
        //public int height { get; private set; }
        public int width;
        public int height;

        public void init()
        {
            allTypeNames = Enum.GetNames(typeof(ColliderType));
            width = allTypeNames.Length;
            height = allTypeNames.Length;

            //TODO resortName into diffrent layer group

            matrixArray = new bool[width * height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    matrixArray[i * height + j] = false;
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
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (matrixArray[i * height + j])
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
                width = allTypeNames.Length;
                height = width;

                matrixArray = new bool[width * height];

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
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
            sb.Append("(width, height) = (").Append(width).Append(", ").Append(height);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (matrixArray[i * height + j])
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
                if (i < width && j < height && matrixArray != null)
                {
                    return matrixArray[i * height + j];
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (i < width && j < height && matrixArray != null)
                {
                    matrixArray[i * height + j] = value;
                }
            }
        }

    }
}