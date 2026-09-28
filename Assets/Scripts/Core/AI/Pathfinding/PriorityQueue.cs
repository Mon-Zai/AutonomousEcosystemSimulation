using System.Collections.Generic;
using AI.Pathfinding;
public class PriorityQueue
{
    private List<Node> heap = new List<Node>();
    private Dictionary<Node, float> priorities = new Dictionary<Node, float>();
    private Dictionary<Node, int> indices = new Dictionary<Node, int>();

    public int Count { get { return heap.Count; } }

    public void Enqueue(Node item, float cost)
    {
        if (priorities.ContainsKey(item))
        {
            UpdatePriority(item, cost);
        }
        else
        {
            heap.Add(item);
            int index = heap.Count - 1;
            indices[item] = index;
            priorities[item] = cost;
            BubbleUp(index);
        }
    }

    public Node Dequeue()
    {
        if (heap.Count == 0) return null;

        Node root = heap[0];

        int lastIndex = heap.Count - 1;
        Node lastItem = heap[lastIndex];
        heap[0] = lastItem;
        indices[lastItem] = 0;
        heap.RemoveAt(lastIndex);

        priorities.Remove(root);
        indices.Remove(root);

        if (heap.Count > 0)
        {
            BubbleDown(0);
        }

        return root;
    }

    private void UpdatePriority(Node item, float newCost)
    {
        float oldCost = priorities[item];
        priorities[item] = newCost;
        int index = indices[item];

        if (newCost < oldCost)
        {
            BubbleUp(index);
        }
        else if (newCost > oldCost)
        {
            BubbleDown(index);
        }
    }

    private void BubbleUp(int index)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;

            if (priorities[heap[index]] < priorities[heap[parentIndex]])
            {
                Swap(index, parentIndex);
                index = parentIndex;
            }
            else break;
        }
    }

    private void BubbleDown(int index)
    {
        int count = heap.Count;

        while (true)
        {
            int leftChild = index * 2 + 1;
            int rightChild = index * 2 + 2;
            int smallest = index;

            if (leftChild < count && priorities[heap[leftChild]] < priorities[heap[smallest]])
                smallest = leftChild;

            if (rightChild < count && priorities[heap[rightChild]] < priorities[heap[smallest]])
                smallest = rightChild;

            if (smallest == index) break;

            Swap(index, smallest);
            index = smallest;
        }
    }

    private void Swap(int i, int j)
    {
        Node temp = heap[i];
        heap[i] = heap[j];
        heap[j] = temp;

        indices[heap[i]] = i;
        indices[heap[j]] = j;
    }
}
