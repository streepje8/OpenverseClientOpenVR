namespace Openverse.Core
{
    using UnityEngine;
    public class Bootstrapper : MonoBehaviour
    {
        void Start()
        {
            OpenverseClient.Instance.ConnectTo(OpenverseClient.Instance.userSettings.HomeServerIP);
        }
    }
}
