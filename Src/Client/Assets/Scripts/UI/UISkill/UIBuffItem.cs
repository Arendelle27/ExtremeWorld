using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuffItem : MonoBehaviour
{
    public Image icon;
    public Image overlay;
    public Text label;
    Buff buff;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(this.buff==null)
        {
            return;
        }
        if(this.buff.time>0)
        {
            if(!overlay.enabled)
            {
                overlay.gameObject.SetActive(true);
            }
            if(!label.enabled)
            {
                label.gameObject.SetActive(true);
            }

            overlay.fillAmount = this.buff.time / this.buff.Define.Duration;
            this.label.text=((int)Mathf.Ceil(this.buff.Define.Duration - this.buff.time)).ToString();
        }
        else
        {
            if (overlay.enabled)
            {
                overlay.enabled = false;
            }
            if (this.label.enabled)
            {
                this.label.enabled = false;
            }
        }
    }

    internal void SetItem(Buff buff)
    {
        this.buff = buff;
        if(this.icon!=null)
        {
            this.icon.overrideSprite=Resloader.Load<Sprite>(buff.Define.Icon);
            this.icon.SetAllDirty();
        }
    }
}
