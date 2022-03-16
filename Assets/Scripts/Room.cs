using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class Room
{

    public Vector2Int position;
    public Vector2Int[] slotPositions;
    public RoomData roomData;


    public Room(int x, int y, RoomData room)
    {
        roomData = room;
        position = new Vector2Int(x, y);

        slotPositions = new Vector2Int[roomData.slots.Length];

        for(int i=0;i<slotPositions.Length;i++)
        {
            slotPositions[i] = position + roomData.slots[i].index;
        }


    }



}
