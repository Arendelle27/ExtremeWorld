using Managers;
using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using UIRIDE;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain>{

    public Text avatarName;
    public Text avaterLevel;

    public UITeam TeamWindow;
	protected override void OnStart () {
        this.UpdateAvater();
	}
	
    void UpdateAvater()
    {
        this.avatarName.text = string.Format("{0}[{1}]", User.Instance.CurrentCharacter.Name, User.Instance.CurrentCharacter.Id);
        this.avaterLevel.text = User.Instance.CurrentCharacter.Level.ToString();
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

    }


    public void ShowTeamUI(bool show)
    {
        TeamWindow.ShowTeam(show);
    }

}
