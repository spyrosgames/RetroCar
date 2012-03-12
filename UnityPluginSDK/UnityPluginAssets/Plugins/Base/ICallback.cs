using System;
using System.Collections;

namespace Gree.Unity
{
	/// <summary>
	/// Interface for every callback.
	/// </summary>
	public interface ICallback
	{
		/// <summary>
		/// Execution for this callback with given args.
		/// </summary>
		/// <param name='args'>
		/// Arguments.
		/// </param>
		void execute(ArrayList args);
	}
}

