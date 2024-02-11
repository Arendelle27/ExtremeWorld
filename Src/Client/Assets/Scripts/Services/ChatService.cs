using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Services
{
    class ChatService : Singleton<ChatService>, IDisposable
    {
        public void Init()
        {

        }

        public ChatService()
        {
            MessageDistributer.Instance.Subscribe<ChatResponse>(this.OnChatResponse);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ChatResponse>(this.OnChatResponse);
        }

        /// <summary>
        /// 发送聊天消息
        /// </summary>
        /// <param name="chatChannel"></param>
        /// <param name="content"></param>
        /// <param name="toid"></param>
        /// <param name="toName"></param>
        public void SendChat(ChatChannel chatChannel,string content,int toid,string toName)
        {
            Debug.Log("SendChat");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.Chat = new ChatRequest();
            message.Request.Chat.Message = new ChatMessage();
            message.Request.Chat.Message.Channel = chatChannel;
            message.Request.Chat.Message.Message = content;
            message.Request.Chat.Message.ToId = toid;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 聊天消息响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnChatResponse(object sender, ChatResponse response)
        {
            Debug.LogFormat("OnChatResponse:{0}", response.Result);
            if(response.Result==Result.Success)
            {
                if (response.localMessages != null)
                {
                    ChatManager.Instance.AddMessage(ChatChannel.Local, response.localMessages);
                }

                if (response.worldMessages != null)
                {
                    ChatManager.Instance.AddMessage(ChatChannel.World, response.worldMessages);
                }

                if (response.systemMessages != null)
                {
                    ChatManager.Instance.AddMessage(ChatChannel.System, response.systemMessages);
                }

                if (response.guildMessages != null)
                {
                    ChatManager.Instance.AddMessage(ChatChannel.Guild, response.guildMessages);
                }

                if (response.teamMessages != null)
                {
                    ChatManager.Instance.AddMessage(ChatChannel.Team, response.teamMessages);
                }

                if (response.privateMessages != null)
                {
                    ChatManager.Instance.AddMessage(ChatChannel.Private, response.privateMessages);
                }
            }
            else
            {
                MessageBox.Show(response.Errormsg, "");
            }

            
        }
    }
}
