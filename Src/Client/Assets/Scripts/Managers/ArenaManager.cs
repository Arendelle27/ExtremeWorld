using Battle;
using Entities;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Managers
{
    public class ArenaManager : Singleton<ArenaManager>
    {
        public int Round = 0;
        private ArenaInfo ArenaInfo;
        public ArenaManager()
        {

        }

        public void EnterArena(ArenaInfo arenaInfo)
        {
            Debug.LogFormat("ArenaManager.EnterArena: {0}", arenaInfo.ArenaId);
            this.ArenaInfo=arenaInfo;
        }

        public void ExitArena(ArenaInfo arenaInfo)
        {
            Debug.LogFormat("ArenaManager.ExitArena: {0}", arenaInfo.ArenaId);
            this.ArenaInfo=null;
        }

        public void SendReady()
        {
            Debug.LogFormat("ArenaManager.SendReady:{0}",this.ArenaInfo.ArenaId);
            ArenaService.Instance.SendArenaReadyRequest(this.ArenaInfo.ArenaId);
        }

        public void OnReady(int round,ArenaInfo arenaInfo)
        {
            Debug.LogFormat("ArenaManager.OnReady:{0}",round);
            this.Round = round;
            if(UIArena.Instance!=null)
            {
                UIArena.Instance.ShowCountDown();
            }
        }

        internal void OnRoundStart(int round, ArenaInfo arenaInfo)
        {
            Debug.LogFormat("ArenaManager.OnRoundStart:{0} Round:{1}",arenaInfo.ArenaId,round);
            if(UIArena.Instance!=null)
            {
                UIArena.Instance.ShowRoundStart(round,arenaInfo);
            }
        }

        internal void OnRoundEnd(int round, ArenaInfo arenaInfo)
        {
            Debug.LogFormat("ArenaManager.OnRoundEnd:{0} Round:{1}",arenaInfo.ArenaId,round);
            if(UIArena.Instance!=null)
            {
                UIArena.Instance.ShowRoundResult(round,arenaInfo);
            }
        }
    }
}
