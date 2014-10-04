<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Rendezvous.aspx.cs" Inherits="WineMan.Transactions.Rendezvous" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .auto-style2 {
            width: 111px;
        }
        .auto-style3 {
            height: 25px;
            width: 111px;
        }
        .auto-style4 {
            width: 15px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        <table style="width:100%;">
            <tr>
                <td class="auto-style2">&nbsp;</td>
                <td rowspan="3" align class="auto-style4">
                    &nbsp;</td>
                <td rowspan="3" valign="top">
                    <asp:Panel ID="Panel1" runat="server" BorderStyle="Solid" BorderWidth="1px" Height="400px" ScrollBars="Auto">
                        <br />
                        <asp:Table ID="Table_Stations" runat="server" BorderStyle="Solid" BorderWidth="1px" CellPadding="1" CellSpacing="1" GridLines="Both" HorizontalAlign="Center" EnableViewState="False" ViewStateMode="Disabled">
                        </asp:Table>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <asp:Label ID="Label_SelectedHour" runat="server" Text="Label" Visible="False"></asp:Label>
        <asp:Calendar ID="Calendar_RDV" runat="server">
                        <SelectedDayStyle BackColor="#0099FF" />
                        <TodayDayStyle BackColor="#999999" />
                        <WeekendDayStyle BackColor="#EEEEEE" />
        </asp:Calendar>
                </td>
            </tr>
            <tr>
                <td align="center"><asp:Button ID="Button_Select" runat="server" OnClick="Button_Select_Click" Text="Select" Height="34px" Width="68px" />
        
                </td>
            </tr>
        </table>
    </p>
    </asp:Content>
