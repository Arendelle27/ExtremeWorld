using Common.Data;
using Managers;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Entities
{
    public class Character : Creature
    {
        public Character(NCharacterInfo info) : base(info)
        {

        }

        public override List<EquipDefine> GetEquips()
        {
            return EquipManager.Instance.GetEquipedDefines();
        }

        public override void Die()
        {
            base.Die();
            if(this.IsPlayer)
            {
                UIMain.Instance.OnTargetChanged(null);
                var msg = MessageBox.Show(string.Format("您已死亡"), "玩家角色死亡", MessageBoxType.Confirm, "返回主城", "退出游戏");
                msg.OnYes = () =>
                {
                    this.Attributes.Init(this.Info.attrDynamic, this.Define, this.Info.Level, GetEquips());
                    UserService.Instance.SendCharacterDeath(this.entityId);
                    this.isDead = false;
                };
                msg.OnNo = () =>
                {
                    Application.Quit();
                };
            }
        }
    }
}
