using Managers;
using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainCity : MonoBehaviour {

    public Text avatarName;
    public Text avaterLevel;
	void Start () {
        this.UpdateAvater();
	}
	
    void UpdateAvater()
    {
        this.avatarName.text = string.Format("{0}[{1}]", User.Instance.CurrentCharacter.Name, User.Instance.CurrentCharacter.Id);
        this.avaterLevel.text = User.Instance.CurrentCharacter.Level.ToString();
    }

	// Update is called once per frame
	void Update () {
		
	}

    public void BackToCharacterSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");
        UserService.Instance.SendGameLeave();
    }
}
