﻿using System;
using Actors;
using UnityEngine;
using Utils;

namespace GameFlow
{
    /// <summary>
    /// Handles the inflicting of damage when rabbit contacs deathzones
    /// </summary>
    public class DeathZoneController : NonGlobalSingleton<DeathZoneController>
    {
        [SerializeField]
        RabbitMonoBehaviour _rabbitBehaviour;
        [SerializeField]
        GameObject[] _deathzones;
        [SerializeField]
        Respawner _respawner;

        static DeathZoneController instance;
        ILivesComponent _rabbitLives;

        void Start()
        {
            var collisionListener = _rabbitBehaviour.gameObject.AddComponent<Trigger2DListener>();
            collisionListener.EnterredTrigger += OnRabbitEnterredCollision;

            _rabbitLives = _rabbitBehaviour.Rabbit.Lives;
        }

        void OnRabbitEnterredCollision(Collider2D collision)
        {
            if(Array.IndexOf(_deathzones, collision.gameObject) >= 0)
            {
                if(_rabbitLives.LoseLife()) 
                    { _respawner.RespawnRabbit(); }
            }
        }


    }
}
