﻿// Copyright (c) Richard Dingwall, Arjen Post. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;

namespace ProtoBuf.Data.Tests
{
    internal class ProtoReaderContext
    {
        private const int ResultFieldHeader = 1;
        private const int ColumnFieldHeader = 2;
        private const int ColumnNameFieldHeader = 1;
        private const int ColumnTypeFieldHeader = 2;
        private const int NoneFieldHeader = 0;
        private const int RecordFieldHeader = 3;

        private const int ColumnSizeFieldHeader = 73;
        private const int ColumnPrecisionFieldHeader = 74;
        private const int ColumnScaleFieldHeader = 75;
        private const int ColumnDataTypeNameFieldHeader = 76;

        private readonly Stack<SubItemToken> tokens = new Stack<SubItemToken>();

        private readonly ProtoReader reader;

        public ProtoReaderContext(ProtoReader reader)
        {
            this.reader = reader;
        }

        public void ReadUntilResultEnd()
        {
            this.ReadUntilFieldValue();

            this.reader.ReadInt32();

            this.ReadExpectedFieldHeader(NoneFieldHeader);

            this.EndSubItem();

            this.ReadExpectedFieldHeader(NoneFieldHeader);

            this.EndSubItem();
        }

        public void ReadUntilField()
        {
            // this.ReadUntilColumnType();
            this.ReadUntilDataTypeName();

            // this.reader.ReadInt32();
            this.reader.ReadString();

            this.ReadExpectedFieldHeader(NoneFieldHeader);
            this.EndSubItem();
        }

        public void ReadUntilSize()
        {
            this.ReadUntilColumnType();

            this.reader.ReadInt32();

            this.ReadExpectedFieldHeader(ColumnSizeFieldHeader);
        }

        public void ReadUntilPrecision()
        {
            this.ReadUntilSize();

            this.reader.ReadInt32();

            this.ReadExpectedFieldHeader(ColumnPrecisionFieldHeader);
        }

        public void ReadUntilScale()
        {
            this.ReadUntilPrecision();

            this.reader.ReadInt32();

            this.ReadExpectedFieldHeader(ColumnScaleFieldHeader);
        }

        public void ReadUntilDataTypeName()
        {
            this.ReadUntilScale();

            this.reader.ReadInt32();

            this.ReadExpectedFieldHeader(ColumnDataTypeNameFieldHeader);
        }

        public void ReadUntilFieldValue()
        {
            this.ReadUntilField();

            this.ReadExpectedFieldHeader(RecordFieldHeader);
            this.StartSubItem();
            this.ReadExpectedFieldHeader(1);
        }

        public void ReadUntilColumnType()
        {
            this.ReadUntilColumnName();

            this.reader.ReadString();

            this.ReadExpectedFieldHeader(ColumnTypeFieldHeader);
        }

        public void ReadUntilColumnName()
        {
            this.ReadExpectedFieldHeader(ResultFieldHeader);
            this.StartSubItem();

            this.ReadExpectedFieldHeader(ColumnFieldHeader);
            this.StartSubItem();
            this.ReadExpectedFieldHeader(ColumnNameFieldHeader);
        }

        public void ReadExpectedFieldHeader(int expectedFieldHeader)
        {
            var fieldHeader = this.reader.ReadFieldHeader();

            if (fieldHeader != expectedFieldHeader)
            {
                throw new InvalidDataException($"Field header {expectedFieldHeader} expected, actual '{fieldHeader}'.");
            }
        }

        public void StartSubItem()
        {
            this.tokens.Push(ProtoReader.StartSubItem(this.reader));
        }

        public void EndSubItem()
        {
            ProtoReader.EndSubItem(this.tokens.Pop(), this.reader);
        }
    }
}
