using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickedUpTextScript : MonoBehaviour
{
    public float aliveTimer = 0;
    public int myItemAmount;
    public string myItemName;
    public TextMeshProUGUI myText;

    private void Start()
    {
        myText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        aliveTimer += Time.deltaTime;
    }

    public void AssignNameandAmount(ItemScriptable item)
    {
        Debug.Log("Name is " + item.name + " and amount is " + item.amount);
        myItemAmount = item.amount;
        myItemName = item.name;
        myText.text = (myItemAmount + "x" + myItemName);
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
        GetComponentInParent<InventoryUI>().pickedUpTexts.Remove(gameObject.GetComponent<GameObject>());
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
