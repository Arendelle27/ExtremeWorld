using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    internal class Monster : Creature
    {
        public Monster(NCharacterInfo info) : base(info)
        {
            this.Attributes.Init(this.Info.attrDynamic,this.Define,this.Info.Level,null);
        }
    }
}
