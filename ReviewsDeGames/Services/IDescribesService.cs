namespace ReviewsDeGames.Services
{
    /// <summary>
    /// Interface que provê métodos para se obter mensagens descritivas
    /// </summary>
    public interface IDescribesService
    {
        public string KeyNotFound(object id);
        public string EntityNotFound(object name, object id);
        public string InvalidCharacters();
        public string MaxLength(object max);
        public string NotEmptyOrMaxLength(object max);
        public string RangeValue(object min, object max);
        public string RangeLength(object min, object max);
        public string InvalidEmail();
        public string UnavaliableEmail(object email);
        public string UnavaliableUserName(object username);
        public string NotEmpty();
        public string FormatNotSupported(object[] supportedFormats);
        public string ContentTypeNotSupported(object format);
        public string MaxSizeOverflowMB(int maxSizeBytesCount);
        public string FileNameMaxLength(int maxLegth);
    }
}
