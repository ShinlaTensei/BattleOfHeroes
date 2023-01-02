using System.Collections;
using System.Collections.Generic;
using Base.Helper;
using Base.Logging;
using Base.Pattern;
using UnityEngine;

namespace PaidRubik
{
    [UIModel("MapUI", SceneName.HomeScene)]
    public class MapUI : UIView
    {
        [SerializeField] private RectTransform[]    mapSpawnPoints;
        [SerializeField] private RectTransform      mapItemParent;
        [SerializeField] private MapSelectItemUI    selectItemPrefab;

        private Queue<MapSelectItemUI> _selectItems = new Queue<MapSelectItemUI>();

        public override void Populate<T>(T viewData)
        {
            this.GetLogger().Info("[MapUI] Populate data of map {0}", viewData);
            if (viewData is MapDataBlueprint data)
            {
                int poolSize = _selectItems.Count;
                int numberNeeded = data.MapItemSelectDatas.Count - poolSize;
                for (int i = 0; i < numberNeeded; ++i)
                {
                    MapSelectItemUI itemUI = Instantiate(selectItemPrefab, Vector3.zero, Quaternion.identity, mapSpawnPoints[i]);
                    itemUI.RectTransform.anchoredPosition = Vector3.zero;
                    _selectItems.Enqueue(itemUI);
                }
                for (int i = 0; i < mapSpawnPoints.Length; ++i)
                {
                    MapSelectItemUI itemUI = _selectItems.Dequeue();
                    itemUI.Install(data.Get(i + 1));
                }
            }
        }
    }
}

