using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinscript : MonoBehaviour
{
    GameObject Player;

    AudioSource audiosource;
    [SerializeField] AudioClip Jingle;

    public float despawnTimer;
    public int coinValue = 1;
    public float attractRadius = 1;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        audiosource = GetComponent<AudioSource>();
        if (Jingle != null) audiosource.PlayOneShot(Jingle);
        despawnTimer += Random.Range(-3f, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        if (despawnTimer <= 0 )
        {
            Destroy(gameObject);
            Destroy(transform.parent.gameObject);
        }

        if (Vector3.Distance(Player.transform.position, transform.parent.position) < attractRadius)
        {
            Vector3.MoveTowards(transform.parent.position, Player.transform.position,attractRadius);
        }
    }

    private void FixedUpdate()
    {
        despawnTimer -= Time.fixedDeltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attractRadius);
    }
}
