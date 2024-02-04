using Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIGUILD
{
    public class UIGuild : UIWindow
    {
        public GameObject itemPrefab;
        public ListView listMain;
        public Transform itemRoot;
        public UIGuildInfo uiInfo;
        public UIGuildMemberItem selectedItem;

        public GameObject panelAdmin;
        public GameObject panelLeader;
        void Start()
        {
            GuildService.Instance.OnGuildUpdate = UpdateUI;
            this.listMain.onItemSelected += this.OnGuildMemberSelected;
            this.UpdateUI();
        }

        private void OnDestroy()
        {
            GuildService.Instance.OnGuildUpdate -= UpdateUI;
        }

        void UpdateUI()
        {
            this.uiInfo.Info = GuildManager.Instance.guildInfo;
            ClearList();
            InitItems();

            this.panelAdmin.SetActive(GuildManager.Instance.myMemberInfo.Title > GuildTitle.None);
            this.panelLeader.SetActive(GuildManager.Instance.myMemberInfo.Title == GuildTitle.President);
        }

        public void OnGuildMemberSelected(ListView.ListViewItem item)
        {
            this.selectedItem = item as UIGuildMemberItem;
        }

        /// <summary>
        /// ��ʼ�������Ա�б�
        /// </summary>
        /// <param name="guilds"></param>
        void InitItems()
        {
            foreach (var item in GuildManager.Instance.guildInfo.Members)
            {
                GameObject go = Instantiate(itemPrefab, this.listMain.transform);
                UIGuildMemberItem ui = go.GetComponent<UIGuildMemberItem>();
                ui.SetGuildMemberInfo(item);
                this.listMain.AddItem(ui);
            }
        }

        void ClearList()
        {
            this.listMain.RemoveAll();
        }

        public void OnClickAppliesList()
        {
            UIManager.Instance.Show<UIGuildApplyLIst>();
        }

        public void OnClickLeave()
        {
            GuildService.Instance.SendGuildLeaveRequest();
        }

        public void OnClickChat()
        {

        }

        public void OnClickKickout()
        {
            if (selectedItem == null)
            {
                MessageBox.Show("��ѡ��Ҫ�߳��ĳ�Ա");
                return;
            }
            MessageBox.Show(string.Format("Ҫ�ߡ�{0}����������", this.selectedItem.Info.Info.Name), "�߳�����", MessageBoxType.Confirm, "��", "ȡ��").OnYes = () => {
                GuildService.Instance.SendAdminCommand(GuildAdminCommand.Kickout, this.selectedItem.Info.Info.Id);
            };
        }

        public void OnClickPromote()
        {
            if (selectedItem == null)
            {
                MessageBox.Show("��ѡ��Ҫ�����ĳ�Ա");
                return;
            }
            if (this.selectedItem.Info.Title != GuildTitle.None)
            {
                MessageBox.Show("�Է��Ѿ�������ͨ��Ա��");
                return;
            }
            MessageBox.Show(string.Format("Ҫ������{0}��Ϊ���᳤��", this.selectedItem.Info.Info.Name), "�������᳤", MessageBoxType.Confirm, "����", "ȡ��").OnYes = () => {
                GuildService.Instance.SendAdminCommand(GuildAdminCommand.Promote, this.selectedItem.Info.Info.Id);
            };
        }

        public void OnClickDepose()
        {
            if (selectedItem == null)
            {
                MessageBox.Show("��ѡ��Ҫ����ĳ�Ա");
            }
            if (selectedItem.Info.Title == GuildTitle.None)
            {
                MessageBox.Show("�Է��Ѿ�����ͨ��Ա��");
                return;
            }
            if (selectedItem.Info.Title == GuildTitle.President)
            {
                MessageBox.Show("�᳤���ܱ�����");
                return;
            }
            MessageBox.Show(string.Format("Ҫ���⡾{0}����ְ����", this.selectedItem.Info.Info.Name), "����ְ��", MessageBoxType.Confirm, "����", "ȡ��").OnYes = () =>
            {
                GuildService.Instance.SendAdminCommand(GuildAdminCommand.Depost, this.selectedItem.Info.Info.Id);
            };
        }

        public void OnClickTransfer()
        {
            if (selectedItem == null)
            {
                MessageBox.Show("��ѡ��Ҫת�õĳ�Ա");
                return;
            }
            MessageBox.Show(string.Format("Ҫ���᳤ת�ø���{0}����", this.selectedItem.Info.Info.Name), "ת�û᳤", MessageBoxType.Confirm, "ת��", "ȡ��").OnYes = () =>
            {
                GuildService.Instance.SendAdminCommand(GuildAdminCommand.Transfer, this.selectedItem.Info.Info.Id);
            };
        }

        public void OnClickSetNotice()
        {
            MessageBox.Show("��չ��ҵ");
        }
    }
}

