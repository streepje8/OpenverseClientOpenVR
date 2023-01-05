//-------------------------------
//PermissionManager
//The client side handler that controls the Permissions.cs Script.
//
//Author: streep
//Creation Date: 12-04-2022
//--------------------------------

namespace Openverse.Permissions
{
    public class PermissionManager : Singleton<PermissionManager>
    {
        private void Awake()
        {
            Instance = this;
            Permissions.LoadPermissions();
        }

        public PermissionState GetPermission(Permission perm)
        {
            return Permissions.GetServerPermission(perm);
        }

        internal void LoadServerPermissions(string currentServer)
        {
            Permissions.LoadServerPermissions(currentServer);
        }
    }
}