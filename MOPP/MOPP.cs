using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOPP
{
    class MOPPCode : ICloneable
    {
        public int ID
        {
            get
            {
                return (int)SourceCode;
            }
        }
        public int ArgCount
        {
            get
            {
                return ArguementOffsets.Count;
            }
        }
        public int SourceCode;
        public long Position;

        public readonly int TargetCode;
        public readonly List<int> ArguementOffsets;
        public List<int> Arguements;
        bool Transposed;

        public MOPPCode(int src_code, int target_code, List<int> arg_offsets)
        {
            SourceCode = src_code;
            TargetCode = target_code;
            Arguements = new List<int>();
            ArguementOffsets = arg_offsets;
            Position = -1;
            Transposed = false;
        }

        public MOPPCode Transpose()
        {
            // Create a complete instance
            MOPPCode code = this.Clone() as MOPPCode;

            for (int i = 0; i < code.Arguements.Count; i++)
            {
                code.Arguements[i] = code.Arguements[i] + code.ArguementOffsets[i];
            }
            code.SourceCode = code.TargetCode;
            code.Transposed = true;

            // Error Checking
            for (int i = 0; i < code.Arguements.Count; i++)
            {
                var arguement = code.Arguements[i];
                if (arguement < 0 || arguement > 255)
                {
                    Console.WriteLine($"0x{code.ID}@0x{code.Position}:Arg[{i}] is invalid ({arguement})");
                }
            }

            return code;
        }

        public object Clone()
        {
            MOPPCode code = new MOPPCode(this.SourceCode, this.TargetCode, this.ArguementOffsets);
            if (this.Arguements.Count != 0)
            {
                code.Arguements.Capacity = Arguements.Count;
                foreach (var arguement in this.Arguements)
                {
                    code.Arguements.Add(arguement);
                }
            }
            return code;
        }

        public override string ToString()
        {
            var str = $"{ID.ToString("X")}";
            for (var i = 0; i < Arguements.Count; i++)
            {
                str += $" {Arguements[i].ToString("X")}";
            }
            return str;
        }
    }

    class MOPP
    {
        List<MOPPCode> mopp_codes = new List<MOPPCode>();

        private void SetMOPPCode(int src_code, int target_code, params int[] offsets)
        {
            mopp_codes[(int)src_code] = new MOPPCode(src_code, target_code == 0 ? src_code : target_code, new List<int>(offsets));
        }

        private MOPPCode GetMOPPCode(MemoryStream stream)
        {
            // Read Instruction
            int instruction = stream.ReadByte();

            // Read MOPP Code
            var mopp_code = mopp_codes[instruction];

            if (mopp_code == null)
            {
                stream.Position--;
                return null;

            }

            var code_instance = mopp_code.Clone() as MOPPCode;
            code_instance.Position = stream.Position - 1;

            // Read Arguements
            for (var i = 0; i < code_instance.ArgCount; i++)
            {
                code_instance.Arguements.Add(stream.ReadByte());
            }

            return code_instance;

            return null;
        }

        private void UnkMOPPCode(params object[] emptyness_forever_alone_forgotten_alas_no_more)
        {

        }

        public List<MOPPCode> transposed_codes = new List<MOPPCode>();
        public List<MOPPCode> source_codes = new List<MOPPCode>();

        public MOPP(string file)
        {
            const int UNKNOWN = 0;
            for (var i = 0; i < 256; i++) mopp_codes.Add(null); // Empty array of 256 nulls


            bool halo_online = true;
            if (halo_online)
            {
                SetMOPPCode(0x00, 0x00);
                /*
                Advance pointer by 4 from start
                */
                SetMOPPCode(0x01, 0x01, 0x00, 0x00, 0x00);              // unknown test
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
                SetMOPPCode(0x53, UNKNOWN, 0x00, 0x00, 0x00, 0x00); // select triangle a+offset 32bit

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
                        var pos = stream.Position.ToString("X");
                        var val = stream.ReadByte().ToString("X");
                        var str = $"0x{val}@0x{pos} is unknown/invalid";
                        Console.WriteLine(str);
                        throw new Exception(str);
                    }
                    else
                    {
                        source_codes.Add(code);
                        transposed_codes.Add(code.Transpose());
                        //Console.WriteLine(code.ToString());
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
           MOPP chill_ho = new MOPP("H3 Campaign/H3_020_bsp_110.raw");
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
            for (var i = 0; i < chill_ho.source_codes.Count; i++)
            {
                var odst_code = i < chill_ho.source_codes.Count ? chill_ho.source_codes[i] : null;

                if (odst_code != null)
                {
                    var id = csv_format($"0x{odst_code.ID.ToString("X")}");
                    var pos = csv_format($"0x{odst_code.Position.ToString("X")}");

                    var str = $"{id},{pos}";
                    foreach (var arguement in odst_code.Arguements)
                    {
                        var inner_str = csv_format($"0x{arguement.ToString("X")}");
                        str += $",{inner_str}";
                    }
                    odst_csv.AppendLine(str);
                }

            }
            File.WriteAllText("output.csv", odst_csv.ToString());
            
            /*
            MOPP chill_ho = new MOPP("HO/HO_guardian.raw");
            MOPP chill_h3 = new MOPP("H3 MP/H3_guardian.raw");

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
                if (ho_code == null || h3_code == null || ho_code.ID != h3_code.ID) no_longer_in_sync = true;

                if (!no_longer_in_sync && ho_code.Arguements.Count > 0)
                {
                    var h3_id = csv_format($"0x{h3_code.ID.ToString("X")}");
                    var ho_id = csv_format($"0x{ho_code.ID.ToString("X")}");
                    var pos = csv_format($"0x{ho_code.Position.ToString("X")}");


                    var str = $"{h3_id},{ho_id},{pos}";
                    var str_cmp = $"{h3_id},{ho_id}";
                    for (var j = 0; j < ho_code.Arguements.Count; j++)
                    {
                        var ho_arguement = ho_code.Arguements[j];
                        var h3_arguement = h3_code.Arguements[j];

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
                if(ho_code != null)
                {
                    var id = csv_format($"0x{ho_code.ID.ToString("X")}");
                    var pos = csv_format($"0x{ho_code.Position.ToString("X")}");

                    var str = $"{id},{pos}";
                    foreach (var arguement in ho_code.Arguements)
                    {
                        var inner_str = csv_format($"0x{arguement.ToString("X")}");
                        str += $",{inner_str}";
                    }
                    ho_csv.AppendLine(str);
                }
                if (h3_code != null)
                {
                    var id = csv_format($"0x{h3_code.ID.ToString("X")}");
                    var pos = csv_format($"0x{h3_code.Position.ToString("X")}");

                    var str = $"{id},{pos}";
                    foreach (var arguement in h3_code.Arguements)
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
            */

            return 0;
        }


    }

}
