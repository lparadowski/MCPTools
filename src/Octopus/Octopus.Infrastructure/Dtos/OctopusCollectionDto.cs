namespace Octopus.Infrastructure.Dtos;

// Octopus paged resource collections are shaped { "TotalResults": N, "Items": [ ... ] }.
internal class OctopusCollectionDto<T>
{
    public List<T> Items { get; set; } = [];
}
