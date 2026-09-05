using System.Diagnostics.CodeAnalysis;

namespace DirectoryService.Application.Common;

/// <summary>
/// Marker interface for queries that produce a response of type <typeparamref name="TResponse"/>.
/// The type parameter is a phantom type used for compile-time handler resolution.
/// </summary>
[SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "Phantom type parameter used for compile-time handler resolution")]
public interface IQuery<out TResponse>;

