using OfficeOpenXml;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CES_WebCrawler
{
    internal class PlantList
    {
        public List<string> PlantPages { get; private set; }
        public PlantList(string listFilePath)
        {
            PlantPages = new List<string>();
            StreamReader fs = File.OpenText(listFilePath);
            while (!fs.EndOfStream)
            {
                string? pname = fs.ReadLine();
                if (pname != null)
                {
                    PlantPages.Add(pname);
                    Console.WriteLine(pname);
                }
                
            }
        }
    }

    internal class ExcelPlantList
    {
        public List<string> PlantPages { get; private set; }

        ExcelPackage ep;
        public ExcelPlantList(ref ExcelPackage excelPackage, string listFilePath)
        {
            ep = excelPackage;
            ep.Load(new FileStream(listFilePath, FileMode.Open, FileAccess.ReadWrite));
            PlantPages = new List<string>();
            ExcelWorksheet ews = ep.Workbook.Worksheets["Plant Page List"];
            ExcelRange er = ews.Cells[ews.FirstValueCell.Start.Row, ews.FirstValueCell.Start.Column, ews.LastValueCell.End.Row, ews.LastValueCell.End.Column];
            if (er.IsTable)
            {
                DataTable dt = er.ToDataTable();
                if (dt.Columns.Contains("Page"))
                    foreach (DataRow dr in dt.Rows)
                        PlantPages.Add((string)dr["Page"]);
                else throw new Exception("Plant Page List page table must have a column named \'Page\'");
            }
            else throw new Exception("Could not find table at top left of page \'Plant Page List\'");
        }
    }
}
