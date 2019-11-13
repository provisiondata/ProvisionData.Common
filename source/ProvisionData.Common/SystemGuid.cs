/*******************************************************************************
 * MIT License
 *
 * Copyright 2020 Provision Data Systems Inc.  https://provisiondata.com
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 *
 *******************************************************************************/

namespace ProvisionData
{
    using System;
#if NETSTANDARD2_1
    using System.Buffers.Text;
#endif
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    [DebuggerNonUserCode]
    public static class SystemGuid
    {
        private static Func<Guid> GetGuid = GetGuidInternal;

        [DebuggerNonUserCode]
        public static void GuidIs(Guid guid) => GetGuid = () => guid;

        [DebuggerNonUserCode]
        public static void GuidIs(String guid)
        {
            var g = Guid.Parse(guid);
            GetGuid = () => g;
        }

        [DebuggerNonUserCode]
        public static Guid NewGuid() => GetGuid?.Invoke() ?? GetGuidInternal();

        [DebuggerNonUserCode]
        public static void Reset() => GetGuid = GetGuidInternal;

        private static Guid GetGuidInternal() => CombGuid.NewGuid();

#if NETSTANDARD2_1
        private const Byte ForwardSlashByte = (Byte)'/';
        private const Byte PlusByte = (Byte)'+';
        private const Char Underscore = '_';
        private const Char Dash = '-';

        public static String ToBase64String(this Guid guid)
        {
            Span<Byte> guidBytes = stackalloc Byte[16];
            Span<Byte> encodedBytes = stackalloc Byte[24];

            MemoryMarshal.TryWrite(guidBytes, ref guid); // write bytes from the Guid 
            Base64.EncodeToUtf8(guidBytes, encodedBytes, out _, out _);

            Span<Char> chars = stackalloc Char[22];

            // replace any characters which are not URL safe 
            // skip the final two bytes as these will be '==' padding we don't need 
            for (var i = 0; i < 22; i++)
            {
                switch (encodedBytes[i])
                {
                    case ForwardSlashByte:
                        chars[i] = Dash;
                        break;
                    case PlusByte:
                        chars[i] = Underscore;
                        break;
                    default:
                        chars[i] = (Char)encodedBytes[i];
                        break;
                }
            }

            return new String(chars);
        }
#endif
    }
}
