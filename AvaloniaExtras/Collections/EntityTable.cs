using System.Collections.Concurrent;
using AutoInterfaceAttributes;

namespace AvaloniaExtras.Collections;

/// <summary>
/// Very simple wrapper over a concurrent dictionary that allows entities to
/// be associated to a string id and retrieved by that id as a specific type, if
/// the entity is of that type.
/// </summary>
public sealed class EntityTable : EntityTable<string>;

/// <summary>
/// Very simple wrapper over a concurrent dictionary that allows entities to
/// be associated to an id and retrieved by that id as a specific type, if
/// the entity is of that type.
/// </summary>
/// <typeparam name="TId">Key type.</typeparam>
[AutoInterface]
public class EntityTable<TId> : IEntityTable<TId>
    where TId : notnull
{
    private readonly ConcurrentDictionary<TId, object> _entities = new();

    /// <summary>
    /// Set an entity in the table.
    /// </summary>
    /// <param name="id">Entity id.</param>
    /// <param name="entity">Entity object.</param>
    public void Set(TId id, object entity) => _entities.TryAdd(id, entity);

    /// <summary>
    /// Remove an entity from the table.
    /// </summary>
    /// <param name="id">Entity id.</param>
    public void Remove(TId? id)
    {
        if (id is null)
        {
            return;
        }

        _entities.TryRemove(id, out _);
    }

    /// <summary>
    /// Retrieve an entity from the table.
    /// </summary>
    /// <typeparam name="TValue">Type to use the entity as — entity must be
    /// assignable to this type.</typeparam>
    /// <param name="id"></param>
    /// <returns>Entity with the associated id as the given type, if the entity
    /// exists and is of that type.</returns>
    public TValue? Get<TValue>(TId? id)
        where TValue : class
    {
        if (
            id is not null
            && _entities.TryGetValue(id, out var entity)
            && entity is TValue expected
        )
        {
            return expected;
        }

        return default;
    }
}
