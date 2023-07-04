using System;
using System.Collections.Generic;

namespace Utility
{
    /// <summary>
    ///     Represents a distributor that provides a random distribution of items from the remaining options.
    /// </summary>
    /// <typeparam name="T">The type of items to distribute.</typeparam>
    public class ItemDistributor<T>
    {
        private readonly List<T> _items;
        private List<T> _remainingItems;
        private readonly Random _random;

        public int Count => _items.Count;
        public int RemainingCount => _remainingItems.Count;
        public int RandomHash => _random.GetHashCode();

        /// <summary>
        ///     Initializes a new instance of the <see cref="ItemDistributor{T}" /> class with the specified items.
        /// </summary>
        /// <param name="items">The list of items to distribute.</param>
        public ItemDistributor(List<T> items)
        {
            _items = items;
            _remainingItems = new List<T>(items);
            _random = new Random();
        }

        /// <summary>
        ///     Gets the next item from the distribution randomly, excluding previously chosen items.
        /// </summary>
        /// <returns>The next item from the remaining options.</returns>
        public T GetNextItem()
        {
            if (_remainingItems.Count == 0) throw new InvalidOperationException("No items available.");

            int randomIndex = _random.Next(_remainingItems.Count);
            T nextItem = _remainingItems[randomIndex];
            _remainingItems.RemoveAt(randomIndex);

            if (_remainingItems.Count == 0) _remainingItems = new List<T>(_items);

            return nextItem;
        }
    }
}