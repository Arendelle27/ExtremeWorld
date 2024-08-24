using Entities;
using Managers;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

public class PlayerInputController:MonoBehaviour
{
    public Rigidbody rb;
    SkillBridge.Message.CharacterState state;

    public Creature character;

    public float rotateSpeed = 2.0f;

    public float turnAngle = 10;

    public int speed;

    public EntityController entityController;

    public bool onAir = false;

    private NavMeshAgent agent;

    private bool autoNav = false;

    public bool enableRigidbody
    {
        get { return!this.rb.isKinematic; }
        set
        {
            this.rb.isKinematic = !value;
            this.rb.detectCollisions = value;
        }
    }
    void Start()
    {
        state = CharacterState.Idle;

        if(agent==null)
        {
            agent=this.gameObject.AddComponent<NavMeshAgent>();
            agent.stoppingDistance = 2f;//停止距离
            agent.updatePosition = false;
        }
    }

    public void StartNav(Vector3 target)
    {
        StartCoroutine(BeginNav(target));
    }

    IEnumerator BeginNav(Vector3 target)
    {
        agent.updatePosition = true;
        agent.nextPosition = this.transform.position;
        agent.SetDestination(target);
        yield return null;
        autoNav = true;
        if(state!=CharacterState.Move)
        {
            state=CharacterState.Move;
            this.character.MoveForward();
            this.SendEntityEvent(EntityEvent.MoveFwd);
            agent.speed=this.character.speed/100f;
        }
    }

    public void StopNav()
    {
        autoNav = false;
        agent.ResetPath();
        if(state!=CharacterState.Idle)
        {
            state=CharacterState.Idle;
            this.rb.velocity=Vector3.zero;
            this.character.Stop();
            this.SendEntityEvent(EntityEvent.Idle);
        }
        NavPathRenderer.Instance.SetPath(null,Vector3.zero);
    }

    /// <summary>
    /// 自动寻路
    /// </summary>
    public void NavMove()
    {
        if (agent.pathPending) return;
        if(agent.pathStatus==NavMeshPathStatus.PathInvalid)
        {
            StopNav();
            return;
        }
        if (agent.pathStatus != NavMeshPathStatus.PathComplete) return;

        if(Mathf.Abs(Input.GetAxis("Vertical"))>0.01||Mathf.Abs(Input.GetAxis("Horizontal"))>0.01)
        {
            StopNav();
            return;
        }

        NavPathRenderer.Instance.SetPath(agent.path,agent.destination);
        if(agent.isStopped||agent.remainingDistance<2f)
        {
            StopNav();
            return;
        }
    }

    void FixedUpdate()
    {
        if (character == null||!character.ready||this.character.isDead)
            return;


        if(autoNav)
        {
            NavMove();
            return;
        }

        if(InputManager.Instance !=null&&InputManager.Instance.isInputMode)
        {
            return;
        }

        float v = Input.GetAxis("Vertical");
        if(v>0.01)
        {
            if(state!=SkillBridge.Message.CharacterState.Move)
            {
                state = SkillBridge.Message.CharacterState.Move;
                this.character.MoveForward();
                this.SendEntityEvent(EntityEvent.MoveFwd);
            }
            this.rb.velocity = /*this.rb.velocity.y * Vector3.up +*/ GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;
        }
        else if(v<-0.01)
        {
            if (state != SkillBridge.Message.CharacterState.Move)
            {
                state = SkillBridge.Message.CharacterState.Move;
                this.character.MoveBack();
                this.SendEntityEvent(EntityEvent.MoveBack);
            }
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;
        }
        else
        {
            if(state!=SkillBridge.Message.CharacterState.Idle)
            {
                state = SkillBridge.Message.CharacterState.Idle;
                this.rb.velocity = Vector3.zero;
                this.character.Stop();
                this.SendEntityEvent(EntityEvent.Idle);
            }
        }

        if(Input.GetButtonDown("Jump"))
        {
            this.SendEntityEvent(EntityEvent.Jump);
        }

        float h = Input.GetAxis("Horizontal");
        if(h<-0.1||h>0.1)
        {
            this.transform.Rotate(0, h * rotateSpeed, 0);
            Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
            Quaternion rot = new Quaternion();
            rot.SetFromToRotation(dir, this.transform.forward);

            if(rot.eulerAngles.y>this.turnAngle&&rot.eulerAngles.y<(360-this.turnAngle))
            {
                character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
                rb.transform.forward = this.transform.forward;
                this.SendEntityEvent(EntityEvent.None);
            }
        }
        //Debug.LogFormat("velocity {0}", this.rb.velocity.magnitude);
    }

    Vector3 lastPos;
    float lastSync = 0;
    private void LateUpdate()//查值
    {
        if(this.character==null||!this.character.ready)
        {
            return;
        }
        Vector3 offset = this.rb.transform.position - lastPos;
        this.speed = (int)(offset.magnitude * 100f / Time.deltaTime);
        this.lastPos = this.rb.transform.position;

        if((GameObjectTool.WorldToLogic(this.rb.transform.position)-this.character.position).magnitude>50)
        {
            this.character.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));
            this.SendEntityEvent(EntityEvent.None);
        }
        this.transform.position = this.rb.transform.position;

        Vector3 dir=GameObjectTool.LogicToWorld(this.character.direction);
        Quaternion rot = new Quaternion();
        rot.SetFromToRotation(dir,this.transform.forward);

        if(rot.eulerAngles.y>this.turnAngle&&rot.eulerAngles.y<(360-this.turnAngle))
        {
            character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
            this.SendEntityEvent(EntityEvent.None);
        }
    }

    public void SendEntityEvent(EntityEvent entityEvent,int param=0)
    {
        if(entityController!=null)
        {
            entityController.OnEntityEvent(entityEvent,param);
        }
        MapService.Instance.SendMapEntitySync(entityEvent, this.character.EntityData,param);
    }

    public void OnLeaveLevel()
    {
        this.enableRigidbody = false;
        this.rb.velocity = Vector3.zero;
    }

    public void OnEnterLevel()
    {
        if(this.rb!=null)
        {
            this.rb.velocity = Vector3.zero;
        }
        this.entityController.UpdateTransform();
        this.lastPos = this.rb.transform.position;
        this.enableRigidbody = true;
    }
}