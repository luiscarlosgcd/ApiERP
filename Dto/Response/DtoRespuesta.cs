namespace ApiERP.Dto.Response
{
    public class DtoRespuesta
    {
        public int Exito { get; set; }
        public string Mensaje { get; set; }
        public object Data { get; set; }

        public DtoRespuesta()
        {
            this.Exito = 0;
        }
    }
}
