using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// A behaviour that is attached to a playable
public class MainUIActivation : ActivationControlPlayable
{
    public bool active;
    private bool activation;
    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
        Debug.Log("OnGraphStart");
        if(UIMain.Instance==null)
        {
            return;
        }
        this.activation = UIMain.Instance.Show;
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        Debug.Log("OnGraphStop");
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);
        Debug.Log("OnBehaviourPlay");
        if(UIMain.Instance==null)
        {
            return;
        }
        UIMain.Instance.Show = active;
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if(UIMain.Instance==null)
        {
            return;
        }
        if(this.postPlayback==PostPlaybackState.Active)
        {
            UIMain.Instance.Show = true;
        }
        else if(this.postPlayback==PostPlaybackState.Inactive)
        {
            UIMain.Instance.Show = false;
        }
        else if(this.postPlayback==PostPlaybackState.Revert)
        {
            UIMain.Instance.Show = this.activation;
        }
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }
}
