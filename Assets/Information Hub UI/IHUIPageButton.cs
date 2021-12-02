using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;

public class IHUIPageButton : MonoBehaviour
{
    InfoHubManager infoHub;
    [InitializationField] public IHPageScript myPage;
    [InitializationField] public GameObject shownSprite;
    [ReadOnly] public TextMeshProUGUI myPageName;

    void Start()
    {
        infoHub = GetComponentInParent<InfoHubManager>(); 
    }


    public void ChangeCurrentPageToThis()
    {
        infoHub.SetPage(myPage.pageNumber);
    }
}
