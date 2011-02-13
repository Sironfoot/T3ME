using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcLibrary.Utils.Collections
{
    public class PagedList<T> : IPagedList<T>
    {
        protected List<T> Items = null;

        public PagedList()
        {
            Items = new List<T>();
        }

        public PagedList(int itemsPerPage, int pageNumber, int totalItems)
        {
            this.PageNumber = pageNumber;
            this.TotalItems = totalItems;

            Items = new List<T>(itemsPerPage);
        }

        public PagedList(IEnumerable<T> collection, int itemsPerPage, int pageNumber, int totalItems)
        {
            this.PageNumber = pageNumber;
            this.TotalItems = totalItems;
            this.ItemsPerPage = itemsPerPage;

            Items = new List<T>(collection);
        }

        public int TotalItems
        {
            get; set;
        }

        public int ItemsPerPage
        {
            get; set;
        }

        public int PageNumber
        {
            get; set;
        }

        public int TotalPages
        {
            get { return (int)Math.Ceiling((double)TotalItems / (double)ItemsPerPage); }
        }

        public int StartIndex
        {
            get { return (PageNumber - 1) * ItemsPerPage; }
        }

        public int EndIndex
        {
            get { return (PageNumber * ItemsPerPage) - 1; }
        }

        public void Add(T item)
        {
            Items.Add(item);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            Items.AddRange(collection);
        }

        public void Clear()
        {
            Items.Clear();
        }

        public bool Contains(T item)
        {
            return Items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return Items.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}