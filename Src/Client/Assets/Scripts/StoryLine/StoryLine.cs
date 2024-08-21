using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[NotKeyable]
public class StoryLine : PlayableAsset
{
    public bool active = true;
    public ActivationControlPlayable.PostPlaybackState postPlayback;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var Playable = ScriptPlayable<MainUIActivation>.Create(graph);
        Playable.GetBehaviour().active = active;
        Playable.GetBehaviour().postPlayback = postPlayback;
        return Playable;
    }
}
