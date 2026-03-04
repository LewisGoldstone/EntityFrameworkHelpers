using System.Reflection;

namespace EntityFrameworkHelpers.Utils.Enums.Extensions;

public static class EnumExtensions
{
    public static T GetAttribute<T>(this Enum value)
    {
        Type type = value.GetType();
        MemberInfo[] memberInfos = type.GetMember(value.ToString());
        MemberInfo memberInfo = Array.Find(memberInfos, x => x.MemberType == MemberTypes.Field)
            ?? throw new ArgumentException("Attribute not found.");

        object[] attributes = memberInfo.GetCustomAttributes(typeof(T), false);

        return (T)attributes[0];
    }
}
