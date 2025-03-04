﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.Identity.Client
{
    /// <summary>
    /// This exception class is to inform developers that UI interaction is required for authentication to
    /// succeed. It's thrown when calling <see cref="ClientApplicationBase.AcquireTokenSilent(System.Collections.Generic.IEnumerable{string}, IAccount)"/> or one
    /// of its overrides, and when the token does not exists in the cache, or the user needs to provide more content, or perform multiple factor authentication based
    /// on Azure AD policies, etc..
    /// For more details, see https://aka.ms/msal-net-exceptions
    /// </summary>
    public class MsalUiRequiredException : MsalServiceException
    {
        /// <summary>
        /// Initializes a new instance of the exception class with a specified
        /// error code and error message.
        /// </summary>
        /// <param name="errorCode">
        /// The error code returned by the service or generated by the client. This is the code you can rely on
        /// for exception handling.
        /// </param>
        /// <param name="errorMessage">The error message that explains the reason for the exception.</param>
        public MsalUiRequiredException(string errorCode, string errorMessage) :
            this(errorCode, errorMessage, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the exception class with a specified
        /// error code, error message and inner exception indicating the root cause.
        /// </summary>
        /// <param name="errorCode">
        /// The error code returned by the service or generated by the client. This is the code you can rely on
        /// for exception handling.
        /// </param>
        /// <param name="errorMessage">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">Represents the root cause of the exception.</param>
        public MsalUiRequiredException(string errorCode, string errorMessage, Exception innerException) :
            base(errorCode, errorMessage, innerException)
        {
        }

        /// <summary>
        /// Classification of the conditional access error, enabling you to do more actions or inform the user depending on your scenario. 
        /// See https://aka.ms/msal-net-UiRequiredException for more details.
        /// </summary>
        /// <remarks>The class <see cref="InvalidGrantClassification"/> lists most classification strings as constants. </remarks>
        internal string Classification
        {
            get
            {
                return InvalidGrantClassification.GetUiExceptionClassification(SubError);
            }
        }
    }
}
