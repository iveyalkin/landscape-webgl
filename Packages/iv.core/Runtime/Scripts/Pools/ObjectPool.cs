using System;
using System.Collections.Generic;

namespace IV.Core.Pools
{
    public class ObjectPool<TBase>
    {
        private readonly Dictionary<Type, List<TBase>> pool = new();

        public TConcrete Get<TConcrete>() where TConcrete : TBase, new()
        {
            var type = typeof(TConcrete);
            if (!pool.TryGetValue(type, out var list))
            {
                list = new List<TBase>();
                pool[type] = list;
            }

            if (list.Count == 0)
            {
                return new TConcrete();
            }

            var obj = list[^1];
            list.RemoveAt(list.Count - 1);
            return (TConcrete)obj;
        }

        public void Return(TBase obj)
        {
            var type = obj.GetType();
            if (!pool.TryGetValue(type, out var list))
            {
                list = new List<TBase>();
                pool[type] = list;
            }

            list.Add(obj);
        }
    }
}