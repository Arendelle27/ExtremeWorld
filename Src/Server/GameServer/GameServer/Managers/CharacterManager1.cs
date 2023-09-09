using Common;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GameServer.Managers
{
    class CharacterManager1:Singleton<CharacterManager>
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

        public CharacterManager1()
        {

        }

        public void Dispose()
        {

        }

        public void Clear()
        {
            this.Characters.Clear();
        }
        public Character AddCharacter(TCharacter cha)
        {
            Character character = new Character(CharacterType.Player, cha);
            this.Characters[cha.ID] = character;
            return character;
        }

        public void RemoveCharacter(int characterId)
        {
            this.Characters.Remove(characterId);
        }
    }
}