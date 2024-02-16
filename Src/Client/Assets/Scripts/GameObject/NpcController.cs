using Managers;
using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using UnityEditor;

public class NpcController : MonoBehaviour {
	public int npcID;

	SkinnedMeshRenderer renderer;
	Animator anim;
	Color orignColor;

	private bool inInteractive = false;

	NpcDefine npc;

	NpcQuestStatus questStatus;

	void Start () {
		renderer = this.GetComponentInChildren<SkinnedMeshRenderer>();
		anim=this.GetComponent<Animator>();
		orignColor=renderer.sharedMaterial.color;
		npc = NpcManager.Instance.GetNpcDefine(npcID);
		NpcManager.Instance.UpdateNpcPosition(this.npcID,this.transform.position);
		this.StartCoroutine(Actions());
		RefreshNpcStatus();
		QuestManager.Instance.OnQuestStatusChanged += OnQuestStatusChanged;
	}
	
	void OnQuestStatusChanged(Quest quest)
	{
		this.RefreshNpcStatus();
	}

	void RefreshNpcStatus()
	{
        this.questStatus=QuestManager.Instance.GetQuestStatusByNpc(npc.ID);
		UIWorldElementManager.Instance.AddNpcQuestStatus(this.transform,this.questStatus);
    }

    private void OnDestroy()
    {
        QuestManager.Instance.OnQuestStatusChanged -= OnQuestStatusChanged;
		if(UIWorldElementManager.Instance!=null)
		{
			   UIWorldElementManager.Instance.RemoveNpcQuestStatus(this.transform);
		}
    }

    IEnumerator Actions()
	{
		while(true)
		{
			if (inInteractive)
				yield return new WaitForSeconds(2f);
			else
				yield return new WaitForSeconds(Random.Range(5f,10f));
			this.Relax();
		}
	}

	void Update () {
		
	}

	void Relax()
	{
        anim.SetTrigger("Relax");
    }

	void Interactive()
	{
		if(!inInteractive)
		{
			inInteractive=true;
			StartCoroutine(DoInteractive());
		}
	}

	IEnumerator DoInteractive()
	{
		yield return FaceToPlayer();
		if(NpcManager.Instance.Interactive(npc))
		{
			anim.SetTrigger("Talk");
		}
		yield return new WaitForSeconds(3f);
		inInteractive=false;
	}

	IEnumerator FaceToPlayer()
	{
		Vector3 faceTo=(User.Instance.CurrentCharacterObject.transform.position-transform.position).normalized;
		while(Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward,faceTo))>5f)
		{
            this.gameObject.transform.forward=Vector3.Lerp(this.gameObject.transform.forward,faceTo,5f*Time.deltaTime);
            yield return null;
        }
	}


	void OnMouseDown()
	{
		if(Vector3.Distance(this.transform.position,User.Instance.CurrentCharacterObject.transform.position)>2f)
		{
			User.Instance.CurrentCharacterObject.StartNav(this.transform.position);
		}
		Interactive();
	}

	private void OnMouseOver()
	{
		Highlight(true);
    }

	private void OnMouseEnter()
	{
        Highlight(true);
    }

	private void OnMouseExit()
	{
        Highlight(false);
    }

	void Highlight(bool highlight)
	{
        if(highlight)
		{
			if(renderer.sharedMaterial.color!=Color.white)
			{
                renderer.sharedMaterial.color=Color.white;
            }
        }
        else
		{
            renderer.sharedMaterial.color=orignColor;
        }
    }
}
