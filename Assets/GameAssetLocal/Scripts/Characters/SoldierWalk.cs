using System.Collections;
using System.Collections.Generic;
using Base.Pattern;
using UnityEngine;

namespace PaidRubik
{
    public class SoldierWalk : CharacterState
    {
        private SoldierController SoldierController;

        private static int Walking = Animator.StringToHash("Walking");
        protected override void Awake()
        {
            base.Awake();
            
            SoldierController = CharacterStateController as SoldierController;
        }

        public override void EnterState(float dt, CharacterState fromState)
        {
            SoldierController.Animator.SetBool(Walking, SoldierController.WalkState);
        }

        public override void FixedUpdateBehaviour(float dt)
        {
            
        }

        public override void CheckExitTransition()
        {
            
        }

        public override void ExitStateBehaviour(float dt, CharacterState toState)
        {
            SoldierController.WalkState = false;
            SoldierController.Animator.SetBool(Walking, false);
        }
    }
}

