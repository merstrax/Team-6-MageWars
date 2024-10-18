using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable : ITargetable
{
    public string Message { get; set; }

    public void OnInteract();
}
