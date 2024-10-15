using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AludyneAltar : MonoBehaviour, IInteractable
{
    [SerializeField] Material outlineMaterial;
    [SerializeField] string interactMessage;
    [SerializeField] GameObject visual;

    // Start is called before the first frame update
    void Start()
    {
        SetupTarget(Instantiate(outlineMaterial));
        Message = interactMessage;
    }

    public void SetComplete(bool isComplete)
    {
        IsTargetDisabled = isComplete;
        visual.SetActive(!isComplete);
    }

    #region Interactable Implementation
    public string Message { get; set; }

    public void OnInteract()
    {
        if (IsTargetDisabled) return;
        SceneManager.LoadScene("Altar of Aludyne");

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
            foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
            {
                if (renderer.material.shader == Shader.Find("Universal Render Pipeline/Particles/Unlit")) continue;
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
