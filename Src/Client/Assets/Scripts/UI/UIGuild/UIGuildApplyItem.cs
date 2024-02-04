using Common.Utils;
using Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIGUILD
{
    public class UIGuildApplyItem : ListView.ListViewItem
    {
        public Text nickname;
        public Text @class;
        public Text level;

        public NGuildApplyInfo Info;

        public void SetItemInfo(NGuildApplyInfo item)
        {
            this.Info = item;
            if (this.nickname != null) this.nickname.text = item.Name;
            if (this.@class != null) this.@class.text = item.Class.ToString();
            if (this.level != null) this.level.text = item.Level.ToString();
        }

        public void OnAccept()
        {
            MessageBox.Show(string.Format("要通过【{0}】的入会申请吗？", this.Info.Name), "入会申请", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
            {
                GuildService.Instance.SendGuildJoinApply(true,this.Info);
            };
        }

        public void OnDecline()
        {
            MessageBox.Show(string.Format("要拒绝【{0}】的入会申请吗？", this.Info.Name), "入会申请", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
            {
                GuildService.Instance.SendGuildJoinApply(false, this.Info);
            };
        }
    }
}
