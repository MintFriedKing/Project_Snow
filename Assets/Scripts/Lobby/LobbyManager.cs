using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PS
{
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject lobbyCharacter;
        private bool isTuch;

        public static LobbyManager Instance;
        public List<Transform> teleportPoints;
        public bool IsTuch { get{ return isTuch; }set { isTuch = value; } }
        private void Awake()
        {
            Instance = this;
            isTuch = true;
        }
     
    }
}
