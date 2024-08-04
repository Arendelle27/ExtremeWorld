using Common;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Battle
{
    class Bullet
    {
        private Skill skill;
        int hit = 0;
        float flyTime=0;
        public float duration=0;

        public bool Stoped=false;

        public Bullet(Skill skill)
        {
            this.skill = skill;
            var target = skill.Target;
            this.hit = skill.Hit;
            int distance=skill.Owner.Distance(target);

            duration = distance / this.skill.Define.BulletSpeed;

        }

        public void Update()
        {
            if(Stoped)
            {
                return;
            }
            this.flyTime+=Time.deltaTime;
            if(this.flyTime>this.duration)
            {
                this.skill.DoHitDamages(this.hit);
                this.Stoped=true;
            }
        }
    }
}
