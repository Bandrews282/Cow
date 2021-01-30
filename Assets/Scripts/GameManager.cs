using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public event Action patrol;

    private ZookeeperAI zookeeper;
    [SerializeField] private PlayerController player;
    private bool patrolFinished = true;
    [SerializeField] Image mischiefBar;
    [SerializeField] AudioClip metalClip;
    [SerializeField] AudioClip regularMusic;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject honkText;
    private AudioSource audioSource;
    private bool gameEnded = false;
    private bool isPaused = false;
    private bool buttonPressed = false;

    public float mischiefMeter;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        mischiefMeter = 0;
        zookeeper.PatrolFinished += Zookeeper_PatrolFinished;
        zookeeper.BeenScared += AddToMeter;
        InvokeRepeating("GoOnPatrol", UnityEngine.Random.Range(5f, 50f), UnityEngine.Random.Range(120f, 300f));
        audioSource.clip = regularMusic;
        audioSource.Play();
        audioSource.loop = true;
        UpdateUI();
        honkText.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetButton("Flap Wings"))
        {
            honkText.SetActive(false);
        }


        if (Input.GetButton("Pause") && isPaused && !buttonPressed)
        {
            buttonPressed = true;
            isPaused = false;
            Unpause();
            StartCoroutine(ButtonCooldown());
        }
        if (Input.GetButton("Pause") && !isPaused && !buttonPressed)
        {
            buttonPressed = true;
            isPaused = true;
            Pause();
            StartCoroutine(ButtonCooldown());
        }

        if (FullBar() && !gameEnded)
        {
            mischiefMeter = 0f;
            UpdateUI();
            gameEnded = true;
            audioSource.clip = metalClip;
            audioSource.Play();
            audioSource.loop = false;
            StartCoroutine(MetalCooldown());
        }
    }

    IEnumerator ButtonCooldown()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        buttonPressed = false;
    }

    private void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    private void Unpause()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    private void Zookeeper_PatrolFinished()
    {
        patrolFinished = true;
    }

    private void GoOnPatrol()
    {
        if (patrolFinished)
        {
            patrol?.Invoke();
            patrolFinished = false;
        }
    }

    public void SetZookeeper(ZookeeperAI zookeeperAI)
    {
        zookeeper = zookeeperAI;
    }


    public void AddToMeter(float mischief)
    {
        mischiefMeter += mischief;
        UpdateUI();
    }

    private void UpdateUI()
    {
        mischiefBar.fillAmount = mischiefMeter;
    }

    public bool FullBar()
    {
        return mischiefMeter >= 1;
    }

    IEnumerator MetalCooldown()
    {
        yield return new WaitForSeconds(20f);
        gameEnded = false;
        audioSource.Stop();
        audioSource.clip = regularMusic;
        audioSource.Play();
        audioSource.loop = true;
    }
}
