using Unity.Netcode.Components;
using UnityEngine;

public class MyNetworkTransform : NetworkTransform
{
   protected override bool OnIsServerAuthoritative()
   {
      return false;
   }
}
