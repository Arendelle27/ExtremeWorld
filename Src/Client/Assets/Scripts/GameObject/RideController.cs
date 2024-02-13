using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideController : MonoBehaviour
{
    public Transform mountPoint;
    public EntityController rider;
    public Vector2 offset;
    private Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        this.anim=this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(this.mountPoint==null||this.rider==null)
        {
            return;
        }
        this.rider.SetRidePosition(this.mountPoint.position+this.mountPoint.TransformDirection(this.offset));
    }

    public void SetRider(EntityController rider)
    {
        this.rider=rider;
    }

    public void OnEntityEvent(EntityEvent entityEvent, int param)
    {
        switch(entityEvent)
        {
            case EntityEvent.Idle:
                this.anim.SetBool("Move",false);
                break;
            case EntityEvent.MoveFwd:
                this.anim.SetBool("Move",true);
                break;
            case EntityEvent.MoveBack:
                this.anim.SetBool("Move",true);
                break;
            case EntityEvent.Jump:
                this.anim.SetTrigger("Jump");
                break;
        }
    }

}
