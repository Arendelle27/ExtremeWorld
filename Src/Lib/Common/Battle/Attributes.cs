﻿using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Battle
{
    public class Attributes
    {
        AttributeData Initial=new AttributeData();
        AttributeData Growth=new AttributeData();
        AttributeData Equip=new AttributeData();
        public AttributeData Basic=new AttributeData();
        public AttributeData Buff=new AttributeData();
        public AttributeData Final=new AttributeData();

        int Level;

        public NAttributeDynamic DynamicAttr;

        public float HP
        {
            get { return DynamicAttr.Hp; }
            set { DynamicAttr.Hp = (int)Math.Min(MaxHP, value); }
        }

        public float MP
        {
            get { return DynamicAttr.Mp; }
            set { DynamicAttr.Mp = (int)Math.Min(MaxMP, value); }
        }

        /// <summary>
        /// 最大生命
        /// </summary>
        public float MaxHP
        {
            get { return this.Final.MaxHP; }
        }

        /// <summary>
        /// 最大法力
        /// </summary>
        public float MaxMP
        {
            get { return this.Final.MaxMP; }
        }

        /// <summary>
        /// 力量
        /// </summary>
        public float STR
        {
            get { return this.Final.STR; }
        }

        /// <summary>
        /// 智力
        /// </summary>
        public float INT
        {
            get { return this.Final.INT; }
        }

        /// <summary>
        /// 敏捷
        /// </summary>
        public float DEX
        {
            get { return this.Final.DEX; }
        }

        /// <summary>
        /// 物理攻击
        /// </summary>
        public float AD
        {
            get { return this.Final.AD; }
        }   

        /// <summary>
        /// 魔法攻击
        /// </summary>
        public float AP
        {
            get { return this.Final.AP; }
        }

        /// <summary>
        /// 物理防御
        /// </summary>
        public float DEF
        {
            get { return this.Final.DEF; }
        }

        /// <summary>
        /// 魔法防御
        /// </summary>
        public float MDEF
        {
            get { return this.Final.MDEF; }
        }

        /// <summary>
        /// 攻击速度
        /// </summary>
        public float SPD
        {
            get { return this.Final.SPD; }
        }

        /// <summary>
        /// 暴击概率
        /// </summary>
        public float CRI
        {
            get { return this.Final.CRI; }
        }

        /// <summary>
        /// 初始化角色属性
        /// </summary>
        /// <param name="define"></param>
        /// <param name="level"></param>
        /// <param name="equips"></param>
        /// <param name="dynamicAttr"></param>
        public void Init(NAttributeDynamic dynamicAttr,CharacterDefine define,int level,List<EquipDefine> equips)
        {
            Log.InfoFormat("Init Attributes");
            this.DynamicAttr = dynamicAttr;

            this.LoadInitAttribute(this.Initial, define);
            this.LoadGrowthAttribute(this.Growth, define);


            this.LoadEquipAttribute(this.Equip, equips);
            this.LoadEquipAttribute(this.Equip, equips);

            if (level>0)
            {
                this.Level = level;
            }
            this.InitBasicAttributes();
            this.InitSecondaryAttributes();

            this.InitFinalAttributes();
            if (this.DynamicAttr==null)
            {
                this.DynamicAttr = new NAttributeDynamic();
            //    this.HP = this.MaxHP;
            //    this.MP = this.MaxMP;
            //}
            //else
            //{
            //    this.HP=this.DynamicAttr.Hp;
            //    this.MP=this.DynamicAttr.Mp;
            }
            this.HP = this.MaxHP;
            this.MP = this.MaxMP;
        }

        /// <summary>
        /// 计算基础属性
        /// </summary>
        public void InitBasicAttributes()
        {
            for(int i=(int)AttributeType.MaxHP;i<(int)AttributeType.MAX;i++)
            {
                this.Basic.Data[i] = this.Initial.Data[i];
            }
            for(int i=(int)AttributeType.STR;i<(int)AttributeType.DEX;i++)
            {
                this.Basic.Data[i] = this.Initial.Data[i] + (this.Level - 1) * this.Growth.Data[i];//一级属性成长
                this.Buff.Data[i] += this.Equip.Data[i];//装备一级属性加成在计算属性前
            }
        }

        public void InitSecondaryAttributes()
        {
            //二级属性成长（包括装备）
            this.Basic.MaxHP=this.Basic.STR*10+this.Initial.MaxHP+this.Equip.MaxHP;
            this.Basic.MaxMP=this.Basic.INT*10+this.Initial.MaxMP+this.Equip.MaxMP;

            this.Basic.AD=this.Basic.STR*5+this.Initial.AD+this.Equip.AD;
            this.Basic.AP=this.Basic.INT*5+this.Initial.AP+this.Equip.AP;

            this.Basic.DEF=this.Basic.STR*2+this.Basic.DEX*1+this.Initial.DEF+this.Equip.DEF;
            this.Basic.MDEF=this.Basic.INT*2+this.Basic.DEX*1+this.Initial.MDEF+this.Equip.MDEF;

            this.Basic.SPD=this.Basic.DEX*0.2f+this.Initial.SPD+this.Equip.SPD;
            this.Basic.CRI=this.Basic.DEX*0.0002f+this.Initial.CRI+this.Equip.CRI;
        }

        public void InitFinalAttributes()
        {
            for (int i = (int)AttributeType.MaxHP; i < (int)AttributeType.MAX; i++)
            {
                this.Final.Data[i] = this.Basic.Data[i] + this.Buff.Data[i];
            }
        }

        public void LoadInitAttribute(AttributeData attr, CharacterDefine define)
        {
            attr.MaxHP = define.MaxHP;
            attr.MaxMP = define.MaxMP;

            attr.STR = define.STR;
            attr.INT = define.INT;
            attr.DEX = define.DEX;
            attr.AD = define.AD;
            attr.AP = define.AP;
            attr.DEF = define.DEF;
            attr.MDEF = define.MDEF;
            attr.SPD = define.SPD;
            attr.CRI = define.CRI;
        }

        private void LoadGrowthAttribute(AttributeData attr,CharacterDefine define)
        {
            attr.STR = define.GrowthSTR;
            attr.INT = define.GrowthINT;
            attr.DEX = define.GrowthDEX;
        }

        private void LoadEquipAttribute(AttributeData attr,List<EquipDefine> equips)
        {
            attr.Reset();
            if (equips == null) return;
            foreach(var equip in equips)
            {
                attr.MaxHP += equip.MaxHP;
                attr.MaxMP += equip.MaxMP;
                attr.STR += equip.STR;
                attr.INT += equip.INT;
                attr.DEX += equip.DEX;
                attr.AD += equip.AD;
                attr.AP += equip.AP;
                attr.DEF += equip.DEF;
                attr.MDEF += equip.MDEF;
                attr.SPD += equip.SPD;
                attr.CRI += equip.CRI;
            }
        }
    }
}
