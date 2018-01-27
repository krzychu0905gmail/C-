using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nfrnew.lib
{
    class Crc16
    {
        static readonly ushort Crc16Seed = 0xA001;
        static readonly ushort[] Crc16Table = new ushort[256];

        public static void InitCrc16Tab()
        {
          ushort crc, c;
          for (int i = 0; i < 256; i++)
          {
            crc = 0;
            c  = (ushort)i;
            for (int j = 0; j < 8; j++)
            {
              if (((crc ^ c) & 0x0001) != 0)
                crc = (ushort)((crc >> 1) ^ Crc16Seed);
              else
                crc =  (ushort)(crc >> 1);
              c = (ushort)(c >> 1);
            }
            Crc16Table[i] = crc;
          }
        }

        public static ushort MakeCrc16(byte[] data)
        {
          uint crc = 0;
          foreach (byte b in data)
            crc = (crc >> 8) ^ Crc16Table[(crc ^ b) & 0xff];
          return (ushort)crc;
        }

        public static ushort MakeCrc161Byte(ushort crc, byte data)
        {
          return (ushort)((crc >> 8) ^ Crc16Table[(crc ^ data) & 0xff]);
        }
    }
}
