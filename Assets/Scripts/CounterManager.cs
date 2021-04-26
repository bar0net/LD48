using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CounterManager : MonoBehaviour
{
    public RectTransform arrow;
    public float inRangeX = 32.0f;
    public float maxX = 64.0f;
    public Text uiText;

    public void UpdateUI(int value, int desiredValue, int range)
    {
        float x = inRangeX * (float)(value - desiredValue) / (float)range;
        x = Mathf.Clamp(x, -maxX, maxX);
        arrow.localPosition = new Vector3(x, arrow.localPosition.y, arrow.localPosition.z);
        uiText.text = value.ToString() + "/" + desiredValue.ToString();
    }
}
