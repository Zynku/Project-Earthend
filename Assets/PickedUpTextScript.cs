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

    private void Start()
    {
        anim = GetComponent<Animator>();   
    }

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
        myText.text = (myItemAmount.ToString() + " x " + myItemName.ToString());
        anim.SetTrigger("Renew Anim");
    }

    public void SetHiAsInactive()
    {
        GetComponentInParent<InventoryUI>().isTextHiActive = false;
    }

    public void SetMidAsInactive()
    {
        GetComponentInParent<InventoryUI>().isTextMidActive = false;
    }

    public void SetLoAsInactive()
    {
        GetComponentInParent<InventoryUI>().isTextLoActive = false;
    }

    public IEnumerator Despawn()
    {
        GetComponentInParent<InventoryUI>().pickedUpTexts.Remove(gameObject);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
