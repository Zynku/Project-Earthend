using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;

public class InfoHubManager : MonoBehaviour
{
    public IHPageScript[] pages;
    public int currentPageNumber;
    public GameObject currentPage;
    public GameObject pageToLeft;
    public GameObject pageToRight;

    private void Start()
    {
        pages = GetComponentsInChildren<IHPageScript>();
        currentPage = pages[0].gameObject;
        for (int i = 0; i < pages.Length; i++)              //Sets each page to its appropriate page number
        {
            pages[i].pageNumber = i;

            if(i != 0)
            {
                pages[i].pagePanel.GetComponentInChildren<Animator>().Play("IH Page Idle OffScreen Right");
            }
        }
    }

    private void Update()
    {
        currentPageNumber = currentPage.GetComponent<IHPageScript>().pageNumber;
    }

    public void ScrollLeft()
    {
        currentPage.GetComponent<IHPageScript>().pagePanel.GetComponentInChildren<Animator>().Play("IH Page Slide to the left");
        if (pageToRight != null) { pageToRight.GetComponent<IHPageScript>().pagePanel.GetComponentInChildren<Animator>().Play("IH Page Slide to the left"); }
        int nextPageNumber = --currentPage.GetComponent<IHPageScript>().pageNumber;
        if (nextPageNumber < 0)
        {
            nextPageNumber = 0;
            currentPage = pages[nextPageNumber].gameObject;
            pages[nextPageNumber].isCurrentPage = true;
        }
        else if (nextPageNumber > pages.Length - 1)
        {
            nextPageNumber = pages.Length - 1;
            currentPage = pages[nextPageNumber].gameObject;
            pages[nextPageNumber].isCurrentPage = true;
        }
        else
        {
            currentPage = pages[nextPageNumber].gameObject;
            pages[nextPageNumber].isCurrentPage = true;
        }
        
    }

    public void ScrollRight()
    {
        currentPage.GetComponent<IHPageScript>().pagePanel.GetComponentInChildren<Animator>().Play("IH Page Slide to the right");
        if (pageToLeft != null) { pageToLeft.GetComponent<IHPageScript>().pagePanel.GetComponentInChildren<Animator>().Play("IH Page Slide to the right"); }
        int nextPageNumber = ++currentPage.GetComponent<IHPageScript>().pageNumber;
        if (nextPageNumber < 0)
        {
            nextPageNumber = 0;
            currentPage = pages[nextPageNumber].gameObject;
        }
        else
        {
            currentPage = pages[nextPageNumber].gameObject;
        }
    }
}

