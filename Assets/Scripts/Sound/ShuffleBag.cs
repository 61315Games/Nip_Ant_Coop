using System.Collections.Generic;
using UnityEngine;

public class ShuffleBag<T>
{
    private readonly List<T> source = new List<T>();
    private readonly List<T> bag = new List<T>();
    private readonly bool avoidRepeatOnRefill;

    private T last;
    private bool hasLast;

    public int Remaining => bag.Count;

    public ShuffleBag(IEnumerable<T> items, bool avoidRepeatOnRefill = true)
    {
        if(items != null)
            foreach(var item in items)
                if(item != null) source.Add(item);

        this.avoidRepeatOnRefill = avoidRepeatOnRefill;
    }

    public T Next()
    {
        if (source.Count == 0) return default;
        if(source.Count == 1) return source[0];

        if (bag.Count == 0)
        {
            bag.AddRange(source);
        }

        int idx = Random.Range(0, bag.Count);

        if (avoidRepeatOnRefill && hasLast && bag.Count > 1 &&
            EqualityComparer<T>.Default.Equals(bag[idx], last))
        {
            idx = (idx + 1 + Random.Range(0, bag.Count - 1)) % bag.Count;
        }

        T pick = bag[idx];
        bag.RemoveAt(idx);
        last = pick;
        hasLast = true;
        return pick;
    }

    public void Reset()
    {
        bag.Clear();
        hasLast = false;
    }
}
