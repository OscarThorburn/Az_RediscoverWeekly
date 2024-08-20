using Microsoft.AspNetCore.DataProtection;

namespace Az_Rediscover.Services
{
	/// <summary>
	/// Encrypts and decrypts data.
	/// </summary>
	public class DataProtectorService
    {
        private readonly IDataProtector _protector;
        public DataProtectorService(IDataProtectionProvider dataProtectionProvider)
        {
            _protector = dataProtectionProvider.CreateProtector("Rediscover.Services.DataProtectorService");
        }
        /// <summary>
        /// Protect the input data.
        /// </summary>
        /// <param name="input"></param>
        public string Protect(string input)
        {
            return _protector.Protect(input);
        }

        /// <summary>
        /// Unprotect the protected data.
        /// </summary>
        /// <param name="protectedData"></param>
        public string Unprotect(string protectedData)
        {
            return _protector.Unprotect(protectedData);
        }
    }
}
