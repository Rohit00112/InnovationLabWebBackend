using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace InnovationLab.Landing.SwaggerFilters;

/// <summary>
/// Schema filter to ensure DTOs with IFormFile properties are properly documented in Swagger
/// </summary>
public class FormDataSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;

        // Process properties that are IFormFile
        if (schema.Properties == null)
            return;

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            if (!schema.Properties.ContainsKey(prop.Name))
                continue;

            var propType = prop.PropertyType;

            // Handle IFormFile
            if (propType == typeof(IFormFile))
            {
                schema.Properties[prop.Name] = new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary",
                    Description = schema.Properties[prop.Name].Description
                };
            }
            // Handle List<IFormFile> or IList<IFormFile>
            else if (propType.IsGenericType &&
                     (propType.GetGenericTypeDefinition() == typeof(List<>) ||
                      propType.GetGenericTypeDefinition() == typeof(IList<>)))
            {
                var genericArg = propType.GetGenericArguments()[0];

                if (genericArg == typeof(IFormFile))
                {
                    schema.Properties[prop.Name] = new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        },
                        Description = schema.Properties[prop.Name].Description
                    };
                }
            }
        }
    }
}
