using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestFailedText : MonoBehaviour
{
    public GameObject myQuestName;
    public Quest myQuest;

    public QuestManager questManager;

    private void Start()
    {
        questManager = GetComponentInParent<QuestManager>();
        //if (!questAssigned) {myQuest = questManager.currentQuest; }             //Assigns its own quest
        //myQuestName.GetComponent<TextMeshProUGUI>().text = myQuest.name;        //Assigns it's own name
        //Can't get a quest reference because referencing a type (such as Quest) is LIVE, as opposed to referencing a variable (such as Quest.name)
        //that makes a copy. Because of this, if we reference Quest, it will always assign the current quest, even if it has changed since we instantiated this object, 
        //meaning it has the potential to show the wrong quest because it could have changed since this object was instantiated.
    }

    public void QuestFailedTextDisappear()
    {
        questManager.questFailedTexts.Remove(gameObject);
        Destroy(gameObject);
    }
}
