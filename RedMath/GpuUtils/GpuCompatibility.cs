using RedMath.Structures;
using System;

namespace RedMath.GpuUtils
{
    public interface IClassStructConverter<ClassType, StructType> where ClassType : class where StructType : struct
    {
        StructType ToStruct(ClassType cl);
        ClassType ToClass(StructType st);
    }

    public interface IStructureFieldOperations<StructType> where StructType : struct
    {
        Func<StructType, StructType, StructType> GetStructAddition();
        Func<StructType, StructType, StructType> GetStructMultiplication();
    }

    public interface IGpuStructManager<FieldType, StructType> : IClassStructConverter<FieldType, StructType>, IStructureFieldOperations<StructType> where FieldType : Field<FieldType> where StructType : struct
    {
        StructType GetStructDefaultValue();
    }

    public interface IGpuCompatible<FieldType, StructType> where FieldType : Field<FieldType> where StructType : struct
    {
        IGpuStructManager<FieldType, StructType> GetDefaultGpuStructManager();
    }
}
