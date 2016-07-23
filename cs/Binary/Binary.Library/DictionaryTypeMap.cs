using System;
using System.Collections.Generic;

namespace Binary.Library
{
    public sealed class DictionaryTypeMap : ITypeMap
    {
        private readonly IReadOnlyDictionary<Type, uint> map;

        public DictionaryTypeMap(IReadOnlyDictionary<Type, uint> map)
        {
            if (map == null) throw new ArgumentNullException(nameof(map));

            this.map = map;
        }

        public uint BinaryTypeFor<T>()
        {
            uint binaryType;
            if (this.map.TryGetValue(typeof(T), out binaryType))
            {
                return binaryType;
            }
            else
            {
                throw new InvalidOperationException($"Unable to find binary type for {typeof(T).FullName}");
            }
        }
    }
}
