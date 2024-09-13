using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hummpty_Collections.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string BoatSerialNumber { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string BoatMake { get; set; }
        public string BoatModel { get; set; }

        // Change List<Barcode> to ObservableCollection<Barcode>
        public ObservableCollection<Barcode> Barcodes { get; set; }

        public Customer()
        {
            // Initialize the Barcodes collection
            Barcodes = new ObservableCollection<Barcode>();
        }
    }
}