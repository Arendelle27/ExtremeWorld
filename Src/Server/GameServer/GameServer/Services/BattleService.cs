﻿using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    public class BattleService : Singleton<BattleService>
    {
        public BattleService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<SkillCastRequest>(this.OnSkillCast);
        }

        public void Init()
        {

        }

        void OnSkillCast(NetConnection<NetSession> sender,SkillCastRequest request)
        {
            Log.InfoFormat("OnSkillCast:Skill:{0} caster:{1} target:{2} pos:{3}", request.castInfo.skillId,request.castInfo.casterId,request.castInfo.targetId,request.castInfo.Position);
            BattleManager.Instance.ProcessBattleMessage(sender, request);
        }

    }

}
