using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gascloudscript : MonoBehaviour
{
    public cloudType CloudType;
    SpriteRenderer sprite;

    [Header("Poison")]
    public float poisonTime = 5;
    public int poisonDamage = 5;

    public enum cloudType
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
        switch (CloudType)
        {
            case cloudType.Poison:
                //Sets sprite renderer color to green
                sprite.color = new Color(0.66f, 1f, 0.41f, 0.7f);
                break;

            case cloudType.Freeze:
                sprite.color = new Color(0.41f, 1f, 0.97f, 0.7f);
                break;

            case cloudType.Fire:
                sprite.color = new Color(1f, 0.46f, 0.41f, 0.7f);
                break;

            case cloudType.Weaken:
                sprite.color = new Color(1f, 0.97f, 0.41f, 0.7f);
                break;

            case cloudType.Strengthen:
                sprite.color = new Color(0.61f, 0.41f, 1f, 0.7f);
                break;

            case cloudType.Slow:
                sprite.color = new Color(0.31f, 0.31f, 0.31f, 0.7f);
                break;
        }
    }

    void Evaporate()
    {
        Destroy(this.gameObject);
    }
}
