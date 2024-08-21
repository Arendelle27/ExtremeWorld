using Battle;
using Common.Data;
using Entities;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Managers
{
    public class StoryManager : Singleton<StoryManager>
    {
        public void Init()
        {
            NpcManager.Instance.RegisterNpcEvent(NpcFunction.InvokeStory, OnOpenStory);
        }

        private bool OnOpenStory(NpcDefine npc)
        {
            this.ShowStoryUI(npc.Param);
            return true;
        }

        public void ShowStoryUI(int shopId)
        {
            StoryDefine story;
            if(DataManager.Instance.Storys.TryGetValue(shopId, out story))
            {
                UIStory uiStory=UIManager.Instance.Show<UIStory>();
                if(uiStory!=null)
                {
                    uiStory.SetStory(story);
                }
            }
        }

        public bool StartStory(int storyId)
        {
            StoryService.Instance.SendStartStory(storyId);
            return true;
        }

        public void OnStoryStart(int storyId)
        {

        }
    }
}
