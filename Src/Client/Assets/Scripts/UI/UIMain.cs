using Entities;
using Managers;
using Models;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UIRIDE;
using UISKILL;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain>{

    public Text avatarName;
    public Text avaterLevel;

    public UITeam TeamWindow;

    public UICreatureInfo targetUI;
	protected override void OnStart () {
        this.UpdateAvater();
        this.targetUI.gameObject.SetActive(false);
        BattleManager.Instance.OnTargetChanged += OnTargetChanged;
	}

    void UpdateAvater()
    {
        this.avatarName.text = string.Format("{0}[{1}]", User.Instance.CurrentCharacterInfo.Name, User.Instance.CurrentCharacterInfo.Id);
        this.avaterLevel.text = User.Instance.CurrentCharacterInfo.Level.ToString();
    }

	// Update is called once per frame
	void Update () {
		
	}


    public void OnClickBag()
    {
        UIManager.Instance.Show<UIBag>();
    }

    public void OnClickCharEquip()
    {
        UIManager.Instance.Show<UICharEquip>();
    }

    public void OnClickQuestSystem()
    {
        UIManager.Instance.Show<UIQuestSystem>();
    }

    public void OnClickFirends()
    {
        UIManager.Instance.Show<UIFriend>();
    }

    public void OnClickGuild()
    {
        GuildManager.Instance.ShowGuild();
    }

    public void OnClickRide()
    {
        UIManager.Instance.Show<UIRide>();
    }

    public void OnClickSetting()
    {
        UIManager.Instance.Show<UISetting>();
    }

    public void OnClickSkill()
    {
        UIManager.Instance.Show<UISkill>();
    }


    public void ShowTeamUI(bool show)
    {
        TeamWindow.ShowTeam(show);
    }

    private void OnTargetChanged(Creature target)
    {
        if(target!=null)
        {
            if(!targetUI.isActiveAndEnabled)
            { 
                targetUI.gameObject.SetActive(true);
            }
            targetUI.Target = target;
        }
        else
        {
            targetUI.gameObject.SetActive(false);
        }
    }
}
