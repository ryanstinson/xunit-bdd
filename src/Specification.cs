﻿using System;
using System.Threading.Tasks;

namespace Xunit.Extensions
{
	/// <summary>
	/// The base specification class
	/// </summary>
	public abstract class Specification : ISpecification
	{
		/// <summary>
		/// The exception that was thrown when Observe was run; null if no exception was thrown.
		/// </summary>
		public Exception ThrownException { get; set; }

		/// <summary>
		/// Performs an action, the outcome of which will be observed to validate the specification.
		/// </summary>
		public abstract void Observe();

		Task ISpecification.ObserveAsync()
		{
			Observe();
			return Task.FromResult(0);
		}
	}
}