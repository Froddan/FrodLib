namespace FrodLib
{
    /// <summary>Represents a collection that accepts a sequence of items.</summary>
    public interface IPush<in T>
    {
        void Push(T item);
    }
    /// <summary>Represents a collection that produces a sequence of items, and can
    /// return the next item without popping it (the Peek operation).</summary>
    /// <remarks>Push/Pop methods that throw an exception on failure, and
    /// TryPush/TryPop methods that don't require a "ref" argument, are
    /// available as extension methods.</remarks>
    public interface IPop<T>
    {
        bool TryPop(out T item);
        bool TryPeek(out T item);
        bool IsEmpty { get; }
    }

    /// <summary>Represents a FIFO (first-in-first-out) queue.</summary>
    /// <typeparam name="T">Type of each element</typeparam>
    public interface IQueue<T> : IPush<T>, IPop<T>, ICount
    {
    }
    /// <summary>Represents a LIFO (last-in-first-out) stack.</summary>
    /// <typeparam name="T">Type of each element</typeparam>
    public interface IStack<T> : IPush<T>, IPop<T>, ICount
    {
    }
    /// <summary>Represents a double-ended queue that allows items to be added or
    /// removed at the beginning or end.</summary>
    /// <typeparam name="T">Type of each element</typeparam>
    public interface IDeque<T> : IIsEmpty, ICount
    {
        void PushFirst(T item);
        void PushLast(T item);
        bool TryPopFirst(out T item);
        bool TryPeekFirst(out T item);
        bool TryPopLast(out T item);
        bool TryPeekLast(out T item);
        /// <summary>Gets the first item in the deque.</summary>
        /// <exception cref="InvalidOperationException"></exception>
        T First { get; set; }
        T Last { get; set; }
    }
}