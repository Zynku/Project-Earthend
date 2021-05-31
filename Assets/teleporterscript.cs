using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleporterscript : MonoBehaviour
{
    public GameObject Player;
    public teleporternetwork Network;
    public float teleportRange;
    public bool DebugDistance;

    [Range(0f, 1f)]
    public float teleportingVolume = 1;
    public AudioClip teleporting;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        Network = GetComponentInParent<teleporternetwork>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Vector3.Distance(Player.transform.position, transform.position) < teleportRange))
        {
            if (Input.GetButtonDown("Interact"))
            {
                Network.showNetworkUI();
                Network.activatedAt = gameObject;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, teleportRange);
    }
}
