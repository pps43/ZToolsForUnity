using System;
using UnityEngine;
namespace ZTools.EditorUtil.Demo
{
    public enum ColliderType
    {
        player,
        enemy,
        weapon,
        detector,
    }


    [Serializable]
    public class ChooseType
    {
        public string name;
        public ColliderType type;
        public bool isChoose;

        public ChooseType(ColliderType t, bool choose = true)
        {
            type = t;
            name = t.ToString();
            isChoose = choose;
        }

        public override string ToString()
        {
            return name + "," + isChoose.ToString();
        }
    }

    
    public class MyTestClass : MonoBehaviour
    {
        //private field must be SerializeField to be viable from editor script
        [SerializeField] private ChooseType[] _correspondType;
        [SerializeField] private ColliderType _selfType = ColliderType.player;//default value


        public bool canEdit = true;



        public void resetCorrespondType()
        {
            Debug.Log(gameObject.name + ", resetCollisionFrom. selfType:" +  _selfType.ToString());
            resetCorrespondType(_selfType);
        }

        //will call in compnent menu -> Reset
        private void Reset()
        {
            resetCorrespondType();
        }

        //reset _correspondType to default settings
        private void resetCorrespondType(ColliderType selfType)
        {
            switch (selfType)
            {
                case ColliderType.player:
                    _correspondType = new ChooseType[] { new ChooseType(ColliderType.weapon), new ChooseType(ColliderType.enemy) };
                    break; 
                
                case ColliderType.enemy:
                    _correspondType = new ChooseType[] { new ChooseType(ColliderType.weapon), new ChooseType(ColliderType.player) };
                    break;
                
                case ColliderType.weapon:
                    _correspondType = new ChooseType[] { new ChooseType(ColliderType.player)};
                    break;
                case ColliderType.detector:
                    _correspondType = new ChooseType[] { new ChooseType(ColliderType.player), new ChooseType(ColliderType.enemy) };
                    break;
                default:
                    break;
            }
        }

        
    }
}