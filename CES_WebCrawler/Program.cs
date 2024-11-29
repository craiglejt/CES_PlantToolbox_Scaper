// See https://aka.ms/new-console-template for more information
using CES_WebCrawler;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Data;
using System.IO;
using System.Runtime.InteropServices.Marshalling;

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
ExcelPackage ep = new ExcelPackage();

string[] intro =
{
    "Welcome to my NCSU CES Plant Toolbox Scraper",
    "Start by making an excel file with a page called \'Plant Page List'",
    "Starting in Cell A1 Make a table with a Column called \'Page\'",
    "In the rows below the \'Page\' column put the urls from the CES plant",
    "toolbox. Enter as many rows as you want, and you can add any other",
    "columns you may want for your own use. This program will create",
    "another page called \'Plant Collection\' containing a table",
    "containing all the details of all the plants you indicated.",
    "Enter the path to the file below.",
    "Include directory, file name, and extension (.xlsx)"
};
foreach (string s in intro) Console.WriteLine(s);
string? uentry = null;
while (string.IsNullOrEmpty(uentry) || string.IsNullOrWhiteSpace(uentry))
{
    Console.Write("Enter File Path:");
    uentry = Console.ReadLine();
    if (uentry == null) Console.WriteLine("Null received. Please enter a file path");
    if (File.Exists(uentry)) Console.WriteLine("Working...");
    else uentry = null;
}
PlantPageCollection ppc = new PlantPageCollection();

Console.WriteLine("Opening plant list file: " + uentry);
ExcelPlantList epl = new ExcelPlantList(ref ep, uentry);
foreach (string ppage in epl.PlantPages)
{
    PlantPage plantpage = PlantPageScraper.Scrape(ppage);
    ppc.Add(plantpage);
}
Console.WriteLine("All Plant Details Loaded!");
Console.WriteLine("Beginning Excel Export...");
DataTable dt = ppc.ToDataTable();
Console.WriteLine("Data Table generated");
if (ep.Workbook.Worksheets["Plant Collection"] != null)
{
    Console.WriteLine("Deleting extant \'Plant Collection\' worksheet");
    ep.Workbook.Worksheets.Delete("Plant Collection");
}
Console.WriteLine("Creating sheet: Plant Collection");
ep.Workbook.Worksheets.Add("Plant Collection");
ExcelWorksheet ews = ep.Workbook.Worksheets["Plant Collection"];
ExcelAddress eab = new ExcelAddress(1, 1, dt.Rows.Count + 2, dt.Columns.Count + 1);
Console.WriteLine("Loading data table into excel sheet");
ews.Cells["A1"].LoadFromDataTable(dt, true);
Console.WriteLine("Converting imported data to table");
ews.Tables.Add(ews.Cells[1, 1, dt.Rows.Count + 1, dt.Columns.Count], "table_plants");
ews.Tables["table_plants"].ShowHeader = true;
ews.Tables["table_plants"].ShowFilter = true;
Console.WriteLine("Saving spreadsheet file...");
ep.SaveAs(new FileInfo(uentry));
Console.WriteLine("File Saved: " + uentry);