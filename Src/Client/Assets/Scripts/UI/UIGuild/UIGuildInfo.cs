using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildInfo : MonoBehaviour
{
    public Text guildName;
    public Text guildID;
    public Text leader;

    public Text notice;

    public Text memberNumber;

    private NGuildInfo info;
    public NGuildInfo Info
    {
        get { return this.info; }
        set { this.info = value; this.UpdateUI(); }
    }

    private void UpdateUI()
    {
        if (this.info == null)
        {
            this.guildName.text = "ÔÝÎÞ";
            this.guildID.text = "0";
            this.leader.text = "ÔÝÎÞ";
            this.notice.text = "ÔÝÎÞ";
            this.memberNumber.text =string.Format( "0/{0}",GameDefine.GuildMaxMemberCount);
        }
        else
        {
            this.guildName.text = this.info.GuildName;
            this.guildID.text = this.info.Id.ToString();
            this.leader.text = this.info.leaderName;
            this.notice.text = this.info.Notice;
            this.memberNumber.text = string.Format("{0}/{1}", this.info.Members.Count, GameDefine.GuildMaxMemberCount);
        }
    }
}
