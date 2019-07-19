﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPLedit
{
    public sealed class RegisterStore : IDisposable
    {
        private readonly Dictionary<Type, List<object>> store;
        private readonly List<IDisposable> disposableReferences;

        public RegisterStore()
        {
            store = new Dictionary<Type, List<object>>();
            disposableReferences = new List<IDisposable>();
        }

        public void Register<T>(T elem)
        {
            Type t = typeof(T);
            if (!store.ContainsKey(t))
                store.Add(typeof(T), new List<object>());
            store[t].Add(elem);
            if (elem is IDisposable disposable)
                disposableReferences.Add(disposable);
        }

        public T[] GetRegistered<T>()
        {
            store.TryGetValue(typeof(T), out List<object> res);

            return res?.Select(o => (T)o).ToArray() ?? new T[0];
        }

        public void Dispose()
        {
            foreach (var d in disposableReferences)
                d?.Dispose();
        }
    }
}
