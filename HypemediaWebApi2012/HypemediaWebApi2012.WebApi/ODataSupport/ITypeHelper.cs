using System;

namespace HypemediaWebApi2012.WebApi.ODataSupport
{
    public interface ITypeHelper
    {
        bool IsPrimitiveType(Type type);
        bool IsCollection(Type type);
        bool IsComplexType(Type type);
        Type GetClrType(Type type);
        Type GetCollectionElementType(Type collectionType);
    }
}