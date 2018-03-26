using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolClass.LinkedList
{
    public interface ILinearList<T>:IEnumerable<T> 
    {
        void Add(T t);
        void AddHead(T t);
        void AddTail(T t);
        void Clear();
        int Count { get; }
        int Find(T t);
        T GetAt(int pos);
        T GetHead();
        T GetTail();
        void InsertAt(T t, int pos);
        bool IsEmpty { get; }
        void RemoveAll();
        void RemoveAt(int pos);
        void RemoveHead();
        void RemoveTail();
        void SetAt(int pos, T t); 

    }
}
