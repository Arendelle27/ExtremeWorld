﻿using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MapTool : MonoBehaviour {

	[MenuItem("Map Tools/Export Telepoters")]
    public static void ExportTelepoters()
    {
        DataManager.Instance.Load();

        Scene current = EditorSceneManager.GetActiveScene();
        string currentScnee = current.name;
        if(current.isDirty)
        {
            EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确定");
            return;
        }

        List<TelepertorObject> allTeleporters = new List<TelepertorObject>();

        foreach(var map in DataManager.Instance.Maps)
        {
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            if(!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("Scene {0} not existed!", sceneFile);
                continue;
            }
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);

            TelepertorObject[] teleporters = GameObject.FindObjectsOfType<TelepertorObject>();
            foreach(var teleporter in teleporters)
            {
                if(!DataManager.Instance.Teleporters.ContainsKey(teleporter.ID))
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图：{0} 中配置的 Teleporter:[{1}]中不存在", map.Value.Resource, teleporter.ID), "确定");
                    return;
                }

                TeleporterDefine def = DataManager.Instance.Teleporters[teleporter.ID];
                if (def.MapID != map.Value.ID)
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图：{0} 中配置的 Teleporter:[{1}] MapID:{2} 错误", map.Value.Resource, teleporter.ID, def.MapID), "确定");
                    return;
                }
                def.Position = GameObjectTool.WorldToLogicN(teleporter.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(teleporter.transform.forward);
            }
        }
        DataManager.Instance.SaveTeleporters();
        EditorSceneManager.OpenScene("Assets/Levels/" + currentScnee + ".unity");
        EditorUtility.DisplayDialog("提示", "传送点导出完成", "确定");
    }

    [MenuItem("Map Tools/Export SpawnPoint")]
    public static void ExportSpawnPoints()
    {
        DataManager.Instance.Load();

        Scene current = EditorSceneManager.GetActiveScene();
        string currentScnee = current.name;
        if (current.isDirty)
        {
            EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确定");
            return;
        }

        List<SpawnPoint> allSpawnPoint = new List<SpawnPoint>();

        foreach (var map in DataManager.Instance.Maps)
        {
            string sceneFile = "Assets/Levels/" + map.Value.Resource + ".unity";
            if (!System.IO.File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("Scene {0} not existed!", sceneFile);
                continue;
            }
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);

            SpawnPoint[] SpawnPoints = GameObject.FindObjectsOfType<SpawnPoint>();

            if(!DataManager.Instance.SpawnPoints.ContainsKey(map.Value.ID))
            {
                DataManager.Instance.SpawnPoints[map.Value.ID]=new Dictionary<int, SpawnPointDefine>();
            }
            foreach (var SpawnPoint in SpawnPoints)
            {
                if (!DataManager.Instance.SpawnPoints.ContainsKey(SpawnPoint.ID))
                {
                    DataManager.Instance.SpawnPoints[map.Value.ID][SpawnPoint.ID] = new SpawnPointDefine();
                }

                SpawnPointDefine def = DataManager.Instance.SpawnPoints[map.Value.ID][SpawnPoint.ID];
                def.ID=SpawnPoint.ID;
                def.MapID = map.Value.ID;
                def.Position = GameObjectTool.WorldToLogicN(SpawnPoint.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(SpawnPoint.transform.forward);
            }
        }
        DataManager.Instance.SaveSpawnPoints();
        EditorSceneManager.OpenScene("Assets/Levels/" + currentScnee + ".unity");
        EditorUtility.DisplayDialog("提示", "刷怪点点导出完成", "确定");
    }

    [MenuItem("Map Tools/Generate NavData")]
    public static void GenerateNavData()
    {
        Material red=new Material(Shader.Find("Particles/Alpha Blended"));
        red.color=Color.red;
        red.SetColor("_TintColor",Color.red);
        red.enableInstancing=true;
        GameObject go = GameObject.Find("MinimapBoundingBox");
        if(go!=null)
        {
            GameObject root = new GameObject("Root");
            BoxCollider bound=go.GetComponent<BoxCollider>();
            float step = 1f;
            for(float x=bound.bounds.min.x;x<bound.bounds.max.x;x+=step)
            {
                for(float z=bound.bounds.min.z;z<bound.bounds.max.z;z+=step)
                {
                    for(float y=bound.bounds.max.y;y>bound.bounds.max.y+10f;y+=step)
                    {
                        Vector3 pos = new Vector3(x,y,z);
                        NavMeshHit hit;

                        if(NavMesh.SamplePosition(pos,out hit,0.5f,NavMesh.AllAreas))
                        {
                            if(hit.hit)
                            {
                                var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                box.name = "Hit" + hit.mask;
                                box.GetComponent<MeshRenderer>().sharedMaterial = red;
                                box.transform.SetParent(root.transform,true);
                                box.transform.position = pos;
                                box.transform.localScale=Vector3.one*0.9f;
                            }
                        }
                    }
                }
            }
        }
    }
}
