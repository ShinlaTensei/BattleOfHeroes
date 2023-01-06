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
    public record UserDataRecord
    {
        public string UserID { get; set; }
        public BoolReactiveProperty IsSound { get; set; } = new BoolReactiveProperty();
        public BoolReactiveProperty IsMusic { get; set; } = new BoolReactiveProperty();
    }
    public class UserDataService : IService<PlayerProto.Types.UserData>
    {
        private UserDataRecord _userData;
        public void UpdateData(PlayerProto.Types.UserData data)
        {
            if (data is not null)
            {
                _userData.UserID = data.Id;
                _userData.IsMusic.Value = data.IsMusic;
                _userData.IsSound.Value = data.IsSound;
            }
        }

        public void Init()
        {
            _userData = new UserDataRecord();
            UpdateData(ServiceLocator.GetBlueprint<BlueprintUserData>()?.Data);
        }

        public void DeInit()
        {
            
        }
    }
}

