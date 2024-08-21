using Common.Data;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStory : UIWindow
{
    public Text title;
    public Text descript;

    StoryDefine story;

    private void Start()
    { 
    }

    public void SetStory(StoryDefine story)
    {
        this.story = story;
        title.text = story.Name;
        descript.text = story.Description;
    }

    public override void OnYesClick()
    {
        base.OnYesClick();
        if (!StoryManager.Instance.StartStory(story.ID))
        {

        }
    }



    //public void OnClickStart()
    //{
    //    if(!StoryManager.Instance.StartStory(story.ID))
    //    {

    //    }
    //}
}
