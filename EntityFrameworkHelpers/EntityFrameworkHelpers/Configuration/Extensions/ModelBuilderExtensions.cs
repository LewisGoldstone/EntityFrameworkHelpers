using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkHelpers.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Required for ContainsKey functionality
    /// </summary>
    /// <param name="modelBuilder">ModelBuilder on creating</param>
    /// <param name="entityTypes">Entity Type(s)</param>
    public static void RegisterHelpers(this ModelBuilder modelBuilder, params Type[] entityTypes)
    {
        foreach (Type entityType in entityTypes)
        {
            modelBuilder
                .Entity(entityType, eb =>
                {
                    eb.Property<int>(nameof(PaginatedRow<object>.Count)).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                });

        }
    }
}
