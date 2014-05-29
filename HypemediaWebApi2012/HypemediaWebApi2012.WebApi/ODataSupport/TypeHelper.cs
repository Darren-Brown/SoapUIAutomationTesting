using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Spatial;
using System.Xml.Linq;
using Microsoft.Data.Edm;
using Microsoft.Data.Edm.Library;

namespace HypemediaWebApi2012.WebApi.ODataSupport
{
    public class TypeHelper : ITypeHelper
    {
        public bool IsPrimitiveType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException();

            type = GetInnermostElementType(type);

            return
                type.IsEnum ||
                type.IsPrimitive ||
                type == typeof(Uri) ||
                (GetEdmPrimitiveTypeOrNull(type) != null);
        }

        public bool IsCollection(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type) && !typeof(String).IsAssignableFrom(type);
        }

        public bool IsComplexType(Type type)
        {
            return !IsPrimitiveType(type) && !IsCollection(type);
        }

        public Type GetCollectionElementType(Type collectionType)
        {
            // Can only get the element type if the collection is generic or if it's an array.
            if (collectionType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(collectionType.GetGenericTypeDefinition()))
                return collectionType.GetGenericArguments()[0];

            if (collectionType.IsArray)
                return collectionType.GetElementType();

            return null;
        }

        private Type GetInnermostElementType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException();

            while (true)
            {
                Type nullableUnderlyingType = Nullable.GetUnderlyingType(type);
                if (nullableUnderlyingType != null)
                {
                    type = nullableUnderlyingType;
                }
                else if (type.HasElementType)
                {
                    type = type.GetElementType();
                }
                else
                {
                    return type;
                }
            }
        }

        public Type GetClrType(Type type)
        {
            var innerType = GetInnermostElementType(type);
            var enumerableType = GetImplementedIEnumerableType(type);
            if (enumerableType != null)
                innerType = enumerableType;

            return innerType;
        }

        private static IEdmPrimitiveType GetEdmPrimitiveTypeOrNull(Type clrType)
        {
            IEdmPrimitiveType primitiveType;
            return _builtInTypesMapping.TryGetValue(clrType, out primitiveType) ? primitiveType : null;
        }

        private static readonly EdmCoreModel _coreModel = EdmCoreModel.Instance;

        private static readonly Dictionary<Type, IEdmPrimitiveType> _builtInTypesMapping =
            new[]
                {
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(string), GetPrimitiveType(EdmPrimitiveTypeKind.String)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Boolean), GetPrimitiveType(EdmPrimitiveTypeKind.Boolean)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Boolean?), GetPrimitiveType(EdmPrimitiveTypeKind.Boolean)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Byte), GetPrimitiveType(EdmPrimitiveTypeKind.Byte)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Byte?), GetPrimitiveType(EdmPrimitiveTypeKind.Byte)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(DateTime), GetPrimitiveType(EdmPrimitiveTypeKind.DateTime)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(DateTime?), GetPrimitiveType(EdmPrimitiveTypeKind.DateTime)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Decimal), GetPrimitiveType(EdmPrimitiveTypeKind.Decimal)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Decimal?), GetPrimitiveType(EdmPrimitiveTypeKind.Decimal)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Double), GetPrimitiveType(EdmPrimitiveTypeKind.Double)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Double?), GetPrimitiveType(EdmPrimitiveTypeKind.Double)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Guid), GetPrimitiveType(EdmPrimitiveTypeKind.Guid)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Guid?), GetPrimitiveType(EdmPrimitiveTypeKind.Guid)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Int16), GetPrimitiveType(EdmPrimitiveTypeKind.Int16)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Int16?), GetPrimitiveType(EdmPrimitiveTypeKind.Int16)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Int32), GetPrimitiveType(EdmPrimitiveTypeKind.Int32)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Int32?), GetPrimitiveType(EdmPrimitiveTypeKind.Int32)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Int64), GetPrimitiveType(EdmPrimitiveTypeKind.Int64)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Int64?), GetPrimitiveType(EdmPrimitiveTypeKind.Int64)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(SByte), GetPrimitiveType(EdmPrimitiveTypeKind.SByte)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(SByte?), GetPrimitiveType(EdmPrimitiveTypeKind.SByte)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Single), GetPrimitiveType(EdmPrimitiveTypeKind.Single)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Single?), GetPrimitiveType(EdmPrimitiveTypeKind.Single)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(byte[]), GetPrimitiveType(EdmPrimitiveTypeKind.Binary)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Stream), GetPrimitiveType(EdmPrimitiveTypeKind.Stream)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Geography), GetPrimitiveType(EdmPrimitiveTypeKind.Geography)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(GeographyPoint), GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPoint)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(GeographyLineString), GetPrimitiveType(EdmPrimitiveTypeKind.GeographyLineString)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(GeographyPolygon), GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPolygon)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(GeographyCollection), GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(GeographyMultiLineString), GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiLineString)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(GeographyMultiPoint), GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPoint)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(GeographyMultiPolygon), GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPolygon)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Geometry), GetPrimitiveType(EdmPrimitiveTypeKind.Geometry)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(GeometryPoint), GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPoint)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(GeometryLineString), GetPrimitiveType(EdmPrimitiveTypeKind.GeometryLineString)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(GeometryPolygon), GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPolygon)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(GeometryCollection), GetPrimitiveType(EdmPrimitiveTypeKind.GeometryCollection)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(GeometryMultiLineString), GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiLineString)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(GeometryMultiPoint), GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPoint)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(GeometryMultiPolygon), GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPolygon)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(TimeSpan), GetPrimitiveType(EdmPrimitiveTypeKind.Time)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(TimeSpan?), GetPrimitiveType(EdmPrimitiveTypeKind.Time)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(DateTimeOffset), GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(DateTimeOffset?), GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset)),

                    // Keep the Binary and XElement in the end, since there are not the default mappings for Edm.Binary and Edm.String.
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(XElement), GetPrimitiveType(EdmPrimitiveTypeKind.String)),
                    new KeyValuePair<Type, IEdmPrimitiveType>(typeof(Binary), GetPrimitiveType(EdmPrimitiveTypeKind.Binary)),
                }
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);


        private static IEdmPrimitiveType GetPrimitiveType(EdmPrimitiveTypeKind primitiveKind)
        {
            return _coreModel.GetPrimitiveType(primitiveKind);
        }

        private static Type GetImplementedIEnumerableType(Type type)
        {
            if (type.IsGenericType && type.IsInterface &&
                (type.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                 type.GetGenericTypeDefinition() == typeof(IQueryable<>)))
            {
                // special case the IEnumerable<T>
                return GetInnerGenericType(type);
            }
            else
            {
                // for the rest of interfaces and strongly Type collections
                Type[] interfaces = type.GetInterfaces();
                foreach (Type interfaceType in interfaces)
                {
                    if (interfaceType.IsGenericType &&
                        (interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                         interfaceType.GetGenericTypeDefinition() == typeof(IQueryable<>)))
                    {
                        // special case the IEnumerable<T>
                        return GetInnerGenericType(interfaceType);
                    }
                }
            }

            return null;
        }

        private static Type GetInnerGenericType(Type interfaceType)
        {
            // Getting the type T definition if the returning type implements IEnumerable<T>
            Type[] parameterTypes = interfaceType.GetGenericArguments();

            if (parameterTypes.Length >= 1)
            {
                return parameterTypes[0];
            }

            return null;
        }
    }
}