using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Entities;
using Managers;
using SkillBridge.Message;
using UnityEngine;

public class EntityController : MonoBehaviour, IEntityNotify, IEntityController
{
    public Animator anim;
    public Rigidbody rb;
    private AnimatorStateInfo currentBaseState;

    public Entity entity;

    public Vector3 position;
    public Vector3 direction;
    Quaternion rotation;

    public Vector3 lastPosition;
    Quaternion lastRotation;

    public float speed;
    public float animSpeed = 1.5f;
    public float jumpPower = 3.0f;

    public bool isPlayer = false;

    public RideController rideController;

    private int currentRide = 0;

    public Transform rideBone;

    public EntityEffectManager EffectMgr;

    void Start()
    {
        if(entity!=null)
        {
            EntityManager.Instance.RegisterEntityChangeNotify(entity.entityId, this);
            this.UpdataTransform();
        }

        if (!this.isPlayer)
            rb.useGravity = false;
    }

    private void UpdataTransform()
    {
        this.position = GameObjectTool.LogicToWorld(entity.position);
        this.direction = GameObjectTool.LogicToWorld(entity.direction);

        this.rb.MovePosition(this.position);
        this.transform.forward = this.direction;
        this.lastPosition = this.position;
        this.lastRotation = this.rotation;
    }

    void OnDestroy()
    {
        if (entity != null)
            Debug.LogFormat("{0} OnDestory:ID:{1} POS:{2} DIR:{3} SPD:{4}", this.name, entity.entityId, entity.position, entity.direction, entity.speed);
        if(UIWorldElementManager.Instance!=null)
        {
            UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
        }
    }

    void FixedUpdate()
    {
        if (this.entity == null)
            return;

        this.entity.OnUpdate(Time.fixedDeltaTime);

        if(!this.isPlayer)
        {
            this.UpdataTransform();
        }
    }

    public void OnEntityRemoved()
    {
        if (UIWorldElementManager.Instance != null)
            UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
        Destroy(this.gameObject);
    }

    public void OnEntityEvent(EntityEvent entityEvent,int param)
    {
        switch (entityEvent)
        {
            case EntityEvent.Idle:
                anim.SetBool("Move", false);
                anim.SetTrigger("Idle");
                Creature target = this.entity as Creature;
                target.BattleState = false;
                break;
            case EntityEvent.MoveFwd:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.MoveBack:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.Jump:
                anim.SetTrigger("Jump");
                break;
            case EntityEvent.Ride:
                {
                    this.Ride(param);
                }
                break;
        }
        if(this.rideController!=null)
        {
            this.rideController.OnEntityEvent(entityEvent, param);
        }
    }

    public void Ride(int rideId)
    {
        if (currentRide == rideId) return;
        currentRide = rideId;

        if(rideId>0)
        {
            this.rideController=GameObjectManager.Instance.LoadRide(rideId, this.transform);
        }
        else
        {
            Destroy(this.rideController.gameObject);
            this.rideController = null;
        }

        if(this.rideController==null)
        {
            this.anim.transform.localPosition = Vector3.zero;
            this.anim.SetLayerWeight(1, 0);
        }
        else
        {
            this.rideController.SetRider(this);
            this.anim.SetLayerWeight(1, 1);
        }
    }

    public void SetRidePosition(Vector3 position)
    {
        this.anim.transform.position=position+(this.anim.transform.position-this.rideBone.position);
    }

    void OnMouseDown()
    {
        Creature target= this.entity as Creature;
        if (target.IsCurrentPlayer)
            return;

        BattleManager.Instance.CurrentTarget=this.entity as Creature;
    }

    public void PlayAnim()
    {
        this.anim.SetTrigger(name);
    }

    public void SetSyandby(bool standby)
    {
        this.anim.SetBool("Standby", standby);
    }

    public void OnEntityChanged(Entity entity)
    {
        Debug.LogFormat("OnEntityChanged:ID:{0} POS:{1} DIR:{2} SPD:{3}", entity.entityId, entity.position, entity.direction, entity.speed);
    }

    public void PlayAnim(string name)
    {
        this.anim.SetTrigger(name);
    }

    public void SetStandby(bool standby)
    {
        this.anim.SetBool("Standby", standby);
    }

    public void UpdateDirection()
    {
        this.direction=GameObjectTool.LogicToWorld(entity.direction);
        this.transform.forward=this.direction;
        this.lastRotation=this.rotation;
    }

    public void PlayEffect(EffectType type, string name, Entity target, float duration)
    {
        Transform transform = target.Controller.GetTransform();
        this.EffectMgr.PlayEffect(type, name, transform, duration);
    }

    public Transform GetTransform()
    {
        return this.transform;
    }
}