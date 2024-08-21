﻿using Common;
using Common.Battle;
using GameServer.AI;
using GameServer.Battle;
using GameServer.Core;
using GameServer.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    class Monster : Creature
    {  
        AIAgent AI;
        private Vector3Int moveTarget;
        Vector3 movePosition;

        public Monster(int tid, int level, Vector3Int pos, Vector3Int dir) : 
            base(CharacterType.Monster, tid, level, pos, dir)
        {
            this.AI=new AIAgent(this);
        }

        public override void OnEnterMap(Map map)
        {
            base.OnEnterMap(map);
            //this.AI.Init();
        }

        public override void Update()
        {
            base.Update();
            this.UpdateMovement();
            this.AI.Update();
        }

        protected override void OnDamage(NDamageInfo damage, Creature source)
        {
            if(this.AI!=null)
            {
                this.AI.OnDamage(damage, source);
            }
        }

        internal Skill FindSkill(BattleContext context,SkillType type)
        {
            Skill cancast= null;
            foreach(var skill in this.SkillMgr.Skills)
            {
                if((skill.Define.Type & type)!=skill.Define.Type)
                {
                    continue;
                }

                var result=skill.CanCast(context);
                if(result==SkILLRESULT.Casting)
                {
                    return null;
                }
                if(result==SkILLRESULT.Ok)
                {
                    cancast=skill;
                }
            }
            return cancast;
        }

        internal void MoveTo(Vector3Int position)
        {
            if(State==CharacterState.Idle)
            {
                State=CharacterState.Move;
            }
            if(this.moveTarget!=position)
            {
                this.moveTarget=position;
                this.movePosition=this.Position;
                var dist = (this.moveTarget - this.Position);
                Log.InfoFormat("Monster:{0} CurPosition {1}", this.entityId, this.Position);

                this.Direction = dist.normalized;
                this.Speed = this.Define.Speed;

                NEntitySync sync = new NEntitySync();
                sync.Entity=this.EntityData;
                sync.Event=EntityEvent.MoveFwd;
                sync.Id=this.entityId;

                this.Map.UpdateEntity(sync);
            }
        }

        private void UpdateMovement()
        {
            if(State==CharacterState.Move)
            {
                if(this.Distance(this.moveTarget)<100)
                {
                    this.StopMove();
                }

                if(this.Speed>0)
                {
                    Vector3 dir=this.Direction;
                    this.movePosition+=dir*this.Speed*Time.deltaTime/100f;
                    this.Position=this.movePosition;
                }
            }
        }

        internal void StopMove()
        {
            State = CharacterState.Idle;
            this.moveTarget = Vector3Int.zero;
            this.Speed=0;

            NEntitySync sync = new NEntitySync();
            sync.Entity = this.EntityData;
            sync.Event = EntityEvent.Idle;
            sync.Id = this.entityId;

            this.Map.UpdateEntity(sync);
        }
    }
}
