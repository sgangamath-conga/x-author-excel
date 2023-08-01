/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System.Collections;
using System.Collections.Generic;

namespace Apttus.XAuthor.Core
{

    public class ApttusObjectTree : IEnumerable<ApttusObjectTree>
    {
        private readonly Dictionary<string, ApttusObjectTree> _childs = new Dictionary<string, ApttusObjectTree>();
        public readonly string Id;
        public ApttusObjectTree Parent { get; private set; }

        public ApttusObjectTree(string id)
        {
            Id = id;
        }

        public ApttusObjectTree GetChild(string id)
        {
            return _childs[id];
        }

        public void Add(ApttusObjectTree item)
        {
            if (item.Parent != null)
            {
                item.Parent._childs.Remove(item.Id);
            }

            item.Parent = this;
            _childs.Add(item.Id, item);
        }

        public IEnumerator<ApttusObjectTree> GetEnumerator()
        {
            return _childs.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get { return _childs.Count; }
        }
    }
    public class Tree
    { }
}
