using System.Collections.Generic;
using UnityEngine;

namespace ZTools.ActionSequence
{
    /// <summary>
    /// All ActionSequence objects run on this manager, and can be reused if you turn on "reuse".
    /// 
    /// TODO：when gameobject that calls the ActionSequence destroys, ActionSequenceManager should auto finish corresponding ActionSequence. 
    /// So far, the gameobject should take this responsibility.
    /// </summary>
    public class ActionSequenceManager : MonoBehaviour
    {
        private static List<ActionSequence> _allSeq = new List<ActionSequence>();
        private static ActionSequenceManager _instance;

        public static bool reuse = false;

        public static ActionSequence Create()
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

            ActionSequence newSeq = null;

            if (reuse)
            {
                for (int i = 0; i < _allSeq.Count; i++)
                {
                    if (_allSeq[i].isFnished)
                    {
                        newSeq = _allSeq[i];
                        break;
                    }
                }
            }

            if (newSeq == null)
            {
                newSeq = _instance.gameObject.AddComponent<ActionSequence>();
                _allSeq.Add(newSeq);
            }

            newSeq.onFinished += onFinish;
            newSeq.isFnished = false;

            return newSeq;
        }


        private static void onFinish(ActionSequence seq)
        {
            seq.isFnished = true;
            seq.onFinished -= onFinish;

            if (!reuse)
            {
                _allSeq.Remove(seq);
                Destroy(seq);
            }
        }
    }
}