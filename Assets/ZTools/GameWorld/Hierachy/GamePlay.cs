using System;
using UnityEngine;
using ZTools.Event;
using ZTools.FSM;
using ZTools.Game.CollisionUtil;
namespace ZTools.Game
{
    /// <summary>
    /// Global singleton as an etry point, manage in-game modules/submanagers.
    /// GamePlay itself has a FSM, which include loading, playing, pause, exit state.
    /// </summary>
    public class GamePlay : MonoBehaviour
    {
        public static GamePlay instance;

        //public InputManager inputManager { get; private set; }
        public EventDispatcher eventDispatcher { get; private set; }
        //public LevelManager levelManager { get; private set; }
        //public EnemyManager enemyManager { get; private set; }
        public CollisionManager collisionManager { get; private set; }

        private EventHelper _eventHelper;
        public FSM<GamePlay, CommonEvent> fsm { get; private set; }

        private void Awake()
        {
            instance = this;//temp
        }

        private void Start()
        {
            //fsm = FSMFactory.createFSM(this, new InitialState(), null);
            fsm.Start();
        }

        private void Update()
        {
            eventDispatcher.Update();
        }


        public void InitGamePlay()
        {
            //get managers

            eventDispatcher = EventDispatcher.Instance;

            _eventHelper = new EventHelper();
            //add listener
        }

        public void UnInitGamePlay()
        {
            fsm.Stop(); fsm = null;

            //remove listener

        }

    }
}