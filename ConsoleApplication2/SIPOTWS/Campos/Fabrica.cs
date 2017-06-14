namespace ConsoleApplication2.SIPOTWS.Campos
{
    public static class Fabrica
    {
        public static Campo FabricarPorTipo(int idTipoCampo)
        {
            Campo campo;

            switch (idTipoCampo)
            {
                case 1:
                    campo = new TextoCorto();
                    break;

                case 2:
                    campo = new TextoLargo();
                    break;

                case 3:
                    campo = new Numero();
                    break;

                case 4:
                    campo = new Fecha();
                    break;

                case 5:
                    campo = new Hora();
                    break;

                case 6:
                    campo = new Moneda();
                    break;

                case 7:
                    campo = new PaginaWeb();
                    break;

                case 8:
                    campo = new Archivo();
                    break;

                case 9:
                    campo = new Catalogo();
                    break;

                case 10:
                    campo = new Tabla();
                    break;

                case 11:
                    campo = new Separador();
                    break;

                case 12:
                    campo = new Anio();
                    break;

                case 13:
                    campo = new FechaActualizacion();
                    break;

                case 14:
                    campo = new Nota();
                    break;

                //case 0:
                default:
                    campo = new Campo();
                    break;
            }

            return campo;
        }
    }
}
