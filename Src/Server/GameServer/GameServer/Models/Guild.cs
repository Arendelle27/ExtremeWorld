﻿using Common;
using Common.Utils;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Guild
    {
        public int Id { get { return this.Data.Id; } }
        public Character Leader;
        public string Name { get { return this.Data.Name; } }
        public List<Character> Members = new List<Character>();

        public double timestamp;
        public TGuild Data;

        public Guild(TGuild guild)
        {
            this.Data = guild;
        }

        public bool JoinApply(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(a => a.CharacterId == apply.characterId);
            if(oldApply!=null)
            {
                return false;
            }

            var dbApply=DBService.Instance.Entities.GuildApplies.Create();
            dbApply.GuildId = apply.GuildId;
            dbApply.CharacterId = apply.characterId;
            dbApply.Class = apply.Class;
            dbApply.Level = apply.Level;
            dbApply.Name = apply.Name;
            dbApply.ApplyTime = DateTime.Now;

            DBService.Instance.Entities.GuildApplies.Add(dbApply);
            this.Data.Applies.Add(dbApply);

            DBService.Instance.Save();

            this.timestamp = Time.timestamp;
            return true;
        }
        
        public bool JoinAppove(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(a => a.CharacterId == apply.characterId&&a.Result==0);
            if(oldApply==null)
            {
                return false;
            }

            oldApply.Result = (int)apply.Result;

            if(apply.Result==ApplyResult.Accept)
            {
                this.AddMember(apply.characterId,apply.Name,apply.Class,apply.Level,GuildTitle.None);
            }

            DBService.Instance.Save();

            this.timestamp = Time.timestamp;
            return true;
        }

        public void AddMember(int id,string name,int @class,int level,GuildTitle title)
        {
            DateTime now = DateTime.Now;
            TGuildMember dbMember = new TGuildMember()
            {
                CharacterId = id,
                Name = name,
                Class = @class,
                Level = level,
                Title = (int)title,
                JoinTime = now,
                LastTime = now
            };
            this.Data.Members.Add(dbMember);
            timestamp = Time.timestamp;
        }

        public void Leave(Character member)
        {
            Log.InfoFormat("Leave Guild : {0}:{1}", member.Id, member.Info.Name);
            this.Members.Remove(member);
            if(member==this.Leader)
            {
                if(this.Members.Count>0)
                {
                    this.Leader = this.Members[0];
                }
                else
                {
                    this.Leader = null;
                }
            }
            member.Guild = null;
            timestamp= TimeUtil.timestamp;
        }

        public void PostProcess(Character from,NetMessageResponse message)
        {
            if(message.Guild==null)
            {
                message.Guild = new GuildResponse();
                message.Guild.Result = Result.Success;
                message.Guild.guildInfo = this.GuildInfo(from);
            }
        }

        public NGuildInfo GuildInfo(Character from)
        {
            NGuildInfo info = new NGuildInfo()
            {
                Id=this.Id,
                GuildName=this.Name,
                Notice=this.Data.Notice,
                leaderId=this.Data.LeaderID,
                leaderName=this.Data.LeaderName,
                createTime=(long)TimeUtil.GetTimestamp(this.Data.CreateTime),
                memberCount=this.Data.Members.Count,
            };

            if(from!=null)
            {
                info.Members.AddRange(GetMemberInfos());
                if(from.Id==this.Data.LeaderID)
                {
                    info.Applies.AddRange(GetApplyInfos());
                }
            }
            return info;
        }

        List<NGuildMemberInfo> GetMemberInfos()
        {
            List<NGuildMemberInfo> members = new List<NGuildMemberInfo>();

            foreach (var member in this.Data.Members)
            {
                var memberInfo = new NGuildMemberInfo
                {
                    Id = member.Id,
                    characterId=member.CharacterId,
                    Title = (GuildTitle)member.Title,
                    joinTime = (long)TimeUtil.GetTimestamp(member.JoinTime),
                    lastTime = (long)TimeUtil.GetTimestamp(member.LastTime),
                };

                var character = CharacterManager.Instance.GetCharacter(member.CharacterId);
                if (character != null) 
                {
                    memberInfo.Info=character.GetBasicInfo();
                    memberInfo.Status = 1;
                    if(member.Id==this.Data.LeaderID)
                    {
                        this.Leader = character;
                    }
                }
                else
                {
                    memberInfo.Info=this.GetMemberInfo(member);
                    memberInfo.Status = 0;
                    if(member.Id==this.Data.LeaderID)
                    {
                        this.Leader = null;
                    }
                }
                members.Add(memberInfo);
            }
            return members;
        }

        NCharacterInfo GetMemberInfo(TGuildMember member)
        {
            return new NCharacterInfo()
            {
                Id = member.CharacterId,
                Name = member.Name,
                Class = (CharacterClass)member.Class,
                Level = member.Level,
            };
        }

        List<NGuildApplyInfo> GetApplyInfos()
        { 
            List<NGuildApplyInfo> applies=new List<NGuildApplyInfo>();
            foreach(var apply in this.Data.Applies)
            {
                applies.Add(new NGuildApplyInfo()
                {
                    characterId=apply.CharacterId,
                    GuildId=apply.GuildId,
                    Name=apply.Name,
                    Class=apply.Class,
                    Level=apply.Level,
                    Result = (ApplyResult)apply.Result,
                });
            }
            return applies;
        }
    }
}
