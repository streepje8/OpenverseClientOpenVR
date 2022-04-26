//-------------------------------
//VirtualPlayer
//The main way of interfacing with the Openverse Player Object
//
//Author: streep
//Creation Date: 12-04-2022
//--------------------------------
namespace Openverse.Core
{
    using Openverse.Permissions;
    using UnityEngine;

    public class VirtualPlayer : MonoBehaviour
    {
        [SerializeField] public ushort id;
        [SerializeField] public string username;

        public GameObject head;
        public GameObject handLeft;
        public GameObject handRight;

        [HideInInspector]public VirtualPlayerInput input;

        public bool sendPositions { get; private set; } = false;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            input = GetComponent<VirtualPlayerInput>();
        }

        private void FixedUpdate()
        {
            OpenversePlayer.SendVRPositions(this);
        }

        public void OnWorldStart()
        {
            GameObject spawn = GameObject.Find("WorldSpawn");
            if (spawn != null)
            {
                transform.position = spawn.transform.position;
            }
            sendPositions = PermissionManager.Instance.GetPermission(Permission.PositionalData) == PermissionState.ALLOW ? true : false;
        }


        public void Move(Vector3 newPosition, Vector3 forward)
        {
            transform.position = newPosition;

            if (id != OpenverseNetworkClient.Instance.Client.Id) // Don't overwrite local player's forward direction to avoid noticeable rotational snapping
                transform.forward = forward;
        }

        private void OnDestroy()
        {
            OpenversePlayer.list.Remove(id);
        }
    }
}