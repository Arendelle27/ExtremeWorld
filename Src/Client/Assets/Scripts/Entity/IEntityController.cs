using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Entities
{
    public interface IEntityController
    {
        Transform GetTransform();
        void PlayAnim(string name);
        void PlayEffect(EffectType type, string name, Entity target, float duration);
        void SetStandby(bool standby);
        void UpdateDirection();
    }
}
