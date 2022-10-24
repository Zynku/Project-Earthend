using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MyBox;

public class ItemPickupable : MonoBehaviour
{
    public ItemScriptable item;
    [Tooltip("The collider that")]
    public Collider2D pickupTrigger;
    Charpickup_inventory inventory;

    public float beenAliveTime = 0f;
    public float canPickupTargetTime = 5f;
    public float despawnTargetTime = 15f;
    public bool canBePickedUp = false;
    TextMeshPro aliveTimer;
    public float radius = 3f;
    Animator anim;
    float despawnAnimTime;

    private void Start()
    {
        aliveTimer = GetComponentInChildren<TextMeshPro>();
        anim = GetComponentInParent<Animator>();
        inventory = GameManager.instance.Player.GetComponent<Charpickup_inventory>();

        UpdateAnimClipTimes();
    }


    public void UpdateAnimClipTimes()
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "Despawn Anim":
                    despawnAnimTime = clip.length;
                    break;
            }
        }
    }

    public void FixedUpdate()
    { 
        if (aliveTimer != null) aliveTimer.text = beenAliveTime.ToString();
        beenAliveTime += Time.fixedDeltaTime;

        if (beenAliveTime > canPickupTargetTime)
        {
            canBePickedUp = true;
        }

        if (beenAliveTime > (despawnTargetTime - despawnAnimTime))
        {
            anim.SetTrigger("Despawn");
        }
    }

    public void DestroyGameObject()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }

    public void Pickup()
    {
        if (canBePickedUp)
        {
            bool wasPickedUp = inventory.AddItem(item);

            if (wasPickedUp)
            {
                Destroy(gameObject.transform.parent.gameObject);
            }
        }
    }
}
