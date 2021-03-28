using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleheatlhbarscript : MonoBehaviour
{
    private GameObject healthbarFill;
    private GameObject stunnedFill;
    public float parentHealth;
    public float parentMaxHealth;
    public bool stunned;
    public float fillPercent = 100;

    // Start is called before the first frame update
    void Start()
    {
        if ( GetComponentInParent<enemy_controller>().currentState == enemy_controller.State.Stunned)
        {
            stunned = true;
        }
        else
        {
            stunned = false;
        }
        healthbarFill = transform.GetChild(0).gameObject;
        stunnedFill = transform.GetChild(1).gameObject;
        stunnedFill.SetActive(false);
    }


    private void Update()
    {
        transform.localScale = transform.localScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        parentMaxHealth = GetComponentInParent<Enemyhealth>().maxHealth;
        parentHealth = GetComponentInParent<Enemyhealth>().currentHealth;

        fillPercent = 1 * parentHealth / parentMaxHealth;

        //Changes x scale based on fill percent
        healthbarFill.transform.localScale = new Vector3(fillPercent, 1, 1);

        //Sets stun bar to true if parent is stunned
        if (stunned == true){ stunnedFill.SetActive(true); }
        else { stunnedFill.SetActive(false); }

        //Stops overflow and underflow
        if (fillPercent > 1) { fillPercent = 1; }
        if (fillPercent < 0) { fillPercent = 0; }

        
    }
}
