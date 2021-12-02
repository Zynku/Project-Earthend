using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class IHPageScript : MonoBehaviour
{
    [Header("Assign these in Inspector!")]
    public GameObject pageCanvas;
    public GameObject pagePanel;
    [ReadOnly] public int pageNumber;
    public string pageName;
    public bool isCurrentPage;
    public Sprite pageSprite;

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
