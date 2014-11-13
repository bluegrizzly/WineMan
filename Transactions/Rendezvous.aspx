<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Rendezvous.aspx.cs" Inherits="WineMan.Transactions.Rendezvous" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .auto-style2 {
            width: 111px;
        }
        .auto-style4 {
            width: 15px;
        }
    .auto-style5 {
        width: 100%;
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
                        <table class="auto-style5">
                            <tr>
                                <td>&nbsp;</td>
                                <td align="center">
                                    <asp:Label ID="Label_Date" runat="server" BorderColor="#666666" BorderStyle="Solid" BorderWidth="2px" Font-Size="Larger" Text="Label"></asp:Label>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                        </table>
                        <table class="auto-style5">
                            <tr>
                                <td>
                                    <asp:Table ID="Table_Stations" runat="server" BorderStyle="Solid" BorderWidth="1px" CellPadding="1" CellSpacing="1" EnableViewState="False" GridLines="Both" HorizontalAlign="Center" ViewStateMode="Disabled">
                                    </asp:Table>
                                </td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <asp:Label ID="Label_SelectedHour" runat="server" Text="Label" Visible="False"></asp:Label>
        <asp:Calendar ID="Calendar_RDV" runat="server" OnSelectionChanged="Calendar_RDV_SelectionChanged" OnDayRender="Calendar_RDV_DayRender">
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
