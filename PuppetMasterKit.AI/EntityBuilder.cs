﻿using System;
using System.Linq;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Utility;

namespace PuppetMasterKit.AI
{
	public class EntityBuilder
	{
		private Entity entity;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.EntityBuilder"/> class.
		/// </summary>
		private EntityBuilder()
		{
			entity = new Entity();
		}

		/// <summary>
		/// Build this instance.
		/// </summary>
		/// <returns>The build.</returns>
		public static EntityBuilder Build()
		{
			return new EntityBuilder();
		}

		/// <summary>
		/// Gets the entity.
		/// </summary>
		/// <returns>The entity.</returns>
		public Entity GetEntity(){
			return entity;
		}

        /// <summary>
		/// Register entity's components having a cetrain type with a component system
		/// </summary>
		/// <returns>The entity.</returns>
		/// <param name="withSystem">With system.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public EntityBuilder Register<T>(ComponentSystem withSystem)
		{
			var toRegister = entity.Components.Where(x => x is T);
			withSystem.AddRange(toRegister.ToArray());
			return this;
		}

		/// <summary>
		/// Register all the entity's components with a component system
		/// </summary>
		/// <returns>The register.</returns>
		/// <param name="withSystem">With system.</param>
		public EntityBuilder Register(ComponentSystem withSystem)
		{
			withSystem.AddRange(entity.Components.ToArray());
			return this;
		}

		/// <summary>
		/// Registers the entity with the system and also adds compoments to the entity
		/// </summary>
		/// <returns>The with.</returns>
		/// <param name="withSystem">With system.</param>
		/// <param name="components">Components.</param>
		public EntityBuilder With(ComponentSystem withSystem,
						   params Component[] components)
		{
			components.ForEach(x => {
				entity.Add(x);
				withSystem.Add(x);
			});
			return this;
		}
	}
}
