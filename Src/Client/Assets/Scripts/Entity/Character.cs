﻿using Common.Data;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    internal class Character : Creature
    {
        public Character(NCharacterInfo info) : base(info)
        {

        }

        public override List<EquipDefine> GetEquips()
        {
            return EquipManager.Instance.GetEquipedDefines();
        }

    }
}
