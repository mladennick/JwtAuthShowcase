namespace JwtAuth.Core.Models
{
    /// <summary>
    /// Represents the JWT configuration values used for token creation and validation.
    /// </summary>
    /// <remarks>
    /// These values are typically bound from the <c>JwtSettings</c> section in application configuration.
    /// </remarks>
    public class JwtSettings
    {
        /// <summary>
        /// Gets or sets the symmetric signing key used to sign and validate JWT tokens.
        /// </summary>
        /// <remarks>
        /// Use a high-entropy secret suitable for HMAC SHA-256 signing.
        /// </remarks>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the token issuer (<c>iss</c>) expected during validation.
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the token audience (<c>aud</c>) expected during validation.
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the token lifetime in minutes.
        /// </summary>
        public int ExpiryMinutes { get; set; } = 60;
    }
}
