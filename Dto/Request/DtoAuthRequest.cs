namespace ApiERP.Dto.Request
{
    public class DtoAuthRequest
    {
        public string Usuario { get; set; }
        public string Password { get; set; }

        public DtoAuthRequest()
        {

        }

        public DtoAuthRequest(string usuario, string password)
        {
            this.Usuario = usuario;
            this.Password = password;
        }
    }
}
