using System.Collections.Generic;
using UnityEngine;

public class MYBSPDungeon : MonoBehaviour
{
    [SerializeField] private int width = 80, height = 48;

    public int maxDepth = 6;
    public int minLeafSize = 10;

    public Vector2Int roomSizeMin = new Vector2Int(4, 4);
    public Vector2Int roomSizeMax = new Vector2Int(12, 10);
    
    public float tileSize = 1.0f;
    public int boarder = 2;
    private int smallestPossibleRoot = 20;
    private Node _root;

    public Material floorMat;
    public GameObject floorPrefab;
    private bool[,] _grid; 
    
    System.Random rng;

    public double BiasToLongerRooms = 0.8f;

    public List<RectInt> _rooms = new List<RectInt>();
    public List<RectInt> _corridors = new List<RectInt>();
    public List<RectInt> _leafs = new List<RectInt>();

    private void Start()
    {
        Generate();
    }
    private class Node //The rectangle of space within the tree
    {
        public RectInt rect;
        public Node left, right;
        public RectInt? room;
        public Node(RectInt rectInt)
        {
            rect = rectInt;
        }

        //a Leaf is part of the split we make in our binary tree
        //rooms must go on leaves as they are able to be linked together through the tree
        bool IsLeaf()
        {
            if (left == null && right == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<Node> GetLeaves()
        {
            if (IsLeaf()) { yield return this; yield break; }
            if (left == null) foreach (var n in left.GetLeaves()) yield return n;
            if (right == null) foreach (var n in right.GetLeaves()) yield return n;
        }
    }

    // Generate():
    void Generate()
    {
        // 1) Clear previously spawned tiles
        ClearPreviousTiles();

        // 2) Initialise RNG
        rng = new System.Random();

        // 3) Create root RectInt for map interior
        //defencive programming to ensure we dont get a 0, Mathf.Max() returns the largest value passed in, smallestPossibleRoot is predefined and private
        RectInt root = new RectInt(boarder, boarder, Mathf.Max(smallestPossibleRoot, width - boarder), Mathf.Max(smallestPossibleRoot, height - boarder));
        _root = new Node(root);

        // 4) Build BSP tree with SplitRecursive
        SplitRecursive(_root, maxDepth);

        // 5) For each leaf: create a room
        foreach(Node leaf in _root.GetLeaves())
        {
            RectInt room = CreateRoomInsideLeaf(leaf.rect);
            leaf.room = room;
            _rooms.Add(room);
            _leafs.Add(leaf.rect);
        }

        // 6) Connect rooms via corridors

        // 7) Rasterize rooms + corridors into _grid
        _grid = new bool[width, height];
        RasterizeRoomsAndCorridors();

        // 8) Instantiate floor cubes from _grid
        SpawnFloorCubes();
    }

    private void SpawnFloorCubes()
    {
        GameObject parent = new GameObject("BSP DUNGEON FLOOR");
        Vector3 pos = new Vector3(0, 0, 0);

        for(int x = 0; x < _grid.GetLength(0) - 1; x++)
            for(int y = 0; y < _grid.GetLength(1) - 1; y++)
            {
                if (_grid[x,y])
                {
                    pos = new Vector3(x, 0, y);
                    Instantiate(floorPrefab, pos, Quaternion.identity, parent.transform);
                }
            }
    }

    private void RasterizeRoomsAndCorridors()
    {
        foreach(var rooms in _rooms)
        {
            FillRect(rooms, true);
        }
    }

    //converts Rects into a grid
    private void FillRect(RectInt r, bool value)
    {
        int x0 = Mathf.Clamp(r.xMin, 0, width - 1);
        int x1 = Mathf.Clamp(r.xMax - 1, 0, width - 1);
        int y0 = Mathf.Clamp(r.yMin, 0, height - 1);
        int y1 = Mathf.Clamp(r.yMax - 1 , 0, height - 1);

        for(int y = y0; y < y1; y++)
        {
            for(int x = x0; x < x1; x++)
            {
                _grid[x, y] = value;
            }
        }
    }

    private void ClearPreviousTiles()
    {
        //destroy everything previously made
        foreach (GameObject g in this.transform)
        {
            Destroy(g);
        }
    }

    void SplitRecursive(Node node, int depth)
    {
        // a) Stopping rules: if depth is high enough OR rect is too small, return
        if(depth >= maxDepth || node.rect.width < 2 + minLeafSize && node.rect.height < 2 + minLeafSize)
        {
            return;
        }
        // b) Decide whether we can split horizontally / vertically
        bool canSplitV = node.rect.width >= 2 + minLeafSize;
        bool canSplitH = node.rect.height >= 2 + minLeafSize;
        if(!canSplitH && !canSplitV)
        {
            return;
        }
        // c) Choose orientation (prefer longer axis) and a split line that keeps both children >= minLeafSize
        bool splitVert;
        if (canSplitH && canSplitV)
        {
            //A split can happen

            //prefer longer axis
            bool widthIsLonger = node.rect.width > node.rect.height;
            //deciding randomly if the room should be longer
            if (rng.NextDouble() < BiasToLongerRooms) 
                splitVert = widthIsLonger;
            else
                splitVert = !widthIsLonger;
        }
        else
        {
            splitVert = canSplitV;
        }

        // d) Create left/right (or top/bottom) child RectInts
        if (splitVert)
        {
            //dont want to put a room somewhere it wont fit
            int minX = node.rect.xMin + minLeafSize;
            int maxX = node.rect.xMax - minLeafSize;

            //overkill safety check (extremely slim chance of needing)
            if(minX >= maxX)
            {
                return;
            }

            //choose random slice of vert
            int splitXRand = rng.Next(minX, maxX);

            //create new two rectangles
            RectInt left = new RectInt(node.rect.xMin, node.rect.yMin, splitXRand - node.rect.xMin, node.rect.height);
            RectInt right = new RectInt(splitXRand, node.rect.yMin, node.rect.xMax - splitXRand, node.rect.height);
            node.left = new Node(left);
            node.right = new Node(right);

            //split vertically
        }
        else
        {
            //dont want to put a room somewhere it wont fit
            int minY = node.rect.yMin + minLeafSize;
            int maxY = node.rect.yMax - minLeafSize;

            //overkill safety check (extremely slim chance of needing)
            if (minY >= maxY)
            {
                return;
            }

            //choose random slice of vert
            int splitYRand = rng.Next(minY, maxY);

            //create new two rectangles
            RectInt top = new RectInt(node.rect.xMin, node.rect.yMin, node.rect.width, splitYRand - node.rect.yMin);
            RectInt bottom = new RectInt(node.rect.xMin, splitYRand, node.rect.width, node.rect.yMax - splitYRand);
            node.left = new Node(top);
            node.right = new Node(bottom);

            //split horisontally
        }

        // e) Create child Nodes and recurse
        
        SplitRecursive(node.left, depth + 1);
        SplitRecursive(node.right, depth + 1);
    }

    private RectInt CreateRoomInsideLeaf(RectInt thisLeaf)
    {
        int maxW = Mathf.Min(roomSizeMax.x, thisLeaf.width - 2 * boarder);
        int maxH = Mathf.Min(roomSizeMax.y, thisLeaf.height - 2 * boarder);

        int minW = Mathf.Min(roomSizeMax.x, maxW);
        int minH = Mathf.Min(roomSizeMax.y, maxH);

        //tiny 1x1 room incase leaf got too small
        if(minW <= 0 || minH <= 0)
        {
            return new RectInt((int)thisLeaf.center.x, (int)thisLeaf.center.y, 1, 1);
        }

        //randomly place room inside leaf
        int w = rng.Next(minW, maxW + 1);
        int h = rng.Next(minH, maxH + 1);

        int x = rng.Next(thisLeaf.xMin + boarder, thisLeaf.xMax - boarder - w + 1);
        int y = rng.Next(thisLeaf.yMin + boarder, thisLeaf.yMax - boarder - h + 1);

        return new RectInt(x, y, w, h);
    }


}

