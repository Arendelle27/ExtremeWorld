using Common.Battle;
using Entities;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Battle
{
    public class Skill
    {
        public NSkillInfo info;
        public Creature Owner;
        public SkillDefine Define;

        private float cd=0;
        private float castTime = 0;

        public bool IsCasting = false;
        private float skillTime;
        private int hit;

        public float CD
        {
            get { return cd; }
        }

        public NDamageInfo Damage { get; private set; }

        public Skill(NSkillInfo info, Creature owner)
        {
            this.info = info;
            this.Owner = owner;
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.Class][info.Id];
            this.cd = 0;
        }

        public SkILLRESULT CanCast(Creature target)
        {
            if(this.Define.CastTarget == TargetType.Target&&BattleManager.Instance.CurrentTarget==null)
            {
                //return SkillResult.InvalidTarget;
                if (target == null || target == this.Owner)
                {
                    return SkILLRESULT.InvalidTarget;
                }
                int distance = (int)Vector3.Distance(this.Owner.position, target.position);
                if (distance>this.Define.CastRange)
                {
                    return SkILLRESULT.OutOfRange;
                }


            }

            if (this.Define.CastTarget == TargetType.Position && BattleManager.Instance.CurrentPosition == null)
            {
                return SkILLRESULT.InvalidTarget;
            }

            if(this.Owner.Attributes.MP<this.Define.MPCost)
            {
                return SkILLRESULT.OutOfMp;
            }

            if(this.cd>0)
            {
                return SkILLRESULT.CoolDown;
            }

            return SkILLRESULT.Ok;
        }

        public void BeginCast(NDamageInfo damage)
        {
            this.IsCasting = true;
            this.hit = 0;
            this.castTime = 0;
            this.skillTime = 0;
            this.cd=this.Define.CD;
            this.Damage=damage;

            this.Owner.PlayAnim(this.Define.SkillAnim);
        }

        public void OnUpdate(float delta)
        {
            if(this.IsCasting)
            {
                this.skillTime += delta;
                if(this.skillTime>0.5f&&this.hit==0)
                {
                    this.OnHit();
                }
                if(this.skillTime>this.Define.CD)
                {
                    this.skillTime=0;
                }
            }
            UpdateCD(delta);
        }

        private void OnHit()
        {
            if(this.Damage!=null)
            {
                var cha=CharacterManager.Instance.GetCharacter(this.Damage.entityId);
                cha.DoDamage(this.Damage);
            }
            this.hit++;
        }

        private void UpdateCD(float delta)
        {
            if(this.cd>0)
            {
                this.cd-=delta;
            }
            if (cd < 0)
            {
                this.cd = 0;
            }
        }
    }
}
