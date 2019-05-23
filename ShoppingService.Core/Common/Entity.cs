using System;

namespace ShoppingService.Core.Common
{
    public abstract class Entity
	{
        public Guid Id { get; internal set; }
        public DateTime CreatedAt { get; internal set; }
    }
}
