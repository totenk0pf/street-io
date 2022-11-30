using UnityEngine;

namespace Game {
    public abstract class Pickup : MonoBehaviour, IPickup {
        public abstract void OnPickup();
        public abstract void Remove();
    }
}