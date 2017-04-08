using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOPP.Codes
{
    class MOPPCode_0x01 : MOPPCode
    {
        public UInt32 param_0 = 0;
        public UInt32 param_1 = 0;

        public override List<UInt32> Arguments { get {
                if (Type == CodeType.Halo3)
                {
                    return new List<UInt32>(new UInt32[] {
                                param_0,
                                param_1
                            });
                }
                if(Type == CodeType.HaloOnline)
                {
                    return new List<UInt32>(new UInt32[] {
                                param_1,
                                param_0
                            });
                }

                return null;
            }
        }
        
        void Process()
        {
            if (Type == CodeType.Halo3)
            {
                param_0 = Parameters[0];
                param_1 = Parameters[1];
            }
            if (Type == CodeType.HaloOnline)
            {
                param_1 = Parameters[0];
                param_0 = Parameters[1];
            }
        }

        public override MOPPCode Transform(CodeType new_type)
        {
            MOPPCode_0x01 code = null;

            var old_type = Type;
            Type = new_type;
            {
                var args = this.Arguments;
                code = new MOPPCode_0x01(new_type, this.Code, this.Position, args);

                
            }
            Type = old_type;

            return code;
        }

        public MOPPCode_0x01(CodeType type, int code, long position, params UInt32[] arguements) : base(type, code, position, arguements)
        {
            Process();
        }

        public MOPPCode_0x01(CodeType type, int code, long position, List<UInt32> arguements) : base(type, code, position, arguements)
        {
            Process();
        }
    }
}
