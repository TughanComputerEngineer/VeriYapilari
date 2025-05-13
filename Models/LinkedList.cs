// Models/LinkedList.cs
using System.Collections.Generic;

namespace webapplication.Models
{
    public class TeamNode
    {
        public Team TeamData { get; set; }
        public TeamNode Next { get; set; }

        public TeamNode(Team team)
        {
            TeamData = team;
            Next = null;
        }
    }

    public class TeamLinkedList
    {
        public TeamNode Head { get; private set; }
        public TeamNode Tail { get; private set; }
        public int Count { get; private set; }

        public TeamLinkedList()
        {
            Head = null;
            Tail = null;
            Count = 0;
        }

        public void Add(Team team)
        {
            TeamNode newNode = new TeamNode(team);

            if (Head == null)
            {
                Head = newNode;
                Tail = newNode;
            }
            else
            {
                Tail.Next = newNode;
                Tail = newNode;
            }

            Count++;
        }

        public bool Remove(Team team)
        {
            if (Head == null)
                return false;

            TeamNode current = Head;
            TeamNode previous = null;

            while (current != null)
            {
                if (current.TeamData.Id == team.Id)
                {
                    if (previous == null)
                    {
                        // Removing head
                        Head = current.Next;
                        if (Head == null)
                        {
                            Tail = null;
                        }
                    }
                    else
                    {
                        previous.Next = current.Next;
                        if (current.Next == null)
                        {
                            Tail = previous;
                        }
                    }

                    Count--;
                    return true;
                }

                previous = current;
                current = current.Next;
            }

            return false;
        }

        public bool Contains(int teamId)
        {
            TeamNode current = Head;

            while (current != null)
            {
                if (current.TeamData.Id == teamId)
                    return true;

                current = current.Next;
            }

            return false;
        }

        public List<Team> ToList()
        {
            List<Team> list = new List<Team>();
            TeamNode current = Head;

            while (current != null)
            {
                list.Add(current.TeamData);
                current = current.Next;
            }

            return list;
        }
    }
}
