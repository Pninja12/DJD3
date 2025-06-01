using UnityEngine;

public class PickUps : MonoBehaviour
{
    [SerializeField] private int _ammoAmount = 3;

    private void OnTriggerEnter(Collider other)
    {
        Gun gun = other.GetComponentInChildren<Gun>();

        if(gun != null)
        {
            gun.AddAmo(_ammoAmount);
            Destroy(gameObject);
        }
    }
}
