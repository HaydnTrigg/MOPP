using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOPP.Codes;

namespace MOPP
{
    class MOPP
    {
        static List<MOPPCode>[] mopp_codes = null;
        MOPPCode.CodeType Type = (MOPPCode.CodeType)(-1);

        private void SetMOPPCode(int src_code, int unused_target_code, params UInt32[] offsets)
        {
            mopp_codes[(int)Type][src_code] = new MOPPCode(Type, src_code, -1, offsets);
        }

        private MOPPCode GetMOPPCode(MemoryStream stream)
        {
            // Read Instruction
            var position = stream.Position;
            int instruction = stream.ReadByte();

            // Read MOPP Code
            MOPPCode mopp_code = mopp_codes[(int)Type][instruction];

            if (mopp_code == null)
            {
                return null;
            }



            List<UInt32> arguemennts = new List<UInt32>();

            // Read Arguements
            for (var i = 0; i < mopp_code.ArgCount; i++)
            {
                arguemennts.Add((UInt32)stream.ReadByte());
            }

            var code_instance = mopp_code.Clone(position, arguemennts) as MOPPCode;

            return code_instance;
        }

        private void UnkMOPPCode(params object[] emptyness_forever_alone_forgotten_alas_no_more)
        {

        }

        public List<MOPPCode> source_codes = new List<MOPPCode>();

        public MOPP(MOPPCode.CodeType type, MOPP old_codes)
        {
            Type = type;

            foreach (var code in old_codes.source_codes)
            {
                var new_code = code.Transform(Type);
                source_codes.Add(new_code);
            }



        }



        public void Write(string file)
        {
            FileStream fs = File.Open(file, FileMode.Create);
            using (BinaryWriter stream = new BinaryWriter(fs))
            {
                foreach (var code in source_codes)
                {
                    stream.Write((Byte)code.Code);
                    foreach (var arguement in code.Arguments)
                    {
                        stream.Write((Byte)arguement);
                    }
                }
            }
        }

        public MOPP(MOPPCode.CodeType type, string file)
        {
            Type = type;
            if (mopp_codes == null)
            {
                mopp_codes = new List<MOPPCode>[(int)MOPPCode.CodeType.NumTypes];
                for (var i = 0; i < (int)MOPPCode.CodeType.NumTypes; i++)
                {
                    mopp_codes[i] = new List<MOPPCode>();
                }
            }
            {

                const int UNKNOWN = 0;
                for (var i = 0; i < 256; i++) mopp_codes[(int)Type].Add(null); // Empty array of 256 nulls


                SetMOPPCode(0x00, 0x00);
                /*
                Advance pointer by 4 from start
                */
                //SetMOPPCode(0x01, 0x01, 0x00, 0x00, 0x00);              // unknown test
                {
                    UInt32[] args = { 0x00, 0x00, 0x00 };
                    mopp_codes[(int)Type][(int)0x1] = new MOPPCode_0x01(MOPPCode.CodeType.Halo3, 0x1, -1, args);
                }
                SetMOPPCode(0x02, 0x02, 0x00, 0x00, 0x00);              // unknown test
                SetMOPPCode(0x03, 0x03, 0x00, 0x00, 0x00);              // unknown test
                SetMOPPCode(0x04, 0x04, 0x00, 0x00, 0x00);              // unknown test

                SetMOPPCode(0x05, 0x05, 0x00);                          // jump 8bit  a
                SetMOPPCode(0x06, 0x06, 0x00, 0x00);                    // jump 16bit (a<<8) + b
                SetMOPPCode(0x07, 0x07, 0x00, 0x00, 0x00);              // Jump 24bit (a<<16) + (b<<8) + c

                UnkMOPPCode(0x08, 0x08);

                SetMOPPCode(0x09, 0x07, 0x00);                          // 64bytes to ptr. 8bit a
                SetMOPPCode(0x0A, 0x07, 0x00, 0x00);                    // 64bytes to ptr. 16bit a + b<<8
                SetMOPPCode(0x0B, 0x07, 0x00, 0x00, 0x00, 0x00);              // 64bytes to ptr. 24bit a + b<<8 + c<<16

                SetMOPPCode(0x0C, UNKNOWN, 0x00, 0x00);                 // 64bytes to ptr. 16bit a + b<<8

                SetMOPPCode(0x0D, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00);  // 64bytes to ptr. 24bit a + b<<8 + c<<16

                UnkMOPPCode(0x0E, 0x0E);
                UnkMOPPCode(0x0F, 0x0F);

                //if x>a then execute block c bytes further; if x<=a and x>=b then execute next block and block c bytes further; if x<b then only execute next block
                SetMOPPCode(0x10, 0x10, 0x00, 0x00, 0x00);
                SetMOPPCode(0x11, 0x11, 0x00, 0x00, 0x00); //like 0x10 but for y
                SetMOPPCode(0x12, 0x12, 0x00, 0x00, 0x00); //like 0x10 but for z
                SetMOPPCode(0x13, 0x13, 0x00, 0x00, 0x00); //like 0x10 but for y + z
                SetMOPPCode(0x14, 0x14, 0x00, 0x00, 0x00); //like 0x10 but for y - z
                SetMOPPCode(0x15, 0x15, 0x00, 0x00, 0x00); //like 0x10 but for x + z
                SetMOPPCode(0x16, 0x16, 0x00, 0x00, 0x00); //like 0x10 but for x - z
                SetMOPPCode(0x17, 0x17, 0x00, 0x00, 0x00); //like 0x10 but for x + y
                SetMOPPCode(0x18, 0x18, 0x00, 0x00, 0x00); //like 0x10 but for x - y
                SetMOPPCode(0x19, 0x19, 0x00, 0x00, 0x00); //like 0x10 but for x + y + z
                SetMOPPCode(0x1A, 0x1A, 0x00, 0x00, 0x00); //like 0x10 but for x + y - z
                SetMOPPCode(0x1B, 0x1B, 0x00, 0x00, 0x00); //like 0x10 but for x - y + z
                SetMOPPCode(0x1C, 0x1C, 0x00, 0x00, 0x00); //like 0x10 but for x - y - z

                // Something happens here. Offsets become important. Why? Well. I don't fucking know.

                //execute next block and/or block at b bytes further depending on result of an unknown test involving a
                SetMOPPCode(0x20, UNKNOWN, UNKNOWN, UNKNOWN);
                SetMOPPCode(0x21, UNKNOWN, UNKNOWN, UNKNOWN); //like 0x20
                SetMOPPCode(0x22, UNKNOWN, UNKNOWN, UNKNOWN); //like 0x20

                SetMOPPCode(0x23, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN);
                SetMOPPCode(0x24, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN); //like 0x20
                SetMOPPCode(0x25, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN); //like 0x20

                // Normally used for AABB collision
                SetMOPPCode(0x26, 0x26, 0x00, 0x00);                // exit if x<a or x>b
                SetMOPPCode(0x27, 0x27, 0x00, 0x00);                // exit if y<a or y>b
                SetMOPPCode(0x28, 0x28, 0x00, 0x00);                // exit if z<a or z>b

                /*
                * According to http://niftools.sourceforge.net/wiki/Nif_Format/Mopp
                These used to be 0x23-25
                * 
                * */


                //execute block c*256 + d bytes further and/or block e*256 + f bytes further depending on result of an unknown test involving a and b
                SetMOPPCode(0x29, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN);
                SetMOPPCode(0x2A, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN); //like 0x23
                SetMOPPCode(0x2B, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN, UNKNOWN); //like 0x23

                //Select triangle. offset + i<0x00 - 0x1F>
                for (var i = 0; i <= 0x1F; i++)
                {
                    SetMOPPCode(0x30 + i, UNKNOWN + i);
                }

                SetMOPPCode(0x50, UNKNOWN, 0x00);                   // select triangle a+offset 8bit
                SetMOPPCode(0x51, UNKNOWN, 0x00, 0x00);             // select triangle a+offset 16bit
                SetMOPPCode(0x52, UNKNOWN, 0x00, 0x00, 0x00);       // select triangle a+offset 24bit
                                                                    //SetMOPPCode(0x53, UNKNOWN, 0x00, 0x00, 0x00, 0x00); // select triangle a+offset 32bit

                {
                    UInt32[] args = { 0x00, 0x00, 0x00, 0x00 };
                    mopp_codes[(int)Type][(int)0x53] = new MOPPCode_0x53(MOPPCode.CodeType.Halo3, 0x53, -1, args);
                }





                SetMOPPCode(0x60, UNKNOWN, 0x00);                   // Something 8bit
                SetMOPPCode(0x61, UNKNOWN, 0x00);                   // Something 8bit
                SetMOPPCode(0x62, UNKNOWN, 0x00);                   // Something 8bit
                SetMOPPCode(0x63, UNKNOWN, 0x00);                   // Something 8bit
                SetMOPPCode(0x64, UNKNOWN, 0x00, 0x00);             // Something 16bit
                SetMOPPCode(0x65, UNKNOWN, 0x00, 0x00);             // Something 16bit
                SetMOPPCode(0x66, UNKNOWN, 0x00, 0x00);             // Something 16bit
                SetMOPPCode(0x67, UNKNOWN, 0x00, 0x00);             // Something 16bit
                SetMOPPCode(0x68, UNKNOWN, 0x00, 0x00, 0x00, 0x00); // Something 32bit
                SetMOPPCode(0x69, UNKNOWN, 0x00, 0x00, 0x00, 0x00); // Something 32bit
                SetMOPPCode(0x6A, UNKNOWN, 0x00, 0x00, 0x00, 0x00); // Something 32bit
                SetMOPPCode(0x6B, UNKNOWN, 0x00, 0x00, 0x00, 0x00); // Something 32bit

            }

            Console.Title = $"Converting {file}";

            // Open the text file using a stream reader.
            MemoryStream stream = new MemoryStream();
            using (FileStream fs = File.OpenRead(file))
            {
                fs.CopyTo(stream);
                stream.Position = 0;
                while (stream.Position < stream.Length)
                {
                    var code = GetMOPPCode(stream);
                    if (code == null)
                    {
                        stream.Position--;
                        var pos = stream.Position.ToString("X");
                        var val = stream.ReadByte().ToString("X");
                        var str = $"0x{val}@0x{pos} is unknown/invalid";
                        Console.WriteLine(str);
                        throw new Exception(str);
                    }
                    else
                    {
                        source_codes.Add(code);
                    }
                }
            }
            Console.WriteLine($"Finished {file}");
        }
    }

    class MainEntry
    {
        static string csv_format(string str)
        {
            return $"\"{str}\"";
        }
        static int Main()
        {
            bool write_testA = true;
            bool write_test_B = false;

            string test_a_output = "H3_chill.raw";
            string test_a_file = "H3MP/H3_chill.raw";
            string test_b_filea = "HO/HO_guardian.raw";
            string test_b_fileb = "H3 MP/H3_guardian.raw";



            if (write_testA)
            {
                MOPP original_mopp = new MOPP(MOPPCode.CodeType.Halo3, test_a_file);

                

                MOPP output_mopp = new MOPP(MOPPCode.CodeType.HaloOnline, original_mopp);
                output_mopp.Write(test_a_output);

                var odst_csv = new StringBuilder();
                odst_csv.AppendLine(
                    $"{csv_format("code")}," +
                    $"{csv_format("position")}," +
                    $"{csv_format("arg1")}," +
                    $"{csv_format("arg2")}," +
                    $"{csv_format("arg3")}," +
                    $"{csv_format("arg4")}," +
                    $"{csv_format("arg5")}," +
                    $"{csv_format("arg6")}");
                var offsets_dict = new Dictionary<string, bool>();
                for (var i = 0; i < original_mopp.source_codes.Count; i++)
                {
                    var odst_code = i < original_mopp.source_codes.Count ? original_mopp.source_codes[i] : null;

                    if (odst_code != null)
                    {
                        var id = csv_format($"0x{odst_code.Code.ToString("X")}");
                        var pos = csv_format($"0x{odst_code.Position.ToString("X")}");

                        var str = $"{id},{pos}";
                        foreach (var arguement in odst_code.Arguments)
                        {
                            var inner_str = csv_format($"0x{arguement.ToString("X")}");
                            str += $",{inner_str}";
                        }
                        odst_csv.AppendLine(str);
                    }

                }
                File.WriteAllText("output.csv", odst_csv.ToString());
            }
            if (write_test_B)
            {
                MOPP chill_ho = new MOPP(MOPPCode.CodeType.HaloOnline, test_b_filea);
                MOPP chill_h3 = new MOPP(MOPPCode.CodeType.Halo3, test_b_fileb);

                var offset_csv = new StringBuilder();
                offset_csv.AppendLine(
                    $"{csv_format("code_h3")}," +
                    $"{csv_format("code_h0")}," +
                    $"{csv_format("position")}," +
                    $"{csv_format("offset1")}," +
                    $"{csv_format("offset2")}," +
                    $"{csv_format("offset3")}," +
                    $"{csv_format("offset4")}," +
                    $"{csv_format("offset5")}," +
                    $"{csv_format("offset6")}");

                var ho_csv = new StringBuilder();
                ho_csv.AppendLine(
                    $"{csv_format("code")}," +
                    $"{csv_format("position")}," +
                    $"{csv_format("arg1")}," +
                    $"{csv_format("arg2")}," +
                    $"{csv_format("arg3")}," +
                    $"{csv_format("arg4")}," +
                    $"{csv_format("arg5")}," +
                    $"{csv_format("arg6")}");

                var h3_csv = new StringBuilder();
                h3_csv.AppendLine(
                    $"{csv_format("code")}," +
                    $"{csv_format("position")}," +
                    $"{csv_format("arg1")}," +
                    $"{csv_format("arg2")}," +
                    $"{csv_format("arg3")}," +
                    $"{csv_format("arg4")}," +
                    $"{csv_format("arg5")}," +
                    $"{csv_format("arg6")}");
                var offsets_dict = new Dictionary<string, bool>();
                bool no_longer_in_sync = false;
                for (var i = 0; i < chill_h3.source_codes.Count; i++)
                {
                    var ho_code = i < chill_ho.source_codes.Count ? chill_ho.source_codes[i] : null;
                    var h3_code = i < chill_h3.source_codes.Count ? chill_h3.source_codes[i] : null;

                    no_longer_in_sync = false;
                    if (ho_code == null || h3_code == null || ho_code.Code != h3_code.Code) no_longer_in_sync = true;

                    if (!no_longer_in_sync && ho_code.Arguments.Count > 0)
                    {
                        var h3_id = csv_format($"0x{h3_code.Code.ToString("X")}");
                        var ho_id = csv_format($"0x{ho_code.Code.ToString("X")}");
                        var pos = csv_format($"0x{ho_code.Position.ToString("X")}");


                        var str = $"{h3_id},{ho_id},{pos}";
                        var str_cmp = $"{h3_id},{ho_id}";
                        for (var j = 0; j < ho_code.Arguments.Count; j++)
                        {
                            var ho_arguement = ho_code.Arguments[j];
                            var h3_arguement = h3_code.Arguments[j];

                            var offset = ho_arguement - h3_arguement;

                            var offset_str = offset >= 0 ? $"0x{offset.ToString("X")}" : $"-0x{(-offset).ToString("X")}";

                            str += $",{csv_format(offset_str)}";
                            str_cmp += $",{csv_format(offset_str)}";
                        }
                        if (!offsets_dict.ContainsKey(str_cmp))
                        {
                            offsets_dict[str_cmp] = true;
                            offset_csv.AppendLine(str);
                        }
                    }
                    if (ho_code != null)
                    {
                        var id = csv_format($"0x{ho_code.Code.ToString("X")}");
                        var pos = csv_format($"0x{ho_code.Position.ToString("X")}");

                        var str = $"{id},{pos}";
                        foreach (var arguement in ho_code.Arguments)
                        {
                            var inner_str = csv_format($"0x{arguement.ToString("X")}");
                            str += $",{inner_str}";
                        }
                        ho_csv.AppendLine(str);
                    }
                    if (h3_code != null)
                    {
                        var id = csv_format($"0x{h3_code.Code.ToString("X")}");
                        var pos = csv_format($"0x{h3_code.Position.ToString("X")}");

                        var str = $"{id},{pos}";
                        foreach (var arguement in h3_code.Arguments)
                        {
                            var inner_str = csv_format($"0x{arguement.ToString("X")}");
                            str += $",{inner_str}";
                        }
                        h3_csv.AppendLine(str);
                    }

                }
                File.WriteAllText("offsets.csv", offset_csv.ToString());
                File.WriteAllText("halo3_mopp.csv", h3_csv.ToString());
                File.WriteAllText("halo_online.csv", ho_csv.ToString());
            }

            return 0;
        }


    }

}
