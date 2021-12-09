using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;

public class InfoHubManager : MonoBehaviour
{
    public IHPageScript[] pages;
    public int previousPageNumber;
    public GameObject pageToLeft;

    public int currentPageNumber;
    public GameObject currentPage;

    public int nextPageNumber;
    public GameObject pageToRight;

    public TextMeshProUGUI whatPageIsThis;           //The raw text that shows at the top of this menu showing what page is currently active.
    public GameObject iHPageButtonPrefab;
    public GameObject iHPageButtonHolder;
    [HideInInspector] public bool firstPageShown = false;

    private void Start()
    {
        this.gameObject.SetActive(false);
        pages = GetComponentsInChildren<IHPageScript>();
        currentPageNumber = 0;
        currentPage = pages[0].gameObject;

        for (int i = 0; i < pages.Length; i++)//Sets each page to its appropriate page number
        {
            pages[i].pageNumber = i;
        }

        for (int i = pages.Length; i-- > 0;)              //Creates a new button at the top of the IHUI for each page
        {
            var newButton = Instantiate(iHPageButtonPrefab, iHPageButtonHolder.transform);
            newButton.GetComponent<IHUIPageButton>().myPage = pages[i];
            newButton.GetComponent<IHUIPageButton>().myPageName.text = pages[i].pageName;
            newButton.GetComponent<IHUIPageButton>().shownSprite.GetComponent<Image>().sprite = pages[i].pageSprite;
        }

        previousPageNumber = -1;
        pageToLeft = null;

        nextPageNumber = 1;
        pageToRight = pages[1]?.gameObject;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.RightArrow)) { ScrollRight(); }  //TODO; Implement arrow scrolling

        currentPage = pages[currentPageNumber].gameObject;
        whatPageIsThis.text = currentPage.GetComponent<IHPageScript>().pageName;

        if (currentPageNumber > 0)
        {
            previousPageNumber = currentPageNumber - 1;
            pageToLeft = pages[previousPageNumber].gameObject;
        }
        else
        {
            previousPageNumber = 0;
            pageToLeft = null;
        }

        if (currentPageNumber < pages.Length - 1)
        {
            nextPageNumber = currentPageNumber + 1;
            pageToRight = pages[nextPageNumber].gameObject;
        }
        else
        {
            nextPageNumber = pages.Length -1;
            pageToRight = null;
        }

        pages[currentPageNumber].isCurrentPage = true;

        if (!firstPageShown)
        {
            currentPage.GetComponent<IHPageScript>().pagePanel.GetComponentInChildren<Animator>().Play("IH Page Idle");
            whatPageIsThis.text = currentPage.GetComponent<IHPageScript>().pageName;
            firstPageShown = true;
        }
    }

    [ButtonMethod]
    public void ScrollLeft()
    {
        if (currentPageNumber <= 0) //There are no more pages to the left, so don't bother scrolling
        {
            return;
        }
        else
        {
            currentPage.GetComponent<IHPageScript>().pagePanel.GetComponentInChildren<Animator>().Play("IH Page Slide to the right offscreen");
            pageToLeft.GetComponent<IHPageScript>().pagePanel.GetComponentInChildren<Animator>().Play("IH Page Slide to the right onscreen");
            currentPageNumber--;
        }
    }

    [ButtonMethod]
    public void ScrollRight()
    {
        int pagesArrayNumber = pages.Length - 1;
        if (currentPageNumber >= pagesArrayNumber)  //There are no more pages to the right, so don't bother scrolling
        {
            return;
        }
        else
        {
            currentPage.GetComponent<IHPageScript>().pagePanel.GetComponentInChildren<Animator>().Play("IH Page Slide to the left offscreen");
            pageToRight.GetComponent<IHPageScript>().pagePanel.GetComponentInChildren<Animator>().Play("IH Page Slide to the left onscreen");
            currentPageNumber++;
        }
    }

    public void SetPage(int pageNumber)
    {
        if (pageNumber == currentPageNumber)
        {
            return;
        }

        int randomNumber = Random.Range(0, 2);
        if (randomNumber == 0)
        {
            currentPage.GetComponent<IHPageScript>().pagePanel.GetComponentInChildren<Animator>().Play("IH Page Slide to the left offscreen");
            currentPageNumber = pageNumber;
            currentPage = pages[pageNumber].gameObject;
            currentPage.GetComponent<IHPageScript>().pagePanel.GetComponentInChildren<Animator>().Play("IH Page Slide to the right onscreen");
        }
        else
        {
            currentPage.GetComponent<IHPageScript>().pagePanel.GetComponentInChildren<Animator>().Play("IH Page Slide to the right offscreen");
            currentPageNumber = pageNumber;
            currentPage = pages[pageNumber].gameObject;
            currentPage.GetComponent<IHPageScript>().pagePanel.GetComponentInChildren<Animator>().Play("IH Page Slide to the left onscreen");
        }
    }
}


