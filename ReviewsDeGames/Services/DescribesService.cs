namespace ReviewsDeGames.Services
{
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
    public class DescribesService : IDescribesService
    {
        public virtual string FormatNotSupported(object[] supportedFormats)
            => "Formato não suportado, o arquivo precisa ser de um dos seguintes formatos: \n" + string.Join("; ", supportedFormats);

        public virtual string InvalidCharacters()
            => "O campo contém caracteres inválidos";

        public virtual string InvalidEmail()
            => "Email inválido";

        public virtual string KeyNotFound(object id)
            => $"Entidade não encontrada pelo identificador {id}";

        public virtual string MaxLength(object max)
        => $"Limite de {max} caracteres excedido";

        public virtual string NotEmpty()
        => "Este campo precisa ter algo";

        public virtual string RangeLength(object min, object max)
        => $"Este campo precisa ter entre {min} e {max} caracteres";

        public virtual string RangeValue(object min, object max)
        => $"O valor precisa estar entre {min} e {max}";

        public virtual string UnavaliableEmail(object email)
        => $"O email {email} está indisponível";

        public virtual string UnavaliableUserName(object username)
        => $"O nome de usuário {username} está indisponível";
        public virtual string ContentTypeNotSupported(object format)
        => $"O formato {format} não é suportado";

        public virtual string MaxSizeOverflowMB(int maxSizeBytesCount)
             => $"O arquivo ultrapassou o limite de {maxSizeBytesCount / (1024 * 1024)} MB";

        public string FileNameMaxLength(int maxLegth)
            => "O nome do arquivo precisa ser menor que " + maxLegth;

        public string NotEmptyOrMaxLength(object max)
            => "Precisa conter algo e ter menos que " + max;

        public string EntityNotFound(object name, object id)
            => $"Nenhum {name} com id {id} foi encontrado";
    }
}
