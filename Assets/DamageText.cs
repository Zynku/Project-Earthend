using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    Rigidbody2D rb2d;
    public float destroyTime = 1f;
    public float floatiness = -0.05f;
    public Vector2 floatDir;
    private float time;
    private float opacity;


    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();

        rb2d.gravityScale = floatiness;

        Destroy(gameObject, destroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        opacity = Mathf.Lerp(255, 0, time / destroyTime);
        GetComponent<TMPro.TextMeshPro>().faceColor = new Color32(255, 255, 255 , (byte)opacity);
    }
}
