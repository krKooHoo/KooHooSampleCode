using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    private List<T> data;
    private IComparer<T> comparer;

    public PriorityQueue(IComparer<T> comparer)
    {
        this.data = new List<T>();
        this.comparer = comparer;
    }

    public void Enqueue(T item)
    {
        data.Add(item);
        int currentIndex = data.Count - 1;

        while (currentIndex > 0)
        {
            int parentIndex = (currentIndex - 1) / 2;

            if (comparer.Compare(data[currentIndex], data[parentIndex]) <= 0)
            {
                break;
            }

            (data[currentIndex], data[parentIndex]) = (data[parentIndex], data[currentIndex]);

            currentIndex = parentIndex;
        }
    }

    public T Dequeue()
    {
        int lastIndex = data.Count - 1;
        T frontItem = data[0];
        data[0] = data[lastIndex];
        data.RemoveAt(lastIndex--);

        int parentIndex = 0;

        while (true)
        {
            int leftChildIndex = parentIndex * 2 + 1;
            if (leftChildIndex > lastIndex) break;  

            int rightChildIndex = leftChildIndex + 1;
            if (rightChildIndex <= lastIndex && comparer.Compare(data[rightChildIndex], data[leftChildIndex]) > 0)
            {
                leftChildIndex = rightChildIndex;  
            }

            if (comparer.Compare(data[parentIndex], data[leftChildIndex]) >= 0) break; 

            (data[parentIndex], data[leftChildIndex]) = (data[leftChildIndex], data[parentIndex]); 

            parentIndex = leftChildIndex;
        }

        return frontItem;
    }

    public T Peek()
    {
        return data[0];
    }

    public int Count()
    {
        return data.Count;
    }

    public bool IsEmpty()
    {
        return data.Count == 0;
    }
}
