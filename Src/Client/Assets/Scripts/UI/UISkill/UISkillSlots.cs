using Common.Data;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISKILL
{
    public class UISkillSlots : MonoBehaviour
    {
        public UISkillSlot[] slots;

        private void Start()
        {
            RefreshUI();
        }

        void RefreshUI()
        {
            var Skills = DataManager.Instance.Skills[(int)User.Instance.CurrentCharacterInfo.Class];
            int skillIdx = 0;
            foreach (var skill in Skills)
            {
                slots[skillIdx].SetSkill(skill.Value);
                skillIdx++;
            }
        }
    }
}
