using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heartscript : MonoBehaviour
{
    AudioSource audiosource;
    [SerializeField] AudioClip Jingle;

    public float despawnTimer;
    public float heartValue = 2;

    // Start is called before the first frame update
    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        if (Jingle != null) audiosource.PlayOneShot(Jingle);
        despawnTimer += Random.Range(-3f, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        if (despawnTimer <= 0)
        {
            Destroy(gameObject);
            Destroy(transform.parent.gameObject);
        }
    }

    private void FixedUpdate()
    {
        despawnTimer -= Time.fixedDeltaTime;
    }
}
