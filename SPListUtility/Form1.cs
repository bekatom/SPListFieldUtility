using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.SharePoint;

namespace SPListUtility
{
    public partial class Form1 : Form
    {
        private List<ListFields> listFields = new List<ListFields>();
        private BindingSource bsListFields  =new BindingSource();
        private BindingSource bsItems = new BindingSource();
        private SPListItemCollection items; 

        private SPWeb web; 

        public Form1()
        {
            InitializeComponent();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lbSiteName.Text) || string.IsNullOrEmpty(txtListName.Text))
            {
                MessageBox.Show("Site name or List Name is empty!");
                return;
            }
            Cursor.Current = Cursors.WaitCursor;
            listFields.Clear();
            listFields = GetAllListColumns(txtSiteName.Text.Trim(), txtListName.Text.Trim());
            if (listFields.Count() > 0)
            {
                
                bsListFields.DataSource = listFields;
                grResult.DataSource = bsListFields;
                grResult.Columns["Title"].Width = 170;
                grResult.Columns["InternalName"].Width = 350;

              

            }

            // Get Items 
            DataTable dt = items.GetDataTable();
            if (dt.Rows.Count > 0)
            {
                bsItems.DataSource = dt;
                grItems.DataSource = bsItems;
            }
            // --------------

            Cursor.Current = Cursors.Default;

        }

        public List<ListFields> GetAllListColumns(string _siteName , string _listName)
        {
            List<ListFields> lsF = new List<ListFields>();

            using (SPSite site = new SPSite(_siteName))
            {
                using (web = site.OpenWeb())
                {
                    try
                    {
                        SPList list = web.Lists[_listName];
                        for (int i = 0; i < list.Fields.Count; i++)
                        {
                            ListFields ls = new ListFields();
                            ls.Title = list.Fields[i].Title;
                            ls.InternalName = list.Fields[i].InternalName;
                            lsF.Add(ls);
                        }
                        SPQuery query = new SPQuery();
                        ///TO DO order by Create Date
                        query.Query = "<OrderBy><FieldRef Name=\"Order\" /></OrderBy>";
                        items = list.GetItems(query);


                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.ToString());
                    }
                   

                }

            }

            return lsF;


        }


        /// <summary>
        /// GET list items 
        /// </summary>
        /// <returns></returns>
        public SPListItemCollection GetOrderedListCollectionItems(string _siteName , string _listName)
        {
            SPListItemCollection items = null;

            using (SPSite site = new SPSite(_siteName))
            {
                using (web = site.OpenWeb())
                {
                    SPList list = web.Lists[_listName];
                    SPQuery query = new SPQuery();
                    ///TO DO order by Create Date
                    query.Query = "<OrderBy><FieldRef Name=\"Order\" /></OrderBy>";
                    items = list.GetItems(query);

                }
            }

            return items;

        }


    }
}
