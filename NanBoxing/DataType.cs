namespace NanBoxing
{
    public enum DataType {
        Invalid,
        Number,
        VariableRef, Opcode, 
        PtrString, PtrHashtable, PtrGrid,
        PtrArray_Int32, PtrArray_UInt32, PtrArray_String, PtrArray_Double,
        PtrSet_String, PtrSet_Int32, 
        PtrLinkedList,
        SmallFlags, ShortString,
        ValInt32, ValUInt32
    }
}