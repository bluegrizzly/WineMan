<%@ Page Title="Transactions" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Transactions.aspx.cs" Inherits="WineMan.Transactions.Transactions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <style type="text/css">
        .auto-style3 {
            width: 116px;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
       Transactions
    </h2>

    <table style="width:100%;">
    <tr>
        <td class="auto-style3" width="120"> 
            <div id="div1"></div>
            <script>
                $('#div1').load('/Transactions/LeftMenu.aspx');
            </script>
        </td>
        <td>
            <table id="jQGridDemo">
            </table>
            <div id="jQGridDemoPager">
            </div>
            <script type="text/javascript">
                jQuery("#jQGridDemo").jqGrid({
                    url: '<%=ResolveUrl("~/Transactions/TransactionHandler.ashx") %>',
                    datatype: "json",
                    colNames: ['ID', 'Client ID', 'Brand', 'Type', 'Category', 'Creation Date', 'Bottling Date', 'Station', 'Done'],
                    colModel: [
                                { name: 'id', index: 'id', width: 30, stype: 'text' },
   		                        { name: 'client_id', index: 'client_id', width: 40, stype: 'text', sortable: true, editable: true, sorttype: 'int' },
   		                        { name: 'wine_brand_id', index: 'wine_brand_id', width: 40, stype: 'text', sortable: true, editable: true },
   		                        { name: 'wine_type_id', index: 'wine_type_id', width: 40, stype: 'text', sortable: true, editable: true },
   		                        { name: 'wine_category_id', index: 'wine_category_id', width: 40, stype: 'text', sortable: true, editable: true},
   		                        { name: 'date_creation', index: 'date_creation', width: 160, sortable: true, editable: true },
                                { name: 'date_bottling', index: 'date_bottling', width: 160, sortable: true, editable: true },
                                { name: 'bottling_station', index: 'bottling_station', width: 40, sortable: true, editable: true },
                                { name: 'done', index: 'done', width: 30, sortable: true, editable: true }
                ],
                    rowNum: 10,
                    height: 250,
                    mtype: 'GET',
                    loadonce: true,
                    rowList: [10, 20, 50, 100],
                    pager: '#jQGridDemoPager',
                    sortname: 'id',
                    viewrecords: true,
                    sortorder: 'desc',
                    caption: "Transactions Details",
                    editurl: '<%=ResolveUrl("~/Transactions/TransactionHandler.ashx") %>',

                    onSelectRow: function (ids) {
                        url: '<%=ResolveUrl("~/Transactions/TransactionHandler.ashx") %>'
                        }
                });

                $('#jQGridDemo').jqGrid('navGrid', '#jQGridDemoPager',
                           {
                               edit: true,
                               add: false,
                               del: true,
                               search: true,
                               searchtext: "Search",
                               addtext: "Add",
                               edittext: "Edit",
                               deltext: "Delete"
                           },
                           {   //EDIT
                               //                       height: 300,
                               //                       width: 400,
                               //                       top: 50,
                               //                       left: 100,
                               //                       dataheight: 280,
                               closeOnEscape: true,//Closes the popup on pressing escape key
                               reloadAfterSubmit: true,
                               drag: true,
                               afterSubmit: function (response, postdata) {
                                   if (response.responseText == "") {

                                       $(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');//Reloads the grid after edit
                                       return [true, '']
                                   }
                                   else {
                                       $(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid'); //Reloads the grid after edit
                                       return [false, response.responseText]//Captures and displays the response text on th Edit window
                                   }
                               },
                               editData: {
                                   EmpId: function () {
                                       var sel_id = $('#jQGridDemo').jqGrid('getGridParam', 'selrow');
                                       var value = $('#jQGridDemo').jqGrid('getCell', sel_id, 'Id');
                                       return value;
                                   }
                               }
                           },
                           {
                               closeAfterAdd: true,//Closes the add window after add
                               afterSubmit: function (response, postdata) {
                                   if (response.responseText == "") {

                                       $(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid')//Reloads the grid after Add
                                       return [true, '']
                                   }
                                   else {
                                       $(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid')//Reloads the grid after Add
                                       return [false, response.responseText]
                                   }
                               }
                           },
                           {   //DELETE
                               closeOnEscape: true,
                               closeAfterDelete: true,
                               reloadAfterSubmit: true,
                               closeOnEscape: true,
                               drag: true,
                               afterSubmit: function (response, postdata) {
                                   if (response.responseText == "") {

                                       $("#jQGridDemo").trigger("reloadGrid", [{ current: true }]);
                                       return [false, response.responseText]
                                   }
                                   else {
                                       $(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid')
                                       return [true, response.responseText]
                                   }
                               },
                               delData: {
                                   EmpId: function () {
                                       var sel_id = $('#jQGridDemo').jqGrid('getGridParam', 'selrow');
                                       var value = $('#jQGridDemo').jqGrid('getCell', sel_id, 'id');
                                       return value;
                                   }
                               }
                           },
                           {//SEARCH
                               closeOnEscape: true

                           }
                    );

            </script>
        </td>
    </tr>
    </table>

</asp:Content>
