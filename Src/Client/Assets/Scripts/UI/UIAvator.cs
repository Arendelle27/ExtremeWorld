using Entities;
using Models;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAvator : MonoBehaviour
{
    public Character owner;
    public Text avatarName;
    public Text avaterLevel;

    public Slider HPBar;
    public Slider MPBar;
    public Text HPText;
    public Text MPText;

    public Image Avatar;

    public void UpdateAvater()
    {
        if(this.owner!=User.Instance.CurrentCharacter)
        {
            this.owner = User.Instance.CurrentCharacter;
        }

        if(this.owner==null)
        {
            return;
        }
        this.avatarName.text = string.Format("{0}[{1}]", this.owner.Info.Name, this.owner.Info.Id);

        string classText = this.owner.Define.Class.ToString();
        this.Avatar.sprite = Resloader.Load<Sprite>("UI/Avatars/" + classText);

        this.UpdateUI();


        
    }

    void UpdateUI()
    {
        if (this.owner == null)
        {
            return;
        }
        string level= this.owner.Info.Level.ToString();
        if (this.avaterLevel.text != level)
        {
            this.avaterLevel.text = this.owner.Info.Level.ToString();
        }


        this.HPBar.maxValue = this.owner.Attributes.MaxHP;
        this.HPBar.value = this.owner.Attributes.HP;
        this.HPText.text = string.Format("{0}/{1}", this.owner.Attributes.HP, this.owner.Attributes.MaxHP);

        this.MPBar.maxValue = this.owner.Attributes.MaxMP;
        this.MPBar.value = this.owner.Attributes.MP;
        this.MPText.text = string.Format("{0}/{1}", this.owner.Attributes.MP, this.owner.Attributes.MaxMP);

    }

    private void Update()
    {
        this.UpdateAvater();
    }
}
