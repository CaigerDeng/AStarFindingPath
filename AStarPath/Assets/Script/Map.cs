using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Map : MonoBehaviour
{
    private Image startImg;
    private Image endImg;
    private const int WIDTH = 32;
    private const int HEIGHT = 18;
    private Node[,] grid;

    void Start()
    {
        InitGrid();
    }

    void Update()
    {
        FindingPath();
    }

    public void SetStartImage(Image img)
    {
        if (startImg != null)
        {
            startImg.color = Color.white;
        }
        startImg = img;
        startImg.color = Color.green;
    }

    public void SetEndImage(Image img)
    {
        if (endImg != null)
        {
            endImg.color = Color.white;
        }
        endImg = img;
        endImg.color = Color.blue;
    }

    private void InitGrid()
    {
        grid = new Node[WIDTH, HEIGHT];
        for (int i = 0; i < WIDTH; i++)
        {
            for (int j = 0; j < HEIGHT; j++)
            {
                // GridLayoutGroup has been set to count from the bottom left corner 
                int index = j * WIDTH + i;
                Transform tra = transform.GetChild(index);
                bool isWall = (tra.tag == "Wall");
                Image img = tra.GetComponent<Image>();
                grid[i, j] = new Node(isWall, i, j, img, this);
            }
        }
    }

    private void GeneratePath(Node startNode, Node endNode)
    {
        if (startImg == null || endImg == null)
        {
            return;
        }
        foreach (Node node in grid)
        {
            node.HidePath();
        }
        if (endNode != null)
        {
            Node node = endNode;
            while (node != startNode)
            {
                node.ShowPath();
                node = node.parent;
            }
        }
    }

    // get neibourhood node used Manhattan valuation
    private List<Node> GetNeibourhoodByMan(Node node)
    {
        List<Node> list = new List<Node>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // skip myself
                if (i == 0 && j == 0)
                {
                    continue;
                }
                // skip the four corners
                if (i == 1 && j == 1)
                {
                    continue;
                }
                if (i == -1 && j == -1)
                {
                    continue;
                }
                if (i == 1 && j == -1)
                {
                    continue;
                }
                if (i == -1 && j == 1)
                {
                    continue;
                }
                int x = node.oriX + i;
                int y = node.oriY + j;
                // determine if it is out of bounds, if not, add it to the list
                if (x < WIDTH && x >= 0 && y < HEIGHT && y >= 0)
                {
                    list.Add(grid[x, y]);
                }
            }
        }
        return list;
    }

    // get the distance between two nodes used Manhattan valuation
    private int GetDistanceNodesByMan(Node a, Node b)
    {
        int cntX = Mathf.Abs(a.oriX - b.oriX);
        int cntY = Mathf.Abs(a.oriY - b.oriY);
        return 10 * (cntX + cntY);
    }

    private Node FindNode(Image img)
    {
        foreach (Node node in grid)
        {
            if (img == node.img)
            {
                return node;
            }
        }
        return null;
    }

    public class Node
    {
        private Map map;
        public bool isWall = false;
        public int oriX = 0;
        public int oriY = 0;
        public Image img;
        // length from the starting point
        public int gCost;
        // length from the ending point
        public int hCost;
        // total path length
        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }
        public Node parent;

        public Node(bool isWall, int x, int y, Image img, Map map)
        {
            this.map = map;
            this.isWall = isWall;
            oriX = x;
            oriY = y;
            this.img = img;
        }

        public void ShowPath()
        {
            if (isWall || img == map.startImg || img == map.endImg)
            {
                return;
            }
            img.color = Color.yellow;
        }

        public void HidePath()
        {
            if (isWall || img == map.startImg || img == map.endImg)
            {
                return;
            }
            img.color = Color.white;
        }

    }

    void FindingPath()
    {
        if (startImg == null || endImg == null)
        {
            return;
        }
        Node startNode = FindNode(startImg);
        Node endNode = FindNode(endImg);
        List<Node> openList = new List<Node>();
        List<Node> closeList = new List<Node>();
        openList.Add(startNode);
        while (openList.Count > 0)
        {
            //find curNode
            Node curNode = openList[0];
            for (int i = 0; i < openList.Count; i++)
            {
                // the total path should be as small as possible and as close as possible to the target
                if (openList[i].fCost <= curNode.fCost && openList[i].hCost <= curNode.hCost)
                {
                    curNode = openList[i];
                }
            }
            openList.Remove(curNode);
            closeList.Add(curNode);
            if (curNode == endNode)
            {
                GeneratePath(startNode, endNode);
                return;
            }
            foreach (Node item in GetNeibourhoodByMan(curNode))
            {
                if (item.isWall || closeList.Contains(item))
                {
                    continue;
                }
                if (!openList.Contains(item))
                {
                    // update length
                    item.gCost = GetDistanceNodesByMan(item, startNode);
                    item.hCost = GetDistanceNodesByMan(item, endNode);
                    item.parent = curNode;
                    openList.Add(item);
                }
            }
        }
        GeneratePath(startNode, null);

    }




}
