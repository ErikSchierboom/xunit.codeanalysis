using Microsoft.CodeAnalysis;

namespace xUnit.CodeAnalysis
{
    namespace Microsoft.CodeAnalysis.Shared.Extensions
    {
        internal static class NamedTypeSymbolExtensions
        {
            public static bool InheritsFromOrEquals(this INamedTypeSymbol type, INamedTypeSymbol baseType)
            {
                var current = type;
                while (current != null)
                {
                    if (current.Equals(baseType))
                        return true;

                    current = current.BaseType;
                }

                return false;
            }
        }
    }
}
