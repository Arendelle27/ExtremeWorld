using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Battle
{
    public class AttributeData
    {
        public float[] Data=new float[(int)AttributeType.MAX];

        /// <summary>
        /// 最大生命
        /// </summary>
        public float MaxHP 
        {   
            get { return this.Data[(int)AttributeType.MaxHP]; }
            set { this.Data[(int)AttributeType.MaxHP] = value; }
        }

        /// <summary>
        /// 最大法力
        /// </summary>
        public float MaxMP
        {
            get { return this.Data[(int)AttributeType.MaxMP]; }
            set { this.Data[(int)AttributeType.MaxMP] = value; }
        }

        /// <summary>
        /// 力量
        /// </summary>
        public float STR
        {
            get { return this.Data[(int)AttributeType.STR]; }
            set { this.Data[(int)AttributeType.STR] = value; }
        }

        /// <summary>
        /// 智力
        /// </summary>
        public float INT
        {
            get { return this.Data[(int)AttributeType.INT]; }
            set { this.Data[(int)AttributeType.INT] = value; }
        }

        /// <summary>
        /// 敏捷
        /// </summary>
        public float DEX
        {
            get { return this.Data[(int)AttributeType.DEX]; }
            set { this.Data[(int)AttributeType.DEX] = value; }
        }

        /// <summary>
        /// 物理攻击
        /// </summary>
        public float AD
        {
            get { return this.Data[(int)AttributeType.AD]; }
            set { this.Data[(int)AttributeType.AD] = value; }
        }

        /// <summary>
        /// 魔法攻击
        /// </summary>
        public float AP
        {
            get { return this.Data[(int)AttributeType.AP]; }
            set { this.Data[(int)AttributeType.AP] = value; }
        }

        /// <summary>
        /// 物理防御力
        /// </summary>
        public float DEF
        {
            get { return this.Data[(int)AttributeType.DEF]; }
            set { this.Data[(int)AttributeType.DEF] = value; }
        }

        /// <summary>
        /// 法术防御
        /// </summary>
        public float MDEF
        {
            get { return this.Data[(int)AttributeType.MDEF]; }
            set { this.Data[(int)AttributeType.MDEF] = value; }
        }

        /// <summary>
        /// 攻击速度
        /// </summary>
        public float SPD
        {
            get { return this.Data[(int)AttributeType.SPD]; }
            set { this.Data[(int)AttributeType.SPD] = value; }
        }

        /// <summary>
        /// 暴击率
        /// </summary>
        public float CRI
        {
            get { return this.Data[(int)AttributeType.CRI]; }
            set { this.Data[(int)AttributeType.CRI] = value; }
        }   

        public void Reset()
        {
            for (int i = 0; i < (int)AttributeType.MAX; i++)
            {
                this.Data[i] = 0;
            }
        }
    }
}
