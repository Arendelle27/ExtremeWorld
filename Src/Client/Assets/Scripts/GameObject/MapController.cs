﻿using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

    public Collider minimapBoundingbox;
	void Start () {
        MiniMapManager.Instance.UpdateMinimap(minimapBoundingbox);
	}
	
}
