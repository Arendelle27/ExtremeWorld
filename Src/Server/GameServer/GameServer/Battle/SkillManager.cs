using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GameServer.Battle
{ 
    class SkillManager
    {
        Creature Owner;

        public List<Skill> Skills { get; private set; }
        public List<NSkillInfo> Infos { get; private set; }

        public SkillManager(Creature owner)
        {
            Owner = owner;
            Skills = new List<Skill>();
            this.Infos=new List<NSkillInfo>();
            this.InitSkills();
        }

        void InitSkills()
        {
            this.Skills.Clear();
            this.Infos.Clear();

            if(!DataManager.Instance.Skills.ContainsKey((int)this.Owner.Define.TID))
            {
                return;
            }
            /*自己实现数据库读取*/

            foreach(var define in DataManager.Instance.Skills[this.Owner.Define.TID])
            {
                NSkillInfo info = new NSkillInfo();
                info.Id = define.Key;
                if(this.Owner.Info.Level>=define.Value.UnlockLevel)
                {
                    info.Level = 5;
                }
                else
                {
                    info.Level = 1;
                }
                this.Infos.Add(info);
                Skill skill = new Skill(info, this.Owner);
                this.AddSkill(skill);
            }
        }

        public void AddSkill(Skill skill)
        {
            this.Skills.Add(skill);
        }

        internal Skill GetSkill(int skillId)
        {
            for(int i=0;i<this.Skills.Count;i++)
            {
                if (this.Skills[i].Define.ID==skillId)
                {
                    return this.Skills[i];
                }
            }
            return null;
        }

        internal void Update()
        {
            for(int i=0;i<this.Skills.Count;i++)
            {
                this.Skills[i].Update();
            }
        }
    }

}