using Microsoft.AspNetCore.Diagnostics;
using webapplication.Models;

// Models/HeapNode.cs
public class HeapNode
{
    public Team team { get; set; }

    public HeapNode? Left { get; set; }
    public HeapNode? Right { get; set; }
    public HeapNode? Parent { get; set; }

    public HeapNode(Team team_data)
    {
        team = team_data;
    }
}

// Models/HeapTree.cs
public class HeapTree
{
    public HeapNode? Root { get; private set; }

    private Queue<HeapNode> insertionQueue = new();

    public void Insert(Team team_data)
    {
        HeapNode newNode = new HeapNode(team_data);

        if (Root == null)
        {
            Root = newNode;
            insertionQueue.Enqueue(newNode);
            return;
        }

        // En uygun parent'ı bul
        HeapNode parent = insertionQueue.Peek();

        newNode.Parent = parent;
        if (parent.Left == null)
        {
            parent.Left = newNode;
        }
        else if (parent.Right == null)
        {
            parent.Right = newNode;
            insertionQueue.Dequeue();
        }

        insertionQueue.Enqueue(newNode);

        HeapifyUp(newNode);
    }

    private void HeapifyUp(HeapNode node)
    {
        while (node.Parent != null && node.team.CurrentScore > node.Parent.team.CurrentScore)
        {
            // Swap değerleri
            (node.team.CurrentScore, node.Parent.team.CurrentScore) = (node.Parent.team.CurrentScore, node.team.CurrentScore);
            node = node.Parent;
        }
    }

    public HeapNode? GetMax()
    {
        return Root;
    }
}
