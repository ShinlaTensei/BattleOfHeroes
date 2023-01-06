using System;
using System.Collections;
using System.Collections.Generic;
using Base.Data.Structure;
using Base.Module;
using Base.Pattern;
using UniRx;
using UnityEngine;

namespace PaidRubik
{
    public class UserDataService : BaseBlueprint<PlayerProto.Types.UserData>, IService<PlayerProto.Types.UserData>
    {
        public void UpdateData(PlayerProto.Types.UserData data)
        {
            
        }

        public void Init()
        {
            
        }

        public void DeInit()
        {
            
        }

        public override void Load()
        {
            
        }

        public override void LoadDummyData()
        {
            throw new NotImplementedException();
        }
        
        public void AddData(PlayerProto.Types.UserData userData)
        {
            Data = userData;
        }
    }
}

