using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinscript : MonoBehaviour
{
    AudioSource audiosource;
    [SerializeField] AudioClip Jingle;

    public float despawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        audiosource.PlayOneShot(Jingle);
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
    }

    private void FixedUpdate()
    {
        despawnTimer -= Time.fixedDeltaTime;
    }
}
