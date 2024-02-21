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
        SkillDefine skill;

        float overlaySpeed = 0;
        float cdRemain = 0;

        private void Start()
        {

        }

        private void Update()
        {
            if (overlay.fillAmount > 0)
            {
                overlay.fillAmount = this.cdRemain / this.skill.CD;
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
            if (this.overlay.fillAmount > 0)
            {
                MessageBox.Show("���ܣ�" + this.skill.Name + "������ȴ��");
            }
            else
            {
                MessageBox.Show("�ͷż��ܣ�" + this.skill.Name);
                this.SetCD(this.skill.CD);
            }
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

        public void SetSkill(SkillDefine value)
        {
            this.skill = value;
            if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.skill.Icon);
            this.SetCD(this.skill.CD);
        }
    }
}
