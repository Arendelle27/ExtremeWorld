using Battle;
using Common.Battle;
using Managers;
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

        public void OnPointerClick(PointerEventData eventData)
        {
            SkILLRESULT result = this.skill.CanCast(BattleManager.Instance.CurrentTarget);

            switch(result)
            {
                //case SkILLRESULT.InvalidTarget:
                //    MessageBox.Show("����[" + this.skill.Define.Name + "]Ŀ����Ч");
                //    return;
                case SkILLRESULT.OutOfMp:
                    MessageBox.Show("���ܣ�" + this.skill.Define.Name + "MP����");
                    return;
                case SkILLRESULT.CoolDown:
                    MessageBox.Show("���ܣ�" + this.skill.Define.Name + "��ȴ��");
                    return;
                case SkILLRESULT.OutOfRange:
                    MessageBox.Show("���ܣ�" + this.skill.Define.Name + "����ʩ����Χ");
                    return;
            }
            BattleManager.Instance.CastSkill(this.skill);

            //MessageBox.Show("�ͷż��ܣ�" + this.skill.Define.Name);
            //this.SetCD(this.skill.Define.CD);
            //this.skill.Owner.BattleState=true;
            //this.skill.BeginCast();

        }

        //public void SetCD(float cd)
        //{
        //    if (!overlay.enabled) overlay.enabled = true;
        //    if (!this.cdText.enabled) this.cdText.enabled = true;
        //    this.cdText.text = ((int)Math.Floor(this.cdRemain)).ToString();
        //    this.overlay.fillAmount = 1f;
        //    this.overlaySpeed = 1f / cd;
        //    this.cdRemain = cd;
        //}

        public void SetSkill(Skill value)
        {
            this.skill = value;
            if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.skill.Define.Icon);
            //this.SetCD(this.skill.Define.CD);
        }
    }
}
