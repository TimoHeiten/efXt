using System;
namespace heitech.efXt
{
    ///<summary>
    /// Marker Interface for the discovery of allowed Type and Id Configurations
    ///</summary>
    public interface IHasId<T>
        where T : IEquatable<T>
    {
        T Id { get; }
    }
}