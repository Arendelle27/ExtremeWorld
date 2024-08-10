﻿using Common;
using Common.Battle;
using Common.Utils;
using GameServer.Core;
using GameServer.Entities;
using GameServer.Managers;
using Microsoft.SqlServer.Server;
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

        public SkillStatus Status;

        private float cd = 0;
        private BattleContext Context;
        NSkillHitInfo HitInfo;

        public float CD
        {
            get { return cd; }
        }

        public bool Instant
        {
            get 
            {
                if (this.Define.CastTime > 0) return false;
                if (this.Define.Bullet) return false;
                if(this.Define.Duration>0) return false;
                if(this.Define.HitTimes!=null && this.Define.HitTimes.Count>0) return false;
                return true;
            }
        }

        public int Hit;
        private float skillTime;
        private float castingTime;

        List<Bullet> Bullets = new List<Bullet>();

        public Skill(NSkillInfo info, Creature owner)
        {
            this.info = info;
            this.Owner = owner;
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.TID][info.Id];
        }

        public SkILLRESULT CanCast(BattleContext context)
        {
            if (this.Status != SkillStatus.None)
            {
                return SkILLRESULT.Casting;
            }
            if (this.Define.CastTarget == Common.Battle.TargetType.Target)
            {
                if (context.Target == null || context.Target == this.Owner)
                {
                    return SkILLRESULT.InvalidTarget;
                }
                int distance = this.Owner.Distance(context.Target);
                if (distance > this.Define.CastRange)
                {
                    return SkILLRESULT.OutOfRange;
                }
            }

            if (this.Define.CastTarget == Common.Battle.TargetType.Position)
            {
                if (context.CastSkill.Position == null)
                {
                    return SkILLRESULT.InvalidTarget;
                }
                if (this.Owner.Distance(context.Position) > this.Define.CastRange)
                {
                    return SkILLRESULT.OutOfRange;
                }
            }

            if (this.Owner.Attributes.MP < this.Define.MPCost)
            {
                return SkILLRESULT.OutOfMp;
            }

            if (this.cd > 0)
            {
                return SkILLRESULT.CoolDown;
            }

            return SkILLRESULT.Ok;
        }

        internal SkILLRESULT Cast(BattleContext context)
        {
            SkILLRESULT result = this.CanCast(context);
            if(result==SkILLRESULT.Ok)
            {
                this.castingTime = 0;
                this.skillTime = 0;
                this.cd = this.Define.CD;
                this.Context = context;
                this.Hit = 0;
                this.Bullets.Clear();

                //this.AddBuff(TriggerType.SkillHit,this.Context.Target);

                if(this.Instant)
                {
                    this.DoHit();
                }
                else
                {
                    if(this.Define.CastTime>0)
                    {
                        this.Status=SkillStatus.Casting;
                    }
                    else
                    {
                        this.Status = SkillStatus.Running;
                    }
                }
            }
            Log.InfoFormat("Skill[{0}].Cast Result:[{1}] Status:{2}", this.Define.Name, result, this.Status);
            return result;
        }

        internal void Update()
        {
            this.UpdateCD();
            if(this.Status==SkillStatus.Casting)
            {
                this.UpdateCasting();
            }
            else if(this.Status==SkillStatus.Running)
            {
                this.UpdateSkill();
            }
        }

        private void UpdateSkill()
        {
            this.skillTime+=Time.deltaTime;

            if(this.Define.Duration>0)
            {
                if(this.skillTime>this.Define.Interval*(this.Hit+1))
                {
                    this.DoHit();
                }
                if(this.skillTime>this.Define.Duration)
                {
                    this.Status=SkillStatus.None;
                    Log.InfoFormat("Skill[{0}].UpdateSkill Finish", this.Define.Name);
                }
            }
            else if(this.Define.HitTimes!=null&&this.Define.HitTimes.Count>0)
            {
                if(this.Hit<this.Define.HitTimes.Count)
                {
                    if (this.skillTime > this.Define.HitTimes[this.Hit])
                    {
                        this.DoHit();
                    }
                }
                else
                {
                    if(!this.Define.Bullet)
                    this.Status=SkillStatus.None;
                    Log.InfoFormat("Skill[{0}].UpdateSkill Finish", this.Define.Name);
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
                if (finish && this.Hit >= this.Define.HitTimes.Count)
                {
                    this.Status = SkillStatus.None;
                    Log.InfoFormat("Skill[{0}].UpdateSkill Finish", this.Define.Name);
                }
            }
        }

        private void UpdateCasting()
        {
            if(this.castingTime<this.Define.CastTime)
            {
                this.castingTime += Time.deltaTime;
            }
            else
            {
                this.castingTime = 0;
                this.Status = SkillStatus.Running;
                Log.InfoFormat("Skill[{0}].UpdateCasting Finish", this.Define.Name);
            }
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

        NSkillHitInfo InitHitInfo(bool isBullet)
        {
            NSkillHitInfo hitInfo =new NSkillHitInfo();
            hitInfo.casterId=this.Context.Caster.entityId;
            hitInfo.skillId = this.info.Id;
            hitInfo.hitId=this.Hit;
            hitInfo.isBullet = isBullet;
            return hitInfo;

        }

        public void DoHit()
        {
            NSkillHitInfo hitInfo=this.InitHitInfo(false);
            Log.InfoFormat("Skill[{0}].DoHit[{1}]", this.Define.Name, this.Hit);
            this.Hit++;
            if (this.Define.Bullet)
            {
                this.CastBullet(hitInfo);
                return;
            }
            DoHit(hitInfo);
        }

        public void DoHit(NSkillHitInfo hitInfo)
        {
            Context.Battle.AddHitInfo(hitInfo);
            Log.InfoFormat("Skill[{0}].DoHit[{1}] IsBullet:{2}", this.Define.Name, hitInfo.hitId,hitInfo.isBullet);
            if (this.Define.AOERange > 0)
            {
                this.HitRange(hitInfo);
                return;
            }

            if (this.Define.CastTarget == Common.Battle.TargetType.Target)
            {
                this.HitTarget(Context.Target,hitInfo);
            }
        }

        private void HitTarget(Creature target,NSkillHitInfo hit)
        {
            if(this.Define.CastTarget==Common.Battle.TargetType.Self&&(target!=Context.Caster))
            {
                return;
            }
            else if(target==Context.Caster)
            {
                return;
            }
            NDamageInfo damage = this.CalcSkillDamage(Context.Caster, target);
            Log.InfoFormat("Skill[{0}].HitTarget[{1}] Damage:{2} Crit:{3}", this.Define.Name, target.Define.Name, damage.Damage,damage.Crit);
            target.DoDamage(damage,Context.Caster);
            hit.Damages.Add(damage);

            this.AddBuff(TriggerType.SkillHit,target);
        }

        private void AddBuff(TriggerType trigger,Creature target)
        {
            if(this.Define.Buff==null||this.Define.Buff.Count==0)
            {
                return;
            }

            foreach(var buffId in this.Define.Buff)
            {
                var buffDefine = DataManager.Instance.Buffs[buffId];
                if (buffDefine.Trigger != trigger) continue;//触发类型不一致
                if(buffDefine.Target == TargetType.Self)
                {
                    this.Owner.AddBuff(this.Context,buffDefine);
                }
                else if(buffDefine.Target==TargetType.Target&&this.Context.Target!=null)
                {
                    target.AddBuff(this.Context,buffDefine);
                }
            }
        }

        /*
战斗计算公式
物理伤害=物理攻击或者技能原始伤害*（1-物理防御/（物理防御+100））
魔法伤害=魔法攻击或者技能原始伤害*（1-魔法防御/（魔法防御+100））
暴击伤害=固定两倍伤害
注：伤害值最小为1，当伤害小于1时取1
注：最终伤害值在最终取舍时随时浮动5%。即最终伤害值*（1-5%）<最终伤害值输出<最终伤害值*（1+5%）
*/
        private NDamageInfo CalcSkillDamage(Creature caster, Creature target)
        {
            float ad = this.Define.AD + caster.Attributes.AD * this.Define.ADFactor;
            float ap = this.Define.AP + caster.Attributes.AP * this.Define.APFactor;

            float addmg=ad * (1 + target.Attributes.DEF/(target.Attributes.DEF+100));
            float apdmg=ap * (1 + target.Attributes.MDEF / (target.Attributes.MDEF + 100));

            float final = addmg + apdmg;
            bool isCrit =IsCrit(caster.Attributes.CRI);
            if(isCrit)
            {
                final = final * 2f;//暴击伤害翻倍
            }
            //随机浮动
            final=final*(float)MathUtil.Random.NextDouble() * 0.1f -0.05f;

            NDamageInfo damage = new NDamageInfo();
            damage.entityId = target.entityId;
            damage.Damage = Math.Max(1, (int)final);
            damage.Crit = isCrit;
            return damage;
        }

        bool IsCrit(float cri)
        {
            return MathUtil.Random.NextDouble() < cri;
        }

        private void CastBullet(NSkillHitInfo hitInfo)
        {
            Log.InfoFormat("Skill[{0}].CastBullet[{1}]", this.Define.Name,this.Define.BulletResource);
            Bullet bullet = new Bullet(this, this.Context.Target,hitInfo);
            this.Bullets.Add(bullet);
        }

        private void HitRange(NSkillHitInfo hit)
        {
            Vector3Int pos;
            if(this.Define.CastTarget==Common.Battle.TargetType.Target)
            {
                pos = Context.Target.Position;
            }
            else if(this.Define.CastTarget==Common.Battle.TargetType.Position)
            {
                pos = Context.Position;
            }
            else
            {
                pos=this.Owner.Position;
            }

            List<Creature> units = this.Context.Battle.FindUnitsInMapRange(pos, this.Define.AOERange);
            foreach(var target in units)
            {
                this.HitTarget(target,hit);
            }
        }
    }
}
