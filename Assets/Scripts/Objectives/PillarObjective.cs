using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PillarObjective : MonoBehaviour, IInteractable
{
    [SerializeField] Material outlineMaterial;
    [SerializeField] string interactMessage;
    [SerializeField] PillarFlags type;

    // Start is called before the first frame update
    void Start()
    {
        SetupTarget(Instantiate(outlineMaterial));
        Message = interactMessage;
    }

    public void SetComplete(bool isComplete)
    {
        IsTargetDisabled = isComplete;
    }

    #region Interactable Implementation
    public string Message { get; set; }

    public void OnInteract()
    {
        if (IsTargetDisabled) return;
        GodrickValleyController.instance.SetPillarComplete(type);

        IsTargetDisabled = true;
    }
    #endregion

    #region Targetable Implementation
    //Targetable Implement
    public Material TargetMaterial { get; set; }
    public bool IsTargetDisabled { get; set; }

    public void SetupTarget(Material resourceMaterial)
    {
        TargetMaterial = Instantiate(resourceMaterial);
        if (TargetMaterial != null)
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                List<Material> materials = new() { TargetMaterial };
                materials.AddRange(renderer.materials);
                renderer.SetMaterials(materials);
            }

            TargetMaterial.SetFloat("_OutlineWidth", 0f);
        }
    }

    public void OnTarget(bool setTarget)
    {
        if (TargetMaterial == null) return;

        if (setTarget && !IsTargetDisabled)
        {
            GameManager.instance.SetInteractMessage(Message);
            TargetMaterial.SetFloat("_OutlineWidth", 0.075f);
        }
        else
        {
            GameManager.instance.SetInteractMessage("");
            TargetMaterial.SetFloat("_OutlineWidth", 0f);
        }
    }

    public GameObject GameObject()
    {
        return gameObject;
    }
    #endregion
}
