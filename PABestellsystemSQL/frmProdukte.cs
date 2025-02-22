﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PABestellsystemSQL
{
    public partial class frmProdukte : Form
    {
        public frmProdukte()
        {
            InitializeComponent();
            UpdateBindingDSource();
            chkbProduktnr.Checked = true;
            dgvProdukte.Columns["pfad"].Visible = false;
            dgvProdukte.Columns["image"].Visible = false;
            GetDataCBox();
        }

        private DataTable dt = null;
        private DataView dv = null;
        private DataRowView drv = null;

        private void UpdateBindingDSource()
        {
            string tableName = "produkte";
            dt = DataAccess.GetAllManagementDB(tableName);
            dgvProdukte.DataSource = dt;
            dv = new DataView(dt);
            dgvProdukte.AutoResizeColumns();
            dgvProdukte.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void GetDataCBox()
        {
            SqlDataAdapter sqlDa = new SqlDataAdapter();
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            string cmdStr = "";
            string kat = "";

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(Helper.CnnVal("managementDB")))
                {
                    sqlConn.Open();
                    cmdStr = "select kategorie from produkte";
                    cmd = new SqlCommand(cmdStr, sqlConn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        kat = reader["kategorie"].ToString();
                        if(!cmbKategorie.Items.Contains(kat))
                            cmbKategorie.Items.Add(kat);
                    }
                    cmbKategorie.SelectedItem = cmbKategorie.Items[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void frmProdukte_Load(object sender, EventArgs e)
        {
            UpdateBindingDSource();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            drv = dv.AddNew();
            frmProduktBearbeiten frmPB = new frmProduktBearbeiten();
            frmPB.EditProdukt(drv);
            frmPB.Dispose();
            UpdateBindingDSource();
        }

        private void btnAktualisieren_Click(object sender, EventArgs e)
        {
            drv = dv[dgvProdukte.CurrentRow.Index];
            frmProduktBearbeiten frmPB = new frmProduktBearbeiten();
            frmPB.EditProdukt(drv);
            frmPB.Dispose();
            UpdateBindingDSource();
        }

        private void btnLöschen_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(Helper.CnnVal("managementDB")))
                {
                    sqlConn.Open();
                    string sqlCmd = "delete from produkte where produktnr = '" + dgvProdukte.CurrentRow.Cells["produktnr"].Value.ToString() + "'";
                    SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd, sqlConn);
                    DataTable dt = new DataTable();
                    sqlDa.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            UpdateBindingDSource();
            dgvProdukte.Refresh();
        }

        private void frmProdukte_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SqlDataAdapter sqlDa = new SqlDataAdapter();
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            string cmdStr = "";

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(Helper.CnnVal("managementDB")))
                {
                    sqlConn.Open();

                    if (chkbProduktnr.Checked)
                    {
                        cmdStr = "select produktnr, produktname, preis, beschreibung, hersteller, " +
                            "kategorie, stueckzahl from produkte where produktnr = @produktnr";
                        cmd = new SqlCommand(cmdStr, sqlConn);
                        cmd.Parameters.AddWithValue("@produktnr", txtProduktnr.Text);
                    }
                    else if (chkbProduktname.Checked)
                    {
                        cmdStr = "select produktnr, produktname, preis, beschreibung, hersteller, " +
                            "kategorie, stueckzahl from produkte where produktname like @produktname";
                        cmd = new SqlCommand(cmdStr, sqlConn);
                        cmd.Parameters.AddWithValue("@produktname", txtProduktname.Text + "%");
                    }
                    else if (chkbKategorie.Checked)
                    {
                        cmdStr = "select produktnr, produktname, preis, beschreibung, hersteller, " +
                            "kategorie, stueckzahl from produkte where kategorie = @kategorie";
                        cmd = new SqlCommand(cmdStr, sqlConn);
                        cmd.Parameters.AddWithValue("@kategorie", cmbKategorie.SelectedItem.ToString());
                    }
                    else
                    {
                        cmdStr = "select * from produkte";
                        cmd = new SqlCommand(cmdStr, sqlConn);
                    }
                    //cmd.CommandType = CommandType.StoredProcedure;
                    sqlDa.SelectCommand = cmd;
                    dt = new DataTable();
                    sqlDa.Fill(dt);
                    dgvProdukte.DataSource = dt;
                    dgvProdukte.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void chkbProduktnr_CheckedChanged(object sender, EventArgs e)
        {
            if (chkbProduktnr.Checked)
            {
                chkbProduktname.Checked = false;
                chkbKategorie.Checked = false;
            }
        }

        private void chkbProduktname_CheckedChanged(object sender, EventArgs e)
        {
            if (chkbProduktname.Checked)
            {
                chkbProduktnr.Checked = false;
                chkbKategorie.Checked = false;
            }
        }

        private void chkbKategorie_CheckedChanged(object sender, EventArgs e)
        {
            if (chkbKategorie.Checked)
            {
                chkbProduktnr.Checked = false;
                chkbProduktname.Checked = false;
            }
        }

        private void btnZurück_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmHauptmenue fH = new frmHauptmenue();
            fH.Show();
        }
    }
}
