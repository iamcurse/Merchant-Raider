using UnityEngine;

public class DestoyGameObject : MonoBehaviour
{
    private void DestroyThis()
    {
        Destroy(gameObject);
    }

    private void DestroyParent()
    {
        Destroy(transform.parent.gameObject);
    }
}
