using System.Collections;
using System.Collections.Generic;
using Base.Pattern;
using UnityEngine;

namespace PaidRubik
{
    public class SoldierWalk : CharacterState
    {
        private SoldierController SoldierController;
        protected override void Awake()
        {
            base.Awake();
            
            SoldierController = CharacterStateController as SoldierController;
        }

        public override void FixedUpdateBehaviour(float dt)
        {
            
        }

        public override void CheckExitTransition()
        {
            if (SoldierController.WalkState)
            {
                SoldierController.EnqueueTransition<SoldierWalk>();
            }
        }
    }
}

