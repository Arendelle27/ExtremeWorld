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
    class BattleManager : Singleton<BattleManager>
    {
        static long bid = 0;

        public void Init()
        {

        }

        public void ProcessBattleMessage(NetConnection<NetSession> sender,SkillCastRequest request)
        {
            Log.InfoFormat("BattleManager.ProcessBattleMessage:skill:{0} target:{2} pos:{3}", request.castInfo.skillId, request.castInfo.casterId, request.castInfo.targetId, request.castInfo.Position);
            Character character=sender.Session.Character;
            var battle = MapManager.Instance[character.Info.mapId].Battle;
            battle.ProcessBattleMessage(sender, request);
        }

        public void CharacterDeathReturnMainCity(NetConnection<NetSession> sender)
        {
            Character character = sender.Session.Character;
            var currentMap = MapManager.Instance[character.Info.mapId];
            currentMap.CharacterLeave(character);
            EntityManager.Instance.RemoveMapEntity(currentMap.ID, currentMap.InstanceID, character);

            TeleporterDefine startPoint = DataManager.Instance.Teleporters[4];
            sender.Session.Character.Position = startPoint.Position;
            sender.Session.Character.Direction = startPoint.Direction;
            Map map = MapManager.Instance[startPoint.MapID];
            map.AddCharacter(sender, character);
            map.CharacterEnter(sender, character);
            EntityManager.Instance.AddMapEntity(map.ID, map.InstanceID, character);

            character.IsDeath = false;
            character.Attributes.Init(character.Info.attrDynamic,character.Define, character.Info.Level, character.GetEquip());
        }
    }

}
