using UnityEngine;

public class SpeedBoostPad : MonoBehaviour
{
    public float boostMultiplier = 2f;
    public float boostDuration = 5f;
    public float boostForce = 500f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Speed boost collided with Player");

            Controller car = other.GetComponentInParent<Controller>();
            if (car != null)
            {
                Debug.Log("Speed boost activated!");
                car.ActivateSpeedBoost(boostMultiplier, boostDuration);

                Rigidbody rb = other.GetComponentInParent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(other.transform.forward * boostForce, ForceMode.Impulse);
                    Debug.Log("Force added to Rigidbody");
                }
            }
            else
            {
                Debug.LogWarning("No Controller script found on Player object or its parent.");
            }
        }
    }
}
