// Copyright (c) Richard Dingwall, Arjen Post. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;

namespace ProtoBuf.Data.Internal
{
    internal static class ColumnsReader
    {
        private const int NoneFieldHeader = 0;
        private const int ColumnFieldHeader = 2;
        private const int ColumnNameFieldHeader = 1;
        private const int ColumnTypeFieldHeader = 2;

        private const int ColumnSizeFieldHeader = 73;
        private const int ColumnPrecisionFieldHeader = 74;
        private const int ColumnScaleFieldHeader = 75;
        private const int ColumnDataTypeNameFieldHeader = 76;

        public static void ReadColumns(ProtoReaderContext context)
        {
            if (context.CurrentFieldHeader != ColumnFieldHeader)
            {
                throw new InvalidDataException($"Field header '{ColumnFieldHeader}' expected, actual '{context.CurrentFieldHeader}'.");
            }

            context.Columns = new List<ProtoDataColumn>(ReadColumnsImpl(context));
        }

        private static IEnumerable<ProtoDataColumn> ReadColumnsImpl(ProtoReaderContext context)
        {
            do
            {
                context.StartSubItem();

                var name = ReadColumnName(context);
                var protoDataType = ReadColumnType(context);
                var size = ReadColumnSize(context);
                var precision = ReadColumnPrecision(context);
                var scale = ReadColumnScale(context);
                var sdtn = ReadColumnSourceDataTypeName(context);

                // Backwards compatibility or unnecessary?
                while (context.ReadFieldHeader() != NoneFieldHeader)
                {
                    context.Reader.SkipField();
                }

                context.EndSubItem();

                yield return new ProtoDataColumn(
                    name: name,
                    dataType: TypeHelper.GetType(protoDataType),
                    protoBufDataType: protoDataType)
                {
                    ColumnSize = size,
                    NumericPrecision = precision,
                    NumericScale = scale,
                    SourceDataTypeName = sdtn
                };
            }
            while (context.ReadFieldHeader() == ColumnFieldHeader);
        }

        private static string ReadColumnName(ProtoReaderContext context)
        {
            context.ReadExpectedFieldHeader(ColumnNameFieldHeader);

            return context.Reader.ReadString();
        }

        private static ProtoDataType ReadColumnType(ProtoReaderContext context)
        {
            context.ReadExpectedFieldHeader(ColumnTypeFieldHeader);

            return (ProtoDataType)context.Reader.ReadInt32();
        }

        private static int ReadColumnSize(ProtoReaderContext context)
        {
            context.ReadExpectedFieldHeader(ColumnSizeFieldHeader);

            return context.Reader.ReadInt32();
        }

        private static int ReadColumnPrecision(ProtoReaderContext context)
        {
            context.ReadExpectedFieldHeader(ColumnPrecisionFieldHeader);

            return context.Reader.ReadInt32();
        }

        private static int ReadColumnScale(ProtoReaderContext context)
        {
            context.ReadExpectedFieldHeader(ColumnScaleFieldHeader);

            return context.Reader.ReadInt32();
        }

        private static string ReadColumnSourceDataTypeName(ProtoReaderContext context)
        {
            context.ReadExpectedFieldHeader(ColumnDataTypeNameFieldHeader);

            return context.Reader.ReadString();
        }
    }
}
