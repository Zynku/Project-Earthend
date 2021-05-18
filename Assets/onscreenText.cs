using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onscreenText : MonoBehaviour
{
    [Header("Player")]
    public GameObject[] Player;

    [Header("On Screen Variables")]
    public GameObject moneyCounter;
    public GameObject levelCounter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Player = GameObject.FindGameObjectsWithTag("Player");
        moneyCounter.GetComponent<TMPro.TextMeshProUGUI>().text = Player[0].GetComponent<Charpickup_inventory>().money.ToString();
        levelCounter.GetComponent<TMPro.TextMeshProUGUI>().text = Player[0].GetComponent<Charhealth>().level.ToString();
    }
}
