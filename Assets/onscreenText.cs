using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onscreenText : MonoBehaviour
{
    [Header("Player")]
    public GameObject Player;

    [Header("On Screen Variables")]
    public GameObject moneyCounter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moneyCounter.GetComponent<TMPro.TextMeshProUGUI>().text = Player.GetComponent<Charpickup_inventory>().money.ToString();
    }
}
