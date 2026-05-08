using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace InnovationLab.Landing.SwaggerFilters;

/// <summary>
/// Swagger filter to properly document IFormFile parameters for file uploads
/// </summary>
public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasFormDataParams = context.MethodInfo
            .GetParameters()
            .Any(p => p.GetCustomAttribute<Microsoft.AspNetCore.Mvc.FromFormAttribute>() != null);

        if (!hasFormDataParams)
            return;

        // Get the FormData DTO parameter
        var formDataParam = context.MethodInfo
            .GetParameters()
            .FirstOrDefault(p => p.GetCustomAttribute<Microsoft.AspNetCore.Mvc.FromFormAttribute>() != null);

        if (formDataParam == null)
            return;

        var paramType = formDataParam.ParameterType;

        // Create multipart/form-data request body
        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = GenerateFormDataProperties(paramType),
                        Required = GetRequiredProperties(paramType)
                    }
                }
            }
        };
    }

    private Dictionary<string, OpenApiSchema> GenerateFormDataProperties(Type dtoType)
    {
        var properties = new Dictionary<string, OpenApiSchema>();
        var dtoProperties = dtoType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        foreach (var prop in dtoProperties)
        {
            var propType = prop.PropertyType;

            if (propType == typeof(IFormFile))
            {
                properties[prop.Name] = new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary"
                };
            }
            else if (propType.IsGenericType &&
                     (propType.GetGenericTypeDefinition() == typeof(List<>) ||
                      propType.GetGenericTypeDefinition() == typeof(IList<>)))
            {
                var genericArg = propType.GetGenericArguments()[0];

                if (genericArg == typeof(IFormFile))
                {
                    properties[prop.Name] = new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        }
                    };
                }
                else
                {
                    // For other list types, use array schema
                    properties[prop.Name] = new OpenApiSchema
                    {
                        Type = "array",
                        Items = GetSchemaForType(genericArg)
                    };
                }
            }
            else if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(propType);
                properties[prop.Name] = GetSchemaForType(underlyingType);
            }
            else
            {
                properties[prop.Name] = GetSchemaForType(propType);
            }
        }

        return properties;
    }

    private ISet<string> GetRequiredProperties(Type dtoType)
    {
        var requiredProps = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var properties = dtoType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        foreach (var prop in properties)
        {
            var requiredAttr = prop.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>();
            if (requiredAttr != null)
            {
                requiredProps.Add(prop.Name);
            }
        }

        return requiredProps;
    }

    private static OpenApiSchema GetSchemaForType(Type? type)
    {
        if (type == null)
            return new OpenApiSchema { Type = "object" };

        return type switch
        {
            _ when type == typeof(string) => new OpenApiSchema { Type = "string" },
            _ when type == typeof(int) => new OpenApiSchema { Type = "integer", Format = "int32" },
            _ when type == typeof(long) => new OpenApiSchema { Type = "integer", Format = "int64" },
            _ when type == typeof(bool) => new OpenApiSchema { Type = "boolean" },
            _ when type == typeof(Guid) => new OpenApiSchema { Type = "string", Format = "uuid" },
            _ when type == typeof(DateTime) => new OpenApiSchema { Type = "string", Format = "date-time" },
            _ when type == typeof(DateTimeOffset) => new OpenApiSchema { Type = "string", Format = "date-time" },
            _ when type.IsEnum => new OpenApiSchema
            {
                Type = "string",
                Enum = Enum.GetNames(type).Select(x => (IOpenApiAny)new OpenApiString(x)).ToList()
            },
            _ => new OpenApiSchema { Type = "object" }
        };
    }
}
