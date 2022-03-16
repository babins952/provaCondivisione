using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotGenerator : MonoBehaviour
{


    public static Configuration config;

    [SerializeField] private Configuration setConfig;


    private void OnValidate()
    {
        config = setConfig;
    }



    public RoomData normal;
    public RoomData bigRoom;
    public RoomData boss;
    public RoomData secret;

    Dictionary<Vector2Int, Room> stanzeLayout;
    List<Room> stanze;
    List<Room> deadEnd;
    Queue<Room> checkForNeighbour;


    public int span = 10;
    public int maxStanze = 10;


    public static Vector2Int[] posHelper =
        {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };


    Room mainInstance;
    Room bossInstance;
    Room secretInstance;



    void Start()
    {
        //inizializza i dati di default
        Initialize();

        Generate();
        //GenerateBoss();
        //GenerateSecretRoom();
        InstantiateRooms();
        //postprocessing

    }



    public void Initialize()
    {
        stanzeLayout = new Dictionary<Vector2Int, Room>();
        stanze = new List<Room>();
        deadEnd = new List<Room>();
        checkForNeighbour = new Queue<Room>();

        //crea la stanza iniziale
        mainInstance = CreateRoom(new Vector2Int(span / 2, span / 2), normal);
    }




    private Room CreateRoom(Vector2Int position, RoomData nuova)
    {
        Room room = new Room(position.x,position.y, nuova);

        for (int i = 0; i < room.slotPositions.Length; i++)
            stanzeLayout.Add(room.slotPositions[i], room);


        stanze.Add(room);


        checkForNeighbour.Enqueue(room);
        

        return room;
    }


    //public void GenerateBoss()
    //{
    //    bool remove = false;
    //    for (int i = deadEnd.Count - 1; i >= 0; i++)
    //    {

    //        if (BossRoomConditions(deadEnd[i].x, deadEnd[i].y + 1))
    //        {
    //            bossInstance = CreateRoom(new Vector2Int(deadEnd[i].x, deadEnd[i].y + 1), boss);
    //            remove = true;

    //        }
    //        else
    //        if (BossRoomConditions(deadEnd[i].x + 1, deadEnd[i].y))
    //        {
    //            bossInstance = CreateRoom(new Vector2Int(deadEnd[i].x + 1, deadEnd[i].y), boss);
    //            remove = true;

    //        }
    //        else
    //        if (BossRoomConditions(deadEnd[i].x, deadEnd[i].y - 1))
    //        {
    //            bossInstance = CreateRoom(new Vector2Int(deadEnd[i].x, deadEnd[i].y - 1), boss);
    //            remove = true;

    //        }
    //        else
    //        if (BossRoomConditions(deadEnd[i].x - 1, deadEnd[i].y))
    //        {
    //            bossInstance = CreateRoom(new Vector2Int(deadEnd[i].x - 1, deadEnd[i].y), boss);
    //            remove = true;

    //        }

    //        if (remove)
    //        {

    //            deadEnd.Remove(deadEnd[i]);
    //            break;
    //        }


    //    }



    //}


    //public bool BossRoomConditions(int x, int y)
    //{
    //    try
    //    {
    //        if (layout[x, y])
    //            return false;
    //    }
    //    catch
    //    {
    //        return false;
    //    }

    //    //int count = 0;
    //    //count += CountVicino(x, y + 1);
    //    //count += CountVicino(x + 1, y);
    //    //count += CountVicino(x, y - 1);
    //    //count += CountVicino(x - 1, y);

    //    //if (count > 1)
    //    //    return false;
    //    return IsDeadEnd(x, y);


    //}


    //private void GenerateSecretRoom()
    //{

    //    int num = 3;
    //    while (num > 0)
    //    {
    //        for (int i = stanze.Count - 2; i >= 0; i--)
    //        {

    //            SimpleRoom room = stanze[i];

    //            for (int p = 0; p < posHelper.Length; p++)
    //            {

    //                Vector2Int pos = room.position + posHelper[p];

    //                if (IsInSpan(pos.x, pos.y) && !layout[pos.x, pos.y] && SecretRoomConditions(pos.x, pos.y, num))
    //                {


    //                    secretInstance = CreateRoom(pos, secret);
    //                    return;
    //                }


    //            }



    //        }

    //        num--;

    //    }

    //}

    //private bool SecretRoomConditions(int x, int y, int num)
    //{

    //    int sum = 0;

    //    for (int i = 0; i < posHelper.Length; i++)
    //    {

    //        Vector2Int pos = new Vector2Int(x, y) + posHelper[i];

    //        if (pos == bossInstance.position)
    //            return false;

    //        sum += CountVicino(pos);

    //    }


    //    return sum >= num;
    //}

    private bool IsInSpan(int x, int y)
    {
        return x >= 0 && x < span && y >= 0 && y < span;
    }

    private void Generate()
    {
        int tries = 0;
        while (stanze.Count < maxStanze || checkForNeighbour.Count > 0)
        {

            if (checkForNeighbour.Count == 0)
            {
                tries++;

                if (tries > 10)
                    throw new UnityException();

                foreach (Room r in stanze)
                {
                    checkForNeighbour.Enqueue(r);
                }

                //deadEnd.Clear();
                RecalculateDeadEnds();
            }




            //estrarre stanza

            Room stanza = checkForNeighbour.Dequeue();

            //verifica condizioni per ogni vicino

            //bool check = false;
            //check |= CheckConditions(new Vector2Int(stanza.x, stanza.y + 1), normal);
            //check |= CheckConditions(new Vector2Int(stanza.x + 1, stanza.y),normal);
            //check |= CheckConditions(new Vector2Int(stanza.x, stanza.y - 1),normal);
            //check |= CheckConditions(new Vector2Int(stanza.x - 1, stanza.y),normal);

            //if (!check)
            //    deadEnd.Add(stanza);



            List<Door> doors = stanza.roomData.GetDoors();

            foreach (Door d in doors)
            {
                Vector2Int doorPosition = stanza.position + d.slotPosition + d.direction;

                if (CheckNormalConditions(doorPosition))
                {
                    //provare a inserire la stanza inserire la stanza

                    Room inserted = null;

                    do
                    {

                        Vector2Int foundPosition;
                        //prendi la stanza
                        RoomData roomToTest = GetDungeonRoom();
                        if (TrySlotPosition(doorPosition, d.direction, roomToTest, out foundPosition))
                            inserted = CreateRoom(foundPosition, roomToTest);


                    } while (inserted == null);

                }
            }



            //for (int i = 0; i < posHelper.Length; i++)
            //{
            //    if (CheckNormalConditions(stanza.position + posHelper[i]))
            //        CreateRoom(stanza.position + posHelper[i], stanza.room);
            //}

            if (IsDeadEnd(stanza))
                deadEnd.Add(stanza);



        }

        Debug.Log($"generated after {tries} tries");
        RecalculateDeadEnds();
    }


    //i criteri con cui scegliere la stanza
    private RoomData GetDungeonRoom()
    {
        if (Random.Range(0, 1f) < 0.3f)
            return bigRoom;

        return normal;
    }


    private bool TrySlotPosition(Vector2Int position, Vector2Int fromDirection, RoomData roomToTest, out Vector2Int foundPosition)
    {
        foundPosition = Vector2Int.zero;
        List<Door> doors = roomToTest.GetDoorsFacing(fromDirection * -1);
        List<Door> possibleFit = new List<Door>();

        foreach (Door d in doors)
        {
            //calcolo l'offset della stanza per questa porta
            Vector2Int offset = position - d.slotPosition;
            bool occupied = false;
            foreach (Slot s in roomToTest.slots)
            {
                occupied |= stanzeLayout.ContainsKey(offset + s.index);
            }

            if (!occupied)
                possibleFit.Add(d);
        }

        if (possibleFit.Count > 0)
        {
            int index= Random.Range(0, possibleFit.Count - 1);

            foundPosition = position - possibleFit[index].slotPosition;

            return true;
        }

        return false;

    }

    private void RecalculateDeadEnds()
    {
        Queue<Room> toRemove = new Queue<Room>();
        foreach (Room s in deadEnd)
        {
            if (!IsDeadEnd(s))
                toRemove.Enqueue(s);

        }

        while (toRemove.Count > 0)
            deadEnd.Remove(toRemove.Dequeue());

    }


    private bool IsDeadEnd(Room room)
    {
        return CountRoomsAround(room) == 1;

    }

    private bool IsDeadEnd(int x, int y)
    {
        return CountRoomsAround(new Vector2Int(x, y)) == 1;
    }


    private int CountRoomsAround(Room stanza)
    {
        List<Door> doors = stanza.roomData.GetDoors();
        List<Room> rooms = new List<Room>();
        int count = 0;
        foreach(Door d in doors)
        {

            Vector2Int pos = stanza.position + d.slotPosition + d.direction;
            count += CountVicino(pos.x,pos.y, rooms);
        }


        return count;
    }


    private int CountRoomsAround(Vector2Int position)
    {
        int count = 0;
        List<Room> rooms=new List<Room>();

        foreach (Vector2Int pos in posHelper)
        {
            count += CountVicino(pos.x + position.x, pos.y + position.y, rooms);
        }

        return count;
    }



    //il vlore booleano ritornato ci serve per capire se la stanza è una dead end
    private bool CheckNormalConditions(Vector2Int pos)
    {
        //non già presente

        if (stanzeLayout.ContainsKey(pos))
            return false;



        //random 50%
        if (Random.Range(0f, 1f) < 0.5f)
            return false;



        if (!IsDeadEnd(pos.x, pos.y))
            return false;

       
        if (stanze.Count == maxStanze)
            return false;

        return true;


    }


    //private int CountVicino(Vector2Int pos)
    //{
    //    return CountVicino(pos.x, pos.y);
    //}

    private int CountVicino(int x, int y,List<Room> rooms)
    {
        Room room;
        if (stanzeLayout.TryGetValue(new Vector2Int(x, y), out room))
        {
            if (!rooms.Contains(room))
            {
                rooms.Add(room);
                return 1;
            }
        }

        return 0;
    }


    private void InstantiateRooms()
    {
        foreach (Room r in stanze)
        {
            r.roomData= Instantiate(r.roomData.gameObject, r.position * config.slotSize , Quaternion.identity).GetComponent<RoomData>();
        }

        //foreach (SimpleRoom r in deadEnd)
        //{
        //    Instantiate(secret, new Vector3(r.x, r.y), Quaternion.identity);
        //}


    }

}
