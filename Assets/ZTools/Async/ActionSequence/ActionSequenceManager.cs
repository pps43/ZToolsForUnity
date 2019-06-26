using System.Collections.Generic;
using UnityEngine;

namespace ZTools.ActionSequence
{
    /// <summary>
    /// All ActionSequence objects run on this manager, and are reuseable.
    /// 
    /// TODO：when gameobject that calls the ActionSequence destroys, ActionSequenceManager should auto finish corresponding ActionSequence. 
    /// So far, the gameobject should take this responsibility.
    /// </summary>
    public class ActionSequenceManager : MonoBehaviour
    {
        private static List<ActionSequence> _allSeq = new List<ActionSequence>();
        private static ActionSequenceManager _instance;

        public static ActionSequence create()
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject
                {
                    name = "ActionSequenceManager"
                };
                //obj.hideFlags = HideFlags.HideInHierarchy;
                _instance = obj.AddComponent<ActionSequenceManager>();
            }

            for (int i = 0; i < _allSeq.Count; i++)
            {
                if (_allSeq[i].isRunning() == false)
                {
                    _allSeq[i].Stop();
                    return _allSeq[i];
                }
            }

            ActionSequence newSeq = _instance.gameObject.AddComponent<ActionSequence>();
            _allSeq.Add(newSeq);

            return newSeq;
        }
    }
}