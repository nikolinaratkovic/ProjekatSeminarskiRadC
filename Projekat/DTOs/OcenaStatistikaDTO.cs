namespace Projekat.DTOs
{
    public class OcenaStatistikaDTO
    {
        public int ID { get; set; }
        public string Naziv { get; set; }
        public int PolozeniIspiti { get; set; }
        public int NepolozeniIspiti { get; set; }
        public int UkupnoIspita => PolozeniIspiti + NepolozeniIspiti;
        public double ProcentaPolozenosti => UkupnoIspita == 0 ? 0 : ((double)PolozeniIspiti / UkupnoIspita) * 100;
    }
}
