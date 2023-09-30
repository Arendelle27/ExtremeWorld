using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class MiniMapManager : Singleton<MiniMapManager>
    {
        public Sprite LoadCurrentMinimap()
        {
            return Resloader.Load<Sprite>("UI/Minimap" + User.Instance.CurrentMapData.MiniMap);
        }
    }
}

