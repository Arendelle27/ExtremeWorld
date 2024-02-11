using Common;
using Common.Data;
using Common.Utils;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class  ChatManager:Singleton<ChatManager>
    {
        public List<ChatMessage> System = new List<ChatMessage>();//系统
        public List<ChatMessage> World = new List<ChatMessage>();//世界
        public Dictionary<int, List<ChatMessage>> Local = new Dictionary<int, List<ChatMessage>>();//本地
        public Dictionary<int, List<ChatMessage>> Team = new Dictionary<int, List<ChatMessage>>();//队伍
        public Dictionary<int, List<ChatMessage>> Guild = new Dictionary<int, List<ChatMessage>>();//公会

        public void Init()
        {

        }

        public void AddMessage(Character form,ChatMessage message)
        {
            message.FromId = form.Id;
            message.FromName = form.Name;
            message.Time=TimeUtil.timestamp;
            switch(message.Channel)
            {
                case ChatChannel.Local:
                    this.AddLocalMessage(form.Info.mapId, message);
                    break;
                case ChatChannel.World:
                    this.AddWorldMessage( message);
                    break;
                case ChatChannel.System:
                    this.AddSystemMessage(message);
                    break;
                case ChatChannel.Team:
                    this.AddTeamMessage(form.Team.Id,message);
                    break;
                case ChatChannel.Guild:
                    this.AddGuildMessage(form.Guild.Id, message);
                    break;
            }
        }
        
        public void AddLocalMessage(int mapId,ChatMessage message)
        {
            if (!this.Local.TryGetValue(mapId,out List<ChatMessage> messages))
            {
                messages=new List<ChatMessage>();
                this.Local[mapId]=messages;
            }
            messages.Add(message);
        }

        private void AddSystemMessage(ChatMessage message)
        {
            this.System.Add(message);
        }

        private void AddWorldMessage(ChatMessage message)
        {
            this.World.Add(message);
        }

        void AddGuildMessage(int id,ChatMessage message)
        {
            if (!this.Guild.TryGetValue(id,out List<ChatMessage> messages))
            {
                messages=new List<ChatMessage>();
                this.Guild[id]=messages;
            }
            messages.Add(message);
        }

        private void AddTeamMessage(int id,ChatMessage message)
        {
            if (!this.Team.TryGetValue(id,out List<ChatMessage> messages))
            {
                messages=new List<ChatMessage>();
                this.Team[id]=messages;
            }
            messages.Add(message);
        }

        public int GetLocalMessages(int mapId,int idx,List<ChatMessage> result)
        {
            if(!this.Local.TryGetValue(mapId,out List<ChatMessage> messages))
            {
                return 0;
            }
            return GetNewMessages(idx,result,messages);
        }

        public int GetWorldMessages(int idx,List<ChatMessage> result)
        {
            return GetNewMessages(idx,result,this.World);
        }

        public int GetSystemMessages(int idx,List<ChatMessage> result)
        {
            return GetNewMessages(idx,result,this.System);
        }

        public int GetTeamMessages(int id,int idx,List<ChatMessage> result)
        {
            if(!this.Team.TryGetValue(id,out List<ChatMessage> messages))
            {
                return 0;
            }
            return GetNewMessages(idx,result,messages);
        }

        public int GetGuildMessages(int id,int idx,List<ChatMessage> result)
        {
            if(!this.Guild.TryGetValue(id,out List<ChatMessage> messages))
            {
                return 0;
            }
            return GetNewMessages(idx,result,messages);
        }

        int GetNewMessages(int idx,List<ChatMessage> result,List<ChatMessage> messages)
        {
            if(idx==0)
            {
                if(messages.Count>GameDefine.MaxChatRecoredNums)
                {
                    idx=messages.Count-GameDefine.MaxChatRecoredNums;
                }
            }

            for(; idx<messages.Count;idx++)
            {
                result.Add(messages[idx]);
            }
            return idx;
        }
    }

}
