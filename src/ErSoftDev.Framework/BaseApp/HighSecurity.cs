namespace ErSoftDev.Framework.BaseApp
{
    public static class HighSecurity
    {
        /// <summary>
        /// Secret key for jwt token , more than 16 character
        /// </summary>
        public static string JwtSecretKey = "8'Sg3Mnv[zJa8}R[p*'_r?P7qkldsfjk((87dfwewencdkjekll";
        /// <summary>
        /// Encrypt key for encrypt generated token by jwt , exactly 16 character
        /// </summary>
        public static string JwtEncryptKey = "o+:EUi2ZF,oqjs,*";
    }
}
