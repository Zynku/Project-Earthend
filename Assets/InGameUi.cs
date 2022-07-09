using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUi : MonoBehaviour
{
    [Header("Player")]
    public GameObject Player;

    [Header("On Screen Variables")]
    public GameObject mainCanvas;
    public GameObject moneyCounter;
    public GameObject levelCounter;
    public Healthbar healthBarOver;
    public Healthbar healthBarUnder;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameManager.instance.Player;
        //Player.GetComponent<Charhealth>().healthbarOver = healthBarOver;
    }

    // Update is called once per frame
    void Update()
    {
        moneyCounter.GetComponent<TMPro.TextMeshProUGUI>().text = Player.GetComponent<Charpickup_inventory>().money.ToString();
        levelCounter.GetComponent<TMPro.TextMeshProUGUI>().text = Player.GetComponent<Charhealth>().level.ToString();
    }
}
