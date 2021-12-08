using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickedUpTextScript : MonoBehaviour
{
    public float aliveTimer = 0;
    public int myItemAmount;
    public string myItemName;
    public ItemScriptable myItem;
    public TextMeshProUGUI myText; //Assigned in Inspector
    Animator anim;

    private void Update()
    {
        aliveTimer += Time.deltaTime;
    }

    public void AssignNameandAmount(ItemScriptable item)
    {
        myItemAmount = item.amount;
        myItemName = item.name;
        myItem = item;
        myText.text = (myItemAmount.ToString() + " x " + myItemName.ToString());
    }

    public void AssignNewNameandAmount()
    {
        anim = GetComponent<Animator>();
        myText.text = (myItemAmount.ToString() + " x " + myItemName.ToString());
        anim.SetTrigger("Extend Anim");
        if (anim = null)
        {
            Debug.Log("No animator!");
        }
    }

    public void AssignNewNameandAmountOffScreen()
    {
        myText.text = (myItemAmount.ToString() + " x " + myItemName.ToString());
    }

    public void SetHiAsInactive()
    {
        if (GetComponentInParent<InventoryUI>() != null) { GetComponentInParent<InventoryUI>().isTextHiActive = false; }
        else if (GetComponentInParent<InventoryUIHelper>() != null) { GetComponentInParent<InventoryUIHelper>().isTextMidActive = false; }
    }

    public void SetMidAsInactive()
    {
        if (GetComponentInParent<InventoryUI>() != null) { GetComponentInParent<InventoryUI>().isTextMidActive = false; }
        else if (GetComponentInParent<InventoryUIHelper>() != null) { GetComponentInParent<InventoryUIHelper>().isTextMidActive = false; }
    }

    public void SetLoAsInactive()
    {
        if (GetComponentInParent<InventoryUI>() != null) { GetComponentInParent<InventoryUI>().isTextLoActive = false; }
        else if (GetComponentInParent<InventoryUIHelper>() != null) { GetComponentInParent<InventoryUIHelper>().isTextLoActive = false; }
    }

    public IEnumerator Despawn()
    {
        if (GetComponentInParent<InventoryUI>() != null) { GetComponentInParent<InventoryUI>().pickedUpTexts.Remove(gameObject); }
        else if (GetComponentInParent<InventoryUIHelper>() != null) { GetComponentInParent<InventoryUIHelper>().pickedUpTexts.Remove(gameObject); }
        yield return new WaitForSeconds(0.01f);
        Destroy(gameObject);
    }
}
