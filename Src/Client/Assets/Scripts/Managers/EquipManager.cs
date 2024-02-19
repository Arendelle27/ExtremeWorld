using System;
using System.Collections;
using System.Collections.Generic;
using Common.Data;
using Models;
using Services;
using SkillBridge.Message;
using UnityEngine;

namespace Managers
{
    public class EquipManager : Singleton<EquipManager>
    {
        public delegate void OnEquipChanageHandler();

        public event OnEquipChanageHandler OnEquipChanaged;

        public Item[] Equips = new Item[(int)EquipSlot.SlotMax];

        byte[] Data;

        unsafe public void Init(byte[] data)
        {
            this.Data = data;
            this.ParseEquipData(data);
        }

        public bool Contains(int equipId)
        {
            for(int i=0;i<this.Equips.Length;i++)
            {
                if (Equips[i] != null && Equips[i].Id == equipId)
                    return false;
            }
            return false;
        }

        public Item GetEquip(EquipSlot slot)
        {
            return Equips[(int)slot];
        }

        unsafe void ParseEquipData(byte[] data)
        {
            fixed(byte* pt=this.Data)
            {
                for(int i=0;i<this.Equips.Length;i++)
                {
                    int itemId = *(int*)(pt + i * sizeof(int));
                    if (itemId > 0)
                        Equips[i] = ItemManager.Instance.Items[itemId];
                    else
                        Equips[i] = null;
                }
            }
        }

        unsafe public byte[] GetEquipData()
        {
            fixed(byte* pt=this.Data)
            {
                for(int i=0;i<(int)EquipSlot.SlotMax;i++)
                {
                    int* itemId=(int*)(pt + i * sizeof(int));
                    if (Equips[i] == null)
                        *itemId = 0;
                    else
                        *itemId = Equips[i].Id;
                }
            }
            return this.Data;
        }

        public void EquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, true);
        }

        public void UnEquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, false);
        }

        public void OnEquipItem(Item equip)
        {
            if(this.Equips[(int)equip.EquipInfo.Slot]!=null&&this.Equips[(int)equip.EquipInfo.Slot].Id==equip.Id)
            {
                return;
            }
            this.Equips[(int)equip.EquipInfo.Slot] = ItemManager.Instance.Items[equip.Id];

            if (OnEquipChanaged != null)
                OnEquipChanaged();
        }

        public void OnUnEquipItem(EquipSlot slot)
        {
            if(this.Equips[(int)slot]!=null)
            {
                this.Equips[(int)slot] = null;
                if (OnEquipChanaged != null)
                    OnEquipChanaged();
            }
        }

        public List<EquipDefine> GetEquipedDefines()
        {
            List<EquipDefine> result= new List<EquipDefine>();
            for(int i = 0;i<(int)EquipSlot.SlotMax;i++)
            {
                if (this.Equips[i]!=null)
                {
                    result.Add(this.Equips[i].EquipInfo);
                }
            }
            return result;
        }
    }
}
