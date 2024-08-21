﻿using System;
using System.Collections;
using System.Collections.Generic;
using UICHAT;
using UIGUILD;
using UIRIDE;
using UISKILL;
using UnityEngine;

namespace Managers
{
    public class UIManager : Singleton<UIManager>
    {
        class UIElement
        {
            public string Resources;
            public bool Cache;
            public GameObject Instance;
        }

        private Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();

        public UIManager()
        {
            this.UIResources.Add(typeof(UISetting), new UIElement() { Resources = "UI/UISetting", Cache = true });
            this.UIResources.Add(typeof(UIBag), new UIElement() { Resources = "UI/UIBag", Cache = false });
            this.UIResources.Add(typeof(UIShop), new UIElement() { Resources = "UI/UIShop", Cache = false });
            this.UIResources.Add(typeof(UICharEquip), new UIElement() { Resources = "UI/UICharEquip", Cache = false });
            this.UIResources.Add(typeof(UIQuestSystem), new UIElement() { Resources = "UI/UIQuestSystem", Cache = false });
            this.UIResources.Add(typeof(UIQuestDialog), new UIElement() { Resources = "UI/UIQuestDialog", Cache = false });
            this.UIResources.Add(typeof(UIFriend), new UIElement() { Resources = "UI/UIFriend", Cache = false });
            this.UIResources.Add(typeof(UIGuild), new UIElement() { Resources = "UI/UIGuild/UIGuild", Cache = false });
            this.UIResources.Add(typeof(UIGuildList), new UIElement() { Resources = "UI/UIGuild/UIGuildList", Cache = false });
            this.UIResources.Add(typeof(UIGuildPopNoGuild), new UIElement() { Resources = "UI/UIGuild/UIGuildPopNoGuild", Cache = false });
            this.UIResources.Add(typeof(UIGuildPopCreate), new UIElement() { Resources = "UI/UIGuild/UIGuildPopCreate", Cache = false });
            this.UIResources.Add(typeof(UIGuildApplyLIst), new UIElement() { Resources = "UI/UIGuild/UIGuildApplyLIst", Cache = false });
            this.UIResources.Add(typeof(UIPopChatMenu), new UIElement() { Resources = "UI/UIChat/UIPopChatMenu", Cache = false });
            this.UIResources.Add(typeof(UIRide), new UIElement() { Resources = "UI/UIRide/UIRide", Cache = false });
            this.UIResources.Add(typeof(UISystemConfig), new UIElement() { Resources = "UI/UISystemConfig/UISystemConfig", Cache = false });
            this.UIResources.Add(typeof(UISkill), new UIElement() { Resources = "UI/UISkill/UISkill", Cache = false });
            this.UIResources.Add(typeof(UIStory), new UIElement() { Resources = "UI/UIStory/UIStory", Cache = false });
        }

        ~UIManager()
        {

        }

        public T Show<T>()
        {
            Type type = typeof(T);
            if (this.UIResources.ContainsKey(type))
            {
                UIElement info = this.UIResources[type];
                if (info.Instance != null)
                {
                    info.Instance.SetActive(true);
                }
                else
                {
                    UnityEngine.Object prefab = Resources.Load(info.Resources);
                    if (prefab == null)
                    {
                        return default(T);
                    }
                    info.Instance = (GameObject)GameObject.Instantiate(prefab);
                }
                return info.Instance.GetComponent<T>();
            }
            return default(T);
        }

        public void Close(Type type)
        {
            if (this.UIResources.ContainsKey(type))
            {
                UIElement info = this.UIResources[type];
                if (info.Cache)
                {
                    info.Instance.SetActive(false);
                }
                else
                {
                    GameObject.Destroy(info.Instance);
                    info.Instance = null;
                }
            }
        }

        public void Close<T>()
        {
            this.Close(typeof(T));
        }
    }

}
