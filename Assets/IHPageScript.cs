using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IHPageScript : MonoBehaviour
{
    [Header("Assign these in Inspector!")]
    public GameObject pageCanvas;
    public GameObject pagePanel;
    public int pageNumber;
    public bool isCurrentPage;

    private void Start()
    {
        
    }

    public void DisableCanvas()
    {
        pageCanvas.SetActive(false);
    }

    public void EnableCanvas()
    {
        pageCanvas.SetActive(true);
    }
}
