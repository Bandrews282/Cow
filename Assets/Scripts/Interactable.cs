using System;
using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    [HideInInspector] public event Action<Transform> OnPickupAction;
    [HideInInspector] public event Action OnDropAction;

    private bool isInRange = false;
    private bool currentlyHeld = false;
    private Transform holdTarget;
    private Pickupable pickupable;
    private PlayerController player;
    private ZookeeperAI zookeeper;


    // Start is called before the first frame update
    void Start()
    {
        pickupable = GetComponentInParent<Pickupable>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        zookeeper = GameObject.FindGameObjectWithTag("Zookeeper").GetComponent<ZookeeperAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Player":
                isInRange = true;
                player.TargetReached += Player_TargetReached; 
                break;

            case "Zookeeper":
                isInRange = true;
                zookeeper.TargetReached += Zookeeper_TargetReached;
                break;
        }
    }

    private void Player_TargetReached(Transform holdPoint)
    {
        holdTarget = holdPoint;

        if (!currentlyHeld)
        {
            Pickup();
        }
        else
        {
            Drop();
        }
    }

    private void Zookeeper_TargetReached(Transform holdPoint)
    {
        holdTarget = holdPoint;
        Debug.Log("Pickup/Drop attempt");
        if (!currentlyHeld)
        {
            Pickup();
            zookeeper.HoldingItem = true;
        }
        else
        {
            Drop();
            zookeeper.HoldingItem = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isInRange = false;
        holdTarget = null;

        if (other.tag == "Zookeeper")
        {
            zookeeper.TargetReached -= Zookeeper_TargetReached;
        }

        if(other.tag == "Player")
        {
            player.TargetReached -= Player_TargetReached;
        }
    }

    public void Pickup()
    {
        currentlyHeld = true;
        Debug.Log("PickedUp");
        OnPickupAction?.Invoke(holdTarget);
    }

    public void Drop()
    {
        currentlyHeld = false;
        Debug.Log("Dropped");
        OnDropAction?.Invoke();
    }
}