using Common;
using Common.Data;
using GameServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class MapManager:Singleton<MapManager>
    {
        Dictionary<int,Dictionary<int,Map>> Maps = new Dictionary<int, Dictionary<int, Map>>();

        public void Init()
        {
            foreach (var mapdefine in DataManager.Instance.Maps.Values)
            {
                Log.InfoFormat("MapManager.Init>Map:{0}:{1}", mapdefine.ID, mapdefine.Name);
                int instanceCount = 1;
                if(mapdefine.Type==MapType.Arena||mapdefine.Type==MapType.Story)
                {
                    instanceCount = ArenaManager.MaxInstance;
                }
                this.Maps[mapdefine.ID] = new Dictionary<int, Map>();
                for(int i=0;i<instanceCount;i++)
                {
                    this.Maps[mapdefine.ID][i] = new Map(mapdefine,i);
                }
            }
        }

        public Map this[int key]
        {
            get
            {
                return this.Maps[key][0];
            }
        }

        public void Update()
        {
            foreach(var map in this.Maps.Values)
            {
                foreach(var instance in map.Values)
                {
                    instance.Update();
                }
            }
        }

        public Map GetInstance(int mapId, int instance)
        {
            return this.Maps[mapId][instance];
        }
    }

}