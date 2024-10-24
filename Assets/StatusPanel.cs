using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Status
{
    NONE,
    HEALTH,
    DAMAGE,
    CRIT,
    MOVESPEED,
    SLOW,
    ROOT,
    STUN,
    POISON
}

public class StatusPanel : MonoBehaviour
{
    [SerializeField] GameObject statusHealth;
    [SerializeField] GameObject statusDamage;
    [SerializeField] GameObject statusCrit;
    [SerializeField] GameObject statusMove;
    [SerializeField] GameObject statusSlow;
    [SerializeField] GameObject statusRoot;
    [SerializeField] GameObject statusStun;
    [SerializeField] GameObject statusPoison;

    List<GameObject> statusIcons;

    private void Start()
    {
        statusIcons = new List<GameObject>() {null, statusHealth, statusDamage, statusCrit, statusMove, statusSlow, statusRoot, statusStun, statusPoison};
    }

    public void ToggleStatus(Status status, bool active = false)
    {
        statusIcons[(int)status].SetActive(active);
    }
}
