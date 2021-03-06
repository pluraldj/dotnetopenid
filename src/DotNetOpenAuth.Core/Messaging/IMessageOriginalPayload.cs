﻿//-----------------------------------------------------------------------
// <copyright file="IMessageOriginalPayload.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DotNetOpenAuth.Messaging {
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Text;

	/// <summary>
	/// An interface that appears on messages that need to retain a description of
	/// what their literal payload was when they were deserialized.
	/// </summary>
	[ContractClass(typeof(IMessageOriginalPayloadContract))]
	public interface IMessageOriginalPayload {
		/// <summary>
		/// Gets or sets the original message parts, before any normalization or default values were assigned.
		/// </summary>
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "By design")]
		IDictionary<string, string> OriginalPayload { get; set; }
	}

	/// <summary>
	/// Code contract for the <see cref="IMessageOriginalPayload"/> interface.
	/// </summary>
	[ContractClassFor(typeof(IMessageOriginalPayload))]
	internal abstract class IMessageOriginalPayloadContract : IMessageOriginalPayload {
		/// <summary>
		/// Gets or sets the original message parts, before any normalization or default values were assigned.
		/// </summary>
		IDictionary<string, string> IMessageOriginalPayload.OriginalPayload {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
	}
}
