using Common;
using Common.Data;
using Common.Utils;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class ArenaManager : Singleton<ArenaManager>
    {
        public const int ArenaMapId = 5;
        public const int MaxInstance = 100;

        Queue<int> InstanceIndexes= new Queue<int>();

        Arena[] Arenas=new Arena[MaxInstance];

        public void Init()
        {
            for(int i=0;i<MaxInstance;i++)
            {
                this.InstanceIndexes.Enqueue(i);
            }
        }

        public Arena NewArena(ArenaInfo info,NetConnection<NetSession> red,NetConnection<NetSession> blue)
        {
            var instance=InstanceIndexes.Dequeue();
            var map=MapManager.Instance.GetInstance(ArenaMapId,instance);
            Arena arena=new Arena(map,info,red,blue);
            this.Arenas[instance] = arena;
            arena.PlayerEnter();
            return arena;
        }

        public void Update()
        {
            for(int i=0;i<Arenas.Length;i++) 
            {
                if (Arenas[i]!=null)
                {
                    Arenas[i].Update();
                }
            }
        }

        public Arena GetArena(int arenaId)
        {
            return this.Arenas[arenaId];
        }
    }

}
