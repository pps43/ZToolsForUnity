using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZTools.Game;

namespace ZTools.Demo
{
    public class EnemyFactory : BaseObjectFactory<EnemyType, Enemy>
    {
        protected override void ListToDic()
        {
            if (_prefabList != null)
            {
                for (int i = 0; i < _prefabList.Length; i++)
                {
                    _prefabDic.Add(_prefabList[i].TypeID, _prefabList[i]);
                }
            }
        }
    }
}