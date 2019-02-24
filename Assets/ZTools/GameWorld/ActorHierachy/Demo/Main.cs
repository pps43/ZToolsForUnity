using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZTools.Game;
namespace ZTools.Demo
{

    public class Main : MonoBehaviour
    {
        private EnemyManager enemyManager;
        public EnemyFactory factory;

        IEnumerator Start()
        {
            enemyManager = new EnemyManager();
            enemyManager.Init(factory);

            yield return null;

            enemyManager.Generate(EnemyType.flyer);

            enemyManager.UnInit();//after unInit, nothing will be generate.

            yield return new WaitForSeconds(2f);

            enemyManager.Generate(EnemyType.walker);
        }

    }
}