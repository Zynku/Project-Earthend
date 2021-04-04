using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breakablestaticobject : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    public bool hit;
    public GameObject brokeObj;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hit = GetComponentInChildren<Hitbyplayer>().hit;
        maxHealth = GetComponentInChildren<Hitbyplayer>().maxHealth;
        currentHealth = GetComponentInChildren<Hitbyplayer>().currentHealth;

        if (currentHealth <= 0)
        {
            onDeactivate();
        }
    }

    private void onDeactivate()
    {
        GameObject clone;
        clone = Instantiate(brokeObj, transform.position, transform.rotation);
        clone.transform.localScale = transform.localScale;

        Destroy(gameObject);
    }
}
