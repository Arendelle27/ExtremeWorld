using Models;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestInfo:MonoBehaviour
{
    public Text title;

    public Text[] targets;

    public Text description;

    public Text overview;

    public UIIconItem rewardItems;

    public Text rewardMoney;
    public Text rewardExp;
    internal void SetQuestInfo(Quest quest)
    {
        this.title.text = string.Format("[{0}]{1}", quest.Define.Type, quest.Define.Name);
        if(this.overview!=null)
        {
            this.overview.text = quest.Define.Dialog;
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
            }
        }

        this.rewardMoney.text = quest.Define.RewardGold.ToString();
        this.rewardExp.text = quest.Define.RewardExp.ToString();

        foreach (var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();
        }
    }

        public void OnClickAbandon()
        { 
        }
}