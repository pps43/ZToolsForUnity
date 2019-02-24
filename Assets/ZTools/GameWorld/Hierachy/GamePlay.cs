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
    public class GamePlay : MonoBehaviour, IUpdatable
    {
        public event Action updateEvent;

        public static GamePlay instance;

        //public InputManager inputManager { get; private set; }
        public EventDispatcher eventDispatcher { get; private set; }
        //public LevelManager levelManager { get; private set; }
        //public EnemyManager enemyManager { get; private set; }
        public CollisionManager collisionManager { get; private set; }

        private EventHelper _eventHelper;
        public FSM<GamePlay, NoEvent, CommonEvent> fsm { get; private set; }

        private void Awake()
        {
            instance = this;//temp
        }

        private void Start()
        {
            //fsm = FSMFactory.createFSM(this, new InitialState(), null);
            fsm.start();
        }

        private void Update()
        {
            updateEvent?.Invoke();
            eventDispatcher.update();
        }


        public void InitGamePlay()
        {
            //get managers

            eventDispatcher = EventDispatcher.instance;

            _eventHelper = new EventHelper();
            //add listener
        }

        public void UnInitGamePlay()
        {
            fsm.stop(); fsm = null;

            //remove listener

        }

    }
}