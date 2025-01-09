using UnityEngine;

[CreateAssetMenu(fileName = "DroppedItem", menuName = "DroppedItem")]
public class DroppedItem : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon; // For UI representation
    public GameObject worldPrefab; // Prefab to spawn in the world
}
