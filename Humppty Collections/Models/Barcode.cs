namespace Hummpty_Collections.Models
{
    public class Barcode
    {
        public int BarcodeId { get; set; }
        public string Code { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}