using System;
using System.Collections;
using System.Collections.Generic;
using Base.Logging;
using Base.Pattern;
using UniRx;
using UnityEngine;

namespace PaidRubik
{
    public class SoldierController : CharacterStateController
    {
        public bool WalkState;
        public bool AttackState;
        public bool IdleState;
        
        // Animation Events
        public void OnAttackEventTrigger()
        {
            if (CurrentState is SoldierAttack soldierAttack)
            {
                this.GetLogger().Debug("Character ({0}) attack", Animator.name);
                soldierAttack.CheckHitEnemy();
            }
        }
    }
}

