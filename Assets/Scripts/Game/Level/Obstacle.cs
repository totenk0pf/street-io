using Core.Events;
using Core.System;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Game {
    public class Obstacle : MonoBehaviour {
        private void OnTriggerEnter(Collider other) {
            if (LayerUtil.CompareLayer(other.gameObject.layer, LayerMask.NameToLayer("Player"))) {
                this.FireEvent(EventType.ObstacleHit);
            }
        }
    }
}