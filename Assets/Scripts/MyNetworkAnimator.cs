using System;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class MyNetworkAnimator : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
