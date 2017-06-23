using System;
using System.Collections.Generic;
using System.Management;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace PCInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            PopulateInfo("Win32_Processor", ref dataGrid, false);
        }

        private void PopulateInfo(string key, ref DataGrid dgLst, bool remNull)
        {
            dgLst.ItemsSource = null;
            dgLst.Items.Refresh();
            Dictionary<string, string> keyPairInfo =
                new Dictionary<string, string>();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + key);
            try
            {
                foreach (var managementBaseObject in searcher.Get())
                {
                    var share = (ManagementObject) managementBaseObject;
                    if (share.Properties.Count <= 0)
                    {
                        this.ShowMessageAsync("No Information Available", "No Info");
                        return;
                    }
                    foreach (PropertyData propertyData in share.Properties)
                    {
                        string infoKey = propertyData.Name;
                        string infoValue;
                        if (propertyData.Value != null && propertyData.Value.ToString() != "")
                        {
                            switch (propertyData.Value.GetType().ToString())
                            {
                                case "System.String[]":
                                    string[] str1 = (string[]) propertyData.Value;
                                    string str2 = "";
                                    foreach (string st in str1)
                                        str2 += st + " ";
                                    infoValue = str2;
                                    break;

                                case "System.UInt16[]":
                                    ushort[] shortData = (ushort[]) propertyData.Value;
                                    string str3 = "";
                                    foreach (ushort ushrt in shortData)
                                        str3 += ushrt.ToString() + " ";
                                    infoValue = str3;
                                    break;

                                default:
                                    infoValue = propertyData.Value.ToString();
                                    break;
                            }
                        }
                        else
                        {
                            if (!remNull)
                                infoValue = "No Information available";
                            else
                                continue;
                        }
                        keyPairInfo.Add(infoKey, infoValue);
                    }
                }
                dgLst.ItemsSource = keyPairInfo; //Set the datagrid Item source to the dictionary
            }
            catch (Exception e)
            {
                this.ShowMessageAsync("Information unavailable because of the following error: \n" + e.Message,
                    "Error");
            }

        }
    }
}
