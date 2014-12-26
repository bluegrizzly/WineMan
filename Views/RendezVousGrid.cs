using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WineMan.Transactions;

namespace WineMan
{
    //public class RendezVousGrid
    //{
    //    Settings m_Settings = new Settings();

    //    public void Init(RendezVous rdvPage)
    //    {
    //        m_RendezVousPage = rdvPage;
    //    }

    //    public void AddTableRow(int hour, int min)
    //    {
    //        RendezVousTableRow row = new RendezVousTableRow();
    //        row.Row = new TableRow();

    //        m_RendezVousPage.m_TableRows.Add(RendezVousTableRow.GetRowKey(hour, min), row);

    //        for (int station = 0; station < m_Settings.NbStations; ++station)
    //        {
    //            RendezVousData data = new RendezVousData();
    //            data.Hour = hour;
    //            data.Min = min;
    //            data.Station = station;
    //            data.RadioButton = new RadioButton();
    //            data.RadioButton.GroupName = "spot";
    //            data.RadioButton.PreRender += new EventHandler(this.RadioButton_PreRender);
    //            data.RadioButton.AutoPostBack = true;
    //            if (m_Hour == RendezVousTableRow.GetRowKey(hour, min) && m_Station == station)
    //            {
    //                // Need to select the line that was passed from the add transaction
    //                data.RadioButton.Checked = true;
    //            }

    //            row.RdvData.Add(station, data);
    //        }
    //    }
    //}
}