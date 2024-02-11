using Managers;
using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UICHAT
{
    public class UIPopChatMenu : UIWindow, IDeselectHandler
    {
        public int targetId;

        public string targetName;

        public void OnDeselect(BaseEventData eventData)
        {
            var ed = eventData as PointerEventData;
            if (ed.hovered.Contains(this.gameObject))
            {
                return;
            }
            this.Close(WindowResult.None);
        }

        public void OnEnable()
        {
            this.GetComponent<Selectable>().Select();
            this.root.transform.position = Input.mousePosition + new Vector3(80, 0, 0);
        }

        public void OnChat()
        {
            ChatManager.Instance.StartPrivateChat(this.targetId, this.targetName);
            this.Close(WindowResult.No);
        }

        public void OnAddFriend()
        {
            for (int i = 0; i < FriendManager.Instance.allFriends.Count; i++)
            {
                if (FriendManager.Instance.allFriends[i].Id == this.targetId)
                {
                    MessageBox.Show("对方已经是你的好友了");
                    this.Close(WindowResult.No);
                }
            }
            FriendService.Instance.SendFriendAddRequest(this.targetId, this.targetName);
            this.Close(WindowResult.No);
        }

        public void OnInviteTeam()
        {
            if(User.Instance.TeamInfo!=null)
            {
                for(int i=0;i< User.Instance.TeamInfo.Members.Count;i++)
                {
                    if (User.Instance.TeamInfo.Members[i].Id== this.targetId)
                    {
                        MessageBox.Show("对方已经在你的队伍中了");
                        this.Close(WindowResult.No);
                    }
                }
            }
            TeamService.Instance.SendTeamInviteRequest(this.targetId, this.targetName);
            this.Close(WindowResult.No);
        }
    }
}
