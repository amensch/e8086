
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

        /// <summary>
        /// Stack operations
        /// </summary>
        ushort PopStack(int offset);
        void PushStack(int offset, ushort value);

        /// <summary>
        /// Get next immediate byte and increment the program counter.
        /// </summary>
        byte NextImmediate();

        /// <summary>
        /// Get/Save from Memory
        /// </summary>
        int GetData(int word_size, int offset);
        void SaveData(int word_size, int offset, int value);

        /// <summary>
        /// Used by repeatable string operations
        /// </summary>
        void SaveString(int word_size, int offset, int data);
        void MoveString(int word_size, int src_offset, int dst_offset);
        int GetDestString(int word_size, int offset);

        /// <summary>
        /// For overriding the segment selection made by GetData/SaveData.
        /// Currently used by the interrupt instruction.
        /// </summary>
        ushort GetWord(int segment, int offset);
    }
}
