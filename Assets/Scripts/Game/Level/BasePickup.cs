using Core.Events;
using Core.Logging;
using Core.System;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Game {
    public class BasePickup : Pickup {
        public override void OnPickup() {
            NCLogger.Log("Picked item up!");
            Remove();
        }

        public override void Remove() {
            Destroy(gameObject);
        }

        protected void OnTriggerEnter(Collider other) {
            if (LayerUtil.CompareLayer(other.gameObject.layer, LayerMask.NameToLayer("Player"))) {
                this.FireEvent(EventType.ItemPickedUp);
                OnPickup();
            }
        }
    }
}