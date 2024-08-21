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
        public Creature Target;
        public SkillDefine Define;

        private float castTime = 0;

        public bool IsCasting = false;
        private float skillTime=0;
        public int Hit=0;
        private SkillStatus Status;

        private float cd = 0;
        public float CD
        {
            get { return cd; }
        }

        Dictionary<int, List<NDamageInfo>> HitMap = new Dictionary<int, List<NDamageInfo>>();

        List<Bullet> Bullets=new List<Bullet>();
        private NVector3 TargetPosition;

        public Skill(NSkillInfo info, Creature owner)
        {
            this.info = info;
            this.Owner = owner;
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.TID][info.Id];
            this.cd = 0;
        }

        public SkILLRESULT CanCast(Creature target)
        {
            if(this.Define.CastTarget == TargetType.Target&&BattleManager.Instance.CurrentTarget==null)
            {
                if (target == null || target == this.Owner)
                {
                    return SkILLRESULT.InvalidTarget;
                }
                int distance = this.Owner.Distance(target);
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

        public void BeginCast(Creature target,NVector3 pos)
        {
            this.IsCasting = true;
            this.Hit = 0;
            this.castTime = 0;
            this.skillTime = 0;
            this.cd=this.Define.CD;
            this.Target = target;
            this.TargetPosition = pos;
            this.Bullets.Clear();
            this.HitMap.Clear();

            this.Owner.PlayAnim(this.Define.SkillAnim);

            if(this.Define.CastTarget==Common.Battle.TargetType.Position)
            {
                this.Owner.FaceTo(this.TargetPosition.ToVector3Int());
            }
            else if(this.Define.CastTarget==Common.Battle.TargetType.Target)
            {
                this.Owner.FaceTo(this.Target.position);
            }

            if(this.Define.CastTime>0)
            {
                this.Status=SkillStatus.Casting;
            }
            else
            {
                this.StartSkill();
            }
        }

        /// <summary>
        /// 技能执行开始
        /// </summary>
        private void StartSkill()
        {
            this.Status = SkillStatus.Running;
            if (!string.IsNullOrEmpty(this.Define.AOEEffect))
            {
                if (this.Define.CastTarget == TargetType.Position)
                {
                    this.Owner.PlayEffect(EffectType.Position, this.Define.AOEEffect, this.TargetPosition);
                }
                else if (this.Define.CastTarget == TargetType.Target)
                {
                    this.Owner.PlayEffect(EffectType.Position, this.Define.AOEEffect, this.Target);
                }
                else if (this.Define.CastTarget == TargetType.Self)
                {
                    this.Owner.PlayEffect(EffectType.Position, this.Define.AOEEffect, this.Owner);
                }
            }
        }

        public void OnUpdate(float delta)
        {
            UpdateCD(delta);
            if(this.Status==SkillStatus.Casting)
            {
                this.UpdateCasting();
            }
            else if(this.Status==SkillStatus.Running)
            {
                this.UpdateSkill();
            }
        }

        void UpdateCasting()
        {
            if(this.castTime<this.Define.CastTime)
            {
                this.castTime+=Time.deltaTime;
            }
            else
            {
                this.castTime = 0;
                this.StartSkill();
                Debug.LogFormat("Skill[{0}].UpdateCasting Finish", this.Define.Name);
            }
        }

        void UpdateSkill()
        {
            this.skillTime+=Time.deltaTime;
            if (this.Define.Duration > 0)
            {
                //持续技能
                if (this.skillTime > this.Define.Interval * (this.Hit + 1))
                {
                    this.DoHit();
                }

                if (this.skillTime > this.Define.Duration)
                {
                    this.Status = SkillStatus.None;
                    this.IsCasting = false;
                    Debug.LogFormat("Skill[{0}].UpdateSkill Finish", this.Define.Name);
                }
            }
            else if (this.Define.HitTimes != null && this.Define.HitTimes.Count > 0)
            {
                if (this.Hit < this.Define.HitTimes.Count)
                {
                    if (this.skillTime > this.Define.HitTimes[this.Hit])
                    {
                        this.DoHit();
                    }
                }
                else
                {
                    if (!this.Define.Bullet)
                    {
                        this.Status = SkillStatus.None;
                        this.IsCasting = false;
                        Debug.LogFormat("Skill[{0}].UpdateSkill Finish", this.Define.Name);
                    }
                }
            }
            if(this.Define.Bullet)
            {
                bool finish = true;
                foreach(Bullet bullet in this.Bullets)
                {
                    bullet.Update();
                    if(!bullet.Stoped)
                    {
                        finish = false;
                    }
                }
                if(finish&&this.Hit>=this.Define.HitTimes.Count)
                {
                    this.Status = SkillStatus.None;
                    this.IsCasting = false;
                    Debug.LogFormat("Skill[{0}].UpdateSkill Finish", this.Define.Name);
                }
            }
        }

        void DoHit()
        {
            if(this.Define.Bullet)
            {
                this.CastBullet();
            }
            else
            {
                this.DoHitDamages(this.Hit);
            }
            this.Hit++;
        }

        public void DoHitDamages(int hit)
        {
            List<NDamageInfo> damages;
            if (this.HitMap.TryGetValue(hit, out damages))
            {
                DoHitDamages(damages);
            }
        }

        private void CastBullet()
        {
            Bullet bullet = new Bullet(this);
            Debug.LogFormat("Skill[{0}].CastBullet[{1}] Target[{2}]", this.Define.Name, this.Hit, this.Target);
            this.Bullets.Add(bullet);
            this.Owner.PlayEffect(EffectType.Bullet, this.Define.BulletResource, this.Target, bullet.duration);
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

        internal void DoHit(NSkillHitInfo hit)
        {
            if(hit.isBullet||!this.Define.Bullet)
            {
                this.DoHit(hit.hitId, hit.Damages);
            }
        }

        internal void DoHit(int hitId, List<NDamageInfo> damages)
        {
            if(hitId>this.Hit)//如果是后续的伤害
            {
                this.HitMap[hitId]=damages;
            }
            else
            {
                DoHitDamages(damages);
            }
        }

        internal void DoHitDamages(List<NDamageInfo> damages)
        {
            foreach(NDamageInfo dmg in damages)
            {
                Creature target=EntityManager.Instance.GetEntity(dmg.entityId) as Creature;
                if(target==null)
                {
                    continue;
                }
                target.DoDamage(dmg,true);
                if(this.Define.HitEffect!=null)
                {
                    target.PlayEffect(EffectType.Hit, this.Define.HitEffect, target);
                }
            }
        }
    }
}
