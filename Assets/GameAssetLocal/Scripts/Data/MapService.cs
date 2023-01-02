using System;
using System.Collections;
using System.Collections.Generic;
using Base.Helper;
using Base.Pattern;
using UnityEngine;

namespace PaidRubik
{
    [Serializable]
    public class MapItemSelectData
    {
        public int Reward;
        public int Index;
        public string Status;
    }

    public class MapDataBlueprint : IViewData
    {
        public string ID;
        public List<MapItemSelectData> MapItemSelectDatas = new List<MapItemSelectData>();

        public MapItemSelectData Get(int index)
        {
            return MapItemSelectDatas.Find(item => item.Index == index);
        }
    }
    
    [Serializable]
    public class MapDataConfig
    {
        public string ID;
        public MapItemSelectData[] Data;
    }
    public class MapService : IService, ISerialize<List<MapDataConfig>>
    {
        private Dictionary<int, MapDataBlueprint> _mapDataBlueprints;
        private int _currentMapID;

        public string MapConfigKey = "MapData";

        public void Init()
        {
            _mapDataBlueprints = new Dictionary<int, MapDataBlueprint>();
        }

        public void DeInit()
        {
            foreach (var data in _mapDataBlueprints.Values)
            {
                data.MapItemSelectDatas.Clear();
            }
            _mapDataBlueprints.Clear();
        }

        public List<MapDataConfig> To()
        {
            throw new NotImplementedException();
        }

        public void From(List<MapDataConfig> data)
        {
            for (int i = 0; i < data.Count; ++i)
            {
                bool status = _mapDataBlueprints.TryGetValue(i + 1, out MapDataBlueprint result);
                if (status)
                {
                    result.MapItemSelectDatas.Clear();
                    result.MapItemSelectDatas.AddRange(data[i].Data);
                    result.ID = data[i].ID;
                }
                else
                {
                    MapDataBlueprint mapData = new MapDataBlueprint();
                    mapData.ID = data[i].ID;
                    mapData.MapItemSelectDatas.AddRange(data[i].Data);
                    _mapDataBlueprints.TryAdd(i + 1, mapData);
                }
            }

            _currentMapID = 1;
        }

        public MapDataBlueprint CurrentMap()
        {
            _mapDataBlueprints.TryGetValue(_currentMapID, out MapDataBlueprint data);
            return data;
        }

        public MapDataBlueprint NextMap()
        {
            _currentMapID = Mathf.Clamp(_currentMapID + 1, 1, 3);
            _mapDataBlueprints.TryGetValue(_currentMapID, out MapDataBlueprint data);
            return data;
        }
        
        public MapDataBlueprint PreviousMap()
        {
            _currentMapID = Mathf.Clamp(_currentMapID - 1, 1, 3);
            _mapDataBlueprints.TryGetValue(_currentMapID, out MapDataBlueprint data);
            return data;
        }

        public void Raise()
        {
            throw new NotImplementedException();
        }
    }
}

