using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onscreenText : MonoBehaviour
{
    [Header("Player")]
    public GameObject Player;

    [Header("On Screen Variables")]
    public GameObject moneyCounter;
    public GameObject levelCounter;
    public Healthbar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        healthBar = GetComponentInChildren<Healthbar>();

        Player.GetComponent<Charhealth>().healthbar = healthBar;
    }

    // Update is called once per frame
    void Update()
    {
        moneyCounter.GetComponent<TMPro.TextMeshProUGUI>().text = Player.GetComponent<Charpickup_inventory>().money.ToString();
        levelCounter.GetComponent<TMPro.TextMeshProUGUI>().text = Player.GetComponent<Charhealth>().level.ToString();
    }
}
