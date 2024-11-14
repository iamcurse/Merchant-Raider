using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInfo", menuName = "Player/Info")]
public class PlayerInfo : ScriptableObject
{
    public int health;
    public int maxHealth;
    public int money;
}
