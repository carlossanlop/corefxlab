﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Arrow;
using Xunit;

namespace Microsoft.Data.Tests
{
    public class BufferTests
    {
        [Fact]
        public void TestNullCounts()
        {
            PrimitiveColumn<int> dataFrameColumn1 = new PrimitiveColumn<int>("Int1", Enumerable.Range(0, 10).Select(x => x));
            dataFrameColumn1.Append(null);
            Assert.Equal(1, dataFrameColumn1.NullCount);

            PrimitiveColumn<int> df2 = new PrimitiveColumn<int>("Int2");
            Assert.Equal(0, df2.NullCount);

            PrimitiveColumn<int> df3 = new PrimitiveColumn<int>("Int3", 10);
            Assert.Equal(0, df3.NullCount);

            // Test null counts with assignments on Primitive Columns
            df2.Append(null);
            df2.Append(1);
            Assert.Equal(1, df2.NullCount);
            df2[1] = 10;
            Assert.Equal(1, df2.NullCount);
            df2[1] = null;
            Assert.Equal(2, df2.NullCount);
            df2[1] = 5;
            Assert.Equal(1, df2.NullCount);
            df2[0] = null;
            Assert.Equal(1, df2.NullCount);

            // Test null counts with assignments on String Columns
            StringColumn strCol = new StringColumn("String", 0);
            Assert.Equal(0, strCol.NullCount);

            StringColumn strCol1 = new StringColumn("String1", 5);
            Assert.Equal(0, strCol1.NullCount);

            StringColumn strCol2 = new StringColumn("String", Enumerable.Range(0, 10).Select(x => x.ToString()));
            Assert.Equal(0, strCol2.NullCount);

            StringColumn strCol3 = new StringColumn("String", Enumerable.Range(0, 10).Select(x => (string)null));
            Assert.Equal(10, strCol3.NullCount);

            strCol.Append(null);
            Assert.Equal(1, strCol.NullCount);
            strCol.Append("foo");
            Assert.Equal(1, strCol.NullCount);
            strCol[1] = "bar";
            Assert.Equal(1, strCol.NullCount);
            strCol[1] = null;
            Assert.Equal(2, strCol.NullCount);
            strCol[1] = "foo";
            Assert.Equal(1, strCol.NullCount);
            strCol[0] = null;
            Assert.Equal(1, strCol.NullCount);

            PrimitiveColumn<int> intColumn = new PrimitiveColumn<int>("Int");
            intColumn.Append(0);
            intColumn.Append(1);
            intColumn.Append(null);
            intColumn.Append(2);
            intColumn.Append(null);
            intColumn.Append(3);
            Assert.Equal(0, intColumn[0]);
            Assert.Equal(1, intColumn[1]);
            Assert.Equal(null, intColumn[2]);
            Assert.Equal(2, intColumn[3]);
            Assert.Equal(null, intColumn[4]);
            Assert.Equal(3, intColumn[5]);

        }

        [Fact]
        public void TestValidity()
        {
            PrimitiveColumn<int> dataFrameColumn1 = new PrimitiveColumn<int>("Int1", Enumerable.Range(0, 10).Select(x => x));
            dataFrameColumn1.Append(null);
            Assert.False(dataFrameColumn1.IsValid(10));
            for (long i = 0; i < dataFrameColumn1.Length - 1; i++)
            {
                Assert.True(dataFrameColumn1.IsValid(i));
            }
        }

        [Fact]
        public void TestAppendMany()
        {
            PrimitiveColumn<int> intColumn = new PrimitiveColumn<int>("Int1");
            intColumn.AppendMany(null, 5);
            Assert.Equal(5, intColumn.NullCount);
            Assert.Equal(5, intColumn.Length);
            for (int i = 0; i < intColumn.Length; i++)
            {
                Assert.False(intColumn.IsValid(i));
            }

            intColumn.AppendMany(5, 5);
            Assert.Equal(5, intColumn.NullCount);
            Assert.Equal(10, intColumn.Length);
            for (int i = 5; i < intColumn.Length; i++)
            {
                Assert.True(intColumn.IsValid(i));
            }

            intColumn[2] = 10;
            Assert.Equal(4, intColumn.NullCount);
            Assert.True(intColumn.IsValid(2));

            intColumn[7] = null;
            Assert.Equal(5, intColumn.NullCount);
            Assert.False(intColumn.IsValid(7));
        }

        [Fact]
        public void TestBasicArrowStringColumn()
        {
            StringArray strArray = new StringArray.Builder().Append("foo").Append("bar").Build();
            Memory<byte> dataMemory = new byte[] { 102, 111, 111, 98, 97, 114 };
            Memory<byte> nullMemory = new byte[] { 0, 0, 0, 0 };
            Memory<byte> offsetMemory = new byte[] { 0, 0, 0, 0, 3, 0, 0, 0, 6, 0, 0, 0 };

            ArrowStringColumn stringColumn = new ArrowStringColumn("String", dataMemory, offsetMemory, nullMemory, strArray.Length, strArray.NullCount);
            Assert.Equal(2, stringColumn.Length);
            Assert.Equal("foo", stringColumn[0]);
            Assert.Equal("bar", stringColumn[1]);
        }

        [Fact]
        public void TestArrowStringColumnWithNulls()
        {
            string data = "joemark";
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            Memory<byte> dataMemory = new Memory<byte>(bytes);
            Memory<byte> nullMemory = new byte[] { 0b1101 };
            Memory<byte> offsetMemory = new byte[] { 0, 0, 0, 0, 3, 0, 0, 0, 3, 0, 0, 0, 7, 0, 0, 0, 7, 0, 0, 0 };
            ArrowStringColumn stringColumn = new ArrowStringColumn("String", dataMemory, offsetMemory, nullMemory, 4, 1);

            Assert.Equal(4, stringColumn.Length);
            Assert.Equal("joe", stringColumn[0]);
            Assert.Null(stringColumn[1]);
            Assert.Equal("mark", stringColumn[2]);
            Assert.Equal("", stringColumn[3]);

            List<string> ret = stringColumn[0, 4];
            Assert.Equal("joe", ret[0]);
            Assert.Null(ret[1]);
            Assert.Equal("mark", ret[2]);
            Assert.Equal("", ret[3]);
        }

    }
}
