using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolClass.LinkedList
{
    public class DoubleLink<T> 
    {
        private T m_Value; 
        private DoubleLink<T> m_Next; 
        private DoubleLink<T> m_Prior; 

        public T Value 
        { 
            get { return m_Value; } 
            set { m_Value = value; } 
        } 

        public DoubleLink<T> Next 
        { 
            get { return m_Next; } 
            set { m_Next = value; } 
        } 

        public DoubleLink<T> Prior 
        { 
            get { return m_Prior; } 
            set { m_Prior = value; } 
        } 

        public DoubleLink() 
        { 

        } 

        public DoubleLink(T t) 
        { 
            m_Value = t; 
        } 
    } 

    public class DoubleLinkedList<T>:ILinearList<T>
    { 
        protected int m_Count; 
        protected DoubleLink<T> m_Head; 
        protected DoubleLink<T> m_Tail; 

        public DoubleLinkedList() 
        { 
            m_Count = 0; 
            m_Head = new DoubleLink<T>(); 
            m_Tail = m_Head; 
        } 

        public DoubleLinkedList(T t) 
            : this() 
        { 
            m_Count = 1; 
            m_Head.Next = new DoubleLink<T>(t); 
            m_Tail = m_Head.Next; 
            m_Tail.Prior = m_Head; 
        } 

        public void Add(T t) 
        { 
            InsertAt(t, m_Count); 
        } 

        public void AddHead(T t) 
        { 
            InsertAt(t, 0); 
        } 

        public void AddTail(T t) 
        { 
            Add(t); 
        } 

        public void Clear() 
        { 
            m_Count = 0; 
            m_Tail = m_Head; 
            m_Head.Next = null; 
            m_Head.Prior = null; 
        } 

        public int Count 
        { 
            get { return m_Count; } 
        } 

        public int Find(T t) 
        { 
            DoubleLink<T> currentNode = m_Head; 
            int pos = 0; 

            while ((currentNode = currentNode.Next) != null) 
            { 
                if (currentNode.Value.Equals(t)) 
                { 
                    return pos; 
                } 

                pos++; 
            } 

            return -1; 
        } 

        public T GetAt(int pos) 
        { 
            return GetNodeAt(pos).Value; 
        } 

        public T GetHead() 
        { 
            return GetNodeAt(0).Value; 
        } 

        public T GetTail() 
        { 
            return m_Tail.Value; 
        } 

        public void InsertAt(T t, int pos) 
        { 
            if (pos > m_Count || pos < 0) 
            { 
                throw new IndexOutOfRangeException("pos"); 
            } 

            if (m_Count == int.MaxValue) 
            { 
                throw new ArithmeticException(); 
            } 

            DoubleLink<T> insertNode = new DoubleLink<T>(t); 

            if (pos == m_Count) 
            { 
                insertNode.Prior = m_Tail; 
                m_Tail.Next = insertNode; 
                m_Tail = insertNode; 
                m_Count++; 
                return; 
            } 

            DoubleLink<T> currentNode = GetNodeAt(pos); 

            insertNode.Prior = currentNode.Prior; 
            insertNode.Next = currentNode; 
            currentNode.Prior.Next = insertNode; 
            currentNode.Prior = insertNode; 
            m_Count++; 
        } 

        private DoubleLink<T> GetNodeAt(int pos) 
        { 
            if (pos >= m_Count || pos < 0) 
            { 
                throw new IndexOutOfRangeException("pos"); 
            } 

            DoubleLink<T> currentNode = null; 

            // 最近的途径找到pos位置的节点 
            if (pos > m_Count / 2) 
            { 
                currentNode = m_Tail; 
                pos = m_Count - pos - 1; 

                while (pos-- > 0) 
                { 
                    currentNode = currentNode.Prior; 
                } 
            } 
            else 
            { 
                currentNode = m_Head.Next; 

                while (pos-- > 0) 
                { 
                    currentNode = currentNode.Next; 
                } 
            } 

            return currentNode; 
        } 

        public bool IsEmpty 
        { 
            get { return m_Count == 0; } 
        } 

        public void RemoveAll() 
        { 
            Clear(); 
        } 

        public void RemoveAt(int pos) 
        { 
            if (pos == m_Count - 1) 
            { 
                m_Tail = m_Tail.Prior; 
                m_Tail.Next = null; 
                m_Count--; 
                return; 
            } 

            DoubleLink<T> currentNode = GetNodeAt(pos); 

            currentNode.Prior.Next = currentNode.Next; 
            currentNode.Next.Prior = currentNode.Prior; 
            m_Count--; 
        } 

        public void RemoveHead() 
        { 
            RemoveAt(0); 
        } 

        public void RemoveTail() 
        { 
            RemoveAt(m_Count - 1); 
        } 

        public void SetAt(int pos, T t) 
        { 
            GetNodeAt(pos).Value = t; 
        } 

        public IEnumerator<T> GetEnumerator() 
        { 
            DoubleLink<T> currentNode = m_Head; 

            while ((currentNode = currentNode.Next) != null) 
            { 
                yield return currentNode.Value; 
            } 
        } 

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() 
        { 
            return GetEnumerator(); 
        } 

    }
}
