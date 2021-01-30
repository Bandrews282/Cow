using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private Animator anim;
    private ZookeeperAI keeper;

    private void Start()
    {
        anim = GetComponent<Animator>();
        keeper = GameObject.FindGameObjectWithTag("Zookeeper").GetComponent<ZookeeperAI>();
        keeper.OpeningGate += Keeper_OpeningGate;
    }

    private void Keeper_OpeningGate()
    {
        anim.SetTrigger("DoorOpen");
        StartCoroutine(CloseGate());
    }

    IEnumerator CloseGate()
    {
        yield return new WaitForSeconds(3f);
        anim.SetTrigger("DoorClose");
        yield return null;
    }
}
