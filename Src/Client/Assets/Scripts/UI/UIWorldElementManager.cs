using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager>
{

    public GameObject nameBarPrefab;

    private Dictionary<Transform, GameObject> element = new Dictionary<Transform, GameObject>();


    public void AddCharacterNameBar(Transform owner,Character character)
    {
        GameObject goNameBar = Instantiate(nameBarPrefab, this.transform);
        goNameBar.name = "NameBar" + character.entityId;
        goNameBar.GetComponent<UIWorldElement>().owner = owner;
        goNameBar.GetComponent<UINameBar>().character = character;
        goNameBar.SetActive(true);
        this.element[owner] = goNameBar;
    }

    public void RemoveCharacterNameBar(Transform owner)
    {
        if(this.element.ContainsKey(owner))
        {
            Destroy(this.element[owner]);
            this.element.Remove(owner);
        }
    }
}
