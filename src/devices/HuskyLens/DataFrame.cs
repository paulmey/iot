﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq;

namespace Iot.Device.HuskyLens
{
    internal class DataFrame
    {
        private readonly byte[] _data;

        public DataFrame(ReadOnlySpan<byte> data)
        {
            _data = data.ToArray();
        }

        public Command Command => (Command)_data[4];

        public ReadOnlySpan<byte> Data => new ReadOnlySpan<byte>(_data, 5, _data[3]);

        public bool Valid()
        {
            return
            _data[0] == 0x55 &&
            _data[1] == 0xAA &&
            _data[2] == 0x11 &&
            _data[3] == _data.Length - 6 &&
            _data[_data.Length - 1] == CalculateChecksum();
        }

        /// <inheritdoc/>
        public override string ToString() => $"Frame: c={Command}, d={string.Join(" ", Data.ToArray().Select(d => $"{d:X}"))}";

        private byte CalculateChecksum() => (byte)(_data.Take(_data.Length - 1).Aggregate(0x00, (a, b) => a + b) & 0xFF);
    }

    internal enum Command : byte
    {
        COMMAND_RETURN_INFO = 0x29,
        COMMAND_RETURN_BLOCK = 0x2A,
        COMMAND_RETURN_ARROW = 0x2B,
    }
}