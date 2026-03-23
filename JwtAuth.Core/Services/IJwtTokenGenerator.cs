namespace JwtAuth.Core.Services
{
    /// <summary>
    /// Defines functionality for generating signed JWT access tokens.
    /// </summary>
    public interface IJwtTokenGenerator
    {
        /// <summary>
        /// Generates a signed JWT for the provided user identity.
        /// </summary>
        /// <param name="userId">The unique identifier of the authenticated user.</param>
        /// <param name="username">The username associated with the authenticated user.</param>
        /// <returns>A compact serialized JWT string.</returns>
        string GenerateToken(Guid userId, string username);
    }
}
