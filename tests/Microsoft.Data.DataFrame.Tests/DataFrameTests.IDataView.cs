// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using Xunit;

namespace Microsoft.Data.Tests
{
    public partial class DataFrameTests
    {
        [Fact]
        public void TestIDataView()
        {
            // IDataView doesn't support null values
            IDataView dataView = MakeDataFrameWithAllColumnTypes(10, withNulls: false);

            DataDebuggerPreview preview = dataView.Preview();
            Assert.Equal(10, preview.RowView.Length);
            Assert.Equal(15, preview.ColumnView.Length);

            Assert.Equal("Byte", preview.ColumnView[0].Column.Name);
            Assert.Equal((byte)0, preview.ColumnView[0].Values[0]);
            Assert.Equal((byte)1, preview.ColumnView[0].Values[1]);

            Assert.Equal("Char", preview.ColumnView[1].Column.Name);
            Assert.Equal((ushort)65, preview.ColumnView[1].Values[0]);
            Assert.Equal((ushort)66, preview.ColumnView[1].Values[1]);

            Assert.Equal("Decimal", preview.ColumnView[2].Column.Name);
            Assert.Equal((double)0, preview.ColumnView[2].Values[0]);
            Assert.Equal((double)1, preview.ColumnView[2].Values[1]);

            Assert.Equal("Double", preview.ColumnView[3].Column.Name);
            Assert.Equal((double)0, preview.ColumnView[3].Values[0]);
            Assert.Equal((double)1, preview.ColumnView[3].Values[1]);

            Assert.Equal("Float", preview.ColumnView[4].Column.Name);
            Assert.Equal((float)0, preview.ColumnView[4].Values[0]);
            Assert.Equal((float)1, preview.ColumnView[4].Values[1]);

            Assert.Equal("Int", preview.ColumnView[5].Column.Name);
            Assert.Equal((int)0, preview.ColumnView[5].Values[0]);
            Assert.Equal((int)1, preview.ColumnView[5].Values[1]);

            Assert.Equal("Long", preview.ColumnView[6].Column.Name);
            Assert.Equal((long)0, preview.ColumnView[6].Values[0]);
            Assert.Equal((long)1, preview.ColumnView[6].Values[1]);

            Assert.Equal("Sbyte", preview.ColumnView[7].Column.Name);
            Assert.Equal((sbyte)0, preview.ColumnView[7].Values[0]);
            Assert.Equal((sbyte)1, preview.ColumnView[7].Values[1]);

            Assert.Equal("Short", preview.ColumnView[8].Column.Name);
            Assert.Equal((short)0, preview.ColumnView[8].Values[0]);
            Assert.Equal((short)1, preview.ColumnView[8].Values[1]);

            Assert.Equal("Uint", preview.ColumnView[9].Column.Name);
            Assert.Equal((uint)0, preview.ColumnView[9].Values[0]);
            Assert.Equal((uint)1, preview.ColumnView[9].Values[1]);

            Assert.Equal("Ulong", preview.ColumnView[10].Column.Name);
            Assert.Equal((ulong)0, preview.ColumnView[10].Values[0]);
            Assert.Equal((ulong)1, preview.ColumnView[10].Values[1]);

            Assert.Equal("Ushort", preview.ColumnView[11].Column.Name);
            Assert.Equal((ushort)0, preview.ColumnView[11].Values[0]);
            Assert.Equal((ushort)1, preview.ColumnView[11].Values[1]);

            Assert.Equal("String", preview.ColumnView[12].Column.Name);
            Assert.Equal("0".AsMemory(), preview.ColumnView[12].Values[0]);
            Assert.Equal("1".AsMemory(), preview.ColumnView[12].Values[1]);

            Assert.Equal("Bool", preview.ColumnView[13].Column.Name);
            Assert.Equal(true, preview.ColumnView[13].Values[0]);
            Assert.Equal(false, preview.ColumnView[13].Values[1]);

            Assert.Equal("ArrowString", preview.ColumnView[14].Column.Name);
            Assert.Equal("foo".ToString(), preview.ColumnView[14].Values[0].ToString());
            Assert.Equal("foo".ToString(), preview.ColumnView[14].Values[1].ToString());
        }

        [Fact]
        public void TestIDataViewSchemaInvalidate()
        {
            DataFrame df = MakeDataFrameWithAllMutableColumnTypes(10, withNulls: false);

            IDataView dataView = df;

            DataViewSchema schema = dataView.Schema;
            Assert.Equal(14, schema.Count);

            df.RemoveColumn("Bool");
            schema = dataView.Schema;
            Assert.Equal(13, schema.Count);

            BaseColumn boolColumn = new PrimitiveColumn<bool>("Bool", Enumerable.Range(0, (int)df.RowCount).Select(x => x % 2 == 1));
            df.InsertColumn(0, boolColumn);
            schema = dataView.Schema;
            Assert.Equal(14, schema.Count);
            Assert.Equal("Bool", schema[0].Name);

            BaseColumn boolClone = boolColumn.Clone();
            boolClone.Name = "BoolClone";
            df.SetColumn(1, boolClone);
            schema = dataView.Schema;
            Assert.Equal("BoolClone", schema[1].Name);
        }
    }
}
