using Common;
using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class EntityManager:Singleton<EntityManager>
    {
        private int idx = 0;
        public List<Entity> AllEntities = new List<Entity>();
        public Dictionary<int, List<Entity>> MapEntites = new Dictionary<int, List<Entity>>();

        public void AddEntity(int mapId,Entity entity)
        {
            AllEntities.Add(entity);
            entity.EntityData.Id = ++this.idx;

            List<Entity> entities = null;
            if(!MapEntites.TryGetValue(mapId,out entities))
            {
                entities = new List<Entity>();
                MapEntites[mapId] = entities;
            }
            entities.Add(entity);
        }

        public void RemoveEntity(int mapId,Entity entity)
        {
            this.AllEntities.Remove(entity);                                                                 
            this.MapEntites[mapId].Remove(entity);
        }
    }
}
