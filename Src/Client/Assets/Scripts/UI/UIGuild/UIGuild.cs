using Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuild : UIWindow
{
    public GameObject itemPrefab;
    public ListView listMain;
    public Transform itemRoot;
    public UIGuildInfo uiInfo;
    public UIGuildItem selectedItem;
    void Start()
    {
        GuildService.Instance.OnGuildUpdate = UpdateUI;
        this.listMain.onItemSelected += this.OnGuildMemberSelected;
        this.UpdateUI();
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate = null;
    }

    void UpdateUI()
    {
        this.uiInfo.Info = GuildManager.Instance.guildInfo;
        ClearList();
        InitItems();
    }

    public void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildItem;
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
   
    }

    public void OnClickLeave()
    {

    }

    public void OnClickChat()
    {

    }

    public void OnClickKickout()
    {

    }

    public void OnClickPromote()
    {

    }

    public void OnClickDepose()
    {

    }

    public void OnClickTransfer()
    {

    }

    public void OnClickSetNotice()
    {

    }
}
