<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddTransaction.aspx.cs" Inherits="WineMan.Transactions.AddTransaction" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .auto-style1 {
            width: 89px;
            height: 21px;
        }
        .auto-style4 {
            height: 4px;
        }
        .auto-style9 {
            width: 69px;
        }
        .auto-style10 {
            width: 774px;
        }
        .auto-style11 {
            width: 89px;
        }
        .auto-style12 {
            height: 13px;
        }
        .auto-style13 {
            width: 31px;
            height: 21px;
        }
        .auto-style14 {
            width: 31px;
        }
    </style>

<link rel="stylesheet" href="//code.jquery.com/ui/1.11.1/themes/smoothness/jquery-ui.css">
<script src="//code.jquery.com/jquery-1.10.2.js"></script>
<script src="//code.jquery.com/ui/1.11.1/jquery-ui.js"></script>

<script language="javascript" type="text/javascript">
    $(function () {
        $('#<%=txtLastName.ClientID%>').autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '<%=ResolveClientUrl("~/Transactions/AddTransaction.aspx/GetAutoCompleteData")%>',
                    data: "{ 'pre':'" + request.term + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        response($.map(data.d, function (item) {
                            return { value: item }
                        }))
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert(textStatus);
                    }
                });
            },
            select: function (event, ui) {
                $.ajax({
                    type: 'POST',
                    url: '<%=ResolveClientUrl("~/Transactions/AddTransaction.aspx/GetAutoCompleteDataDone")%>',
                    data: '{name: "' + $("#<%=txtLastName.ClientID%>")[0].value + '" }',
                    contentType: "application/json; charset=utf-8",
                    dataType: 'json'
                });
            }
        });
    });
</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table style="width:100%;">
    <tr>
        <td class="auto-style1" > 
            <asp:Panel ID="Panel1" runat="server" BorderWidth="2"  width="200" Height="300" >
                <h3>1. Choose Customer</h3>
                <fieldset>
                <div class="ui-widget" style="text-align:left">
                      <asp:TextBox ID="txtLastName" runat="server" Width="150px" CssClass="textboxAuto"  Font-Size="12px" OnTextChanged="txtLastName_TextChanged" BackColor="#E1E8F0" AutoPostBack="True" />
                </div>    
                </fieldset>
                <table width="100%">
                    <tr>
                        <td class="auto-style9" align="right">ID:</td>
                        <td>
                            <asp:Label ID="Label_ID" runat="server" Text="none"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style9" align="right">First Name:</td>
                        <td>
                            <asp:Label ID="Label_FirstName" runat="server" Text="none"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style9" align="right">Last Name:</td>
                        <td>
                            <asp:Label ID="Label_LastName" runat="server" Text="none"></asp:Label>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
        <td  > 


        <asp:Panel ID="Panel2" runat="server" BorderWidth="2" Width="400" Height="300">
            
            <table style="width:100%;" cellspacing="3">
            <tr>
                <td align="left"><h3>&nbsp;2. Choose Wine</h3></td>

                <td>
                    &nbsp;</td>
            </tr>
                <tr>
                    <td align="right">Brand :&nbsp;</td>
                    <td>
                        <asp:DropDownList ID="DropDownList_Brand" runat="server" OnSelectedIndexChanged="DropDownList_Brand_SelectedIndexChanged" BackColor="#E1E8F0" AutoPostBack="True">
                        </asp:DropDownList>
                    </td>
                </tr>
            <tr>
                <td align="right">Type :&nbsp;</td>
                <td>
                    <asp:DropDownList ID="DropDownList_Type" runat="server" AutoPostBack="True" BackColor="#E1E8F0" OnSelectedIndexChanged="DropDownList_Type_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td align="right">Category :&nbsp;</td>
                <td>
                    <asp:DropDownList ID="DropDownList_Category" runat="server" AutoPostBack="True" BackColor="#E1E8F0" OnSelectedIndexChanged="DropDownList_Category_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
            </tr>
                <tr>
                    <td class="auto-style4" colspan="2" style="background-color: #000080"></td>
                </tr>
                <tr>
                    <td align="right">Price :</td>
                    <td>
                        <asp:Label ID="Label_Price" runat="server" Text="0"></asp:Label>
                </td>
            </tr>

            </table>
            </asp:Panel>
        </td>
        <td class="auto-style14"  > 


            <asp:Panel ID="Panel15" runat="server" BorderWidth="2" Width="315" Height="300" HorizontalAlign="Left">
                <table>
                <tr>
                    <td align="left"><h3>&nbsp;3. Rendez-vous</h3></td>
                </tr>
                <tr>
                    <td>
                    <asp:Calendar ID="Calendar_RDV" runat="server" BorderStyle="Solid" BorderWidth="1px" ShowGridLines="True"></asp:Calendar>
                    </td>

                </tr>
                    <tr>
                        <td valign="middle">
                            Bottling :
                    <asp:TextBox ID="TextBox_RDV" runat="server" TextMode="DateTime"></asp:TextBox>
                            </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td class="auto-style11">
            <asp:Panel ID="Panel4" runat="server" BorderWidth="2"  width="200" Height="100" >
                <table>
                    <tr>
                        <td>
                            <h3> 4. Complete</h3>
                            <p>
                                Transaction ID:
                                <asp:Label ID="Label_TransactionID" runat="server" Text="Label" Name="TxID" DefaultValue="0"></asp:Label>

                                <br />
                                <asp:Button ID="Button_Commit" runat="server" Text="Create New" OnClick="Button_Commit_Click" CausesValidation="False" UseSubmitBehavior="False" />

                            </p>
                        </td>
                    </tr>
                </table>
            </asp:Panel>



        </td>
        <td class="auto-style13" colspan="2" > 

            <asp:Panel ID="Panel3" runat="server" BorderWidth="2" Width="725" Height="100" CssClass="Panal">
                <table style="width:100%; height:100%" cellspacing="0">
                    <tr >
                        <td colspan="0" valign="top" class="auto-style12" rowspan="0"><h3>Dates</h3></td>
                    </tr>
                    <tr>
                        <td class="auto-style10" valign="top">
                            <asp:GridView ID="GridView1" runat="server" DataSourceID="SqlDataSource4" Font-Size="Smaller">
                            </asp:GridView>
                            <asp:SqlDataSource ID="SqlDataSource4" runat="server" ConnectionString="<%$ ConnectionStrings:winemanConnectionString %>" ProviderName="<%$ ConnectionStrings:winemanConnectionString.ProviderName %>" 
                                SelectCommand="SELECT levure, transfert_1, transfert_2, transfert_3, degazage, clarification, aromate, chene, frigo_debut, frigo_fin, filtration, embouteillage FROM steps WHERE transaction_id = @TxID">
                                <selectparameters>
                                      <asp:parameter name="TxID"/>
                                </selectparameters>
                            </asp:SqlDataSource>
                        </td>
                    </tr>
                </table>
                
            </asp:Panel>
        </td>
    </tr>
    </table>

</asp:Content>
