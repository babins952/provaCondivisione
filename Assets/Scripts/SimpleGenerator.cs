using System.Collections;
using System.Collections.Generic;
using UnityEngine;




class SimpleRoom
{
    public Vector2Int position;
    public GameObject room;

    public int x => position.x;
    public int y => position.y;

    public SimpleRoom(Vector2Int pos, GameObject room)
    {

        position = pos;
        this.room = room;
    }
}


public class SimpleGenerator : MonoBehaviour
{

    public GameObject normal;
    public GameObject boss;
    public GameObject secret;

    bool[,] layout;
    List<SimpleRoom> stanze;
    List<SimpleRoom> deadEnd;
    Queue<SimpleRoom> checkForNeighbour;


    public int span = 10;
    public int maxStanze=10;


    public static Vector2Int[] posHelper =
        {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };


    SimpleRoom mainInstance;
    SimpleRoom bossInstance;
    SimpleRoom secretInstance;


   
    void Start()
    {
        Initialize();
        Generate();
        GenerateBoss();
        GenerateSecretRoom();
        InstantiateRooms();

    }

  

    public void Initialize()
    {
        layout = new bool[span, span];
        stanze = new List<SimpleRoom>();
        deadEnd = new List<SimpleRoom>();
        checkForNeighbour = new Queue<SimpleRoom>();

        //crea la stanza iniziale
        mainInstance= CreateRoom(new Vector2Int(span / 2, span / 2), normal);
    }




    private SimpleRoom CreateRoom(Vector2Int position, GameObject nuova)
    {
        SimpleRoom room = new SimpleRoom(position, nuova);
        layout[position.x, position.y] = true;
        checkForNeighbour.Enqueue(room);
        stanze.Add(room);

        return room;
    }


    public void GenerateBoss()
    {
        bool remove = false;
        for (int i = deadEnd.Count - 1; i >= 0; i++)
        {

            if (BossRoomConditions(deadEnd[i].x, deadEnd[i].y + 1))
            {
                bossInstance= CreateRoom(new Vector2Int(deadEnd[i].x, deadEnd[i].y+1), boss);
                remove = true;
               
            }
            else
            if (BossRoomConditions(deadEnd[i].x+1, deadEnd[i].y ))
            {
                bossInstance = CreateRoom(new Vector2Int(deadEnd[i].x+1, deadEnd[i].y), boss);
                remove = true;
               
            }
            else
            if (BossRoomConditions(deadEnd[i].x, deadEnd[i].y -1))
            {
                bossInstance = CreateRoom(new Vector2Int(deadEnd[i].x, deadEnd[i].y-1), boss);
                remove = true;
                
            }
            else
            if (BossRoomConditions(deadEnd[i].x-1, deadEnd[i].y))
            {
                bossInstance = CreateRoom(new Vector2Int(deadEnd[i].x-1, deadEnd[i].y), boss);
                remove = true;
                
            }

            if (remove)
            {

                deadEnd.Remove(deadEnd[i]);
                break;
            }
                

        }

        

    }


    public bool BossRoomConditions(int x, int y)
    {
        try
        {
            if (layout[x, y])
                return false;
        }
        catch
        {
            return false;
        }

        //int count = 0;
        //count += CountVicino(x, y + 1);
        //count += CountVicino(x + 1, y);
        //count += CountVicino(x, y - 1);
        //count += CountVicino(x - 1, y);

        //if (count > 1)
        //    return false;
        return IsDeadEnd(x, y);


}


    private void GenerateSecretRoom()
    {
        
        int num = 3;
        while (num > 0)
        {
            for (int i = stanze.Count - 2; i >= 0; i--)
            {

                SimpleRoom room = stanze[i];

                for (int p = 0; p < posHelper.Length; p++)
                {

                    Vector2Int pos = room.position + posHelper[p];

                    if (IsInSpan(pos.x, pos.y) && !layout[pos.x, pos.y] && SecretRoomConditions(pos.x, pos.y, num))
                    {


                        secretInstance= CreateRoom(pos, secret);
                        return;
                    }


                }



            }

            num--;

        }
    
    }

    private bool SecretRoomConditions(int x, int y, int num)
    {

        int sum = 0;

        for (int i = 0; i < posHelper.Length; i++)
        {

            Vector2Int pos = new Vector2Int(x, y) + posHelper[i];

            if (pos == bossInstance.position)
                return false;

            sum += CountVicino(pos);

        }


        return sum >= num;
    }

    private bool IsInSpan(int x, int y)
    {
        return x >= 0 && x < span && y >= 0 && y < span;
    }

    private void Generate()
    {
        int tries = 0;
        while (stanze.Count < maxStanze || checkForNeighbour.Count>0)
        {

            if (checkForNeighbour.Count == 0)
            {
                tries++;

                if (tries > 10)
                    throw new UnityException();

                foreach (SimpleRoom r in stanze)
                {
                    checkForNeighbour.Enqueue(r);
                }

                //deadEnd.Clear();
                RecalculateDeadEnds();
            }




            //estrarre stanza

            SimpleRoom stanza = checkForNeighbour.Dequeue();

            //verifica condizioni per ogni vicino

            //bool check = false;
            //check |= CheckConditions(new Vector2Int(stanza.x, stanza.y + 1), normal);
            //check |= CheckConditions(new Vector2Int(stanza.x + 1, stanza.y),normal);
            //check |= CheckConditions(new Vector2Int(stanza.x, stanza.y - 1),normal);
            //check |= CheckConditions(new Vector2Int(stanza.x - 1, stanza.y),normal);

            //if (!check)
            //    deadEnd.Add(stanza);

            for (int i = 0; i < posHelper.Length; i++)
            {
                if (CheckNormalConditions(stanza.position + posHelper[i]))
                    CreateRoom(stanza.position + posHelper[i], stanza.room);
            }

            if(IsDeadEnd(stanza))
                deadEnd.Add(stanza);



        }

        Debug.Log($"generated after {tries} tries");
        RecalculateDeadEnds();
    }



    private void RecalculateDeadEnds()
    {
        Queue<SimpleRoom> toRemove = new Queue<SimpleRoom>();
        foreach (SimpleRoom s in deadEnd)
        { 
            if(!IsDeadEnd(s))
                toRemove.Enqueue(s);
        
        }

        while (toRemove.Count > 0)
            deadEnd.Remove(toRemove.Dequeue());
    
    }


    private bool IsDeadEnd(SimpleRoom room)
    {
        return CountRoomAroundPoint(new Vector2Int(room.x, room.y)) == 1;

    }

    private bool IsDeadEnd(int x, int y)
    {
        return CountRoomAroundPoint(new Vector2Int(x, y)) == 1;
    }


    private int CountRoomAroundPoint(Vector2Int position)
    {
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            count += CountVicino(position + posHelper[i]);
        }


        return count;
    }


    //il vlore booleano ritornato ci serve per capire se la stanza è una dead end
    private bool CheckNormalConditions(Vector2Int pos)
    {
        //non già presente
        try
        {
            if (layout[pos.x, pos.y])
                return false;
        }
        catch
        {
            return false; 
        }

        //random 50%
        if (Random.Range(0f, 1f) < 0.5f)
            return false;



        if (!IsDeadEnd(pos.x, pos.y))
            return false;

        //conta vicini
        //int count = 0;
        //count += CountVicino(pos.x, pos.y + 1);
        //count += CountVicino(pos.x + 1, pos.y);
        //count += CountVicino(pos.x, pos.y - 1);
        //count += CountVicino(pos.x - 1, pos.y);

        //if (count > 1)
        //    return false;

        if (stanze.Count == maxStanze)
            return false;

        return true;


    }


    private int CountVicino(Vector2Int pos)
    {
        return CountVicino(pos.x, pos.y);
    }

    private int CountVicino(int x, int y)
    {
        try
        {
            return layout[x, y] ? 1 : 0;
        }
        catch
        {
            return 0;
        }
    }


    private void InstantiateRooms()
    {
        foreach (SimpleRoom r in stanze)
        {
            Instantiate(r.room, new Vector3(r.x, r.y), Quaternion.identity); 
        }

        //foreach (SimpleRoom r in deadEnd)
        //{
        //    Instantiate(secret, new Vector3(r.x, r.y), Quaternion.identity);
        //}


    }
}
