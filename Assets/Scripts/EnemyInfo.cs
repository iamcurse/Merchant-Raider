using UnityEngine;

[CreateAssetMenu(fileName = "EnemyInfo", menuName = "Enemy/EnemyInfo")]
public class EnemyInfo : ScriptableObject
{
    public int health;
    public int maxHealth;
}
