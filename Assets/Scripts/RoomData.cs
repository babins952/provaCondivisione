using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//script del prefab stanza che contiene i dati della stanza
public class RoomData :MonoBehaviour
{
    public Slot[] slots;


    [SerializeField] bool adjust;

    private void OnValidate()
    {
        if (adjust)
        {
            adjust = false;

            foreach (Slot s in slots)
            {

                float x = s.index.x * SlotGenerator.config.slotSize.x;
                float y = s.index.y * SlotGenerator.config.slotSize.y;

                s.transform.localPosition = new Vector3(x, y, 0);

                foreach (Door d in s.doors)
                {
                    float dx = d.direction.x * SlotGenerator.config.slotSize.x/2;
                    float dy = d.direction.y * SlotGenerator.config.slotSize.y/2;

                    d.transform.localPosition = new Vector3(dx, dy, 0);
                    d.slotPosition = s.index;
                }
            }
        
        }
    }



    public List<Door> GetDoorsFacing(Vector2Int direction)
    {

        List<Door> doors = new List<Door>();

        foreach (Slot s in slots)
        { 
            
            foreach(Door d in s.doors)
            {
                if (d.direction == direction)
                    doors.Add(d);

            }
        }

        return doors;
    
    }


    public List<Door> GetDoors()
    {
        List<Door> doors = new List<Door>();

        foreach (Slot s in slots)
        {

            foreach (Door d in s.doors)
            {
                doors.Add(d);
            }
        }
        return doors;
    }

    //funzione che restituisce la porta che da sullo slot in posizione towardPosition, che ha come direzione di porta 
    //facingDirection, se la stanza si trovasse in posizione referencePosition
    public Door GetDoorFacingPosition(Vector2Int referencePosition, Vector2Int towardPosition, Vector2Int facingDirection)
    {
        //trovo lo slot della stanza che contiene la porta
        Vector2Int slotIndex = towardPosition - facingDirection;

        foreach (Slot s in slots)
        {
            if (referencePosition + s.index == slotIndex)
            {
                foreach (Door d in s.doors)
                {
                    if (d.direction == facingDirection)
                        return d;
                }
            }
        }

        return null;//c'è un problema
    }

  


}
