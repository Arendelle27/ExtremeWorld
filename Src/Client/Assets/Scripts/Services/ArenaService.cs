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
    class ArenaService : Singleton<ArenaService>, IDisposable
    {
        public void Init()
        {
            
        }
        public ArenaService()
        {
            MessageDistributer.Instance.Subscribe<ArenaChallengeRequest>(this.OnArenaChallengeRequest);
            MessageDistributer.Instance.Subscribe<ArenaChallengeResponse>(this.OnArenaChallengeResponse);
            MessageDistributer.Instance.Subscribe<ArenaBeginResponse>(this.OnArenaBeginResponse);
            MessageDistributer.Instance.Subscribe<ArenaEndResponse>(this.OnArenaEndResponse);
            MessageDistributer.Instance.Subscribe<ArenaReadyResponse>(this.OnArenaReady);
            MessageDistributer.Instance.Subscribe<ArenaRoundStartResponse>(this.OnArenaRoundStart);
            MessageDistributer.Instance.Subscribe<ArenaRoundEndResponse>(this.OnArenaRoundEnd);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ArenaChallengeRequest>(this.OnArenaChallengeRequest);
            MessageDistributer.Instance.Unsubscribe<ArenaChallengeResponse>(this.OnArenaChallengeResponse);
            MessageDistributer.Instance.Unsubscribe<ArenaBeginResponse>(this.OnArenaBeginResponse);
            MessageDistributer.Instance.Unsubscribe<ArenaEndResponse>(this.OnArenaEndResponse);
            MessageDistributer.Instance.Unsubscribe<ArenaRoundStartResponse>(this.OnArenaRoundStart);
            MessageDistributer.Instance.Unsubscribe<ArenaRoundEndResponse>(this.OnArenaRoundEnd);
        }

        private void OnArenaReady(object sender, ArenaReadyResponse message)
        {
            ArenaManager.Instance.OnReady(message.Round,message.ArenaInfo);
        }

        private void OnArenaRoundStart(object sender, ArenaRoundStartResponse message)
        {
            ArenaManager.Instance.OnRoundStart(message.Round,message.ArenaInfo);
        }

        private void OnArenaRoundEnd(object sender, ArenaRoundEndResponse message)
        {
            ArenaManager.Instance.OnRoundEnd(message.Round,message.ArenaInfo);
        }

        public void SendArenaChallengeRequest(int friendId,string friendName)
        {
            Debug.Log("SendArenaChallengeRequest:"+friendId+" "+friendName);
            NetMessage message=new NetMessage();
            message.Request=new NetMessageRequest();
            message.Request.arenaChallengeReq=new ArenaChallengeRequest();
            message.Request.arenaChallengeReq.ArenaInfo=new ArenaInfo();
            message.Request.arenaChallengeReq.ArenaInfo.Red=new ArenaPlayer() 
            {
                EntityId=User.Instance.CurrentCharacter.Id,
                Name=User.Instance.CurrentCharacter.Name,
            };
            message.Request.arenaChallengeReq.ArenaInfo.Blue=new ArenaPlayer()
            {
                EntityId=friendId,
                Name=friendName,
            };
            NetClient.Instance.SendMessage(message);
        }

        private void SendArenaChallengeResponse(bool accept,ArenaChallengeRequest request)
        {
            Debug.Log("SendArenaChallengeResponse:"+accept);
            NetMessage message=new NetMessage();
            message.Request=new NetMessageRequest();
            message.Request.arenaChallengeRes=new ArenaChallengeResponse();
            message.Request.arenaChallengeRes.Result=accept?Result.Success:Result.Failed;
            message.Request.arenaChallengeRes.Errormsg=accept?"":"对方拒绝了你的请求";
            message.Request.arenaChallengeRes.ArenaInfo=request.ArenaInfo;
            NetClient.Instance.SendMessage(message);
        }

        private void OnArenaChallengeRequest(object sender, ArenaChallengeRequest request)
        {
            Debug.Log("OnArenaChallengeResponse:");
            var confirm = MessageBox.Show(string.Format("{0} 邀请你竞技场对战", request.ArenaInfo.Red.Name), "竞技场对战", MessageBoxType.Confirm, "接受", "拒绝");
            confirm.OnYes = () =>
            {
                this.SendArenaChallengeResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                this.SendArenaChallengeResponse(false, request);
            };
        }

        private void OnArenaChallengeResponse(object sender, ArenaChallengeResponse message)
        {
            Debug.Log("OnArenaChallengeResponse:"+message.Result);
            if(message.Result!=Result.Success)
            {
                MessageBox.Show(message.Errormsg, "对方拒绝挑战");
            }
        }

        private void OnArenaBeginResponse(object sender, ArenaBeginResponse message)
        {
            Debug.Log("OnArenaBeginResponse:");
            ArenaManager.Instance.EnterArena(message.ArenaInfo);
        }

        private void OnArenaEndResponse(object sender, ArenaEndResponse message)
        {
            Debug.Log("OnArenaEndResponse:");
            ArenaManager.Instance.ExitArena(message.ArenaInfo);
        }

        internal void SendArenaReadyRequest(int arenaId)
        {
            Debug.Log("SendArenaReadyResponse");
            NetMessage message=new NetMessage();
            message.Request=new NetMessageRequest();
            message.Request.arenaReady=new ArenaReadyRequest();
            message.Request.arenaReady.entityId=User.Instance.CurrentCharacter.entityId;
            message.Request.arenaReady.arenaId=arenaId;
            NetClient.Instance.SendMessage(message);
        }
    }
}
