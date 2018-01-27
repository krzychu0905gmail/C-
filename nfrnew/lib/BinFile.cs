using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace nfrnew.lib
{
    public class BinFile
    {
        public int length;
        public int totalPackets;
        public string filename;
        public bool okTC = false;
        public bool okCU = false;
        public string deviceVersion = "";
        private byte[] bin;

        OpenFileDialog openFileDialog = new OpenFileDialog();

        public BinFile()
        {
            length = 0;
            totalPackets = 0;

            Stream myStream = null;
            string filePath = string.Empty;
            bin = new byte[32768 * 4];

            openFileDialog.Filter = "bin files (*.bin)|*.bin|(*.hex)|*.hex";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            int i = 0;

                            StreamReader readerHex = new StreamReader(myStream);

                            filePath = openFileDialog.FileName;

                            if (openFileDialog.FileName.IndexOf(".hex") != -1)
                            {
                                bin = ConvertBinToHexProgram(filePath, myStream);
                                if (bin == null) return;
                                length = bin.Length;
                            }
                            else
                            {
                                BinaryReader reader = new BinaryReader(myStream);
                                length = (int)reader.BaseStream.Length;

                                if (length < 32768 * 4)
                                {
                                    while (i < length)
                                    {
                                        bin[i++] = (byte)reader.Read();
                                    }
                                    reader.Close();
                                }
                                else
                                    bin = null;
                            }



                            totalPackets = ((length - 1) / 512) + 1;

                            if (length > 32768 * 4)
                            {
                                length = 32768 * 4;
                                totalPackets = ((length - 1) / 512) + 1;
                            }
                            if (length < 32768 * 4)
                            {
                                okTC = false;
                                okCU = false;
                                deviceVersion = "";
                                for (int j = length - 10; j >= 0; j--)
                                {
                                    if (bin[j] == 'T' && bin[j + 1] == 'C' && bin[j + 2] >= (int)'2' &&
                                        bin[j + 2] <= (int)'3' && bin[j + 3] == (int)'v')
                                    {
                                        okTC = true;
                                        for (int k = 0; k < 8; k++)
                                            if (bin[j + k] != 0)
                                                deviceVersion += (char)bin[j + k];
                                            else
                                                break;
                                        break;
                                    }
                                    if (bin[j] == 'C' && bin[j + 1] == 'U' && bin[j + 2] >= (int)'2' &&
                                        bin[j + 2] <= (int)'9' && (bin[j + 3] == (int)'v' || bin[j + 3] == (int)'k'))
                                    {
                                        okCU = true;
                                        for (int k = 0; k < 8; k++)
                                            if (bin[j + k] != 0)
                                                deviceVersion += (char)bin[j + k];
                                            else
                                                break;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Zbyt duży plik bin, " + length + " bajtów!");
                            }
                        }
                        filename = filePath;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        public void UpdateFile()
        {
            length = 0;
            totalPackets = 0;

            Stream myStream = null;
            string filePath = string.Empty;
            bin = new byte[32768 * 4];

            openFileDialog.Filter = "bin files (*.bin)|*.bin|(*.hex)|*.hex";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            int i = 0;

                            StreamReader reader = new StreamReader(myStream);

                            filePath = openFileDialog.FileName;

                            if (openFileDialog.FileName.IndexOf(".hex") != -1)
                            {
                                string[] readedBytesHex = reader.ReadToEnd().Split('\n');

                                reader.Close();

                                filePath = filePath.Replace(".hex", "conv.bin");

                                FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);

                                for (int k = 0; k < readedBytesHex.Length - 2; k++)
                                {
                                    List<string> outByte = new List<string>();
                                    List<byte> outByteB = new List<byte>();

                                    string s = readedBytesHex[k];

                                    for (int f = 9; f < s.Length - 3; f += 2)
                                    {
                                        string hexStr = s[f].ToString() + s[f + 1].ToString();
                                        outByte.Add(hexStr);
                                        byte b = (byte)int.Parse(hexStr, System.Globalization.NumberStyles.HexNumber);
                                        bin[i++] = b;
                                        outByteB.Add(b);
                                    }

                                    try
                                    {
                                        fs.Write(outByteB.ToArray(), 0, outByteB.Count);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Problem with create file" + ex.ToString());
                                    }
                                }

                                fs.Close();
                            }
                            else
                            {
                                if (length < 32768 * 4)
                                {
                                    while (i < length)
                                    {
                                        bin[i++] = (byte)reader.Read();
                                    }
                                    reader.Close();
                                }
                            }



                            totalPackets = ((length - 1) / 512) + 1;

                            if (length > 32768 * 4)
                            {
                                length = 32768 * 4;
                                totalPackets = ((length - 1) / 512) + 1;
                            }
                            if (length < 32768 * 4)
                            {
                                okTC = false;
                                okCU = false;
                                deviceVersion = "";
                                for (int j = length - 10; j >= 0; j--)
                                {
                                    if (bin[j] == 'T' && bin[j + 1] == 'C' && bin[j + 2] >= (int)'2' &&
                                        bin[j + 2] <= (int)'3' && bin[j + 3] == (int)'v')
                                    {
                                        okTC = true;
                                        for (int k = 0; k < 8; k++)
                                            if (bin[j + k] != 0)
                                                deviceVersion += (char)bin[j + k];
                                            else
                                                break;
                                        break;
                                    }
                                    if (bin[j] == 'C' && bin[j + 1] == 'U' && bin[j + 2] >= (int)'2' &&
                                        bin[j + 2] <= (int)'9' && (bin[j + 3] == (int)'v' || bin[j + 3] == (int)'k'))
                                    {
                                        okCU = true;
                                        for (int k = 0; k < 8; k++)
                                            if (bin[j + k] != 0)
                                                deviceVersion += (char)bin[j + k];
                                            else
                                                break;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Zbyt duży plik bin, " + length + " bajtów!");
                            }
                        }
                        filename = filePath;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }


        private byte[] ConvertBinToHexProgram(string filePath, Stream myStream)
        {
            StreamReader reader = new StreamReader(myStream);
            List<byte> outBin = new List<byte>();

            string[] readedBytesHex = reader.ReadToEnd().Split('\n');
            reader.Close();

            filePath = filePath.Replace(".hex", "conv.bin");
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);

            string hexLen = readedBytesHex[0][1].ToString() + (string)readedBytesHex[0][2].ToString();

            int RowLength =  (int)int.Parse(hexLen, System.Globalization.NumberStyles.HexNumber);

            for (int k = 0; k < readedBytesHex.Length - 1; k++)
            {
                List<string> outByte = new List<string>();
                List<byte> outByteB = new List<byte>();

                //if (k > readedBytesHex.Length - 8)
                //    i = 0;

                string s = readedBytesHex[k];

                int length = s.Length;


                if (k == 1)
                {
                    if (s.IndexOf("9E4FC0B4") != -1) MessageBox.Show("Zaladowano plik hex odczytany z pamieci urzadzenia");
                    else if (s.IndexOf("0000A0E1") == -1) { MessageBox.Show("Canot convert value canot find 0000A0E1 value"); fs.Close(); return null; }
                    s = s.Replace("0000A0E1", "9E4FC0B4"); //Change the value as Flash Magic 
                }

                for (int f = 9; f < (RowLength * 2 + 8); f += 2)
                {
                     string hexStr = string.Empty;

                    if (f < s.Length - 3)
                        hexStr = s[f].ToString() + s[f + 1].ToString();
                    else 
                        hexStr = "FF";

                    outByte.Add(hexStr);
                    byte b = (byte)int.Parse(hexStr, System.Globalization.NumberStyles.HexNumber);
                    outBin.Add(b);
                    outByteB.Add(b);
                }

                try { fs.Write(outByteB.ToArray(), 0, outByteB.Count); }
                catch (Exception ex) { Console.WriteLine("Problem with create file" + ex.ToString()); }
            }

            for (int f = outBin.Count; f < 8192; f++)
            {
                byte b = (byte)int.Parse("FF", System.Globalization.NumberStyles.HexNumber);
                outBin.Add(b);
                fs.Write(new byte[] {b}, 0, 1);

            }

            fs.Close();

            return outBin.ToArray();
        }

        public byte GetByte(int idx)
        {
            if (idx < this.length)
            {
                return bin[idx];
            }
            else
            {
                return 0xFF;
            }
        }
    }
}
