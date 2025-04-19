using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System.Collections.Generic;

public class NetworkPlayer : NetworkBehaviour
{
    public NetworkVariable<ulong> clientID = new NetworkVariable<ulong>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            clientID.Value = OwnerClientId;
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
    }

    [ServerRpc(RequireOwnership = false)]
    private void LogClientIDServerRpc(ulong clientId)
    {
        Debug.Log($"Client connected with ID: {clientId}");
    }

    [ClientRpc]
    public void ClearThingsClientRpc()
    {
        // Stop music and clear UI
        AudioManager.Instance.StopMusic();
        UIManager.Instance.ClearAllMenus();
    }

}
