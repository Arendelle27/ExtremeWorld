using Candlelight.UI;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UICHAT
{
    public class UIChat : MonoBehaviour
    {
        public HyperText textArea;//聊天内容显示区域

        public TabView channelTab;

        public InputField chatText;//聊天输入框
        public Text chatTarget;

        public Dropdown channelSelect;
        void Start()
        {
            this.channelTab.OnTabSelect += OnDisplayChannelSelected;
            ChatManager.Instance.OnChat += RefreshUI;
        }

        void OnDestory()
        {
            ChatManager.Instance.OnChat -= RefreshUI;
        }

        void Update()
        {
            InputManager.Instance.isInputMode = chatText.isFocused;
        }

        void OnDisplayChannelSelected(int idx)
        {
            ChatManager.Instance.displayChannel = (ChatManager.LocalChannel)idx;
            RefreshUI();
        }

        public void RefreshUI()
        {
            this.textArea.text = ChatManager.Instance.GetCurrentMessages();
            this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
            if(ChatManager.Instance.SendChannel == SkillBridge.Message.ChatChannel.Private)
            {
                this.chatTarget.gameObject.SetActive(true);
                if(ChatManager.Instance.PrivateID!=0)
                {
                    this.chatTarget.text = ChatManager.Instance.PrivateName+":";
                }
                else
                {
                    this.chatTarget.text = "<无>";
                }
            }
            else
            {
                this.chatTarget.gameObject.SetActive(false);
            }
        }

        public void OnClickChatLink(HyperText text,HyperText.LinkInfo link)
        {
            if(string.IsNullOrEmpty(link.Name))
                return;

            if(link.Name.StartsWith("c:"))
            {
                string[] strs = link.Name.Split(":".ToCharArray());
                UIPopChatMenu menu=UIManager.Instance.Show<UIPopChatMenu>();
                menu.targetId = int.Parse(strs[1]);
                menu.targetName = strs[2];
            }
        }

        public void OnClickSend()
        {
            OnEndInput(this.chatText.text);
        }

        public void OnEndInput(string text)
        {
            if(!string.IsNullOrEmpty(text.Trim())) 
            {
                this.SendChat(text);
            }
            this.chatText.text = ""; 
        }

        void SendChat(string content)
        {
            ChatManager.Instance.SendChat(content,ChatManager.Instance.PrivateID, ChatManager.Instance.PrivateName);
        }

        public void OnSendChannelChanaged()
        {
            if (ChatManager.Instance.sendChannel == (ChatManager.LocalChannel)(channelSelect.value + 1))
                return;

            if(!ChatManager.Instance.SetSendChannel((ChatManager.LocalChannel)(channelSelect.value + 1)))
            {
                this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
            }
            else
            {
                this.RefreshUI();
            }
        }
    }
}
