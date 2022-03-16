using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//dati della porta
public class Door : MonoBehaviour
{

    public Transform visual;
    public Vector2Int direction;
    public Vector2Int slotPosition;

    private Door connection;
    private bool disabled = false;


    public void SetConnection(Door altra)
    {
        connection = altra;
    }

    public void Disable()
    {
        disabled = true;
        visual.gameObject.SetActive(false);
    }

}
