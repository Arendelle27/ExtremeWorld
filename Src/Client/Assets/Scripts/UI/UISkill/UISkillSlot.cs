using Battle;
using Common.Battle;
using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UISKILL
{
    public class UISkillSlot : MonoBehaviour, IPointerClickHandler
    {
        public Image icon;
        public Image overlay;
        public Text cdText;
        Skill skill;

        float overlaySpeed = 0;
        float cdRemain = 0;

        private void Start()
        {

        }

        private void Update()
        {
            if(this.skill==null)
            {
                return;
            }
            if(this.skill.CD>0)
            {
                if(overlay.enabled)
                {
                    overlay.enabled=true;
                }
                if(!cdText.enabled)
                {
                    cdText.enabled = true;
                }

                overlay.fillAmount = this.skill.CD / this.skill.Define.CD;
                this.cdText.text = ((int)Math.Ceiling(this.skill.CD)).ToString();
            }
            else
            {
                if (overlay.enabled) overlay.enabled = false;
                if (this.cdText.enabled) this.cdText.enabled = false;
            }
        }

        public void OnPositionSelected(Vector3 pos)
        {
            BattleManager.Instance.CurrentPosition=GameObjectTool.WorldToLogicN(pos);
            this.CastSkill();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(this.skill.Define.CastTarget==Common.Battle.TargetType.Position)
            {
                TargetSelector.ShowSelector(User.Instance.CurrentCharacter.position,this.skill.Define.CastRange, this.skill.Define.AOERange, this.OnPositionSelected);
                return;
            }
            CastSkill();
        }

        public void CastSkill()
        { 
            SkILLRESULT result = this.skill.CanCast(BattleManager.Instance.CurrentTarget);

            switch(result)
            {
                case SkILLRESULT.InvalidTarget:
                    MessageBox.Show("技能[" + this.skill.Define.Name + "]目标无效");
                    return;
                case SkILLRESULT.OutOfMp:
                    MessageBox.Show("技能：" + this.skill.Define.Name + "MP不足");
                    return;
                case SkILLRESULT.CoolDown:
                    MessageBox.Show("技能：" + this.skill.Define.Name + "冷却中");
                    return;
                case SkILLRESULT.OutOfRange:
                    MessageBox.Show("技能：" + this.skill.Define.Name + "超出施法范围");
                    return;
            }
            BattleManager.Instance.CastSkill(this.skill);

        }

        public void SetSkill(Skill value)
        {
            this.skill = value;
            if (this.icon != null)
            {
                this.icon.overrideSprite = Resloader.Load<Sprite>(this.skill.Define.Icon);
                this.icon.SetAllDirty();
            }
        }
    }
}
