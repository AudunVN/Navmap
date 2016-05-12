using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DAM
{
    public partial class HashcodeWindow : Form
    {
        FLGameData gd;

        public HashcodeWindow(DamDataSet dataStore, FLGameData gd)
        {
            InitializeComponent();
            gameDataTableBindingSource.DataSource = dataStore;
            this.gd = gd;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string filterText = FLUtility.EscapeLikeExpressionString(textBox1.Text);
            if (filterText == "")
            {
                gameDataTableBindingSource.Filter = null;
                return;
            }
            string expr = "(itemInfo LIKE '%" + filterText + "%')";
            expr += " OR (itemKeys LIKE '%" + filterText + "%')";
            expr += " OR (itemGameDataType LIKE '%" + filterText + "%')";
            expr += " OR (itemNickname LIKE '%" + filterText + "%')";
            gameDataTableBindingSource.Filter = expr;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new FLGameData();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                StringBuilder sb = new StringBuilder();

                DamDataSet.GameDataTableRow dataRow = (DamDataSet.GameDataTableRow)((DataRowView)row.DataBoundItem).Row;
                if (dataRow.itemDetailsObject is FLGameData.ShipInfo)
                {
                    FLGameData.ShipInfo info = (FLGameData.ShipInfo)dataRow.itemDetailsObject;

                    sb.AppendFormat("{0}\n", dataRow.itemInfo);
                    sb.AppendFormat("Nickname:\t{0} {1}\n", dataRow.itemNickname, dataRow.itemHash);
                    sb.AppendFormat("Default engine:\t{0} {1}\n", info.defaultEngine, gd.GetItemDescByHash(info.defaultEngine));
                    sb.AppendFormat("Default powergen:\t{0} {1}\n", info.defaultPowerGen, gd.GetItemDescByHash(info.defaultPowerGen));
                    sb.AppendFormat("Default sound:\t{0} {1}\n", info.defaultSound, gd.GetItemDescByHash(info.defaultSound));
                    sb.AppendFormat("Hold size:\t{0}\n", info.holdSize);
                    sb.AppendFormat("Max bats/bots:\t{0}/{1}\n", info.maxBats, info.maxBots);
                    foreach (FLGameData.ShipInfo.Hardpoint hp in info.hardPoints)
                    {
                        if (hp.defaultEquip>0)
                            sb.AppendFormat("{0}:\t{1} {2} ", hp.name.PadRight(10), hp.defaultEquip, gd.GetItemDescByHash(hp.defaultEquip));
                        else
                            sb.AppendFormat("{0}:\t", hp.name.PadRight(10));

                        foreach (string mountType in hp.mountableItemTypes)
                            sb.Append(mountType + " ");
                        sb.Append("\n");
                    }

                    sb.Append("CMP/3DB hardpoints: ");
                    foreach (string hp in info.validHardPointNames)
                    {
                        sb.Append(hp + " ");
                    }
                    sb.Append("\n");
                }
                else if (dataRow.itemDetailsObject is FLGameData.EquipInfo)
                {
                    FLGameData.EquipInfo info = (FLGameData.EquipInfo)dataRow.itemDetailsObject;
                    sb.AppendFormat("{0}\n", dataRow.itemInfo);
                    sb.AppendFormat("Nickname:\t{0} {1}\n", dataRow.itemNickname, dataRow.itemHash);
                    sb.AppendFormat("Mount type:\t{0}\n", info.hpType);
                    sb.AppendFormat("Volume:\t{0}\n", info.volume);
                }
                else
                {
                    sb.Append("No information available");
                }

                new InformationDialog(sb.ToString()).ShowDialog();
            }
        }
    }
}
