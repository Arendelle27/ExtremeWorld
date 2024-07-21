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
        public Dictionary<int,Entity> AllEntities = new Dictionary<int,Entity>();
        public Dictionary<int, List<Entity>> MapEntites = new Dictionary<int, List<Entity>>();

        public void AddEntity(int mapId,Entity entity)
        {
            entity.EntityData.Id = ++this.idx;
            AllEntities.Add(entity.EntityData.Id,entity);

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
            this.AllEntities.Remove(entity.entityId);                                                                 
            this.MapEntites[mapId].Remove(entity);
        }

        public Entity GetEntity(int entityId)
        {
            Entity result = null;
            this.AllEntities.TryGetValue(entityId, out result);
            return result;
        }

        internal Creature GetCreature(int casterId)
        {
            return GetEntity(casterId) as Creature;
        }
    }
}
