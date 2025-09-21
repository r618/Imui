using System;
using System.Runtime.CompilerServices;

namespace Imui.Utility
{
    public struct ImCircularBuffer<T>
    {
        public readonly int Capacity;

        public ref T this[Index index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Array[(Head + (index.IsFromEnd ? Count - index.Value : index.Value)) % Capacity];
        }
        
        public int Head;
        public int Count;
        public T[] Array;

        public ImCircularBuffer(int capacity)
        {
            Capacity = capacity;

            Head = 0;
            Count = 0;
            Array = new T[capacity];
        }

        public ImCircularBuffer(T[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            Capacity = array.Length;

            Head = 0;
            Count = 0;
            Array = array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(int index)
        {
            return ref Array[(Head + index) % Capacity];
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int index, T value)
        {
            Array[(Head + index) % Capacity] = value;
        }

        public void Clear()
        {
            Head = 0;
            Count = 0;
        }

        public int PushBack(T value)
        {
            Head = (Head - 1) % Capacity;

            if (Head < 0)
            {
                Head += Capacity;
            }

            Array[Head] = value;

            if (Count < Capacity)
            {
                Count++;
            }

            return Head;
        }

        public bool TryPopBack(out T value)
        {
            if (Count == 0)
            {
                value = default;
                return false;
            }

            value = Array[Head];
            Head = (Head + 1) % Capacity;
            Count--;
            return true;
        }

        public int PushFront(T value)
        {
            var index = (Head + Count) % Capacity;
            
            Array[index] = value;

            if (Count == Capacity)
            {
                Head = (Head + 1) % Capacity;
            }
            else
            {
                Count++;
            }

            return index;
        }
        
        public bool TryPeekFront(out T value)
        {
            if (Count == 0)
            {
                value = default;
                return false;
            }

            value = Array[(Head + Count - 1) % Capacity];
            return true;
        }

        public bool TryPopFront(out T value)
        {
            if (Count == 0)
            {
                value = default;
                return false;
            }

            value = Array[(Head + Count - 1) % Capacity];
            Count--;
            return true;
        }
    }
}
