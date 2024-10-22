using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI FireText;
    [SerializeField] TextMeshProUGUI WindText;
    [SerializeField] TextMeshProUGUI ArcaneText;
    [SerializeField] TextMeshProUGUI FrostText;
    [SerializeField] Color completionColor;


    List<TextMeshProUGUI> ObjectiveList;

    void Start()
    {
        ObjectiveList = new List<TextMeshProUGUI>() {ArcaneText, FireText, FrostText, WindText};

    }
    public void SetObjectiveComplete(int objectiveID)
    {
        ObjectiveList[objectiveID].color = completionColor;
    }
}
