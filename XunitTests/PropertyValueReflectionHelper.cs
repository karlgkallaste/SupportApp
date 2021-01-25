using System;
using System.Linq.Expressions;
using System.Reflection;

public static class PropertyValueReflectionHelper
{
    private const BindingFlags AllInstanceBindingFlags =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    /// <summary>
    /// Sets property value (even with private/protected setter) by reflection (magic).
    /// Example: .SetProperty(a => a.AccountId, account.Id);
    /// </summary>
    public static T SetProperty<T, TProperty>(this T instance, Expression<Func<T, TProperty>> propertySelector, TProperty value) where T : class
    {
        if (propertySelector.Body.NodeType != ExpressionType.MemberAccess)
        {
            throw new MemberAccessException();
        }
        var memberExpression = (MemberExpression)propertySelector.Body;
        SetProperty(instance, value, memberExpression.Member.Name);
        return instance;
    }
    /// <summary>
    /// Sets property value (even with private/protected setter) by reflection (magic).
    /// </summary>
    public static T SetProperty<T, TProperty>(this T instance, string propertyName, TProperty value) where T : class
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException(nameof(string.IsNullOrWhiteSpace), nameof(propertyName));
        }
        SetProperty(instance, value, propertyName);
        return instance;
    }
    public static T GetPropertyValue<T>(this object instance, string propertyName)
    {
        var propertyValue = FindPropertyInfo(instance.GetType(), propertyName)
            .GetValue(instance);
        return (T)propertyValue;
    }
    private static void SetProperty<T, TProperty>(T instance, TProperty value, string propertyName) where T : class
    {
        var setMethod = FindPropertySetMethod(instance.GetType(), propertyName);
        setMethod.Invoke(instance, new object[] { value });
    }
    private static MethodInfo FindPropertySetMethod(Type type, string propertyName)
    {
        var propertyInfo = type.GetProperty(propertyName, AllInstanceBindingFlags);
        var baseType = type.GetTypeInfo().BaseType;
        if (propertyInfo?.SetMethod != null)
            return propertyInfo.SetMethod;
        if (propertyInfo == null || baseType == typeof(object))
            throw new InvalidOperationException(
                $"Property or property set method not found for [{propertyName}]. If you have a getter-only property you can turn it into a private set property to ease testing.");
        return FindPropertySetMethod(baseType, propertyName);
    }
    private static PropertyInfo FindPropertyInfo(Type type, string propertyName)
    {
        var propertyInfo = type.GetProperty(propertyName, AllInstanceBindingFlags);
        if (propertyInfo != null)
        {
            return propertyInfo;
        }
        var baseType = type.GetTypeInfo().BaseType;
        if (baseType == typeof(object))
        {
            throw new InvalidOperationException($"Property named \"{propertyName}\" could not be found.");
        }
        return FindPropertyInfo(baseType, propertyName);
    }
}