namespace ReviewsDeGames.Helpers
{
    public class Messages
    {
        private readonly static SortedDictionary<Message, string> messages = new SortedDictionary<Message, string>
        {
            { Message.KeyNotFound, "Entidade não encontrada pelo identificador {0}"},
            { Message.InvalidCharacters, "O campo contém caracteres inválidos"},
            { Message.RangeValue, "O valor precisa estar entre {0} e {1}"},
            { Message.MaxLenght, "Limite de caracteres excedido, precisa ser menor que {0}"},
            { Message.NotEmpty, "Este campo não pode ser vazio"},
            { Message.RangeLength, "Este campo precisa ter entre {0} e {1} caracteres"},
            { Message.InvalidEntry, "Entrada inválida"},
            { Message.InvalidEmail, "Email inválido"},
            { Message.UnavaliableUserName, "O nome de usuário {0} não está disponível"},
        };

        public static string Get(Message key, params object[] args)
        {
            try
            {
                if (messages.ContainsKey(key))
                    return string.Format(messages[key], args);
                else
                    return key.ToString();

            }
            catch (FormatException)
            {
                return key.ToString();
                
            }
        }

    }
    public enum Message
    {
        KeyNotFound, InvalidCharacters, RangeValue, MaxLenght, NotEmpty, RangeLength, InvalidEntry, InvalidEmail, UnavaliableUserName,
    }
}
