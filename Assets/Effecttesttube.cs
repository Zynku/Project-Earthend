using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effecttesttube : MonoBehaviour
{
    public tubeType tubetype;
    SpriteRenderer sprite;

    public enum tubeType
    {
        Poison,
        Freeze,
        Fire,
        Weaken,
        Strengthen,
        Slow
    }

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (tubetype)
        {
            case tubeType.Poison:
                //Sets sprite renderer color to green
                sprite.color = new Color(0.66f, 1f, 0.41f);
                break;

            case tubeType.Freeze:
                sprite.color = new Color(0.41f, 1f, 0.97f);
                break;

            case tubeType.Fire:
                sprite.color = new Color(1f, 0.46f, 0.41f);
                break;

            case tubeType.Weaken:
                sprite.color = new Color(1f, 0.97f, 0.41f);
                break;

            case tubeType.Strengthen:
                sprite.color = new Color(0.61f, 0.41f, 1f);
                break;

            case tubeType.Slow:
                sprite.color = new Color(0.31f, 0.31f, 0.31f);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || (collision.CompareTag("Ground")))
        {
            Explode();
        }
    }
    
    void Explode()
    {
        GameObject go;
        go = Instantiate(Resources.Load("Sprites/Status effects/Gas Cloud") as GameObject, transform.position, Quaternion.identity);

        switch (tubetype)
        {
            case tubeType.Poison:
                go.GetComponent<Gascloudscript>().CloudType = Gascloudscript.cloudType.Poison;
                break;
            case tubeType.Freeze:
                go.GetComponent<Gascloudscript>().CloudType = Gascloudscript.cloudType.Freeze;
                break;
            case tubeType.Fire:
                go.GetComponent<Gascloudscript>().CloudType = Gascloudscript.cloudType.Fire;
                break;
            case tubeType.Weaken:
                go.GetComponent<Gascloudscript>().CloudType = Gascloudscript.cloudType.Weaken;
                break;
            case tubeType.Strengthen:
                go.GetComponent<Gascloudscript>().CloudType = Gascloudscript.cloudType.Strengthen;
                break;
            case tubeType.Slow:
                go.GetComponent<Gascloudscript>().CloudType = Gascloudscript.cloudType.Slow;
                break;
        }
                
        
        Destroy(this.gameObject);
    }
}
