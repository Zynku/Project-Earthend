using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleffectscript : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyThis(float despawnTime)
    {
        Destroy(this.gameObject);
    }
}
