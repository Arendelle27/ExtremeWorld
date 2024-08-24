using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestInfo:MonoBehaviour
{
    public Text title;

    public Text[] targets;

    public Text description;

    public Text overview;

    public List<Image> rewardItems;

    public Text rewardMoney;
    public Text rewardExp;

    public Button navButton;
    private int npc=0;
    internal void SetQuestInfo(Quest quest)
    {
        this.title.text = string.Format("[{0}]{1}", quest.Define.Type, quest.Define.Name);
        if(this.overview!=null)
        {
            this.overview.text = quest.Define.Overview;
        }

        if(this.description!=null)
        {
            if (quest.Info == null)
            {
                this.description.text = quest.Define.Dialog;
            }
            else
            {
                if (quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
                {
                    this.description.text = quest.Define.DialogFinish;
                }
                else
                {
                    this.description.text = quest.Define.DialogIncomplete;
                }
            }
        }

        this.rewardMoney.text = quest.Define.RewardGold.ToString();
        this.rewardExp.text = quest.Define.RewardExp.ToString();
        if(quest.Define.RewardItem1!=0)
        {
            this.rewardItems[0].sprite = Resloader.Load<Sprite>(DataManager.Instance.Items[quest.Define.RewardItem1].Icon);
        }
        if (quest.Define.RewardItem2 != 0)
        {
            this.rewardItems[1].sprite = Resloader.Load<Sprite>(DataManager.Instance.Items[quest.Define.RewardItem2].Icon);
        }
        if(quest.Define.RewardItem3!=0)
        {
            this.rewardItems[2].sprite = Resloader.Load<Sprite>(DataManager.Instance.Items[quest.Define.RewardItem3].Icon);
        }

        if(quest.Info==null)
        {
            this.npc = quest.Define.AcceptNPC;
        }
        else if(quest.Info.Status==QuestStatus.Complated)
        {
            this.npc=quest.Define.SubmitNPC;
        }

        if(this.navButton!=null)
        {
            this.navButton.gameObject.SetActive(this.npc > 0);
        }

        foreach (var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();
        }
    }

    public void OnClickAbandon()
    {

    }

    public void OnClickNav()
    {
        Vector3 pos = NpcManager.Instance.GetNpcPosition(this.npc);
        User.Instance.CurrentCharacterObject.StartNav(pos);
        UIManager.Instance.Close<UIQuestSystem>();
    }
}