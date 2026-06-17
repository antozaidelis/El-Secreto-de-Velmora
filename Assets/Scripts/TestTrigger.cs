using UnityEngine;

public class TestTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("PIMIENTO TOCADO POR: " + collision.gameObject.name);
    }
}