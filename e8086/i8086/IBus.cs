
namespace KDS.e8086
{
    public enum SegmentOverrideState
    {
        UseCS,
        UseDS,
        UseSS,
        UseES,
        NoOverride
    };

    public interface IBus
    {
        ushort CS { get; set; }  // code segment
        ushort DS { get; set; }  // data segment
        ushort SS { get; set; }  // stack segment
        ushort ES { get; set; }  // extra segmemt
        ushort IP { get; set; }  // instruction pointer
        SegmentOverrideState SegmentOverride { get; set; }
        bool UsingBasePointer { get; set; }
        RAM Ram { get; set; }

        void SaveByteString(int offset, byte data);
        void SaveWordString(int offset, ushort data);
        void SaveData(int word_size, int offset, int value);
        void SaveByte(int offset, byte value);
        void SaveWord(int offset, ushort value);

        ushort PopStack(int offset);
        void PushStack(int offset, ushort value);

        byte NextIP();

        void MoveByteString(int src_offset, int dst_offset);
        void MoveWordString(int src_offset, int dst_offset);

        int GetDestString(int word_size, int offset);
        byte GetByteDestString(int offset);
        ushort GetWordDestString(int offset);

        int GetData(int word_size, int offset);
        ushort GetWord(int segment, int offset);
    }
}
