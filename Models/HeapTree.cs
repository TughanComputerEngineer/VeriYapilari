using System;
using System.Collections.Generic;
using webapplication.Models;

public class HeapTree
{
    private List<Team> heap;

    public HeapTree()
    {
        heap = new List<Team>();
    }

    public void Insert(Team team)
    {
        heap.Add(team);
        HeapifyUp(heap.Count - 1);
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;
            if (heap[index].CurrentScore > heap[parent].CurrentScore)
            {
                Swap(index, parent);
                index = parent;
            }
            else break;
        }
    }

    private void HeapifyDown(int index)
    {
        int left, right, largest;

        while (true)
        {
            left = 2 * index + 1;
            right = 2 * index + 2;
            largest = index;

            if (left < heap.Count && heap[left].CurrentScore > heap[largest].CurrentScore)
                largest = left;

            if (right < heap.Count && heap[right].CurrentScore > heap[largest].CurrentScore)
                largest = right;

            if (largest == index)
                break;

            Swap(index, largest);
            index = largest;
        }
    }

    private void Swap(int i, int j)
    {
        var temp = heap[i];
        heap[i] = heap[j];
        heap[j] = temp;
    }

    
    public List<Team> ToSortedList()
    {
        var result = new List<Team>();
        var tempHeap = new List<Team>(heap); 

        while (tempHeap.Count > 0)
        {
            result.Add(tempHeap[0]);

            
            int last = tempHeap.Count - 1;
            tempHeap[0] = tempHeap[last];
            tempHeap.RemoveAt(last);

            
            int i = 0;
            while (true)
            {
                int left = 2 * i + 1;
                int right = 2 * i + 2;
                int largest = i;

                if (left < tempHeap.Count && tempHeap[left].CurrentScore > tempHeap[largest].CurrentScore)
                    largest = left;
                if (right < tempHeap.Count && tempHeap[right].CurrentScore > tempHeap[largest].CurrentScore)
                    largest = right;

                if (largest == i)
                    break;

                var tmp = tempHeap[i];
                tempHeap[i] = tempHeap[largest];
                tempHeap[largest] = tmp;
                i = largest;
            }
        }

        return result;
    }
}
