using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nfrnew.lib
{
    static class Common
    {
        const ushort P_16 = 0xA001;

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        static private ushort getValCRCtab(int i)
        {

            int j;
            ushort crc, c;

            crc = 0;
            c = (ushort)i;

            for (j = 0; j < 8; j++)
            {

                if (((crc ^ c) & 0x0001) > 0)
                    crc = Convert.ToUInt16((crc >> 1) ^ P_16);
                else
                    crc = Convert.ToUInt16(crc >> 1);

                c = Convert.ToUInt16(c >> 1);
            }

            return crc;
        }
        public static ushort MakeCrc(byte[] data, int size)
        {
            ushort crc = 0;  //normal crc 16 or 0xffff for modbus crc

            ushort data16, tmp;
            int i;

            for (i = 1; i < size; i++)
            {
                data16 = Convert.ToUInt16(0x00ff & (ushort)data[i]);
                tmp = Convert.ToUInt16(crc ^ data16);
                crc = Convert.ToUInt16((crc >> 8) ^ getValCRCtab(tmp & 0xff)); //Zamiast tworzenia tablicy, liczy jej konkretną wartość w locie
            }
            return crc;
        }

        public static ushort MakeCrc0(byte[] data, int size)
        {
            ushort crc = 0;  //normal crc 16 or 0xffff for modbus crc

            ushort data16, tmp;
            int i;

            for (i = 0; i < size; i++)
            {
                data16 = Convert.ToUInt16(0x00ff & (ushort)data[i]);
                tmp = Convert.ToUInt16(crc ^ data16);
                crc = Convert.ToUInt16((crc >> 8) ^ getValCRCtab(tmp & 0xff)); //Zamiast tworzenia tablicy, liczy jej konkretną wartość w locie
            }
            return crc;
        }

        public static string uint16ToHexStr(ushort value)
        {
            return "0x" + value.ToString("X");
        }

        static public uint[] lowPassFilter(uint[] tab, uint _numSampl)
        {
            uint[] tabs = new uint[tab.Length];

            tab.CopyTo(tabs, 0);

            for (int i = 0; i < tab.Length - _numSampl; i++)
            {
                for (int j = i + 1; j < i + _numSampl; j++)
                {
                    tabs[i] += tab[j];
                }
                tabs[i] /= _numSampl;
            }

            return tabs;
        }


        public static uint searchMax(uint[] tab, bool valIdx, uint beginIdx, uint endIdx)
        {
            uint i, max = tab[beginIdx], idx = 0;

            for (i = (ushort)(beginIdx + 1); i < endIdx; i++)
            {
                if (max < tab[i])
                {
                    idx = i;
                    max = tab[i];
                }
            }

            return (valIdx ? max : idx);
        }
        public static uint searchMin(uint[] tab, bool valIdx, uint beginIdx, uint endIdx)
        {
            uint i, min = tab[beginIdx], idx = 0;

            for (i = (ushort)(beginIdx + 1); i < endIdx; i++)
            {
                if (min > tab[i])
                {
                    idx = i;
                    min = tab[i];
                }
            }

            return (valIdx ? min : idx);
        }


    }
}
