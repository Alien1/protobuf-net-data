// Copyright (c) Richard Dingwall, Arjen Post. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;

namespace ProtoBuf.Data.Internal
{
    internal static class ProtoDataColumnFactory
    {
        private static readonly bool IsRunningOnMono;

        static ProtoDataColumnFactory()
        {
            // From http://stackoverflow.com/a/721194/91551
            IsRunningOnMono = Type.GetType("Mono.Runtime") != null;
        }

        public static IList<ProtoDataColumn> GetColumns(IDataReader reader, ProtoDataWriterOptions options)
        {
            using (var schemaTable = reader.GetSchemaTable())
            {
                var schemaSupportsExpressions = schemaTable.Columns.Contains("Expression");

                var columns = new List<ProtoDataColumn>(schemaTable.Rows.Count);

                for (var i = 0; i < schemaTable.Rows.Count; i++)
                {
                    // Assumption: rows in the schema table are always ordered by
                    // Ordinal position, ascending
                    var row = schemaTable.Rows[i];

                    // Skip computed columns unless requested.
                    if (schemaSupportsExpressions)
                    {
                        bool isComputedColumn;

                        if (IsRunningOnMono)
                        {
                            isComputedColumn = Equals(row["Expression"], string.Empty);
                        }
                        else
                        {
                            isComputedColumn = !(row["Expression"] is DBNull);
                        }

                        if (isComputedColumn && !options.IncludeComputedColumns)
                        {
                            continue;
                        }
                    }

                    var columnName = (string)row["ColumnName"];
                    var dataType = (Type)row["DataType"];
                    var protoBufDataType = TypeHelper.GetProtoDataType(dataType);

                    var columnSize = schemaTable.Columns.Contains("ColumnSize")
                        ? (int)row["ColumnSize"]
                        : -1;

                    var columnPrecision = !schemaTable.Columns.Contains("NumericPrecision")
                        ? (short)-1
                        : row["NumericPrecision"] != DBNull.Value
                            ? (short)row["NumericPrecision"]
                            : (short)-1;

                    var columnScale = !schemaTable.Columns.Contains("NumericScale")
                        ? (short)-1
                        : row["NumericScale"] != DBNull.Value
                            ? (short)row["NumericScale"]
                            : (short)-1;

                    var dataTypeName = schemaTable.Columns.Contains("DataTypeName")
                        ? row["DataTypeName"].ToString()
                        : ((Type)row["DataType"]).Name;

                    columns.Add(new ProtoDataColumn(columnName, dataType, protoBufDataType)
                    {
                        ColumnSize = columnSize,
                        NumericPrecision = columnPrecision,
                        NumericScale = columnScale,
                        SourceDataTypeName = dataTypeName
                    });
                }

                return columns;
            }
        }
    }
}