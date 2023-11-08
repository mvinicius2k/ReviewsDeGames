namespace ReviewsDeGames.Services
{
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

        public virtual string FileNameMaxLength(int maxLegth)
            => "O nome do arquivo precisa ser menor que " + maxLegth;

        public virtual string NotEmptyOrMaxLength(object max)
            => "Precisa conter algo e ter menos que " + max;

        public virtual string EntityNotFound(object name, object id)
            => $"Nenhum {name} com id {id} foi encontrado";
    }
}
