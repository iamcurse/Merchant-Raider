using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    [ShowOnly][SerializeField] private bool inRange;
    [SerializeField] private UnityEvent interactEvent;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
        }
    }

    public void Interact()
    {
        if (!inRange) return;
        
        Debug.Log("Interacting with object");
        interactEvent.Invoke();
    }
}
