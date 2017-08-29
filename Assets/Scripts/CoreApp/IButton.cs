﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IButton : IPointerDownHandler, IPointerUpHandler {
    
    void Clicked();
}
