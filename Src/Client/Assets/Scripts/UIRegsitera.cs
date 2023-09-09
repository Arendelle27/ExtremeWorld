using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using UnityEngine.UI;

public class UIRegsitera : MonoBehaviour {

    public InputField username;
    public InputField password;
    public InputField passwordConfirm;
    public Button buttonRegister;

    public GameObject uiLogin;
    // Use this for initialization
    void Start () {
        UserService.Instance.OnRegister = OnRegister;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnRegister(Result result,string message)
    {
        if(result==Result.Success)
        {
            MessageBox.Show("注册成功，请登录", "提示", MessageBoxType.Information).OnYes = this.CloseRegister;
        }
        else
        {
            MessageBox.Show(message, "错误", MessageBoxType.Error);
        }
    }

    public void OnClickRegister()
    {
        if (string.IsNullOrEmpty(this.username.text))
        {
            MessageBox.Show("请输入账号");
            return;
        }
        if (string.IsNullOrEmpty(this.password.text))
        {
            MessageBox.Show("请输入密码");
            return;
        }
        if (string.IsNullOrEmpty(this.passwordConfirm.text))
        {
            MessageBox.Show("请输入确认密码");
            return;
        }
        if(this.password.text!=this.passwordConfirm.text)
        {
            MessageBox.Show("两次输的密码不一致");
            return;
        }

        UserService.Instance.SendRegister(this.username.text, this.password.text);
    }

    void CloseRegister()
    {
        this.gameObject.SetActive(false);
        uiLogin.SetActive(true);
    }
}
