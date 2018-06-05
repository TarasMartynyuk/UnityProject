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
        Rabbit _rabbit;
        [SerializeField]
        GameObject[] _deathzones;
        [SerializeField]
        Respawner _respawner;

        static DeathZoneController instance;
        LivesComponent _rabbitLives;

        void Start()
        {
            var collisionListener = _rabbit.gameObject.AddComponent<Trigger2DListener>();
            collisionListener.EnterredTrigger += OnRabbitEnterredCollision;

            _rabbitLives = _rabbit.Lives;
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