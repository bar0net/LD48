using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlankTile : Tile
{
    Manager manager;
    bool active = false;
    
    private void Awake()
    {
        manager = FindObjectOfType<Manager>();
    }

    private void OnMouseEnter()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2)) return;
        if (manager.Paused()) return;

        active = true;
        manager.overlayTile.SetActive(true);
        manager.overlayTile.transform.position = this.transform.position - Vector3.forward;
    }


    private void OnMouseExit()
    {
        if (!active) return;
        manager.overlayTile.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (!active) return;
        manager.overlayTile.SetActive(false);
        manager.PlaceTile(coords);
    }
}
