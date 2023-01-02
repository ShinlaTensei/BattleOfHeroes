using System;
using System.Collections;
using System.Collections.Generic;
using Base.Pattern;
using UniRx;
using UnityEngine;

namespace PaidRubik
{
    [Serializable]
    public class UserDataBlueprint
    {
        public string ID;
        public UserSettingBlueprint SettingBlueprint;
    }
    
    [Serializable]
    public class UserSettingBlueprint
    {
        public bool IsSound;
        public bool IsMusic;
    }

    public class UserData
    {
        public StringReactiveProperty ID;
        public BoolReactiveProperty IsSound;
        public BoolReactiveProperty IsMusic;

        public UserData()
        {
            ID = new StringReactiveProperty(string.Empty);
            IsSound = new BoolReactiveProperty(true);
            IsMusic = new BoolReactiveProperty(true);
        }
    }
    
    public class UserDataService : IService, ISerialize<UserDataBlueprint>
    {
        private UserData _userData = new UserData();
        
        public void Init()
        {
            throw new NotImplementedException();
        }

        public void DeInit()
        {
            
        }

        public UserDataBlueprint To()
        {
            return new UserDataBlueprint
            {
                ID = _userData.ID.Value,
                SettingBlueprint = new UserSettingBlueprint
                {
                    IsMusic = _userData.IsMusic.Value,
                    IsSound = _userData.IsSound.Value
                }
            };
        }

        public void From(UserDataBlueprint data)
        {
            _userData.ID.Value = data.ID;
            _userData.IsMusic.Value = data.SettingBlueprint.IsMusic;
            _userData.IsSound.Value = data.SettingBlueprint.IsSound;
        }

        public void Raise()
        {
            _userData.ID.SetValueAndForceNotify(_userData.ID.Value);
            _userData.IsMusic.SetValueAndForceNotify(_userData.IsMusic.Value);
            _userData.IsSound.SetValueAndForceNotify(_userData.IsSound.Value);
        }
    }
}

