using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSwapper : NetworkBehaviour
{
    public void SwitchState(GameObject state)
    {
        if(!isServer)
        {
            Player.Instance.CmdGiveAuthority(netIdentity);
            ActionManager.RunWhen(() => netIdentity.hasAuthority, () => CmdSwapState(state.name));
        }
        else
        {
            RpcSwapState(state.name);
        }
    }

    [Command]
    private void CmdSwapState(string stateID)
    {
        RpcSwapState(stateID);
    }

    [ClientRpc]
    private void RpcSwapState(string stateID)
    {
        _SwitchState(stateID);
    }

    private void _SwitchState(string stateID)
    {
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }
        transform.Find(stateID).gameObject.SetActive(true);
    }
}
