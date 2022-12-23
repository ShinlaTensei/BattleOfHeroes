using System;
using System.Collections;
using System.Collections.Generic;
using Base.Pattern;
using UnityEngine;

namespace PaidRubik
{
    [Serializable]
    public class MapItemSelectData
    {
        public int Reward;
        public int Index;
    }

    public class MapDataBlueprint
    {
        public List<MapItemSelectData> MapItemSelectDatas = new List<MapItemSelectData>();
    }
    
    [Serializable]
    public class MapDataConfig
    {
        public string ID;
        public MapItemSelectData[] Data;
    }
    public class MapService : IService, IDisposable, ISerialize<List<MapDataConfig>>
    {
        private Dictionary<string, MapDataBlueprint> _mapDataBlueprints;

        public string MapConfigKey = "MapData";

        public void Init()
        {
            _mapDataBlueprints = new Dictionary<string, MapDataBlueprint>();
        }

        public void Dispose()
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
                bool status = _mapDataBlueprints.TryGetValue(data[i].ID, out MapDataBlueprint result);
                if (status)
                {
                    result.MapItemSelectDatas.Clear();
                    result.MapItemSelectDatas.AddRange(data[i].Data);
                }
                else
                {
                    MapDataBlueprint mapData = new MapDataBlueprint();
                    mapData.MapItemSelectDatas.AddRange(data[i].Data);
                    _mapDataBlueprints.TryAdd(data[i].ID, mapData);
                }
            }
        }

        public void Raise()
        {
            throw new NotImplementedException();
        }
    }
}

