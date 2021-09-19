using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways, ExecuteInEditMode]
public class QuestSystemCleanerUpper : MonoBehaviour
{
    GameObject[] questObjects;
    private bool spritesEnabled;

    public void ToggleQuestObjectSprites()
    {
        questObjects = GameObject.FindGameObjectsWithTag("QuestObject");
        if (!spritesEnabled)
        {
            EnableQuestObjectSprites();
            spritesEnabled = true;
        }
        else
        {
            DisableQuestObjectSprites();
        }
    }

    void EnableQuestObjectSprites()
    {
        foreach (GameObject questobject in questObjects)
        {
            if (questobject.GetComponent<SpriteRenderer>())
            {
                if (questobject.GetComponent<SpriteRenderer>().enabled == false)
                {
                    questobject.GetComponent<SpriteRenderer>().enabled = true;
                    spritesEnabled = true;
                }
            }
        }
    }


    void DisableQuestObjectSprites()
    {
        foreach (GameObject questobject in questObjects)
        {
            if (questobject.GetComponent<SpriteRenderer>())
            {
                if (questobject.GetComponent<SpriteRenderer>().enabled == true)
                {
                    questobject.GetComponent<SpriteRenderer>().enabled = false;
                    spritesEnabled = false;
                }
            }
        }
    }
}
