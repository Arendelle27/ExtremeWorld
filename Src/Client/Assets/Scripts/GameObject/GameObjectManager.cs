using Entities;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameObjectManager : MonoSingleton<GameObjectManager> {

    Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();
	protected override void OnStart () {//单例的Mono对象中不能写Start
        StartCoroutine(InitGameObjects());
        CharacterManager.Instance.OnCharacterEnter += OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave += OnCharacterLeave;
    }


    private void OnDestroy()
    {
        CharacterManager.Instance.OnCharacterEnter -= OnCharacterEnter;
        CharacterManager.Instance.OnCharacterLeave -= OnCharacterLeave;
    }

    private void OnCharacterEnter(Character cha)
    {
        CreateCharacterObjection(cha);
    }

    private void OnCharacterLeave(Character cha)
    {
        if (!Characters.ContainsKey(cha.entityId))
            return;
        if(Characters[cha.entityId]!=null)
        {
            Destroy(Characters[cha.entityId]);
            this.Characters.Remove(cha.entityId);
        }
    }

    IEnumerator InitGameObjects()
    {
        foreach(var cha in CharacterManager.Instance.Characters.Values)
        {
            CreateCharacterObjection(cha);
                yield return null;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}


    private void CreateCharacterObjection(Character character)
    {
        if(!Characters.ContainsKey(character.entityId) ||Characters[character.entityId]==null)
        {
            Object obj = Resloader.Load<Object>(character.Define.Resource);
            if(obj==null)
            {
                Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.", character.Define.TID, character.Define.Resource);
                return;
            }
            GameObject go = (GameObject)Instantiate(obj,this.transform);
            go.name = "Character_" + character.Id + "_" + character.Info.Name;
            Characters[character.entityId] = go;
            UIWorldElementManager.Instance.AddCharacterNameBar(go.transform, character);
        }
        this.InitGameObject(Characters[character.entityId], character);
    }

    private void InitGameObject(GameObject go,Character character)
    {
        go.transform.position = GameObjectTool.LogicToWorld(character.position);
        go.transform.forward = GameObjectTool.LogicToWorld(character.direction);

        EntityController ec = go.GetComponent<EntityController>();
        if (ec != null)
        {
            ec.entity = character;
            ec.isPlayer = character.IsCurrentPlayer;
        }

        PlayerInputController pc = go.GetComponent<PlayerInputController>();
        if (pc != null)
        {
            if (character.IsCurrentPlayer)//判断是否是当前玩家控制的角色
            {
                User.Instance.CurrentCharacterObject = go;
                MainPlayerCamera.Instance.player = go;
                pc.enabled = true;
                pc.character = character;
                pc.entityController = ec;
            }
            else
            {
                pc.enabled = false;
            }
        }
    }
}
