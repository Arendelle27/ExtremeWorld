using Common.Utils;
using SkillBridge.Message;
using System;
using UnityEngine;
using UnityEngine.UI;


namespace UIGUILD
{
    public class UIGuildMemberItem : ListView.ListViewItem
    {
        public Text nickname;
        public Text @class;
        public Text level;
        public Text title;
        public Text joinTime;
        public Text status;

        public Image background;
        public Sprite normalBg;
        public Sprite selectedBg;

        public override void onSelected(bool selected)
        {
            this.background.overrideSprite = selected ? selectedBg : normalBg;
        }

        public NGuildMemberInfo Info;

        public void SetGuildMemberInfo(NGuildMemberInfo item)
        {
            this.Info = item;
            if (this.nickname != null) this.nickname.text = item.Info.Name;
            if (this.@class != null) this.@class.text = item.Info.Class.ToString();
            if (this.level != null) this.level.text = item.Info.Level.ToString();
            if (this.title != null) this.title.text = item.Title.ToString();
            if (this.joinTime != null) this.joinTime.text = TimeUtil.GetTime(this.Info.joinTime).ToShortDateString();
            if (this.status != null) this.status.text = this.Info.Status == 1 ? "在线" : TimeUtil.GetTime(this.Info.lastTime).ToShortDateString();
        }
    }
}