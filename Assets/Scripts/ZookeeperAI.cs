using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum ZookeeperStates
{
    MoveTo,
    ChooseItem,
    Scared,
    Chase,
    Enter,
    Leave
}

public enum DestinationState
{
    Enter,
    Item,
    Target,
    Player,
    Exit
}

public class ZookeeperAI : MonoBehaviour
{
    [SerializeField]
    public Transform holdPoint;

    [SerializeField]
    private Transform putDownPoint;
    [SerializeField]
    private Transform entrance;
    [SerializeField]
    private Transform exit;
    [SerializeField]
    private int chaseTime;
    [SerializeField]
    private int checkDelay;
    [SerializeField]
    private float targetTriggerRange;

    private Transform destination;
    private NavMeshAgent agent;
    private Stack<GameObject> items = new Stack<GameObject>();
    private ZookeeperStates currentState;
    private DestinationState destinationState;
    private GameObject playerGO;
    private PlayerController playerController;
    private GameManager GM;
    private Animator anim;

    private bool pickedUp = false;
    private bool isWalking = false;
    private bool puttingDown = false;
    private bool openingGate = false;
    private bool isScared = false;
    private bool isLeaving = false;

    public event Action<Transform> TargetReached;
    public event Action PatrolFinished;
    public event Action<float> BeenScared;
    public event Action OpeningGate;

    public bool HoldingItem { get; set; } = false;

    private void Awake()
    {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        GM.SetZookeeper(this);
    }

    void Start()
    {
        items.Clear();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        playerGO = GameObject.FindGameObjectWithTag("Player");
        playerController = playerGO.GetComponent<PlayerController>();
        
        GM.patrol += StartPatrol;
        currentState = ZookeeperStates.Leave;
    }

    private void LookForItems()
    {
        GameObject[] itemFinder = GameObject.FindGameObjectsWithTag("Item");

        foreach (GameObject item in itemFinder)
        {
            items.Push(item);
        }
    }

    private void StartPatrol()
    {
        Debug.Log("Patrol Started");
        LookForItems();
        currentState = ZookeeperStates.Enter;
        InvokeRepeating("StateChecker", 0, checkDelay);
    }

    private void StateChecker()
    {
        switch (currentState)
        {
            case ZookeeperStates.MoveTo:
                MoveTo();
                break;
            case ZookeeperStates.Chase:
                Chase();
                break;
            case ZookeeperStates.Enter:
                Enter();
                break;
            case ZookeeperStates.Scared:
                FallOver();
                break;
            case ZookeeperStates.Leave:
                Leave();
                break;
            case ZookeeperStates.ChooseItem:
                ChooseItem();
                break;
        }
    }

    private void Update()
    {
        if (currentState == ZookeeperStates.MoveTo)
            MoveTo();

        Debug.Log(currentState.ToString());
    }

    private void MoveTo()
    {
        agent.SetDestination(destination.position);

        if (Vector3.Distance(transform.position, destination.position) <= targetTriggerRange)
        {
            switch (destinationState)
            {
                case DestinationState.Enter:
                    if (!openingGate)
                    {
                        openingGate = true;
                        agent.isStopped = true;
                        anim.SetTrigger("Gate");
                    }
                    break;

                case DestinationState.Item:
                    Debug.Log("Attempt to pickup");
                    if (!pickedUp)
                    {
                        anim.SetTrigger("Pickup");
                        pickedUp = true;
                    }
                    break;

                case DestinationState.Target:

                    if (!puttingDown)
                    {
                        anim.SetTrigger("PutDown");
                        puttingDown = true;
                    }

                    break;

                case DestinationState.Exit:
                    Leave();
                    break;

                case DestinationState.Player:
                    TargetReached?.Invoke(holdPoint);
                    Debug.Log("Caught Player");
                    if (HoldingItem)
                    {
                        destinationState = DestinationState.Target;
                        destination = putDownPoint;
                    }
                    else
                    {
                        destinationState = DestinationState.Item;
                        destination = putDownPoint;
                    }
                    break;

                default:
                    break;
            }
        }
    }

    private void Chase()
    {
        agent.isStopped = false;
        currentState = ZookeeperStates.MoveTo;
        destinationState = DestinationState.Player;
        StartCoroutine(Cooldown());
    }

    private void Enter()
    {
        gameObject.SetActive(true);
        destination = entrance;
        currentState = ZookeeperStates.MoveTo;
    }

    public void OpenGate()
    {
        OpeningGate?.Invoke();
        agent.isStopped = false;
        if (!isLeaving)
        {
            ChooseItem();
            destinationState = DestinationState.Item;
            currentState = ZookeeperStates.MoveTo;
        }
        else
        {
            destinationState = DestinationState.Exit;
            destination = exit;
        }

        openingGate = false;
    }

    private void Leave()
    {
        CancelInvoke();
        PatrolFinished?.Invoke();
        isLeaving = false;
    }

    private void ChooseItem()
    {
        if (!items.Any())
        {
            destination = entrance;
            currentState = ZookeeperStates.Leave;
            destinationState = DestinationState.Enter;
        }
        else
        {
            destination = items.Peek().transform;
        }
    }

    public void RemoveItemFromStack()
    {
        if (!items.Any())
            Leave();
        else
            items.Pop();
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(chaseTime);
        ChooseItem();
        currentState = ZookeeperStates.MoveTo;
    }

    private void FallOver()
    {
        if (Vector3.Distance(transform.position, playerGO.transform.position) <= 5f && !agent.isStopped && !isScared)
        {
            agent.isStopped = true;
            isScared = true;
            currentState = ZookeeperStates.Scared;
            Debug.Log("Gets Scared");
            BeenScared?.Invoke(0.2f);
            anim.SetTrigger("Scared");
            TargetReached?.Invoke(holdPoint);
            StartCoroutine(AnimResetter());
        }
        else
        {
            return;
        }
    }

    public void Pickup()
    {
        TargetReached?.Invoke(holdPoint);
        if (HoldingItem)
        {
            destinationState = DestinationState.Target;
            destination = putDownPoint;
        }

        pickedUp = false;
    }

    public void PutDown()
    {
        TargetReached?.Invoke(holdPoint);

        RemoveItemFromStack();

        if (!items.Any())
        {
            destination = entrance;
            destinationState = DestinationState.Enter;
            isLeaving = true;
        }
        else
        {
            ChooseItem();
            destinationState = DestinationState.Item;
        }

        puttingDown = false;
    }

    public void GetUp()
    {
        StopAllCoroutines();
        destination = playerGO.transform;
        currentState = ZookeeperStates.Chase;
        isScared = false;
    }

    IEnumerator AnimResetter()
    {
        yield return new WaitForSeconds(10f);
        currentState = ZookeeperStates.Chase;
        destination = playerGO.transform;
        isScared = false;
    }
}