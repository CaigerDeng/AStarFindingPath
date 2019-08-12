using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Map : MonoBehaviour
{
    private Image startImg;
    private Image endImg;
    private int width, height;
    private Node[,] grid; //地形

    void Start()
    {
        InitGrid(); 
    }

    void Update()
    {
        MyFindingPath();
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
        width = 32;
        height = 18;
        grid = new Node[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //控件已经设置成从左小角开始数
                int num = j * width + i;
                Transform tra = transform.GetChild(num);
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

    // 取得周围的节点 曼哈顿估价
    private List<Node> GetNeibourhoodByMan(Node node)
    {
        List<Node> list = new List<Node>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // 如果是自己，则跳过
                if (i == 0 && j == 0)
                    continue;
                //跳过四角
                if (i == 1 && j == 1)
                    continue;
                if (i == -1 && j == -1)
                    continue;
                if (i == 1 && j == -1)
                    continue;
                if (i == -1 && j == 1)
                    continue;
                int x = node.oriX + i;
                int y = node.oriY + j;
                // 判断是否越界，如果没有，加到列表中
                if (x < width && x >= 0 && y < height && y >= 0)
                    list.Add(grid[x, y]);
            }
        }
        return list;
    }

    //获取两个节点之间的距离
    //曼哈顿估价法
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
        //与起点的长度
        public int gCost;
        //与终点的长度
        public int hCost;
        // 总的路径长度
        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }
        //父节点
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
            if (isWall || img == map.startImg || img == map.endImg) return;
            img.color = Color.yellow;
        }

        public void HidePath()
        {
            if (isWall || img == map.startImg || img == map.endImg) return;
            img.color = Color.white;
        }

    }

    void MyFindingPath()
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
                //总路径要尽可能小，离目标点尽可能近
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
                    //更新数值
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
