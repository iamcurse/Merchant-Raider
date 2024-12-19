using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInfo", menuName = "Player/Info")]
public class PlayerInfo : ScriptableObject
{
    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField] public int MaxHealth { get; private set; }
    [field: SerializeField] public int Money { get; private set; }

    public void RestoreHealth()
    {
        Health = MaxHealth;
    }

    public void RestoreHealth(int amount)
    {
        Health += amount;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }
    
    public void TakeDamage()
    {
        Health --;
        if (Health < 0)
        {
            Health = 0;
        }
    }
    
    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health < 0)
        {
            Health = 0;
        }
    }
    
    public void AddMoney(int amount)
    {
        Money += amount;
    }
    
    public void SpendMoney(int amount)
    {
        Money -= amount;
        if (Money < 0)
        {
            Money = 0;
        }
    }
    
    public void SetMaxHealth(int amount)
    {
        MaxHealth = amount;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }
    
    public bool IsDead()
    {
        return Health <= 0;
    }
}
