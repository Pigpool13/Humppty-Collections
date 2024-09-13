using System.Linq;
using System.Windows;
using Hummpty_Collections.Data;
using Hummpty_Collections.Models;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;


namespace Hummpty_Collections
{
    public partial class CustomerDetailsWindow : Window
    {
        private int _customerId;
        private AppDbContext _context;
        private Customer _customer;

        public CustomerDetailsWindow(Customer customer = null)
        {
            InitializeComponent();
            _context = new AppDbContext();
            _customer = customer ?? new Customer();
            DataContext = _customer;

            if (_customer.CustomerId != 0)
            {
                // Ensure barcodes are loaded along with the customer data
                _customer = _context.Customers.Include(c => c.Barcodes)
                              .FirstOrDefault(c => c.CustomerId == _customer.CustomerId);

                LoadCustomerData();
                BarcodeListBox.ItemsSource = _customer.Barcodes;
            }
        }

        private void LoadCustomerData()
        {
            if (_customer != null)
            {
                NameTextBox.Text = _customer.Name;
                BoatSerialNumberTextBox.Text = _customer.BoatSerialNumber;
                EmailTextBox.Text = _customer.Email;
                PhoneTextBox.Text = _customer.PhoneNumber;
                BarcodeListBox.ItemsSource = _customer.Barcodes.Select(b => b.Code).ToList();
            }
        }

        private void SaveCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            _customer.Name = NameTextBox.Text;
            _customer.BoatSerialNumber = BoatSerialNumberTextBox.Text;
            _customer.Email = EmailTextBox.Text;
            _customer.PhoneNumber = PhoneTextBox.Text;
            _customer.BoatMake = BoatMakeTextBox.Text;
            _customer.BoatModel = BoatModelTextBox.Text;

            if (_customer.CustomerId == 0)
            {
                _context.Customers.Add(_customer);
            }

            _context.SaveChanges();
            MessageBox.Show("Customer saved successfully.");
            Close();
        }

        private void BarcodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string barcodeText = BarcodeTextBox.Text;

                if (!string.IsNullOrWhiteSpace(barcodeText))
                {
                    if (_customer.Barcodes == null)
                    {
                        _customer.Barcodes = new ObservableCollection<Barcode>();
                    }

                    var barcode = new Barcode
                    {
                        Code = barcodeText,
                        Customer = _customer // Ensure correct association
                    };

                    _customer.Barcodes.Add(barcode);

                    // Save changes to the database
                    _context.SaveChanges();

                    // Refresh the ListBox
                    BarcodeListBox.ItemsSource = _customer.Barcodes;

                    BarcodeTextBox.Clear();
                }
            }
        }

        private void LoadCustomer(int customerId)
        {
            _customer = _context.Customers
                .Include(c => c.Barcodes)
                .FirstOrDefault(c => c.CustomerId == customerId);

            if (_customer != null)
            {
                // Set the data context for binding
                DataContext = _customer;
                // Refresh BarcodeListBox
                BarcodeListBox.ItemsSource = _customer.Barcodes;
            }
        }

        private void ProcessScannedBarcode(string barcodeText)
        {
            // Ensure that the barcodeText is not null or empty
            if (string.IsNullOrWhiteSpace(barcodeText))
            {
                MessageBox.Show("Invalid barcode.");
                return;
            }

            // Create a new Barcode object
            var barcode = new Barcode
            {
                Code = barcodeText,
                Customer = _customer,  // Associate with the current customer (if needed)
                CustomerId = _customer.CustomerId // Make sure the CustomerId is set if relevant
            };

            // Add the barcode to the ListBox (UI display)
            BarcodeListBox.Items.Add(barcodeText);

            // Add the barcode to the customer's barcode collection (if it exists)
            if (_customer.Barcodes == null)
            {
                _customer.Barcodes = new ObservableCollection<Barcode>();
            }

            var newBarcode = new Barcode
            {
                Code = barcodeText,
                Customer = _customer
            };

            _customer.Barcodes.Add(newBarcode);
            _context.Barcodes.Add(newBarcode);

            _customer.Barcodes.Add(barcode);

            // Add the barcode to the database context
            _context.Barcodes.Add(barcode);

            // Save the changes to the database
            try
            {
                _context.SaveChanges();
                MessageBox.Show("Barcode saved successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving barcode: {ex.Message}");
            }

            // Clear the input field or perform any other additional processing here
        }
    }
}

