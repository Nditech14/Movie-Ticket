using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Runtime.Serialization;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();
            var enumNames = Enum.GetNames(context.Type);
            foreach (var name in enumNames)
            {
                // Use EnumMember attribute values
                var memberInfo = context.Type.GetMember(name);
                var enumMemberAttribute = memberInfo[0].GetCustomAttributes(typeof(EnumMemberAttribute), false).FirstOrDefault() as EnumMemberAttribute;
                var enumValue = enumMemberAttribute != null ? enumMemberAttribute.Value : name;

                schema.Enum.Add(new OpenApiString(enumValue));
            }
        }
    }
}
