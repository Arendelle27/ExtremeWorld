using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour {
    public Collider miniMapBoundingBox;
    public Image miniMap;
    public Image arrow;
    public Text mapName;

    private Transform playerTransform;
	void Start () {
        this.MapInit();
	}

    void MapInit()
    {
        this.mapName.text = User.Instance.CurrentMapData.Name;
        if (this.miniMap.overrideSprite == null)
        {
            this.miniMap.overrideSprite = MiniMapManager.Instance.LoadCurrentMinimap();
        }
        this.miniMap.SetNativeSize();
        this.miniMap.transform.localPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update () {
        if (this.playerTransform == null)
        {
            this.playerTransform = MiniMapManager.Instance.PlayerTranform;
        }

        if (this.playerTransform == null || this.miniMapBoundingBox == null) return;

        float realWidth = miniMapBoundingBox.bounds.size.x;
        float realHeight = miniMapBoundingBox.bounds.size.z;

        float relaX = playerTransform.position.x - miniMapBoundingBox.bounds.min.x;
        float relaY = playerTransform.position.z - miniMapBoundingBox.bounds.min.z;

        float pivotX = relaX / realWidth;
        float pivotY = relaY / realHeight;

        this.miniMap.rectTransform.pivot = new Vector2(pivotX, pivotY);
        this.miniMap.rectTransform.localPosition = Vector2.zero;
        this.arrow.transform.eulerAngles = new Vector3(0, 0, -playerTransform.eulerAngles.y);
    }

}
