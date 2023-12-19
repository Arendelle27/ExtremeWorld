using UnityEngine;

internal class InputBox
{
    static Object cacheObject = null;

    public static UIInputBox Show(string message, string title = "", string btnOK="",string btnCancel="",string emptyTips="")
    {
        if (cacheObject == null)
        {
            cacheObject = Resloader.Load<Object>("UI/UIInputBox");
        }

        GameObject go = (GameObject)GameObject.Instantiate(cacheObject);
        UIInputBox input = go.GetComponent<UIInputBox>();
        input.Init(title, message, btnOK, btnCancel,emptyTips);
        return input;
    }
}