using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A simple static class that keeps track of layer names, holds ready to use layermasks for most common layers and layermasks combinations
/// Of course if you happen to change the layer order or numbers, you'll want to update this class.
/// </summary>
public static class LayerManager
{
    public static int UILayer = 5;
    public static int GroundLayer = 6;
    public static int BlockLayer = 7;
    public static int PlayerLayer = 8;

    public static int GroundLayerMask = 1 << GroundLayer;
    public static int BlockLayerMask = 1 << BlockLayer;
    public static int PlayerLayerMask = 1 << PlayerLayer;

}