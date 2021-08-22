// Copyright (c) Richard Dingwall, Arjen Post. See LICENSE in the project root for license information.

namespace ProtoBuf.Data.Internal
{
    internal static class ColumnsWriter
    {
        private const int ColumnFieldHeader = 2;
        private const int ColumnNameFieldHeader = 1;
        private const int ColumnTypeFieldHeader = 2;

        private const int ColumnSizeFieldHeader = 73;
        private const int ColumnPrecisionFieldHeader = 74;
        private const int ColumnScaleFieldHeader = 75;
        private const int ColumnDataTypeNameFieldHeader = 76;

        public static void WriteColumns(ProtoWriterContext context)
        {
            foreach (var column in context.Columns)
            {
                ProtoWriter.WriteFieldHeader(ColumnFieldHeader, WireType.StartGroup, context.Writer);

                context.StartSubItem(column);

                WriteColumnName(context, column);
                WriteColumnType(context, column);
                WriteColumnSize(context, column);
                WriteColumnPrcn(context, column);
                WriteColumnScle(context, column);
                WriteColumnSDTN(context, column);

                context.EndSubItem();
            }
        }

        private static void WriteColumnName(ProtoWriterContext context, ProtoDataColumn column)
        {
            ProtoWriter.WriteFieldHeader(ColumnNameFieldHeader, WireType.String, context.Writer);
            ProtoWriter.WriteString(column.Name, context.Writer);
        }

        private static void WriteColumnType(ProtoWriterContext context, ProtoDataColumn column)
        {
            ProtoWriter.WriteFieldHeader(ColumnTypeFieldHeader, WireType.Variant, context.Writer);
            ProtoWriter.WriteInt32((int)column.ProtoDataType, context.Writer);
        }

        private static void WriteColumnSize(ProtoWriterContext context, ProtoDataColumn column)
        {
            ProtoWriter.WriteFieldHeader(ColumnSizeFieldHeader, WireType.Variant, context.Writer);
            ProtoWriter.WriteInt32(column.ColumnSize, context.Writer);
        }

        private static void WriteColumnPrcn(ProtoWriterContext context, ProtoDataColumn column)
        {
            ProtoWriter.WriteFieldHeader(ColumnPrecisionFieldHeader, WireType.Variant, context.Writer);
            ProtoWriter.WriteInt32(column.NumericPrecision, context.Writer);
        }

        private static void WriteColumnScle(ProtoWriterContext context, ProtoDataColumn column)
        {
            ProtoWriter.WriteFieldHeader(ColumnScaleFieldHeader, WireType.Variant, context.Writer);
            ProtoWriter.WriteInt32(column.NumericScale, context.Writer);
        }

        private static void WriteColumnSDTN(ProtoWriterContext context, ProtoDataColumn column)
        {
            ProtoWriter.WriteFieldHeader(ColumnDataTypeNameFieldHeader, WireType.String, context.Writer);
            ProtoWriter.WriteString(column.SourceDataTypeName, context.Writer);
        }
    }
}