using ReviewsDeGames.Helpers;

namespace ReviewsDeGames.Services
{
    public interface IHostImageService
    {
        /// <summary>
        /// Escreve no banco de dados a imagem. 
        /// Caso já exista uma com o mesmo nome e endereço, ela será sobrescrita. 
        /// Para evitar isso, crie um nome único com Guid, com a opção de comprimir para base64 <see cref="Extensions.ToSafeBase64(Guid)"/> ou use <see cref="UniqueName(string)"/>
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public Task<string> Write(byte[] image, string filename, string userId, DateOnly date);
        /// <summary>
        /// Transforma <paramref name="filename"/> em um nome único
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public string UniqueName(string filename);
        public void Delete(string filename, string userId, DateOnly date);

    }
}
