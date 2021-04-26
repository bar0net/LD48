using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HandTile : MonoBehaviour
{
    public Image foreground;
    public Image background;

    public void SetForeground(Sprite sprite, Color color)
    {
        foreground.sprite = sprite;
        foreground.color  = color;
    }

    public void SetBackgroundColor(Color color)
    {
        background.color = color;
    }
}
