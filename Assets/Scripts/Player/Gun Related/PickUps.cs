using UnityEngine;

public class PickUps : MonoBehaviour
{
    [SerializeField] private int _ammoAmount = 3;

    private void OnCollisionEnter(Collision collision)
    {

        Gun gun = collision.gameObject.GetComponentInChildren<Gun>();

        if(gun != null)
        {
            Debug.Log("Picked up ammo!");
            gun.AddAmo(_ammoAmount);
        }
    }
}
