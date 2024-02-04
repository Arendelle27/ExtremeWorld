using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIGUILD
{
    public class UIGuildApplyLIst : UIWindow
    {
        public GameObject itemPrefab;
        public ListView listMain;
        public Transform itemRoot;

        private void Start()
        {
            GuildService.Instance.OnGuildUpdate += UpdateList;
            GuildService.Instance.SendGuildListRequest();
            this.UpdateList();
        }

        private void OnDestroy()
        {
            GuildService.Instance.OnGuildUpdate -= UpdateList;
        }

        void UpdateList()
        {
            ClearList();
            InitItems();
        }

        /// <summary>
        /// 初始化所有的装备列表
        /// </summary>
        void InitItems()
        {
            foreach (var item in GuildManager.Instance.guildInfo.Applies)
            {
                GameObject go = Instantiate(itemPrefab, this.listMain.transform);
                UIGuildApplyItem ui = go.GetComponent<UIGuildApplyItem>();
                ui.SetItemInfo(item);
                this.listMain.AddItem(ui);
            }
        }

        void ClearList()
        {
            this.listMain.RemoveAll();
        }
    }
}
