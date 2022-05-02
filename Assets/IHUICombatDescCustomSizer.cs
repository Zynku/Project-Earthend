using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class IHUICombatDescCustomSizer : MonoBehaviour
{
    
    public GameObject largestChild;

    public void Start()
    {
        
    }
    // Update is called once per frame
    public void Update()
    {
        largestChild = FindLargestChild();
        FindUpperAndLowerBounds();
    }

    public GameObject FindLargestChild()
    {
        
        GameObject largestObj = null;
        Transform[] allChildren = GetComponentsInChildren<Transform>(); //Get all children. Strangely this includes this gameObject
        List<Transform> allChildrenList = new List<Transform>(allChildren); //Convert to a list first
        allChildrenList.RemoveAt(0);    //Removes the first result since it is always this gameObject
/*        foreach (var item in allChildrenList)
        {
            Debug.Log($"{item.name}");
        }*/
        if (largestObj == null)
        {
            largestObj = allChildrenList[0].gameObject;
        }

        RectTransform largestRect = null;
        if (largestObj != null)
        {
            largestRect = largestObj.GetComponent<RectTransform>();
        }

        foreach (var childObj in allChildrenList)
        {
            RectTransform childRect = childObj.GetComponent<RectTransform>();
            if (childRect.rect.size.x > largestRect.rect.size.x && childRect.rect.size.y > largestRect.rect.size.y)
            {
                largestObj = childObj.gameObject;
            }
        }
        return largestObj;
    }

    public Vector2 FindUpperAndLowerBounds()    //No Idea if this works
    {
        Vector2 leftbounds = new Vector2(999f, 999f);
        Vector2 rightbounds = new Vector2(0f,0f);
        Transform[] allChildren = GetComponentsInChildren<Transform>(); //Get all children. Strangely this includes this gameObject
        List<Transform> allChildrenList = new List<Transform>(allChildren); //Convert to a list first
        allChildrenList.RemoveAt(0);    //Removes the first result since it is always this gameObject
        foreach (var item in allChildrenList)
        {
            Debug.Log($"{item.name}");
        }

        foreach (var childObj in allChildrenList)
        {
            RectTransform childRect = childObj.GetComponent<RectTransform>();
            if (childRect.rect.xMin < leftbounds.x)
            {
                leftbounds.x = childRect.rect.xMin;
            }

            if (childRect.rect.xMax > rightbounds.x)
            {
                rightbounds.x = childRect.rect.xMax;
            }

            if (childRect.rect.yMin < leftbounds.y)
            {
                leftbounds.y = childRect.rect.yMin;
            }

            if (childRect.rect.yMax > rightbounds.y)
            {
                rightbounds.y = childRect.rect.yMax;
            }
        }
        Debug.Log($"{new Vector2(leftbounds.x, rightbounds.y)}");
        return new Vector2(leftbounds.x, rightbounds.y);
    }
}
