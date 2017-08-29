using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartButton : InteractionButton {

    public override void Clicked()
    {
        base.Clicked();
        Debug.Log("Heart Clicked");
    }
}
