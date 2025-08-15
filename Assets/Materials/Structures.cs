using System;
using UnityEngine;

namespace Shit
{
    public class Structures
    {
        
    }
    
    [Serializable]
    public class Stat
    {
        public float Max;
        public float Current;
        public float CurrentMax;

        private float _mods;

        public Stat()
        {
            Max = 100;
            Current = 100;
            CurrentMax = 100;
        }

        public Stat(float max, float current, float currentMax)
        {
            Max = max;
            Current = current;
            CurrentMax = currentMax;
        }

        /// <summary>
        /// Increases the max stat point
        /// </summary>
        /// <param name="mod"></param>
        public void Increase(float mod)
        {
            CurrentMax += mod;
            _mods += mod;
        }

        /// <summary>
        /// Removes only modified stat points
        /// </summary>
        /// <param name="mod"></param>
        public void Decrease(float mod)
        {
            if (mod > _mods)
            {
                CurrentMax -= _mods;
                _mods -= _mods;
            }
            else
            {
                CurrentMax -= mod;
                _mods -= mod;
            }
        }
        
        /// <summary>
        /// Reset the modified stat points
        /// </summary>
        /// <param name="mod"></param>
        public void Reset(float mod)
        {
            CurrentMax = Max;
            _mods = 0;
        }
    }

    [Serializable]
    public class AttackStats
    {
        
        public GameObject Projectile;
    
        public Vector3 Offset;

        public KeyCode AttackKey;
        public bool IsAttacking = false;

       
        [Range(0f, 5f)]
        public float Scale = 1;
        [Range(0f, 100f)]
        public float Speed = 1;
        [Range(0f, 100f)]
        public float Duration = 1;
        [Range(1, 20)]
        public int Durability = 1;
    
        [Range(0f, 100f)]
        public float AtkCooldown = 1;
        private Timer atkCooldownTimer;

        public AudioClip AttackSound;

        public float AtkPowerMultiplier;
    
        public ForceMode ForceMode = ForceMode.Impulse;

        public bool FollowTarget;
        
        public bool Visible = false;

        public void SetTimer(Timer timer)
        {
            atkCooldownTimer = timer;
        }

        public Timer GetTimer()
        {
            return atkCooldownTimer;
        }
    }
}