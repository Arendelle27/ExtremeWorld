using Entities;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICreatureInfo : MonoBehaviour
{
    public Text Name;
    public Slider HPBar;
    public Slider MPBar;
    public Text HPText;
    public Text MPText;
    public Image Avatar;

    public UIBuffIcons buffIcons;
    void Start()
    {
        
    }

    private Creature target;
    public Creature Target
    {
        get { return this.target; }
        set
        {
            this.target = value;
            buffIcons.SetOwner(value);
            this.UpdateUI();
        }
    }

    public void UpdateUI()
    {
        if (this.target == null)
            return;
        this.Name.text = string.Format("{0} Lv.{1}", this.target.Name, this.target.Info.Level);

        this.HPBar.maxValue = this.target.Attributes.MaxHP;
        this.HPBar.value = this.target.Attributes.HP;
        this.HPText.text = string.Format("{0}/{1}", this.target.Attributes.HP, this.target.Attributes.MaxHP);

        this.MPBar.maxValue = this.target.Attributes.MaxMP;
        this.MPBar.value = this.target.Attributes.MP;
        this.MPText.text = string.Format("{0}/{1}", this.target.Attributes.MP, this.target.Attributes.MaxMP);


        string classText = "";
        if (this.target.Define.Type == CharacterType.Player)
        {
            classText = this.target.Define.Class.ToString();
        }
        else
        {
            classText = "Monster";
        }

        if (this.Avatar.sprite.name != classText)
        {
            this.Avatar.sprite = Resloader.Load<Sprite>("UI/Avatars/" + classText);
        }
    }

    void Update()
    {
        this.UpdateUI();
    }

    
}
