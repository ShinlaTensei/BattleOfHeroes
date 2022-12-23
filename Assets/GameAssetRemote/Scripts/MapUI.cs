using System.Collections;
using System.Collections.Generic;
using Base.Helper;
using UnityEngine;

namespace PaidRubik
{
    [UIModel("MapUI", SceneName.HomeScene)]
    public class MapUI : UIView
    {
        [SerializeField] private RectTransform[] mapSpawnPoints;
    }
}

