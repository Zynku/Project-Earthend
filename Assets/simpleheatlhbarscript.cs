using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleheatlhbarscript : MonoBehaviour
{
    Enemymain enemymain;

    private GameObject healthbarFill;
    private GameObject stunnedFill;
    public float parentHealth;
    public float parentMaxHealth;
    public bool stunned;
    public float fillPercent = 1;
    public Enemyattachedto currentEnemy;

    // Start is called before the first frame update
    void Start()
    {
        enemymain = GetComponentInParent<Enemymain>();
        healthbarFill = transform.GetChild(0).gameObject;
        stunnedFill = transform.GetChild(1).gameObject;
        stunnedFill.SetActive(false);
        fillPercent = 1;

        if (transform.parent.CompareTag("enemy"))
        {
            currentEnemy = Enemyattachedto.Enemy;
        }

        if (transform.parent.CompareTag("target_dummy"))
        {
            currentEnemy = Enemyattachedto.Target_dummy;
        }
    }

    public enum Enemyattachedto
    {
        Enemy,
        Target_dummy
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentEnemy)
        {
            case Enemyattachedto.Enemy:
                parentMaxHealth = enemymain.maxHealth;
                parentHealth = enemymain.currentHealth;
                break;

            case Enemyattachedto.Target_dummy:
                //parentMaxHealth = GetComponentInParent<target_dummy_controller>().maxHealth;
                //parentHealth = GetComponentInParent<target_dummy_controller>().currentHealth;
                break;
        }

        //Ensures healthbar doesnt rotate with enemy.
        transform.localScale = transform.localScale;
        fillPercent = parentHealth / parentMaxHealth;
        if (fillPercent < 0) { fillPercent = 0f; }

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
