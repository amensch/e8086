using KDS.Loader;

namespace KDS.e8086
{
    interface ICodeDisassembler
    {
        string Disassemble(ICodeLoader loader);
    }
}
