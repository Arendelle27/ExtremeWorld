﻿using Common.Battle;
using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    class Skill
    {
        public NSkillInfo info;
        public Creature Owner;
        public SkillDefine Define;

        private float cd = 0;
        public float CD
        {
            get { return cd; }
        }

        public Skill(NSkillInfo info, Creature owner)
        {
            this.info = info;
            this.Owner = owner;
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.Class][info.Id];
        }

        internal SkILLRESULT Cast(BattleContext context)
        {
            SkILLRESULT result = SkILLRESULT.Ok;
            if(this.cd>0)
            {
                return SkILLRESULT.CoolDown;
            }

            if(context.Target!=null)
            {
                this.DoSkillDamage(context);
            }

            this.cd=this.Define.CD;
            return result;
        }

        internal void Update()
        {
            this.UpdateCD();
        }

        private void UpdateCD()
        {
            if(this.cd>0)
            {
                this.cd -= Time.deltaTime;
            }
            if(this.cd<0)
            {
                this.cd = 0;
            }
        }

        private void DoSkillDamage(BattleContext context)
        {
            context.Damage = new NDamageInfo();
            context.Damage.entityId = context.Target.entityId;
            context.Damage.Damage = 100;
            context.Target.DoDamage(context.Damage);
        }
    }
}
