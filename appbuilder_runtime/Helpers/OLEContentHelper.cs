/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Apttus.XAuthor.AppRuntime
{
    /// <summary>
    /// This Class finds 3 occurrences of fileExtension. The "Native" format used to read OLEObjects (ExcelHelper.OleObjectToByte) has a content header, which
    /// contains the filename 3 times: one pure filename and then 2 filenames with full path.
    /// After last filename there are 5 extra bytes and then the actual file content starts. 
    /// The actual file content ends 2 bytes before the "Native" format data ends 
    /// To remove above content and make Deserialization work correctly those extra line(data) needs to be removed.
    /// </summary>
    public static class OLEContentHelper
    {
        public static int ReadHeader(MemoryStream ms)
        {
            var header = new byte[2];
            int read = ms.Read(header, 0, header.Length);
            if (read != header.Length)
                throw new FormatException("End of stream while reading header");
            if (header[0] != 2 || header[1] != 0)
                throw new FormatException("Bad header");
            return read;
        }

        public static string ReadString(MemoryStream ms)
        {
            var sb = new StringBuilder();
            while (true)
            {
                int b = ms.ReadByte();
                if (b == -1)
                    throw new FormatException("End of stream while reading string");
                if (b == 0)
                    return sb.ToString();
                sb.Append((char)b);
            }
        }

        public static int ReadInt(MemoryStream ms)
        {
            var bytes = new byte[4];
            int read = ms.Read(bytes, 0, bytes.Length);
            if (read != bytes.Length)
                throw new FormatException("End of stream while reading int");
            return BitConverter.ToInt32(bytes, 0);
        }

        public static byte[] ReadBytes(MemoryStream ms, int count)
        {
            var bytes = new byte[count];
            int read = ms.Read(bytes, 0, count);
            if (read != count)
                throw new FormatException("End of stream while reading bytes");
            return bytes;
        }

        public static string GetOLEContent(MemoryStream ms)
        {
            ReadHeader(ms);
            string name = ReadString(ms);
            string path = ReadString(ms);
            int reserved = ReadInt(ms);
            if (reserved != 0x30000)
                throw new FormatException(string.Format("Unexpected reserved bytes : got {0} but expected {1}", reserved.ToString("x"), 0x30000.ToString("x")));
            int tempLength = ReadInt(ms);
            string tempPath = ReadString(ms);
            if (tempPath.Length + 1 != tempLength)
                throw new FormatException(string.Format("Mismatch between temp length {0} and temp full path length {1}", tempLength, tempPath.Length + 1));
            int contentLength = ReadInt(ms);
            byte[] content = ReadBytes(ms, contentLength);
            int delta = sizeof(int) * 3 + (name.Length + path.Length + tempPath.Length) * 2;
            if (ms.Length != ms.Position + delta)
                throw new FormatException("Unexpected end of file");
            return UTF8Encoding.UTF8.GetString(content);
        }

        public static byte[] GetOLEContentInBytes(MemoryStream ms)
        {
            ReadHeader(ms);
            string name = ReadString(ms);
            string path = ReadString(ms);
            int reserved = ReadInt(ms);
            if (reserved != 0x30000)
                throw new FormatException(string.Format("Unexpected reserved bytes : got {0} but expected {1}", reserved.ToString("x"), 0x30000.ToString("x")));
            int tempLength = ReadInt(ms);
            string tempPath = ReadString(ms);
            if (tempPath.Length + 1 != tempLength)
                throw new FormatException(string.Format("Mismatch between temp length {0} and temp full path length {1}", tempLength, tempPath.Length + 1));
            int contentLength = ReadInt(ms);
            byte[] content = ReadBytes(ms, contentLength);
            int delta = sizeof(int) * 3 + (name.Length + path.Length + tempPath.Length) * 2;
            if (ms.Length != ms.Position + delta)
                throw new FormatException("Unexpected end of file");
            return content;
        } 
    }
}
