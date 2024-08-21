using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Services
{
    public class StoryService : Singleton<StoryService>, IDisposable
    {
        public void Init()
        {
            StoryManager.Instance.Init();
        }
        public StoryService()
        {
            MessageDistributer.Instance.Subscribe<StoryStartResponse>(this.OnStartStory);
            MessageDistributer.Instance.Subscribe<StoryEndResponse>(this.OnStoryEnd);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<StoryStartResponse>(this.OnStartStory);
            MessageDistributer.Instance.Unsubscribe<StoryEndResponse>(this.OnStoryEnd);
        }

        public void SendStartStory(int storyId)
        {
            Debug.Log("SendStartStory:"+storyId);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.storyStart = new StoryStartRequest();
            message.Request.storyStart.storyId = storyId;
            NetClient.Instance.SendMessage(message);
        }

        private void OnStartStory(object sender, StoryStartResponse response)
        {
            Debug.Log("OnStartStory:" + response.storyId);
            StoryManager.Instance.OnStoryStart(response.storyId);
        }

        public void SendEndStory(int storyId)
        {
            Debug.Log("SenderEndStory:" + storyId);

            NetMessage message = new NetMessage(); 
            message.Request = new NetMessageRequest();
            message.Request.storyEnd = new StoryStartRequest();
            message.Request.storyEnd.storyId = storyId;
            NetClient.Instance.SendMessage(message);
        }

        void OnStoryEnd(object sender, StoryEndResponse response)
        {
            Debug.Log("StoryEndResponse:" + response.storyId);
            if(response.Result==Result.Success)
            {
                
            }

        }
    }
}
