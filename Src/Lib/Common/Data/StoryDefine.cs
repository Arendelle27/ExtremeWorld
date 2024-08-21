using Common.Battle;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data
{

    public class StoryDefine
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String SubType { get; set; }
        public int MapId { get; set; }
        public int LimitTime { get; set; }
        public int PreQuest { get; set; }
        public int Quest { get; set; }

    }
}
