using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable
{
    public Material TargetMaterial{ get; set; }
    public bool IsTargetDisabled { get; set; }

    public void SetupTarget(Material resourceMaterial);
    public void OnTarget(bool setTarget);
    public GameObject GameObject();
}
