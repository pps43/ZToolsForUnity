namespace ZTools.Demo
{
    public enum EventID
    {
        OnTurn,
        OnDamage,
    }

    public abstract class EventData
    {
        
    } 

    public class DamageData : EventData
    {
        public int damage;
        
    }
}