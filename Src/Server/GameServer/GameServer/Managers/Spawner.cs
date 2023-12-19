using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Models;
using System.Data;
using Common.Data;

namespace GameServer.Managers
{
    class Spawner
    {
        public SpawnRuleDefine Define { get; set; }

        private Map Map;

        /// <summary>
        /// 刷怪时间
        /// </summary>
        private float spawnTime = 0;

        /// <summary>
        /// 消失时间
        /// </summary>
        private float unspawnTime = 0;

        private bool spawned = false;

        private SpawnPointDefine spawnPoint=null;

        public Spawner(SpawnRuleDefine define,Map map)
        {
            this.Define = define;
            this.Map = map;

            if(DataManager.Instance.SpawnPoints.ContainsKey(this.Map.ID))
            {
                if (DataManager.Instance.SpawnPoints[this.Map.ID].ContainsKey(this.Define.SpawnPoint))
                {
                   this.spawnPoint = DataManager.Instance.SpawnPoints[this.Map.ID][this.Define.SpawnPoint];
                }
                else
                {
                    Log.ErrorFormat("SpawnRule[{0}] SpawnPoint[{1}] not existed",this.Define.ID, this.Define.SpawnPoint);
                }
            }
        }

        public void Update()
        {
            if(this.CanSpawn())
            {
                this.Spawn();
            }
        }

        bool CanSpawn()
        {
            if(this.spawned)
            {
                return false;
            }
            if(this.unspawnTime+this.Define.SpawnPeriod>Time.time)
            {
                return false;
            }
            return true;
        }

        public void Spawn()
        {
            this.spawned = true;
            Log.InfoFormat("Map[{0}] Spawn[{1}:Mon:{2},Lv:{3} At Point:{4}", this.Define.MapID, this.Define.SpawnType, this.Define.SpawnMonID,this.Define.SpawnLevel, this.Define.SpawnPoint);
            this.Map.MonsterManager.Create(this.Define.SpawnMonID, this.Define.SpawnLevel, this.spawnPoint.Position, this.spawnPoint.Direction);
        }
    }
}
