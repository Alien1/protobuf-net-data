// Copyright (c) Richard Dingwall, Arjen Post. See LICENSE in the project root for license information.

using System;

namespace ProtoBuf.Data.Internal
{
    internal sealed class ProtoDataColumn
    {
        public ProtoDataColumn(string name, Type dataType, ProtoDataType protoBufDataType)
        {
            this.Name = name;
            this.DataType = dataType;

            this.ProtoDataType = protoBufDataType;
        }

        public string Name { get; }

        public Type DataType { get; }

        public int ColumnSize { get; set; }

        public int NumericPrecision { get; set; }

        public int NumericScale { get; set; }

        public string SourceDataTypeName { get; set; }

        public ProtoDataType ProtoDataType { get; }
    }
}
