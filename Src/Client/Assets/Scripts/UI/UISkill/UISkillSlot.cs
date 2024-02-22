using Battle;
using Common.Battle;
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
            if (overlay.fillAmount > 0)
            {
                overlay.fillAmount = this.cdRemain / this.skill.Define.CD;
                this.cdText.text = ((int)Math.Ceiling(this.cdRemain)).ToString();
                this.cdRemain -= Time.deltaTime;
            }
            else
            {
                if (overlay.enabled) overlay.enabled = false;
                if (this.cdText.enabled) this.cdText.enabled = false;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SkillResult result= this.skill.CanCast();

            switch(result)
            {
                case SkillResult.InvalidTarget:
                    MessageBox.Show("技能：" + this.skill.Define.Name + "需要选择目标");
                    return;
                case SkillResult.OutOfMP:
                    MessageBox.Show("技能：" + this.skill.Define.Name + "法力值不足");
                    return;
                case SkillResult.Cooldown:
                    MessageBox.Show("技能：" + this.skill.Define.Name + "还在冷却中");
                    return;
            }

            MessageBox.Show("释放技能：" + this.skill.Define.Name);
            this.SetCD(this.skill.Define.CD);
            this.skill.Cast();

        }

        public void SetCD(float cd)
        {
            if (!overlay.enabled) overlay.enabled = true;
            if (!this.cdText.enabled) this.cdText.enabled = true;
            this.cdText.text = ((int)Math.Floor(this.cdRemain)).ToString();
            this.overlay.fillAmount = 1f;
            this.overlaySpeed = 1f / cd;
            this.cdRemain = cd;
        }

        public void SetSkill(Skill value)
        {
            this.skill = value;
            if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.skill.Define.Icon);
            this.SetCD(this.skill.Define.CD);
        }
    }
}
