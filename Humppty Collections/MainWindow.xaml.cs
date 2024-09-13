using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using Hummpty_Collections.Data;
using Hummpty_Collections.Models;
using Microsoft.EntityFrameworkCore;

namespace Hummpty_Collections
{
    public partial class MainWindow : Window
    {
        private AppDbContext _context;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                _context = new AppDbContext();
                LoadCustomers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveCustomerAsPdf(Customer customer)
        {
            if (customer == null)
            {
                MessageBox.Show("Customer information is not available.");
                return;
            }

            // Ensure barcodes are loaded before generating the PDF
            var customerWithBarcodes = _context.Customers
                .Include(c => c.Barcodes)
                .FirstOrDefault(c => c.CustomerId == customer.CustomerId);

            if (customerWithBarcodes == null)
            {
                MessageBox.Show("Customer not found.");
                return;
            }

            PdfDocument document = new PdfDocument();
            document.Info.Title = $"Customer Details - {customerWithBarcodes.Name}";

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 12);

            int yOffset = 20;  // Starting vertical position

            gfx.DrawString($"Customer Name: {customerWithBarcodes.Name}", font, XBrushes.Black,
                new XRect(20, yOffset, page.Width, page.Height),
                XStringFormats.TopLeft);

            yOffset += 30;  // Add space between lines

            gfx.DrawString($"Boat Serial Number: {customerWithBarcodes.BoatSerialNumber}", font, XBrushes.Black,
                new XRect(20, yOffset, page.Width, page.Height),
                XStringFormats.TopLeft);

            yOffset += 30;

            gfx.DrawString($"Email: {customerWithBarcodes.Email}", font, XBrushes.Black,
                new XRect(20, yOffset, page.Width, page.Height),
                XStringFormats.TopLeft);

            yOffset += 30;

            gfx.DrawString($"Phone Number: {customerWithBarcodes.PhoneNumber}", font, XBrushes.Black,
                new XRect(20, yOffset, page.Width, page.Height),
                XStringFormats.TopLeft);

            yOffset += 30;

            gfx.DrawString($"Boat Make: {customer.BoatMake}", font, XBrushes.Black,
                new XRect(20, yOffset, page.Width, page.Height),
                XStringFormats.TopLeft);

            yOffset += 30;

            gfx.DrawString($"Boat Model: {customer.BoatModel}", font, XBrushes.Black,
                new XRect(20, yOffset, page.Width, page.Height),
                XStringFormats.TopLeft);

            yOffset += 40;  // Add extra space before the "Barcodes" section

            gfx.DrawString("Barcodes:", font, XBrushes.Black,
                new XRect(20, yOffset, page.Width, page.Height),
                XStringFormats.TopLeft);

            yOffset += 30;

            if (customerWithBarcodes.Barcodes != null && customerWithBarcodes.Barcodes.Any())
            {
                foreach (var barcode in customerWithBarcodes.Barcodes)
                {
                    gfx.DrawString(barcode.Code, font, XBrushes.Black,
                        new XRect(40, yOffset, page.Width, page.Height),
                        XStringFormats.TopLeft);
                    yOffset += 30;
                }
            }
            else
            {
                gfx.DrawString("No barcodes available.", font, XBrushes.Black,
                    new XRect(40, yOffset, page.Width, page.Height),
                    XStringFormats.TopLeft);
            }

            string directoryPath = @"C:\CustomerPDFs";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = Path.Combine(directoryPath, $"{customerWithBarcodes.Name}_Details.pdf");
            document.Save(filePath);

            MessageBox.Show($"PDF saved successfully at: {filePath}");
        }

        private void LoadCustomers()
        {
            CustomerListView.ItemsSource = _context.Customers.ToList();
        }

        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            var customerWindow = new CustomerDetailsWindow();
            customerWindow.ShowDialog();
            LoadCustomers(); // Refresh the customer list after adding a new customer
        }

        private void SavePdfButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerListView.SelectedItem is Customer selectedCustomer)
            {
                SaveCustomerAsPdf(selectedCustomer);
            }
            else
            {
                MessageBox.Show("Please select a customer first.");
            }
        }

        private void CustomerListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (CustomerListView.SelectedItem is Customer selectedCustomer)
            {
                OpenCustomerDetailsWindow(selectedCustomer);
            }
        }

        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerListView.SelectedItem is Customer selectedCustomer)
            {
                OpenCustomerDetailsWindow(selectedCustomer);
            }
            else
            {
                MessageBox.Show("Please select a customer first.");
            }
        }

        private void OpenCustomerDetailsWindow(Customer selectedCustomer)
        {
            // Open the CustomerDetailsWindow with the selected customer's details
            var customerDetailsWindow = new CustomerDetailsWindow(selectedCustomer);
            customerDetailsWindow.ShowDialog();
        }

        private void CustomerListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CustomerListView.SelectedItem is Customer selectedCustomer)
            {
                // Open the CustomerDetailsWindow
                SelectedCustomerNameTextBox.Text = selectedCustomer.Name;
                var customerDetailsWindow = new CustomerDetailsWindow(selectedCustomer);
                //customerDetailsWindow.ShowDialog();
            }
        }
    }
}
