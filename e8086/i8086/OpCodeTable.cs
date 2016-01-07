using System;

namespace KDS.e8086
{
    public class OpCodeTable
    {
        public UInt16 op { get; set; }
        public string dasm_format { get; set; }
        public int num_operands { get; set; }
        public string arg1_mode { get; set; }
        public string arg1_operand { get; set; }
        public string arg2_mode { get; set; }
        public string arg2_operand { get; set; }

        public OpCodeTable(UInt16 _op, string _dasm_format, int _num_operands, string _arg1_mode, string _arg1_operand, string _arg2_mode, string _arg2_operand)
        {
            op = _op;
            dasm_format = _dasm_format;
            num_operands = _num_operands;
            arg1_mode = _arg1_mode;
            arg1_operand = _arg1_operand;
            arg2_mode = _arg2_mode;
            arg2_operand = _arg2_operand;
        }
    }
}
