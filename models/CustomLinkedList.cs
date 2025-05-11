using Microsoft.AspNetCore.Diagnostics;
using webapplication.Models;

namespace webapplication.Models
{
    public class LinkNode
    {
        public Team team { get; set; }
        public LinkNode? next { get; set; }

        public LinkNode(Team team_data)
        {
            team = team_data;
        }
    }
    public class CustomLinkedList
    {
        public LinkNode root { get; set; }

        public CustomLinkedList()
        {
            root = null;
        }

        public void Add(Team team_data)
        {
            LinkNode newNode = new LinkNode(team_data);
            if(root == null)
            {
                root = newNode;
            }

            else
            {
                LinkNode temp = root;
                while(temp.next != null)
                {
                    temp = temp.next;
                }
                temp.next = newNode;
            }
        }

     public void Remove(LinkNode teamNode)
{
    if (root == null) return;

    if (root == teamNode)
    {
        root = root.next;
        return;
    }

    LinkNode current = root;
    while (current.next != null)
    {
        if (current.next == teamNode)
        {
            current.next = current.next.next;
            return;
        }
        current = current.next;
    }
}


        public Team? getRoot()
        {
            return root?.team;
        }
    }
}
