using Common.Utils;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildItem: ListView.ListViewItem
{
    public Text Id;
    public Text guildname;
    public Text memberNumber;
    public Text leaderName;

    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }

    public NGuildInfo Info { get; internal set; }

    public void SetGuildInfo(NGuildInfo item)
    {
        this.Info=item;
        if (this.Id != null) this.Id.text = item.Id.ToString();
        if (this.guildname != null) this.guildname.text = item.GuildName;
        if (this.memberNumber != null) this.memberNumber.text = item.memberCount.ToString();
        if (this.leaderName != null) this.leaderName.text = item.leaderName;
    }
}
