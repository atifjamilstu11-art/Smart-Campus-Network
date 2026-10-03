using System;
using System.Collections.Generic;

namespace SmartCampusNetwork.API.Algorithms;

public class PriorityQueue<T>
{
    private readonly List<T> _items = new();
    private readonly IComparer<T> _comparer;

    public PriorityQueue(IComparer<T>? comparer = null)
    {
        _comparer = comparer ?? Comparer<T>.Default;
    }

    public int Count => _items.Count;

    public void Enqueue(T item)
    {
        _items.Add(item);
        var index = _items.Count - 1;

        while (index > 0)
        {
            var parentIndex = (index - 1) / 2;

            if (_comparer.Compare(_items[parentIndex], _items[index]) <= 0)
            {
                break;
            }

            (_items[parentIndex], _items[index]) = (_items[index], _items[parentIndex]);
            index = parentIndex;
        }
    }

    public T Dequeue()
    {
        if (_items.Count == 0)
        {
            throw new InvalidOperationException("Priority queue is empty.");
        }

        var result = _items[0];
        var lastItem = _items[^1];
        _items.RemoveAt(_items.Count - 1);

        if (_items.Count > 0)
        {
            _items[0] = lastItem;
            Heapify(0);
        }

        return result;
    }

    private void Heapify(int index)
    {
        var smallestIndex = index;
        var leftIndex = index * 2 + 1;
        var rightIndex = index * 2 + 2;

        if (leftIndex < _items.Count && _comparer.Compare(_items[leftIndex], _items[smallestIndex]) < 0)
        {
            smallestIndex = leftIndex;
        }

        if (rightIndex < _items.Count && _comparer.Compare(_items[rightIndex], _items[smallestIndex]) < 0)
        {
            smallestIndex = rightIndex;
        }

        if (smallestIndex != index)
        {
            (_items[index], _items[smallestIndex]) = (_items[smallestIndex], _items[index]);
            Heapify(smallestIndex);
        }
    }
}
