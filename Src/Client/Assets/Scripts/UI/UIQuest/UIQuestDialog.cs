using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuestDialog: UIWindow
{
    public UIQuestInfo questInfo;

    public Quest quest;

    public GameObject openButton;
    public GameObject submitButton;

    public void SetQueset(Quest quest)
    {
        this.quest = quest;
        this.UpdateQuest();
        if (this.quest.Info == null)
        {
            this.openButton.SetActive(true);
            this.submitButton.SetActive(false);
        }
        else
        {
            if (this.quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
            {
                this.openButton.SetActive(false);
                this.submitButton.SetActive(true);
            }
            else
            {
                this.openButton.SetActive(false);
                this.submitButton.SetActive(false);
            }
        }
    }

    void UpdateQuest()
    {
        if (this.quest != null)
        {
            if (this.questInfo != null)
            {
                this.questInfo.SetQuestInfo(quest);
            }
        }
    }
}
