using Entities;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameObjectManager : MonoBehaviour {

    Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();
	void Start () {
        StartCoroutine(InitGameObjects());
        CharacterManager.Instance.OnCharacterEnter = OnCharacterEmter;
	}

    private void OnDestroy()
    {
        CharacterManager.Instance.OnCharacterEnter = null;
    }

    private void OnCharacterEmter(Character cha)
    {
        CreateCharacterObjection(cha);
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
        if(!Characters.ContainsKey(character.Info.Id)||Characters[character.Info.Id]==null)
        {
            Object obj = Resloader.Load<Object>(character.Define.Resource);
            if(obj==null)
            {
                Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.", character.Define.TID, character.Define.Resource);
                return;
            }
            GameObject go = (GameObject)Instantiate(obj);
            go.name = "Character_" + character.Info.Id + "_" + character.Info.Name;

            go.transform.position = GameObjectTool.LogicToWorld(character.position);
            go.transform.forward = GameObjectTool.LogicToWorld(character.direction);
            Characters[character.Info.Id] = go;

            EntityController ec = go.GetComponent<EntityController>();
            if(ec!=null)
            {
                ec.entity = character;
                ec.isPlayer = character.IsPlayer;
            }

            PlayerInputController pc = go.GetComponent<PlayerInputController>();
            if(pc!=null)
            {
                if(character.Info.Id==Models.User.Instance.CurrentCharacter.Id)//判断是否是当前玩家控制的角色
                {
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
            UIWorldElementManager.Instance.AddCharacterNameBar(go.transform, character);
        }
    }
}
