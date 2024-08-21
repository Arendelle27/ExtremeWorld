using Models;
using Services;
using SkillBridge.Message;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using static UIWindow;

namespace Managers
{
    public enum NpcQuestStatus
    {
        None,//没有任务
        Available,//有任务可接
        InComplete,//有任务未完成
        Complete//有任务已完成
    }

    public class QuestManager : Singleton<QuestManager>
    {
        public List<NQuestInfo> questInfo;
        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();

        public Dictionary<int,Dictionary<NpcQuestStatus,List<Quest>>> npcQuests=new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>();

        public UnityAction<Quest> OnQuestStatusChanged;

        public void Init(List<NQuestInfo> quests)
        {
            this.questInfo = quests;
            this.allQuests.Clear();
            this.npcQuests.Clear();
            InitQuests();
        }

        void InitQuests()
        {
            //初始化已有任务
            foreach (var info in this.questInfo)
            {
                Quest quest = new Quest(info);
                this.allQuests[quest.Info.QuestId] = quest;
            }
            this.CheckAvailableQuests();

            foreach (var kv in this.allQuests)
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }
        }

        //初始化可用任务
        void CheckAvailableQuests()
        { 
            foreach(var kv in DataManager.Instance.Quests)
            {
                if(kv.Value.LimitClass!=CharacterClass.None&&kv.Value.LimitClass!=User.Instance.CurrentCharacterInfo.Class)
                {
                    continue;//职业不符合
                }
                if(kv.Value.LimitLevel > User.Instance.CurrentCharacterInfo.Level)
                {
                    continue;//等级不符合
                }
                if(this.allQuests.ContainsKey(kv.Key))
                {
                    continue;//已有任务
                }

                if(kv.Value.PreQuest>0)
                {
                    Quest preQuest;
                    if(this.allQuests.TryGetValue(kv.Value.PreQuest,out preQuest))
                    {
                        if(preQuest.Info==null)
                            continue;//前置任务未接取
                        if(preQuest.Info.Status!=QuestStatus.Finished)
                        {
                            continue;//前置任务未完成
                        }
                    }
                    else
                        continue;//前置任务未接取
                }
                Quest quest = new Quest(kv.Value);
                this.allQuests[quest.Define.ID] = quest;
            }
        }

        void AddNpcQuest(int npcId,Quest quest)
        {
            if(!this.npcQuests.ContainsKey(npcId))
                this.npcQuests[npcId] = new Dictionary<NpcQuestStatus, List<Quest>>();
            List<Quest> available;
            List<Quest> complates;
            List<Quest> incomplete;

            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Available, out available))
            {
                available = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Available] = available;
            }
            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Complete, out complates))
            {
                complates = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Complete] = complates;
            }
            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.InComplete, out incomplete))
            {
                incomplete = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.InComplete] = incomplete;
            }
            if(quest.Info==null)
            {
                if (npcId == quest.Define.AcceptNPC && !this.npcQuests[npcId][NpcQuestStatus.Available].Contains(quest))
                {
                    this.npcQuests[npcId][NpcQuestStatus.Available].Add(quest);
                }
            }
            else
            {
                if (quest.Define.SubmitNPC ==npcId && quest.Info.Status == QuestStatus.Complated)
                {
                    if (!this.npcQuests[npcId][NpcQuestStatus.Complete].Contains(quest))
                        this.npcQuests[npcId][NpcQuestStatus.Complete].Add(quest);
                }
                if (quest.Define.SubmitNPC == npcId && quest.Info.Status ==QuestStatus.InProgress )
                {
                    if (!this.npcQuests[npcId][NpcQuestStatus.InComplete].Contains(quest))
                        this.npcQuests[npcId][NpcQuestStatus.InComplete].Add(quest);
                }
            }

        }

        /// <summary>
        /// 获取NPC任务状态
        /// </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public NpcQuestStatus GetQuestStatusByNpc(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status=new Dictionary<NpcQuestStatus, List<Quest>>();
            if(this.npcQuests.TryGetValue(npcId,out status))
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                {
                    return NpcQuestStatus.Complete;
                }
                if (status[NpcQuestStatus.Available].Count > 0)
                {
                    return NpcQuestStatus.Available;
                }
                if (status[NpcQuestStatus.InComplete].Count > 0)
                {
                    return NpcQuestStatus.InComplete;
                }
            }
            return NpcQuestStatus.None;
        }

        public bool OpenNpcQuest(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (this.npcQuests.TryGetValue(npcId, out status))
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                {
                    return ShowQuestDialog(status[NpcQuestStatus.Complete].First());
                }
                if (status[NpcQuestStatus.Available].Count > 0)
                {
                    return ShowQuestDialog(status[NpcQuestStatus.Available].First());
                }
                if (status[NpcQuestStatus.InComplete].Count > 0)
                {
                    return ShowQuestDialog(status[NpcQuestStatus.InComplete].First());
                }
            }
            return false;
        }

        public NpcQuestStatus GetQuestSystem(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status=new Dictionary<NpcQuestStatus, List<Quest>>();
            if(this.npcQuests.TryGetValue(npcId,out status))//获取NPC任务
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                    return NpcQuestStatus.Complete;
                if (status[NpcQuestStatus.Available].Count>0)
                    return NpcQuestStatus.Available;
                if (status[NpcQuestStatus.InComplete].Count > 0)
                    return NpcQuestStatus.InComplete;
            }
            return NpcQuestStatus.None;
        }


        bool ShowQuestDialog(Quest quest)
        {
            if(quest.Info==null||quest.Info.Status==QuestStatus.Complated)
            {
                UIQuestDialog dlg = UIManager.Instance.Show<UIQuestDialog>();
                dlg.SetQueset(quest);
                dlg.OnClose += OnQuestDialogClose;
                return true;
            }
            if(quest.Info!=null||quest.Info.Status==QuestStatus.Complated)
            {
                if(!string.IsNullOrEmpty(quest.Define.DialogIncomplete))
                {
                    MessageBox.Show(quest.Define.DialogIncomplete, "任务完成", MessageBoxType.Information);
                }
            }
            return true;
        }

        void OnQuestDialogClose(UIWindow sender,WindowResult result)
        {
            UIQuestDialog dlg = (UIQuestDialog)sender;
            if (result == WindowResult.Yes)
            {
                if (dlg.quest.Info == null)
                {
                    QuestService.Instance.SendQuestAccept(dlg.quest);
                }
                else if (dlg.quest.Info.Status == QuestStatus.Complated)
                {
                    QuestService.Instance.SendQuestSubmit(dlg.quest);
                }
            }
            else if(result==WindowResult.No)
            {
                MessageBox.Show("放弃任务：" + dlg.quest.Define.DialogDeny, "任务放弃", MessageBoxType.Information);
            }
        }

        public void OnQuestAccepted(NQuestInfo quest)
        {
            //this.questInfo.Add(quest);
            Quest q = new Quest(quest);
            this.allQuests[quest.QuestId] = q;

            this.npcQuests.Clear();
            foreach (var kv in this.allQuests)
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }
            if (this.OnQuestStatusChanged!=null)
            {
                this.OnQuestStatusChanged(q);
            }
        }

        public void OnQuestSubmited(NQuestInfo quest)
        {
            Quest q = new Quest(quest);
            this.allQuests[quest.QuestId] = q;
            this.CheckAvailableQuests();//检查是否有新任务可接
            this.npcQuests.Clear();
            foreach (var kv in this.allQuests)
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }
            if (this.OnQuestStatusChanged != null)
            {
                this.OnQuestStatusChanged(q);
            }
        }
    }
}