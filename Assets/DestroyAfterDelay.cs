using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
    [SerializeField] float delayTime = 10f;

    // Start is called before the first frame update
    void Start()
    {
        // Invoke the DestroyObject function after the specified delay time
        Invoke("DestroyObject", delayTime);
    }

    // Function to destroy the GameObject
    void DestroyObject()
    {
        // Check if the GameObject still exists before attempting to destroy it
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
