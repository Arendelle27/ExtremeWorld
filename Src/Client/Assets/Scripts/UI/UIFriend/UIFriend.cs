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
        InputBox.Show("�������������", "��Ӻ���").OnSubmit += OnFriendAddSubmit;
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
        if(friendId==User.Instance.CurrentCharacter.Id||friendName==User.Instance.CurrentCharacter.Name)
        {
            tips = "����Ц���𣿲�������Լ�Ϊ����Ŷ";
            return false;
        }
        FriendService.Instance.SendFriendAddRequest(friendId, friendName);
        return true;
    }

    public void OnClicFriendChat()
    {
        MessageBox.Show("��δ����");
    }

    public void OnClickFriendRemove()
    {
        if(selectedItem==null)
        {
            MessageBox.Show("��ѡ��Ҫɾ���ĺ���");
            return;
        }
        MessageBox.Show(string.Format("ȷ��Ҫɾ������{0}��", selectedItem.Info.friendInfo.Name), "ɾ������", MessageBoxType.Confirm, "ɾ��", "ȡ��").OnYes = () =>
        {
            FriendService.Instance.SendFriendRemoveRequest(this.selectedItem.Info.Id, this.selectedItem.Info.friendInfo.Id);
        };
    }

    private void RefreshUI()
    {
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
