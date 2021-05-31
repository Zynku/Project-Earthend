using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
public class teleporternetwork : MonoBehaviour
{
    public GameObject Player;
    public GameObject[] teleporters;
    public GameObject activatedAt;
    public GameObject teleportedTo;

    public TextMeshProUGUI[] teleporterNames;
    public TextMeshProUGUI pageNumberText;

    public GameObject TeleportCanvas;
    public GameObject firstSelected;

    Animator animator;
    Animator animatorTo;

    AudioSource audiosource;
    AudioSource audiosourceTo;

    public int pageNumber = 0;
    public int startTeleporterNumber;

    private void Start()
    {
        Player = GameObject.FindWithTag("Player");
        teleporters = GameObject.FindGameObjectsWithTag("teleporter");
        AssignTeleporters(0);
        TeleportCanvas.gameObject.SetActive(false);
    }

    public void showNetworkUI()
    {
        TeleportCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);
        gamemanager.instance.PauseGame();
    }

    public void hideNetworkUI()
    {
        TeleportCanvas.SetActive(false);
        gamemanager.instance.ResumeGame();
    }

    // Update is called once per frame
    void Update()
    {
        pageNumberText.text = pageNumber.ToString();

        if (Input.GetKeyDown(KeyCode.P))
        {
            NextPage();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            PreviousPage();
        }
    }
    public void AssignTeleporters(int tp)
    {
        int q = 0;
        for (int i = tp; i < tp + 3; i++, q++)
        {
            if (i < teleporters.Length)
            {
                teleporterNames[q].text = teleporters[i].name.ToString();
            }
            else
            {
                teleporterNames[q].text = "[No Teleporter Found]";
            }
        }
    }

    public void NextPage()
    {
        pageNumber++;
        if (pageNumber < 1) { pageNumber = 1; }
        if (pageNumber > (teleporters.Length/3 + 1)) { pageNumber = teleporters.Length / 3 + 1; }
        startTeleporterNumber = (3 * (pageNumber - 1));
        AssignTeleporters(startTeleporterNumber);
    }

    public void PreviousPage()
    {
        pageNumber--;
        if (pageNumber < 1) { pageNumber = 1; }
        if (pageNumber > (teleporters.Length / 3 + 1)) { pageNumber = teleporters.Length / 3 + 1; }
        startTeleporterNumber = (3 * (pageNumber - 1));
        AssignTeleporters(startTeleporterNumber);
    }

    public void TeleportToOne()
    {
        Debug.Log(teleporters[3 * (pageNumber - 1)]);
        if (teleporters[3 * (pageNumber - 1)] != null)
        {
            Player.transform.position = teleporters[3 * (pageNumber - 1)].transform.position + new Vector3(0, 0.5f, 0);
            Player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

            animator = activatedAt.GetComponent<Animator>();
            animatorTo = teleporters[3 * (pageNumber - 1)].GetComponent<Animator>();

            audiosource = activatedAt.GetComponent<AudioSource>();
            audiosourceTo = teleporters[3 * (pageNumber - 1)].GetComponent<AudioSource>();

            animator.SetTrigger("Teleport");
            animatorTo.SetTrigger("Teleport");

            audiosource.volume = GetComponent<teleporterscript>().teleportingVolume;
            audiosource.PlayOneShot(GetComponent<teleporterscript>().teleporting);
            audiosourceTo.volume = GetComponent<teleporterscript>().teleportingVolume;
            audiosourceTo.PlayOneShot(GetComponent<teleporterscript>().teleporting);

            TeleportCanvas.SetActive(false);

            gamemanager.instance.ResumeGame();
        }
    }

    public void TeleportToTwo()
    {
        if (teleporters[(3 * (pageNumber - 1) + 1)] != null)
        {
            Debug.Log(teleporters[(3 * (pageNumber - 1) + 1)]);

            Player.transform.position = teleporters[(3 * (pageNumber - 1) + 1)].transform.position + new Vector3(0, 0.5f, 0);
            Player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

            animator = activatedAt.GetComponent<Animator>();
            animatorTo = teleporters[3 * (pageNumber - 1)].GetComponent<Animator>();

            TeleportCanvas.SetActive(false);

            gamemanager.instance.ResumeGame();
        }
    }

    public void TeleportToThree()
    {
        if (teleporters[(3 * (pageNumber - 1) + 2)] != null)
        {
            Debug.Log(teleporters[(3 * (pageNumber - 1) + 2)]);

            Player.transform.position = teleporters[(3 * (pageNumber - 1) + 2)].transform.position + new Vector3(0, 0.5f, 0);
            Player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

            animator = activatedAt.GetComponent<Animator>();
            animatorTo = teleporters[3 * (pageNumber - 1)].GetComponent<Animator>();

            TeleportCanvas.SetActive(false);

            gamemanager.instance.ResumeGame();
        }
    }
}
