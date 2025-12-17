using UnityEngine;

public class Target : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Call coin and target logic from GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CollectCoin();
                GameManager.Instance.TargetDestroyed();
            }

            Destroy(gameObject,0.6f); // Destroy this target
        }
    }
}
