using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    Rigidbody2D rb2d;
    public float destroyTime = 3f;
    public float floatiness = -0.05f;
    public Vector2 floatDir;
    private float opacity;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        //Floatiness is set in Rigidbody grav scale
        rb2d.AddForce(floatDir, ForceMode2D.Impulse);
        rb2d.gravityScale = floatiness;
        Destroy(gameObject, destroyTime);
        
    }

    // Update is called once per frame
    void Update()
    {
        opacity = Mathf.Lerp(0, 255, destroyTime -= Time.deltaTime);
        GetComponent<TMPro.TextMeshPro>().faceColor = new Color32(255, 128, 0, (byte)opacity);
    }
}
