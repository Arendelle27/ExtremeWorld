﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Models;
using System.Data;
using System.Diagnostics;

namespace GameServer.Managers
{
    /// <summary>
    /// 刷怪管理器
    /// </summary>
    class SpawnManager
    {
        private List<Spawner> Rules = new List<Spawner>();

        private Map Map;

        public void Init(Map map)
        {
            this.Map = map;
            if(DataManager.Instance.SpawnRules.ContainsKey(map.Define.ID))
            {
                foreach(var define in DataManager.Instance.SpawnRules[map.Define.ID].Values)
                {
                    this.Rules.Add(new Spawner(define,this.Map));
                }
            }
        }

        public void Update()
        {
            if(Rules.Count==0) return;
            for(int i=0;i<Rules.Count;i++)
            {
                this.Rules[i].Update();
            }
        }
    }
}
