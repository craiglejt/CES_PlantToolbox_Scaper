using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CES_WebCrawler
{
    internal class PlantDetailEntry
    {
        public string AttributeName { get; set; }
        public List<string> AttributeValues { get; set; }
        public PlantDetailEntry()
        {
            AttributeValues = new List<string>();
        }

        public new string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\t\t[" + AttributeName + "]\r\n");
            foreach (string att in AttributeValues)
                sb.Append("\t\t\t" + att + "\r\n");
            return sb.ToString();
        }
    }

    internal class PlantDetailCategory
    {
        public string CategoryName { get; set; }
        public List<PlantDetailEntry> CategoryEntries { get; set; }
        public PlantDetailCategory()
        {
            CategoryEntries = new List<PlantDetailEntry>();
        }

        public new string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\t{" + CategoryName + "}\r\n");
            foreach (PlantDetailEntry entry in CategoryEntries)
                sb.Append($"{entry.ToString()}");
            return sb.ToString();
        }
    }

    internal class PlantPage
    {
        public string PlantName { get; set; }
        public Uri SourceURL { get; set; }
        public List<PlantDetailCategory> PlantDetails { get; set; }
        public PlantPage()
        {
            PlantDetails = new List<PlantDetailCategory>();
        }

        public new string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(PlantName + "\r\n");
            sb.Append("Source URL: " +  SourceURL + "\r\n");
            foreach (PlantDetailCategory pdc in PlantDetails)
                sb.Append(pdc.ToString());
            return sb.ToString();
        }
    }

    internal class PlantPageCollection : IList<PlantPage>
    {
        public List<PlantPage> Pages;

        public int Count => ((ICollection<PlantPage>)Pages).Count;

        public bool IsReadOnly => ((ICollection<PlantPage>)Pages).IsReadOnly;

        public PlantPage this[int index] { get => ((IList<PlantPage>)Pages)[index]; set => ((IList<PlantPage>)Pages)[index] = value; }

        public PlantPageCollection()
        {
            Pages = new List<PlantPage>();
        }

        public DataTable ToDataTable()
        {
            DataTable dt = StartFreshTable();
            dt.BeginLoadData();
            foreach (PlantPage p in Pages)
            {
                FillTableRow(ref dt, p);
            }
            dt.EndLoadData();
            dt.AcceptChanges();
            return dt;
        }

        private DataTable StartFreshTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PlantName", typeof(string));
            dt.Columns.Add("PlantInfoSourceURL", typeof(string));
            foreach (PlantPage p in Pages)
            {
                foreach (PlantDetailCategory pdc in p.PlantDetails)
                {
                    if (!dt.Columns.Contains(pdc.CategoryName)) dt.Columns.Add(pdc.CategoryName);
                    foreach (PlantDetailEntry pde in pdc.CategoryEntries)
                    {
                        if (!dt.Columns.Contains(pde.AttributeName)) dt.Columns.Add(pde.AttributeName);
                    }
                }
            }
            return dt;
        }

        private void FillTableRow(ref DataTable table, PlantPage plantPage)
        {
            DataRow dr = table.NewRow();
            dr["PlantName"] = plantPage.PlantName;
            dr["PlantInfoSourceURL"] = plantPage.SourceURL;
            foreach (PlantDetailCategory pdc in plantPage.PlantDetails)
                foreach (PlantDetailEntry pde in pdc.CategoryEntries)
                    dr[pde.AttributeName] = string.Join(",\r\n", pde.AttributeValues.ToArray());
            table.LoadDataRow(dr.ItemArray, false);
        }

        public int IndexOf(PlantPage item)
        {
            return ((IList<PlantPage>)Pages).IndexOf(item);
        }

        public void Insert(int index, PlantPage item)
        {
            ((IList<PlantPage>)Pages).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<PlantPage>)Pages).RemoveAt(index);
        }

        public void Add(PlantPage item)
        {
            ((ICollection<PlantPage>)Pages).Add(item);
        }

        public void Clear()
        {
            ((ICollection<PlantPage>)Pages).Clear();
        }

        public bool Contains(PlantPage item)
        {
            return ((ICollection<PlantPage>)Pages).Contains(item);
        }

        public void CopyTo(PlantPage[] array, int arrayIndex)
        {
            ((ICollection<PlantPage>)Pages).CopyTo(array, arrayIndex);
        }

        public bool Remove(PlantPage item)
        {
            return ((ICollection<PlantPage>)Pages).Remove(item);
        }

        public IEnumerator<PlantPage> GetEnumerator()
        {
            return ((IEnumerable<PlantPage>)Pages).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Pages).GetEnumerator();
        }
    }
}
