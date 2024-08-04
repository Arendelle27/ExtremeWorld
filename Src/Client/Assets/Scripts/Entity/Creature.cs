﻿using Battle;
using Common.Battle;
using Common.Data;
using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class Creature : Entity
    {
        public NCharacterInfo Info;

        public CharacterDefine Define;

        public Attributes Attributes;

        public SkillManager SkillMgr;
        public BuffManager BuffMgr;
        public EffectManager EffectMgr;

        public Action<Buff> OnBuffAdd;
        public Action<Buff> OnBuffRemove;

        bool battleState = false;
        public bool BattleState
        {
            get { return battleState; }
            set
            {
                if(battleState!=value)
                {
                    battleState = value;
                    this.SetStandby(value);
                }
            }
        }

        public int Id
        {
            get { return this.Info.Id; }
        }

        public Skill CastringSkill = null;

        public string Name
        {
            get
            {
                if (this.Info.Type == CharacterType.Player)
                    return this.Info.Name;
                else
                    return this.Define.Name;
            }
        }

        public bool IsPlayer
        {
            get { return this.Info.Type == CharacterType.Player; }
        }

        public bool IsCurrentPlayer
        {
            get 
            {
                if(!IsPlayer) return false;
                return this.Info.Id == User.Instance.CurrentCharacterInfo.Id;
            }
        }

        public Creature(NCharacterInfo info) : base(info.Entity)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Characters[info.ConfigId];
            this.Attributes = new Attributes();

            this.Attributes.Init(this.Define, this.Info.Level,GetEquips(),this.Info.attrDynamic);
            this.SkillMgr = new SkillManager(this);
            this.BuffMgr = new BuffManager(this);
            this.EffectMgr = new EffectManager(this);
        }

        public virtual List<EquipDefine> GetEquips()
        {
            return null;
        }

        internal void UpdateInfo(NCharacterInfo info)
        {
            this.SetEntityData(info.Entity);
            this.Info = info;
            this.Attributes.Init(this.Define, this.Info.Level, GetEquips(), this.Info.attrDynamic);
            this.SkillMgr.UpdateSkills();
        }

        public void MoveForward()
        {
            //Debug.LogFormat("MoveForward");
            this.speed = this.Define.Speed;
        }

        public void MoveBack()
        {
            //Debug.LogFormat("MoveBack");
            this.speed = -this.Define.Speed;
        }

        public void Stop()
        {
            //Debug.LogFormat("Stop");
            this.speed = 0;
        }

        public void SetDirection(Vector3Int direction)
        {
            //Debug.LogFormat("SetDirection:{0}", direction);
            this.direction = direction;
        }

        public void SetPosition(Vector3Int position)
        {
            //Debug.LogFormat("SetPosition:{0}", position);
            this.position = position;
        }

        public void CastSkill(int skillId,Creature target,NVector3 pos)
        {
            this.SetStandby(true);
            var skill=this.SkillMgr.GetSkill(skillId);
            skill.BeginCast(target,pos);
        }

        public void PlayAnim(string anim)
        {
            if(this.Controller!=null)
            {
                this.Controller.PlayAnim(anim);
            }
        }

        public void SetStandby(bool standby)
        {
            if(this.Controller!=null)
            {
                this.Controller.SetStandby(standby);
            }
        }

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
            this.SkillMgr.OnUpdate(delta);
            this.BuffMgr.OnUpdate(delta);
        }

        public void DoDamage(NDamageInfo damage)
        {
            Debug.LogFormat("DoDamage:{0} DMG:{1} CRIT:{2}", this.Name,damage.Damage,damage.Crit);
            this.Attributes.HP -= damage.Damage;
            this.PlayAnim("Hurt");
        }

        internal void DoSkillHit(NSkillHitInfo hit)
        {
            var skill=this.SkillMgr.GetSkill(hit.skillId);
            skill.DoHit(hit);
        }

        internal int Distance(Creature target)
        {
            return (int)Vector3.Distance(this.position, target.position);
        }

        internal void DoBuffAction(NBuffInfo buff)
        {
            switch(buff.Action)
            {
                case BuffAction.Add:
                    this.AddBuff(buff.buffId, buff.buffType, buff.casterId);
                    break;
                case BuffAction.Remove:
                    this.RemoveBuff(buff.buffId);
                    break;
                case BuffAction Hit:
                    this.DoDamage(buff.Damage);
                    break;
                default:
                    break;
            }
        }

        private void AddBuff(int buffId, int buffType, int casterId)
        {
            var buff= this.BuffMgr.AddBuff(buffId, buffType,casterId);
            if(buff!=null&&this.OnBuffAdd!=null)
            {
                Debug.LogFormat("AddBuff:{0}:{1}[{2}]", buff.BuffId, buff.Define.Name, buff.Define.Effect);
                this.OnBuffAdd(buff);
            }
        }

        public void RemoveBuff(int buffId)
        {
            var buff=this.BuffMgr.RemoveBuff(buffId);
            if(buff!=null&&this.OnBuffRemove!=null)
            {
                this.OnBuffRemove(buff);
            }
        }

        internal void AddBuffEffect(BuffEffect effect)
        {
            this.EffectMgr.AddEffect(effect);
        }

        internal void RemoveBuffEffect(BuffEffect effect)
        {
            this.EffectMgr.RemoveEffect(effect);
        }

        internal void FaceTo(Vector3Int position)
        {
            this.SetDirection(GameObjectTool.WorldToLogic(GameObjectTool.LogicToWorld(position-this.position).normalized));
            this.UpdateEntityData();
            if(this.Controller!=null)
            {
                this.Controller.UpdateDirection();
            }
        }

        private void UpdateEntityData()
        {

        }

        internal void PlayEffect(EffectType type, string name, Entity target, float duration)
        {
            if(string.IsNullOrEmpty(name))
            {
                return;
            }
            if(this.Controller!=null)
            {
                this.Controller.PlayEffect(type, name, target, duration);
            }
        }
    }
}
