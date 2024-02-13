using Models;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UIRIDE;
using UnityEngine;
using UnityEngine.UI;

namespace UIRIDE
{    public class UIRideItem : ListView.ListViewItem
    {
        public Image icon;
        public Text title;
        public Text level;

        public Image background;
        public Sprite normalBg;
        public Sprite selectedBg;

        public override void onSelected(bool selected)
        {
            if (this.background == null)
                return;
            background.overrideSprite = selected ? selectedBg : normalBg;
        }

        public Item item;


        public void SetEquipItem(Item item, UIRide owner, bool equiped)
        {
            this.item = item;

            if (this.title != null) this.title.text = item.Define.Name;
            if (this.level != null) this.level.text = item.Define.Level.ToString();
            if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon);
        }
    }
}
