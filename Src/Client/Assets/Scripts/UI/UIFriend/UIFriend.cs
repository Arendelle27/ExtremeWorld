using Managers;
using Models;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class UIFriend : UIWindow
{
    public GameObject itemPrefab;
    public ListView listMain;
    public Transform itemRoot;
    public UIFriendItem selectedItem;

    void Start()
    {
        FriendService.Instance.OnFriendUpdate = RefreshUI;
        this.listMain.onItemSelected += this.OnFriendSelected;
        RefreshUI();
    }

    private void OnFriendSelected(ListView.ListViewItem item)
    {
        this.selectedItem=item as UIFriendItem;
    }

    public void OnClickFriendAdd()
    {
        InputBox.Show("请输入好友名称", "添加好友").OnSubmit += OnFriendAddSubmit;
    }

    private bool OnFriendAddSubmit(string input,out string tips)
    {
        tips = "";
        int friendId = 0;
        string friendName = "";
        if(!int.TryParse(input.ToString(),out friendId))
        {
            friendName = input;
        }
        if(friendId==User.Instance.CurrentCharacterInfo.Id||friendName==User.Instance.CurrentCharacterInfo.Name)
        {
            tips = "开玩笑的吗？不能添加自己为好友哦";
            return false;
        }
        FriendService.Instance.SendFriendAddRequest(friendId, friendName);
        return true;
    }

    public void OnClicFriendChat()
    {
        MessageBox.Show("暂未开放");
    }

    /// <summary>
    /// 发送组队邀请
    /// </summary>
    public void OnClickFriendTeamInvite()
    {
        if(selectedItem==null)
        {
            MessageBox.Show("请选择要邀请的好友");
            return;
        }
        if(selectedItem.Info.Status==0)
        {
            MessageBox.Show("请选择在线的好友");
            return;
        }
        MessageBox.Show(string.Format("确定要邀请好友{0}加入队伍吗？", selectedItem.Info.friendInfo.Name), "邀请好友", MessageBoxType.Confirm, "邀请", "取消").OnYes = () =>
        {
            TeamService.Instance.SendTeamInviteRequest(this.selectedItem.Info.friendInfo.Id,this.selectedItem.Info.friendInfo.Name);
        };
    }

    public void OnClickFriendChallenge()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要挑战的好友");
            return;
        }
        if (selectedItem.Info.Status == 0)
        {
            MessageBox.Show("请选择在线的好友");
            return;
        }
        MessageBox.Show(string.Format("确定要与好友{0}进行竞技场挑战吗？", selectedItem.Info.friendInfo.Name), "竞技场挑战", MessageBoxType.Confirm, "邀请", "取消").OnYes = () =>
        {
            ArenaService.Instance.SendArenaChallengeRequest(this.selectedItem.Info.friendInfo.Id, this.selectedItem.Info.friendInfo.Name);
        };
    }

    /// <summary>
    /// 删除好友
    /// </summary>
    public void OnClickFriendRemove()
    {
        if(selectedItem==null)
        {
            MessageBox.Show("请选择要删除的好友");
            return;
        }
        MessageBox.Show(string.Format("确定要删除好友{0}吗？", selectedItem.Info.friendInfo.Name), "删除好友", MessageBoxType.Confirm, "删除", "取消").OnYes = () =>
        {
            FriendService.Instance.SendFriendRemoveRequest(this.selectedItem.Info.Id, this.selectedItem.Info.friendInfo.Id);
        };
    }

    private void RefreshUI()
    {
        Debug.Log("RefreshUI");
        ClearFriendList();
        InitFriendItems();
    }

    private void InitFriendItems()
    {
        foreach(var item in FriendManager.Instance.allFriends)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIFriendItem ui = go.GetComponent<UIFriendItem>();
            ui.SetFriendInof(item);
            this.listMain.AddItem(ui);
        }
    }

    private void ClearFriendList()
    {
        this.listMain.RemoveAll();
    }

}
