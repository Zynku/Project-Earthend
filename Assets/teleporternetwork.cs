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
        //Shows teleport menu, makes sure the event system can select the menu objects, and pauses game.
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
        //This functions assigns the first, second, and third teleporter names in the teleporter array to the teleporter menu.
        int q = 0;
        for (int i = tp; i < tp + 3; i++, q++)  //Loops through the first 3 names and assigns to the 3 name slots
        {
            if (i < teleporters.Length)
            {
                teleporterNames[q].text = teleporters[i].name.ToString();   //If there is a teleporter in the "i"th slot, it assigns the name
            }
            else
            {
                teleporterNames[q].text = "[No Teleporter Found]";          //If there are no teleporters in the "i" th slot, it says no teleporter found
            }
        }
    }

    public void NextPage()
    {
        //Increments page number, stops page numbers from overflowing or underflowing, reassigns teleporters based on the number teleporter we should
        //be starting at. This is done using some algebra and the page number to calculate the teleporter.
        pageNumber++;
        if (pageNumber < 1) { pageNumber = 1; }
        if (pageNumber > (teleporters.Length/3 + 1)) { pageNumber = teleporters.Length / 3 + 1; }
        startTeleporterNumber = (3 * (pageNumber - 1));
        AssignTeleporters(startTeleporterNumber);
    }

    public void PreviousPage()
    {
        //Does the same as above but decrements page number instead.
        pageNumber--;
        if (pageNumber < 1) { pageNumber = 1; }
        if (pageNumber > (teleporters.Length / 3 + 1)) { pageNumber = teleporters.Length / 3 + 1; }
        startTeleporterNumber = (3 * (pageNumber - 1));
        AssignTeleporters(startTeleporterNumber);
    }

    public void TeleportToOne()
    {
        //Grabs player transform and sets it to the teleporter transform with some offset so player does not clip into ground.
        //Grabs both teleporters' animators, audiosources, and audioclips and activates them all
        //Deactivates the teleporter menu, resumes game.
        Debug.Log(teleporters[3 * (pageNumber - 1)]);
        if (teleporters[3 * (pageNumber - 1)] != null)
        {
            Vector3 teleportoffset = new Vector3 (0, 0.5f, 0);
            Player.transform.position = teleporters[3 * (pageNumber - 1)].transform.position + teleportoffset;
            Player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

            animator = activatedAt.GetComponent<Animator>();
            animatorTo = teleporters[3 * (pageNumber - 1)].GetComponent<Animator>();

            audiosource = activatedAt.GetComponent<AudioSource>();
            audiosourceTo = teleporters[3 * (pageNumber - 1)].GetComponent<AudioSource>();

            /*animator.SetTrigger("Teleport");
            animatorTo.SetTrigger("Teleport");

            audiosource.volume = GetComponent<teleporterscript>().teleportingVolume;
            audiosource.PlayOneShot(GetComponent<teleporterscript>().teleporting);
            audiosourceTo.volume = GetComponent<teleporterscript>().teleportingVolume;
            audiosourceTo.PlayOneShot(GetComponent<teleporterscript>().teleporting);*/

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
