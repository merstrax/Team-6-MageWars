using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitInterface : MonoBehaviour
{
    [SerializeField] Canvas unitInterface;
    [SerializeField] Image healthBarSprite;
    [SerializeField] GameObject floatingNumber;

    [SerializeField] float floatingNumberTimer;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        Vector3 angles = transform.rotation.eulerAngles;
        angles.x = 0;
        transform.rotation = Quaternion.Euler(angles);
    }

    public void UpdateHealthBar(float current, float max)
    {
        healthBarSprite.fillAmount = (current / max);
    }

    public void CreateFloatingNumber(int value)
    {
        GameObject number = Instantiate(floatingNumber, unitInterface.transform);
        TextMeshProUGUI text = number.GetComponent<TextMeshProUGUI>();
        
        text.text = value.ToString();

        StartCoroutine(MoveFloatingNumber(number));
        Destroy(number, floatingNumberTimer + 0.1f);
    }

    IEnumerator MoveFloatingNumber(GameObject number)
    {
        float startTime = Time.time;
        Vector3 position;

        float randY = Random.Range(-5.0f, 5.0f) * Time.deltaTime;
        float randX = Random.Range(-5.0f, 5.0f) * Time.deltaTime;
        float fade = (0.33f * Time.deltaTime);

        while (Time.time < startTime + floatingNumberTimer)
        {
            position = number.transform.position;
            position.x += randX;
            position.y += randY;
            number.transform.SetPositionAndRotation(position, number.transform.rotation);
            number.GetComponent<TextMeshProUGUI>().alpha -= fade;
            yield return null;
        }
    }
}
