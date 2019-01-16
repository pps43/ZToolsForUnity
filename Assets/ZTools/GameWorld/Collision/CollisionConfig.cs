using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZTools.DebugUtil;

namespace ZTools.Game.CollisionUtil
{
    [CreateAssetMenu(fileName = "NewCollisonConfig", menuName = "ZTools/CollisionConfig")]
    public class CollisionConfig : ScriptableObject
    {
        [SerializeField] public Dictionary<int, bool> matrix2D = new Dictionary<int, bool>();
        [SerializeField] public string[] names;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public long Count { get { return matrix2D.Count; } }

        private void OnEnable()
        {
            Reset();
        }

        public void Reset()
        {
            names = Enum.GetNames(typeof(GameColliderType));
            Width = names.Length;
            Height = Width;

            //default value : only collides with self
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    this[i, j] = i == j;
                }
            }
        }

        public bool this[int r, int c]
        {
            get
            {
                int index = r * Width + c;
                bool res = false;
                if (!matrix2D.TryGetValue(index, out res))
                {
                    ZLog.error("No definition found");
                }
                return res;
            }
            set
            {
                int index = r * Width + c;
                matrix2D[index] = value;
            }
        }

        public bool this[GameColliderType row, GameColliderType col]
        {
            get
            {
                int index = (int)row * Width + (int)col;
                bool res = false;
                if(!matrix2D.TryGetValue(index, out res))
                {
                    ZLog.error("No definition found between",row.ToString(), "and", col.ToString());
                }
                return res;
            }
            set
            {
                int index = (int)row * Width + (int)col;
                matrix2D[index] = value;
            }
        }

    }
}