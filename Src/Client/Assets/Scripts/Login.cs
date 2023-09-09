using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Network.NetClient.Instance.Init("127.0.0.1", 8000);
        Network.NetClient.Instance.Connect();

        SkillBridge.Message.NetMessage msg= new SkillBridge.Message.NetMessage();//构建主消息
        msg.Request = new SkillBridge.Message.NetMessageRequest();//构建请求消息
        msg.Request.firstRequest = new SkillBridge.Message.FirstTestRequest();//构建自己写的消息
        msg.Request.firstRequest.Hellowworld = "helloworld";//填上对应的值
        Network.NetClient.Instance.SendMessage(msg);//发送
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
