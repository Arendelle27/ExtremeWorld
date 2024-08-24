using Common.Battle;
using Common.Data;
using Entities;
using SkillBridge.Message;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Models
{
    class User : Singleton<User>
    {
        SkillBridge.Message.NUserInfo userInfo;


        public SkillBridge.Message.NUserInfo Info
        {
            get { return userInfo; }
        }


        public void SetupUserInfo(SkillBridge.Message.NUserInfo info)
        {
            this.userInfo = info;
        }

        public MapDefine CurrentMapData { get; set; }

        public Character CurrentCharacter;
        public NCharacterInfo CurrentCharacterInfo { get; set; }

        public PlayerInputController CurrentCharacterObject { get; set; }

        public NTeamInfo TeamInfo { get; set; }

        public void AddGold(int gold)
        {
            this.CurrentCharacterInfo.Gold += gold;
        }

        public void AddLevel(int level)
        {
            this.CurrentCharacterInfo.Level += level;
            Attributes attribute = this.CurrentCharacter.Attributes;
            this.CurrentCharacter.Attributes.Init(this.CurrentCharacterInfo.attrDynamic,this.CurrentCharacter.Define,this.CurrentCharacterInfo.Level,null);
        }

        public int CurrentRide = 0;

        public void Ride(int id)
        {
            CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride, 0);
            if (this.CurrentRide != id)
            {
                CurrentRide = id;
                CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride, CurrentRide);
            }
            else
            {
                CurrentRide = 0;
                //CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride, CurrentRide);
            }
        }

        public delegate void CharacterInitHandler();
        public event CharacterInitHandler OnCharacterInit;

        internal void CharacterInited()
        {
            if(OnCharacterInit != null)
            {
                OnCharacterInit();
            }
        }
    }
}
