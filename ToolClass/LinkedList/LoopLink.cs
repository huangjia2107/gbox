using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolClass.LinkedList
{
    public class LoopLink<T> : DoubleLinkedList<T> 
    {
        private DoubleLink<T> m_CurrentNode; 
        private int m_CurrentIndex; 

        public int CurrentIndex 
        { 
            get { return m_CurrentIndex; } 
        } 

        public LoopLink() : base() 
        { 
            m_CurrentNode = m_Head.Next; 
            m_CurrentIndex = 0; 
        }

        public LoopLink(T t) : base(t) 
        { 
            m_CurrentNode = m_Head.Next; 
            m_CurrentIndex = 0; 
        } 

        public T GetCurrent() 
        { 
            if (m_Count == 0) 
            { 
                throw new IndexOutOfRangeException(); 
            } 

            return m_CurrentNode.Value; 
        } 

        public T GetNext() 
        { 
            if (m_Count == 0) 
            { 
                throw new IndexOutOfRangeException(); 
            } 

            if (m_CurrentNode != null) 
            { 
                m_CurrentNode = m_CurrentNode.Next; 
                m_CurrentIndex++; 
            } 

            if (m_CurrentNode == null) 
            { 
                m_CurrentNode = m_Head.Next; 
                m_CurrentIndex = 0; 
            } 

            return m_CurrentNode.Value; 
        } 

        public T GetPrevious() 
        { 
            if (m_Count == 0) 
            { 
                throw new IndexOutOfRangeException(); 
            } 

            if (m_CurrentNode != null) 
            { 
                m_CurrentNode = m_CurrentNode.Prior; 
                m_CurrentIndex--; 
            } 

            if (m_CurrentNode == null || m_CurrentNode == m_Head) 
            { 
                m_CurrentNode = m_Tail; 
                m_CurrentIndex = m_Count - 1; 
            } 

            return m_CurrentNode.Value; 
        } 
    }
}
