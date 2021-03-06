﻿//-----------------------------------------------------------------------
// <copyright file="IAssociateSuccessfulResponseRelyingParty.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DotNetOpenAuth.OpenId.Messages {
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Text;
	using DotNetOpenAuth.Messaging;

	/// <summary>
	/// A successful association response as it is received by the relying party.
	/// </summary>
	[ContractClass(typeof(IAssociateSuccessfulResponseRelyingPartyContract))]
	internal interface IAssociateSuccessfulResponseRelyingParty : IProtocolMessage {
		/// <summary>
		/// Called to create the Association based on a request previously given by the Relying Party.
		/// </summary>
		/// <param name="request">The prior request for an association.</param>
		/// <returns>The created association.</returns>
		Association CreateAssociationAtRelyingParty(AssociateRequest request);
	}
}
