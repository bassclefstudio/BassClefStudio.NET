﻿using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Sync
{
    /// <summary>
    /// Represents synced information about the whole of a synced collection of items.
    /// </summary>
    /// <typeparam name="TKey">The unique key which this table of items is built on.</typeparam>
    public interface ISyncCollectionInfo<T, TKey> : ISyncInfo<T> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets a collection of <typeparamref name="TKey"/> keys for every item found in the collection.
        /// </summary>
        IEnumerable<TKey> GetKeys();
    }

    /// <summary>
    /// Represents a synced keyed <see cref="ISyncCollection{T}"/> of <see cref="ISyncItem{T}"/>s of type <typeparamref name="TItem"/>.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection. This must inherit from <see cref="IKeyedSyncItem{T, TKey}"/> to support required syncing of items and getting/setting by key.</typeparam>
    /// <typeparam name="TItem">The type of items in the collection.</typeparam>
    /// <typeparam name="TKey">The type of the key used to sync each item in the collection.</typeparam>
    public abstract class SyncCollection<T, TItem, TKey> : Observable, ISyncCollection<T> where T : IKeyedSyncItem<TItem, TKey> where TItem : IIdentifiable<TKey> where TKey : IEquatable<TKey>
    {
        private ObservableCollection<T> item;
        /// <inheritdoc/>
        public ObservableCollection<T> Item { get => item; set => Set(ref item, value); }

        private bool isLoading;
        /// <inheritdoc/>
        public bool IsLoading { get => isLoading; set => Set(ref isLoading, value); }

        /// <summary>
        /// Creates a new empty <see cref="SyncCollection{T, TItem, TKey}"/>.
        /// </summary>
        public SyncCollection()
        {
            Item = new ObservableCollection<T>();
        }

        /// <summary>
        /// Gets an <see cref="ILink{T}"/> connection for a child item with the given key.
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> key of the item.</param>
        protected abstract ILink<TItem> GetLink(TKey key);

        /// <summary>
        /// Gets the generic collection info which is used to sync and build the collection.
        /// </summary>
        protected abstract Task<ISyncCollectionInfo<TItem, TKey>> GetCollectionInfo();

        /// <summary>
        /// Creates a <typeparamref name="T"/> item to populate a new item in the collection.
        /// </summary>
        /// <param name="link">The <see cref="ILink{T}"/> created for the syncing of the item.</param>
        protected abstract T CreateSyncItem(ILink<TItem> link);

        /// <inheritdoc/>
        public async Task UpdateAsync(ISyncInfo<ObservableCollection<T>> info = null)
        {
            IsLoading = true;
            var collectionInfo = await GetCollectionInfo();
            if(Item == null)
            {
                Item = new ObservableCollection<T>();
            }
            Item.Sync(collectionInfo.GetKeys(), i => CreateSyncItem(GetLink(i)));
            await Item.Select(i => i.UpdateAsync(collectionInfo)).RunParallelAsync();
            IsLoading = false;
        }

        /// <inheritdoc/>
        public async Task PushAsync(ISyncInfo<ObservableCollection<T>> info = null)
        {
            IsLoading = true;
            if (info is ISyncInfo<TItem> syncInfo)
            {
                await Item.Select(i => i.PushAsync(syncInfo)).RunParallelAsync();
            }
            else
            {
                await Item.Select(i => i.PushAsync()).RunParallelAsync();
            }
            IsLoading = false;
        }
    }
}
