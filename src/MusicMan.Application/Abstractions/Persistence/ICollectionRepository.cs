using MusicMan.Domain.Entities;

namespace MusicMan.Application.Abstractions.Persistence;

public interface ICollectionRepository
{
    Task AddAsync(CollectionItem item, CancellationToken cancellationToken = default);
}
