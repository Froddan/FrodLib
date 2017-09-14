namespace FrodLib
{
    /// <summary>Holds the Count property found in nearly all collection interfaces.</summary>
    /// <remarks>
    /// Microsoft has made this interface unusable by not defining it themselves in
    /// .NET 4.5. Now that I've replaced my original interface
    /// <code>
    /// interface ISource&lt;out T> : IEnumerable&lt;T>, ICount {}
    /// </code>
    /// with Microsoft's IReadOnlyCollection(T), the compiler complains constantly about
    /// "Ambiguity between IReadOnlyCollection(T).Count and ICount.Count". Eliminating
    /// ICount from most places seems to be the only solution.
    /// </remarks>
    public interface ICount : IIsEmpty
    {
        /// <summary>Gets the number of items in the collection.</summary>
        int Count { get; }
    }
    /// <summary>Holds the IsEmpty property that tells you if a collection is empty.</summary>
    public interface IIsEmpty
    {
        bool IsEmpty { get; }
    }
}