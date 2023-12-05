using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UIQuest
{
    public class UIQuestStatus:MonoBehaviour
    {
        public Image[] statusImage;

        private NpcQuestStatus questStatus;
        internal void SetQuestStatus(NpcQuestStatus status)
        {
            this.questStatus = status;
            for(int i=0;i<4;i++) 
            {
                if (this.statusImage[i]!=null)
                {
                    this.statusImage[i].gameObject.SetActive(i==(int)status);
                }
            }
        }
    }
}
