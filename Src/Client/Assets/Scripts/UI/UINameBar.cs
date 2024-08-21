using Entities;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINameBar : MonoBehaviour {

    public Image avatar;
    public Text characterName; 

    public Character character;

    public UIBuffIcons buffIcons;

	// Use this for initialization
	void Start () {
        if( character!=null)
        {
            buffIcons.SetOwner(character);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (this.character == null)
            return;
        this.UpdateInfo();
        if(character.Attributes.HP<=0)
        {
            this.gameObject.SetActive(false);
        }
	}

    void UpdateInfo()
    {
        if(this.character!=null)
        {
            string name = this.character.Name + "Lv." + this.character.Info.Level;
            if(name!=this.characterName.text)
            {
                this.characterName.text = name;
            }


            string classText = "";
            if (this.character.Define.Type == CharacterType.Player)
            {
                classText = this.character.Define.Class.ToString();
            }
            else
            {
                classText = "Monster";
            }

            if (this.avatar.sprite.name != classText)
            {
                this.avatar.sprite = Resloader.Load<Sprite>("UI/Avatars/" + classText);
            }
        }
    }
}
