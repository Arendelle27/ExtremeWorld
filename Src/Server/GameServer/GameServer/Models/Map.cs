using Common;
using Common.Data;
using GameServer.Entities;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GameServer.Models
{
    class Map
    {
        internal class MapCharacter
        {
            public NetConnection<NetSession> connection;
            public Character character;

            public MapCharacter(NetConnection<NetSession>conn,Character cha)
            {
                this.connection = conn;
                this.character = cha;
            }
        }

        public int ID
        {
            get { return this.Define.ID; }
        }
        internal MapDefine Define;

        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();

        internal Map(MapDefine define)
        {
            this.Define = define;
        }

        internal void Update()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="character"></param>
        internal void CharacterEnter(NetConnection<NetSession> conn,Character character)
        {
            Log.InfoFormat("CharacterEnter:Map:{0} characterId:{1}", this.Define.ID, character.Id);
                
            character.Info.mapId = this.ID;

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();

            message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            message.Response.mapCharacterEnter.mapId = this.Define.ID;
            message.Response.mapCharacterEnter.Characters.Add(character.Info);

            foreach(var kv in this.MapCharacters)
            {
                message.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                this.SendCharacterEnterMap(kv.Value.connection, character.Info);
            }

            this.MapCharacters[character.Id] = new MapCharacter(conn, character);

            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data, 0, data.Length);
        }

        internal void CharacterLeave(NCharacterInfo cha)
        {
            Log.InfoFormat("CharacterLeave:Map:{0} characterId:{1}", this.Define.ID, cha.Id);
            this.MapCharacters.Remove(cha.Id);
            foreach(var kv in this.MapCharacters)
            {
                this.SendCharacterLeaveMap(kv.Value.connection, cha);
            }
        }


        void SendCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();

            message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            message.Response.mapCharacterEnter.mapId = this.Define.ID;
            message.Response.mapCharacterEnter.Characters.Add(character);

            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data, 0, data.Length);
        }

        private void SendCharacterLeaveMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();

            message.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            message.Response.mapCharacterLeave.characterId = character.Id;

            byte[] data = PackageHandler.PackMessage(message);
            conn.SendData(data, 0, data.Length);
        }
    }
}