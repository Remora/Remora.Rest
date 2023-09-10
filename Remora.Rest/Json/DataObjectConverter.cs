//
//  SPDX-FileName: DataObjectConverter.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Remora.Rest.Core;
using Remora.Rest.Extensions;
using Remora.Rest.Json.Internal;
using Remora.Rest.Json.Reflection;

namespace Remora.Rest.Json;

/// <summary>
/// Converts to and from an object interface and its corresponding implementation..
/// </summary>
/// <typeparam name="TInterface">The interface that is seen in the objects.</typeparam>
/// <typeparam name="TImplementation">The concrete implementation.</typeparam>
[PublicAPI]
public class DataObjectConverter<TInterface, TImplementation> : JsonConverterFactory
    where TImplementation : TInterface
{
    // Stores the initialization info.
    private readonly IInitializationInfo _dtoInitialization;

    private readonly IReadOnlyList<PropertyInfo> _dtoProperties;

    // JSON writers for all properties in the DTO
    private readonly IReadOnlyDictionary<PropertyInfo, DTOPropertyWriter> _dtoPropertyWriters;

    // Empty optionals for all properties of type Optional<T> (for polyfilling default values)
    private readonly IReadOnlyDictionary<Type, object?> _dtoEmptyOptionals;

    private readonly Dictionary<PropertyInfo, string[]> _readNameOverrides = new();
    private readonly Dictionary<PropertyInfo, string> _writeNameOverrides = new();
    private readonly HashSet<PropertyInfo> _includeReadOnlyOverrides = new();
    private readonly HashSet<PropertyInfo> _excludeOverrides = new();

    private readonly Dictionary<PropertyInfo, JsonConverter> _converterOverrides = new();
    private readonly Dictionary<PropertyInfo, JsonConverterFactory> _converterFactoryOverrides = new();

    // Lazily initialized based on what specific converter is requested
    private ObjectFactory<TImplementation>? _implementationDtoFactory;
    private ObjectFactory<TInterface>? _interfaceDtoFactory;

    /// <summary>
    /// Holds a value indicating whether extra undefined properties should be allowed.
    /// </summary>
    private bool _allowExtraProperties = true;

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(TImplementation) || typeToConvert == typeof(TInterface);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataObjectConverter{TInterface, TImplementation}"/> class.
    /// </summary>
    public DataObjectConverter()
    {
        var implementationType = typeof(TImplementation);
        var interfaceType = typeof(TInterface);

        var visibleProperties = implementationType.GetPublicProperties().ToArray();

        var dtoConstructor = FindBestMatchingConstructor(visibleProperties);

        if (dtoConstructor is not null)
        {
            _dtoInitialization = new ConstructorInitializationInfo(dtoConstructor);
            _dtoProperties = ReorderProperties(visibleProperties, dtoConstructor);
        }
        else
        {
            _dtoInitialization = new ObjectInitializationInfo(implementationType, visibleProperties);
            _dtoProperties = visibleProperties;
        }

        var interfaceMap = implementationType.GetInterfaceMap(interfaceType);
        _dtoPropertyWriters = _dtoProperties
            .ToDictionary
            (
                p => p,
                p => CreatePropertyWriter(p, interfaceMap)
            );

        _dtoEmptyOptionals = _dtoProperties
            .Select(p => p.PropertyType)
            .Where(t => t.IsOptional())
            .Distinct()
            .ToDictionary(t => t, Activator.CreateInstance);
    }

    private static DTOPropertyWriter CreatePropertyWriter
    (
        PropertyInfo property,
        InterfaceMapping interfaceMap
    )
    {
        var getterMethod = property.GetGetMethod();
        if (getterMethod is null)
        {
            throw new InvalidOperationException($"The property {property.Name} has no public getter.");
        }

        var interfaceGetterIndex = Array.IndexOf(interfaceMap.TargetMethods, getterMethod);

        if (interfaceGetterIndex != -1)
        {
            getterMethod = interfaceMap.InterfaceMethods[interfaceGetterIndex];
        }

        return ExpressionFactoryUtilities.CreatePropertyWriter(getterMethod);
    }

    /// <summary>
    /// Reorders the input properties based on the order and names of the parameters in the given constructor.
    /// </summary>
    /// <param name="visibleProperties">The properties.</param>
    /// <param name="constructor">The constructor.</param>
    /// <returns>The reordered properties.</returns>
    /// <exception cref="MissingMemberException">
    /// Thrown if no match between a property and a parameter can be established.
    /// </exception>
    private static IReadOnlyList<PropertyInfo> ReorderProperties
    (
        PropertyInfo[] visibleProperties,
        ConstructorInfo constructor
    )
    {
        var reorderedProperties = new List<PropertyInfo>(visibleProperties.Length);

        var constructorParameters = constructor.GetParameters();
        foreach (var constructorParameter in constructorParameters)
        {
            var matchingProperty = visibleProperties.FirstOrDefault
            (
                p =>
                    p.Name.Equals(constructorParameter.Name, StringComparison.InvariantCultureIgnoreCase) &&
                    p.PropertyType == constructorParameter.ParameterType
            );

            if (matchingProperty is null)
            {
                throw new MissingMemberException(typeof(TInterface).Name, constructorParameter.Name);
            }

            reorderedProperties.Add(matchingProperty);
        }

        // Add leftover properties at the end
        reorderedProperties.AddRange(visibleProperties.Except(reorderedProperties));

        return reorderedProperties;
    }

    /// <summary>
    /// Finds the best matching constructor on the implementation type. A valid constructor must have a matching
    /// set of types in its parameters as the visible properties that will be considered in serialization; the order
    /// need not match.
    /// </summary>
    /// <param name="visibleProperties">The visible set of properties.</param>
    /// <returns>The constructor, or <c>null</c> if none was found and construction is to fall back
    /// to using an object initializer.</returns>
    private static ConstructorInfo? FindBestMatchingConstructor(PropertyInfo[] visibleProperties)
    {
        var visiblePropertyTypes = visibleProperties
            .Where(p => p.CanWrite)
            .Select(p => p.PropertyType).ToArray();

        var implementationType = typeof(TImplementation);

        var implementationConstructors = implementationType.GetConstructors();
        if (implementationConstructors.Length == 1)
        {
            var singleCandidate = implementationConstructors[0];
            return IsMatchingConstructor(singleCandidate, visiblePropertyTypes)
                ? singleCandidate
                : null;
        }

        var matchingConstructors = implementationType.GetConstructors()
            .Where(c => IsMatchingConstructor(c, visiblePropertyTypes)).ToList();

        if (matchingConstructors.Count == 1)
        {
            return matchingConstructors[0];
        }

        return null;
    }

    /// <summary>
    /// Sets whether extra JSON properties without a matching DTO property are allowed. Such properties are, if
    /// allowed, ignored. Otherwise, they throw a <see cref="JsonException"/>.
    ///
    /// By default, this is true.
    /// </summary>
    /// <param name="allowExtraProperties">Whether to allow extra properties.</param>
    /// <returns>The converter, with the new setting.</returns>
    public DataObjectConverter<TInterface, TImplementation> AllowExtraProperties(bool allowExtraProperties = true)
    {
        _allowExtraProperties = allowExtraProperties;
        return this;
    }

    /// <summary>
    /// Explicitly marks a property as included in the set of serialized properties. This is useful when readonly
    /// properties need to be serialized for some reason.
    /// </summary>
    /// <param name="propertyExpression">The property expression.</param>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <returns>The converter, with the inclusion.</returns>
    public DataObjectConverter<TInterface, TImplementation> IncludeWhenSerializing<TProperty>
    (
        Expression<Func<TImplementation, TProperty>> propertyExpression
    )
    {
        if (propertyExpression.Body is not MemberExpression memberExpression)
        {
            throw new InvalidOperationException();
        }

        var member = memberExpression.Member;
        if (member is not PropertyInfo property)
        {
            throw new InvalidOperationException();
        }

        if (!_dtoProperties.Contains(property))
        {
            throw new InvalidOperationException();
        }

        if (_includeReadOnlyOverrides.Contains(property))
        {
            return this;
        }

        _includeReadOnlyOverrides.Add(property);
        return this;
    }

    /// <summary>
    /// Explicitly marks a property as excluded in the set of serialized properties. This is useful when read-write
    /// properties need to be kept off the wire for some reason.
    /// </summary>
    /// <param name="propertyExpression">The property expression.</param>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <returns>The converter, with the inclusion.</returns>
    public DataObjectConverter<TInterface, TImplementation> ExcludeWhenSerializing<TProperty>
    (
        Expression<Func<TImplementation, TProperty>> propertyExpression
    )
    {
        if (propertyExpression.Body is not MemberExpression memberExpression)
        {
            throw new InvalidOperationException();
        }

        var member = memberExpression.Member;
        if (member is not PropertyInfo property)
        {
            throw new InvalidOperationException();
        }

        if (!_dtoProperties.Contains(property))
        {
            throw new InvalidOperationException();
        }

        if (_excludeOverrides.Contains(property))
        {
            return this;
        }

        _excludeOverrides.Add(property);
        return this;
    }

    /// <summary>
    /// Overrides the name of the given property when serializing and deserializing JSON.
    /// </summary>
    /// <param name="propertyExpression">The property expression.</param>
    /// <param name="name">The new name.</param>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <returns>The converter, with the property name.</returns>
    public DataObjectConverter<TInterface, TImplementation> WithPropertyName<TProperty>
    (
        Expression<Func<TImplementation, TProperty>> propertyExpression,
        string name
    )
    {
        if (propertyExpression.Body is not MemberExpression memberExpression)
        {
            throw new InvalidOperationException();
        }

        var member = memberExpression.Member;
        if (member is not PropertyInfo property)
        {
            throw new InvalidOperationException();
        }

        if (!_dtoProperties.Contains(property))
        {
            throw new InvalidOperationException();
        }

        // Resolve the matching interface property
        property = _dtoProperties.First(p => p.Name == property.Name);

        _writeNameOverrides.Add(property, name );
        _readNameOverrides.Add(property, new[] { name });
        return this;
    }

    /// <summary>
    /// Overrides the name of the given property when serializing JSON.
    /// </summary>
    /// <param name="propertyExpression">The property expression.</param>
    /// <param name="name">The new name.</param>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <returns>The converter, with the property name.</returns>
    public DataObjectConverter<TInterface, TImplementation> WithWritePropertyName<TProperty>
    (
        Expression<Func<TImplementation, TProperty>> propertyExpression,
        string name
    )
    {
        if (propertyExpression.Body is not MemberExpression memberExpression)
        {
            throw new InvalidOperationException();
        }

        var member = memberExpression.Member;
        if (member is not PropertyInfo property)
        {
            throw new InvalidOperationException();
        }

        if (!_dtoProperties.Contains(property))
        {
            throw new InvalidOperationException();
        }

        // Resolve the matching interface property
        property = _dtoProperties.First(p => p.Name == property.Name);

        _writeNameOverrides.Add(property, name );
        return this;
    }

    /// <summary>
    /// Overrides the name of the given property when deserializing JSON.
    /// </summary>
    /// <param name="propertyExpression">The property expression.</param>
    /// <param name="name">The new name.</param>
    /// <param name="fallbacks">The fallback names to use if the primary name isn't present.</param>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <returns>The converter, with the property name.</returns>
    public DataObjectConverter<TInterface, TImplementation> WithReadPropertyName<TProperty>
    (
        Expression<Func<TImplementation, TProperty>> propertyExpression,
        string name,
        params string[] fallbacks
    )
    {
        if (propertyExpression.Body is not MemberExpression memberExpression)
        {
            throw new InvalidOperationException();
        }

        var member = memberExpression.Member;
        if (member is not PropertyInfo property)
        {
            throw new InvalidOperationException();
        }

        if (!_dtoProperties.Contains(property))
        {
            throw new InvalidOperationException();
        }

        // Resolve the matching interface property
        property = _dtoProperties.First(p => p.Name == property.Name);

        var overrides =
            fallbacks.Length == 0
                ? new[] { name }
                : new[] { name }.Concat(fallbacks).ToArray();

        _readNameOverrides.Add(property, overrides);

        return this;
    }

    /// <summary>
    /// Overrides the converter of the given property.
    /// </summary>
    /// <param name="propertyExpression">The property expression.</param>
    /// <param name="converter">The JSON converter.</param>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <returns>The converter, with the property name.</returns>
    public DataObjectConverter<TInterface, TImplementation> WithPropertyConverter<TProperty>
    (
        Expression<Func<TImplementation, TProperty>> propertyExpression,
        JsonConverter<TProperty> converter
    ) => AddPropertyConverter(propertyExpression, converter);

    /// <summary>
    /// Overrides the converter of the given property.
    /// </summary>
    /// <param name="propertyExpression">The property expression.</param>
    /// <param name="converter">The JSON converter.</param>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <returns>The converter, with the property name.</returns>
    public DataObjectConverter<TInterface, TImplementation> WithPropertyConverter<TProperty>
    (
        Expression<Func<TImplementation, Optional<TProperty>>> propertyExpression,
        JsonConverter<TProperty> converter
    ) => AddPropertyConverter(propertyExpression, converter);

    /// <summary>
    /// Overrides the converter of the given property.
    /// </summary>
    /// <param name="propertyExpression">The property expression.</param>
    /// <param name="converter">The JSON converter.</param>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <returns>The converter, with the property name.</returns>
    public DataObjectConverter<TInterface, TImplementation> WithPropertyConverter<TProperty>
    (
        Expression<Func<TImplementation, TProperty?>> propertyExpression,
        JsonConverter<TProperty> converter
    )
        where TProperty : struct
        => AddPropertyConverter(propertyExpression, converter);

    /// <summary>
    /// Overrides the converter of the given property.
    /// </summary>
    /// <param name="propertyExpression">The property expression.</param>
    /// <param name="converter">The JSON converter.</param>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <returns>The converter, with the property name.</returns>
    public DataObjectConverter<TInterface, TImplementation> WithPropertyConverter<TProperty>
    (
        Expression<Func<TImplementation, Optional<TProperty?>>> propertyExpression,
        JsonConverter<TProperty> converter
    )
        where TProperty : struct
        => AddPropertyConverter(propertyExpression, converter);

    /// <summary>
    /// Overrides the converter of the given property.
    /// </summary>
    /// <param name="propertyExpression">The property expression.</param>
    /// <param name="converter">The JSON converter.</param>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <typeparam name="TEnumerable">The enumerable type.</typeparam>
    /// <returns>The converter, with the property name.</returns>
    public DataObjectConverter<TInterface, TImplementation> WithPropertyConverter<TProperty, TEnumerable>
    (
        Expression<Func<TImplementation, TEnumerable>> propertyExpression,
        JsonConverter<TProperty> converter
    )
        where TProperty : struct
        where TEnumerable : IEnumerable<TProperty>
        => AddPropertyConverter(propertyExpression, converter);

    /// <summary>
    /// Overrides the converter of the given property.
    /// </summary>
    /// <param name="propertyExpression">The property expression.</param>
    /// <param name="converter">The JSON converter.</param>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <typeparam name="TEnumerable">The enumerable type.</typeparam>
    /// <returns>The converter, with the property name.</returns>
    public DataObjectConverter<TInterface, TImplementation> WithPropertyConverter<TProperty, TEnumerable>
    (
        Expression<Func<TImplementation, Optional<TEnumerable>>> propertyExpression,
        JsonConverter<TProperty> converter
    )
        where TProperty : struct
        where TEnumerable : IEnumerable<TProperty>
        => AddPropertyConverter(propertyExpression, converter);

    /// <summary>
    /// Overrides the converter of the given property.
    /// </summary>
    /// <param name="propertyExpression">The property expression.</param>
    /// <param name="converterFactory">The JSON converter factory.</param>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <returns>The converter, with the property name.</returns>
    public DataObjectConverter<TInterface, TImplementation> WithPropertyConverter<TProperty>
    (
        Expression<Func<TImplementation, TProperty>> propertyExpression,
        JsonConverterFactory converterFactory
    ) => AddPropertyConverter(propertyExpression, converterFactory);

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var writeProperties = new List<DTOPropertyInfo>();
        var readProperties = new List<DTOPropertyInfo>();

        var parameters = (_dtoInitialization as ConstructorInitializationInfo)?.Constructor.GetParameters();
        var properties = _dtoProperties;
        for (int i = 0; i < properties.Count; i++)
        {
            var property = properties[i];

            Optional<object?> defaultValue = default;

            if (_dtoInitialization is ConstructorInitializationInfo)
            {
                // Properties are currently sorted to match the parameter order, followed by read-only properties.
                // As such, we assume that this code will give us the matching parameter for a property.
                var parameter = (uint)i < (uint)parameters!.Length ? parameters[i] : null;

                // Just to be sure, we assert this behavior here. This is essentially how property reordering associated property and parameter.
                Debug.Assert
                (
                    parameter == null || (property.Name.Equals(parameter.Name, StringComparison.InvariantCultureIgnoreCase) && property.PropertyType == parameter.ParameterType),
                    "Expectations around property/parameter order are upheld."
                );

                defaultValue = GetDefaultValueForParameter(property.PropertyType, parameter);
            }

            var converter = GetConverter(property, options);
            var propertyOptions = converter == null ? options : CreatePropertyConverterOptions(options, converter);
            var readNames = GetReadJsonPropertyName(property, options);
            var writeNames = GetWriteJsonPropertyName(property, options);
            var writer = GetPropertyWriter(property);

            // We cache this as well since the check is somewhat complex
            var allowsNull = property.AllowsNull();

            var data = new DTOPropertyInfo
            (
                property,
                readNames,
                writeNames,
                writer,
                allowsNull,
                defaultValue,
                propertyOptions,
                readProperties.Count
            );

            if (property.CanWrite)
            {
                // If a property is writable, it can be *read* from JSON.
                readProperties.Add(data);
            }

            if ((property.CanWrite || ShouldIncludeReadOnlyProperty(property)) && !_excludeOverrides.Contains(property))
            {
                // Any property that is writable and not excluded due to being read-only,
                // can be *written* to JSON.
                writeProperties.Add(data);
            }
        }

        if (typeToConvert == typeof(TInterface))
        {
            _interfaceDtoFactory ??= ExpressionFactoryUtilities.CreateFactory<TInterface>(_dtoInitialization);
            return new BoundDataObjectConverter<TInterface>
            (
                _interfaceDtoFactory,
                _allowExtraProperties,
                writeProperties.ToArray(),
                readProperties.ToArray()
            );
        }

        // ReSharper disable once InvertIf
        if (typeToConvert == typeof(TImplementation))
        {
            _implementationDtoFactory ??= ExpressionFactoryUtilities.CreateFactory<TImplementation>(_dtoInitialization);
            return new BoundDataObjectConverter<TImplementation>
            (
                _implementationDtoFactory,
                _allowExtraProperties,
                writeProperties.ToArray(),
                readProperties.ToArray()
            );
        }

        throw new ArgumentException("This converter cannot convert the provided type.", nameof(typeToConvert));
    }

    private static JsonSerializerOptions CreatePropertyConverterOptions
    (
        JsonSerializerOptions options,
        JsonConverter converter
    )
    {
        var cloned = new JsonSerializerOptions(options);
        cloned.Converters.Insert(0, converter);

        return cloned;
    }

    /// <summary>
    /// Gets the JSON property names for reading the specified property.
    /// </summary>
    /// <param name="dtoProperty">The property to get the names for.</param>
    /// <param name="options">The active serializer options.</param>
    /// <returns>An array of the supported names for this property.</returns>
    private string[] GetReadJsonPropertyName(PropertyInfo dtoProperty, JsonSerializerOptions options)
    {
        return _readNameOverrides.TryGetValue(dtoProperty, out var overriddenName)
            ? overriddenName
            : new[] { options.PropertyNamingPolicy?.ConvertName(dtoProperty.Name) ?? dtoProperty.Name };
    }

    /// <summary>
    /// Gets the JSON property name for writing the specified property.
    /// </summary>
    /// <param name="dtoProperty">The property to get the name for.</param>
    /// <param name="options">The active serializer options.</param>
    /// <returns>The name to write the property with.</returns>
    private string GetWriteJsonPropertyName(PropertyInfo dtoProperty, JsonSerializerOptions options)
    {
        if (_writeNameOverrides.TryGetValue(dtoProperty, out var overriddenName))
        {
            return overriddenName;
        }

        return options.PropertyNamingPolicy?.ConvertName(dtoProperty.Name) ?? dtoProperty.Name;
    }

    /// <summary>
    /// Gets the property converter for a specified property.
    /// </summary>
    /// <param name="dtoProperty">The property to get a property converter for.</param>
    /// <param name="options">The active serializer options.</param>
    /// <returns>
    /// The registered property converter, or <see langword="null"/> if no property converter was added.
    /// </returns>
    private JsonConverter? GetConverter(PropertyInfo dtoProperty, JsonSerializerOptions options)
    {
        if (_converterOverrides.TryGetValue(dtoProperty, out var converter))
        {
            return converter;
        }

        if (!_converterFactoryOverrides.TryGetValue(dtoProperty, out var converterFactory))
        {
            return null;
        }

        var innerType = dtoProperty.PropertyType.Unwrap();

        // Special case: enums
        if (converterFactory is JsonStringEnumConverter)
        {
            while (!innerType.IsEnum)
            {
                if (!innerType.IsGenericType)
                {
                    throw new JsonException("The innermost type of the property isn't an enum.");
                }

                innerType = innerType.GetGenericArguments()[0];
            }
        }

        var createdConverter = converterFactory.CreateConverter(innerType, options);
        return createdConverter;
    }

    /// <summary>
    /// Gets the default value for a parameter. If this is an <see cref="Optional{TValue}"/> parameter,
    /// uses <see langword="default"/> if there is no explicit default.
    /// </summary>
    /// <param name="propertyType">The type of the associated property. This should be equal to the parameter's type, unless it is null.</param>
    /// <param name="parameter">The parameter to get the default value for.</param>
    /// <returns>Empty, if there is no default, otherwise the default parameter value.</returns>
    private Optional<object?> GetDefaultValueForParameter(Type propertyType, ParameterInfo? parameter)
    {
        // If there is an explicit default parameter, we use that.
        object? defaultValue;
        if (parameter != null && parameter.HasDefaultValue)
        {
            defaultValue = parameter.DefaultValue;
            if (propertyType.IsValueType && defaultValue is null)
            {
                // "default" default parameters for value-types are null here. Instantiate the appropriate value.
                // We try to grab an empty optional first since there is a good chance we're dealing with an Optional<T>.
                defaultValue = _dtoEmptyOptionals.GetValueOrDefault(propertyType) ?? Activator.CreateInstance(propertyType);
            }

            return defaultValue;
        }

        // Polyfill default parameters for Optional<T> properties.
        if (_dtoEmptyOptionals.TryGetValue(propertyType, out defaultValue))
        {
            return defaultValue;
        }

        // Otherwise, we have no default value.
        return default;
    }

    /// <summary>
    /// Returns whether the specified property should be included when serializing even if it is read-only.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>Whether the property should be included even if it is read-only.</returns>
    private bool ShouldIncludeReadOnlyProperty(PropertyInfo property)
    {
        return _includeReadOnlyOverrides.Contains(property);
    }

    /// <summary>
    /// Gets a delegate that can write the <paramref name="property"/> to JSON given an instance of
    /// <typeparamref name="TImplementation"/>.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>A <see cref="DTOPropertyWriter"/> for the specified property.</returns>
    private DTOPropertyWriter GetPropertyWriter(PropertyInfo property)
    {
        return _dtoPropertyWriters[property];
    }

    private DataObjectConverter<TInterface, TImplementation> AddPropertyConverter<TExpression>
    (
        Expression<TExpression> expression,
        JsonConverter converter
    )
    {
        if (expression.Body is not MemberExpression memberExpression)
        {
            throw new InvalidOperationException();
        }

        var member = memberExpression.Member;
        if (member is not PropertyInfo property)
        {
            throw new InvalidOperationException();
        }

        // Resolve the matching interface property
        property = _dtoProperties.First(p => p.Name == property.Name);

        _converterOverrides.Add(property, converter);
        return this;
    }

    private DataObjectConverter<TInterface, TImplementation> AddPropertyConverter<TExpression>
    (
        Expression<TExpression> expression,
        JsonConverterFactory converterFactory
    )
    {
        if (expression.Body is not MemberExpression memberExpression)
        {
            throw new InvalidOperationException();
        }

        var member = memberExpression.Member;
        if (member is not PropertyInfo property)
        {
            throw new InvalidOperationException();
        }

        // Resolve the matching interface property
        property = _dtoProperties.First(p => p.Name == property.Name);

        _converterFactoryOverrides.Add(property, converterFactory);
        return this;
    }

    private static bool IsMatchingConstructor
    (
        ConstructorInfo constructor,
        IReadOnlyCollection<Type> visiblePropertyTypes
    )
    {
        if (constructor.GetParameters().Length != visiblePropertyTypes.Count)
        {
            return false;
        }

        var parameterTypeCounts = new Dictionary<Type, int>();
        foreach (var parameterType in constructor.GetParameters().Select(p => p.ParameterType))
        {
            if (parameterTypeCounts.ContainsKey(parameterType))
            {
                parameterTypeCounts[parameterType] += 1;
            }
            else
            {
                parameterTypeCounts.Add(parameterType, 1);
            }
        }

        var propertyTypeCounts = new Dictionary<Type, int>();
        foreach (var propertyType in visiblePropertyTypes)
        {
            if (propertyTypeCounts.ContainsKey(propertyType))
            {
                propertyTypeCounts[propertyType] += 1;
            }
            else
            {
                propertyTypeCounts.Add(propertyType, 1);
            }
        }

        if (parameterTypeCounts.Count != propertyTypeCounts.Count)
        {
            return false;
        }

        foreach (var (propertyType, propertyTypeCount) in propertyTypeCounts)
        {
            if (!parameterTypeCounts.TryGetValue(propertyType, out var parameterTypeCount))
            {
                return false;
            }

            if (propertyTypeCount != parameterTypeCount)
            {
                return false;
            }
        }

        // This constructor matches
        return true;
    }
}
