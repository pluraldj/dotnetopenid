﻿//-----------------------------------------------------------------------
// <copyright file="EndUserAuthorizationSuccessAccessTokenResponse.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DotNetOpenAuth.OAuth2.Messages {
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Text;

	using DotNetOpenAuth.Messaging;
	using DotNetOpenAuth.OAuth2.ChannelElements;

	/// <summary>
	/// The message sent by the Authorization Server to the Client via the user agent
	/// to indicate that user authorization was granted, carrying only an access token,
	/// and to return the user to the Client where they started their experience.
	/// </summary>
	internal class EndUserAuthorizationSuccessAccessTokenResponse : EndUserAuthorizationSuccessResponseBase, IAuthorizationCarryingRequest, IHttpIndirectResponse {
		/// <summary>
		/// Initializes a new instance of the <see cref="EndUserAuthorizationSuccessAccessTokenResponse"/> class.
		/// </summary>
		/// <param name="clientCallback">The URL to redirect to so the client receives the message. This may not be built into the request message if the client pre-registered the URL with the authorization server.</param>
		/// <param name="version">The protocol version.</param>
		internal EndUserAuthorizationSuccessAccessTokenResponse(Uri clientCallback, Version version)
			: base(clientCallback, version) {
			Requires.NotNull(version, "version");
			Requires.NotNull(clientCallback, "clientCallback");
			this.TokenType = Protocol.AccessTokenTypes.Bearer;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EndUserAuthorizationSuccessAccessTokenResponse"/> class.
		/// </summary>
		/// <param name="clientCallback">The URL to redirect to so the client receives the message. This may not be built into the request message if the client pre-registered the URL with the authorization server.</param>
		/// <param name="request">The authorization request from the user agent on behalf of the client.</param>
		internal EndUserAuthorizationSuccessAccessTokenResponse(Uri clientCallback, EndUserAuthorizationRequest request)
			: base(clientCallback, request) {
			Requires.NotNull(clientCallback, "clientCallback");
			Requires.NotNull(request, "request");
			((IMessageWithClientState)this).ClientState = request.ClientState;
			this.TokenType = Protocol.AccessTokenTypes.Bearer;
		}

		#region IAuthorizationCarryingRequest Members

		/// <summary>
		/// Gets or sets the verification code or refresh/access token.
		/// </summary>
		/// <value>The code or token.</value>
		string IAuthorizationCarryingRequest.CodeOrToken {
			get { return this.AccessToken; }
			set { this.AccessToken = value; }
		}

		/// <summary>
		/// Gets the type of the code or token.
		/// </summary>
		/// <value>The type of the code or token.</value>
		CodeOrTokenType IAuthorizationCarryingRequest.CodeOrTokenType {
			get { return CodeOrTokenType.AccessToken; }
		}

		/// <summary>
		/// Gets or sets the authorization that the token describes.
		/// </summary>
		/// <value></value>
		IAuthorizationDescription IAuthorizationCarryingRequest.AuthorizationDescription { get; set; }

		#endregion

		#region IHttpIndirectResponse Members

		/// <summary>
		/// Gets a value indicating whether the payload for the message should be included
		/// in the redirect fragment instead of the query string or POST entity.
		/// </summary>
		bool IHttpIndirectResponse.Include301RedirectPayloadInFragment {
			get { return true; }
		}

		#endregion

		/// <summary>
		/// Gets or sets the token type.
		/// </summary>
		/// <value>Usually "bearer".</value>
		/// <remarks>
		/// Described in OAuth 2.0 section 7.1.
		/// </remarks>
		[MessagePart(Protocol.token_type, IsRequired = true)]
		public string TokenType { get; internal set; }

		/// <summary>
		/// Gets or sets the access token.
		/// </summary>
		/// <value>The access token.</value>
		[MessagePart(Protocol.access_token, IsRequired = true)]
		public string AccessToken { get; set; }

		/// <summary>
		/// Gets or sets the scope of the <see cref="AccessToken"/> if one is given; otherwise the scope of the authorization code.
		/// </summary>
		/// <value>The scope.</value>
		[MessagePart(Protocol.scope, IsRequired = false, Encoder = typeof(ScopeEncoder))]
		public new ICollection<string> Scope {
			get { return base.Scope; }
			protected set { base.Scope = value; }
		}

		/// <summary>
		/// Gets or sets the lifetime of the authorization.
		/// </summary>
		/// <value>The lifetime.</value>
		[MessagePart(Protocol.expires_in, IsRequired = false, Encoder = typeof(TimespanSecondsEncoder))]
		internal TimeSpan? Lifetime { get; set; }
	}
}
