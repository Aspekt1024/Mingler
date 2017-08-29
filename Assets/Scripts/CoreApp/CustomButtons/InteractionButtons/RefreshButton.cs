using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshButton : InteractionButton
{

    public override void Clicked()
    {
        base.Clicked();
        Debug.Log("Heart Clicked");
    }
}
