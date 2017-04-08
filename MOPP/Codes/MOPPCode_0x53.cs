using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOPP.Codes
{
    class MOPPCode_0x53 : MOPPCode
    {
        public override List<UInt32> Arguments { get {
                if (Type == CodeType.Halo3)
                {
                    var args = new UInt32[] {
                                (flags + 0x20) & 0xFF,
                                block_index & 0xFF,
                                triangle_index & 0xFF00,
                                triangle_index & 0xFF,
                            };
                    return new List<UInt32>(args);
                }
                if(Type == CodeType.HaloOnline)
                {
                    var args = new UInt32[] {
                                (block_index * 0x4 + 0x20) & 0xFF,
                                0,
                                triangle_index & 0xFF00,
                                triangle_index & 0xFF,
                            };
                    return new List<UInt32>(args);
                }

                return null;
            }
        }

        public UInt32 flags = 0;
        public UInt32 block_index = 0;
        public UInt32 triangle_index = 0;
        void Process()
        {
            if (Type == CodeType.Halo3)
            {
                flags = Parameters[0] - 0x20;
                block_index = Parameters[1];
                triangle_index = (UInt32)(Parameters[3] + (Parameters[2] << 8));
            }
            if (Type == CodeType.HaloOnline)
            {
                flags = Parameters[1];
                block_index = (Parameters[0] - 0x20) / 0x4;
                triangle_index = (UInt32)(Parameters[3] + (Parameters[2] << 8));
            }


        }

        public override MOPPCode Transform(CodeType new_type)
        {
            MOPPCode_0x53 code = null;

            var old_type = Type;
            Type = new_type;
            {
                var args = this.Arguments;
                code = new MOPPCode_0x53(new_type, this.Code, this.Position, args);

                
            }
            Type = old_type;

            return code;
        }

        public MOPPCode_0x53(CodeType type, int code, long position, params UInt32[] arguements) : base(type, code, position, arguements)
        {
            Process();
        }

        public MOPPCode_0x53(CodeType type, int code, long position, List<UInt32> arguements) : base(type, code, position, arguements)
        {
            Process();
        }
    }
}
