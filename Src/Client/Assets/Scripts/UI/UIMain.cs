using Managers;
using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain>{

    public Text avatarName;
    public Text avaterLevel;
	protected override void OnStart () {
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

    public void OnClickTest()
    {
        UITest test=UIManager.Instance.Show<UITest>();
        test.SetTitle("测试");
        test.OnClose += Test_OnClose;
    }

    private void Test_OnClose(UIWindow sender,UIWindow.WindowResult result)
    {
        MessageBox.Show("点击了对话框的：" + result, "对话框响应结果", MessageBoxType.Information);
    }

    public void OnClickBag()
    {
        UIManager.Instance.Show<UIBag>();
    }
}
