using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pickupable : MonoBehaviour
{
    bool isPickedUp = false;
    Rigidbody rb;
    SphereCollider itemCollider;

    private void Start()
    {
        itemCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        Interactable interactable = GetComponentInChildren<Interactable>();
        interactable.OnPickupAction += Interactable_OnPickupAction;
        interactable.OnDropAction += Interactable_OnDropAction;
    }


    private void Interactable_OnPickupAction(Transform holdPoint) // Pickup|drop item event
    {
        //Interactable_OnDropAction();
        isPickedUp = true;
        transform.parent = holdPoint;
        rb.useGravity = false;
        rb.isKinematic = true;
        transform.position = holdPoint.position;
        itemCollider.enabled = false;
    }

    private void Interactable_OnDropAction()
    {
        itemCollider.enabled = true;
        isPickedUp = false;
        rb.isKinematic = false;
        rb.useGravity = true;
        transform.parent = null;
    }

    public bool HasBeenPickedUp()
    {
        return isPickedUp;
    }
}
