<%@ Page Title="Customers" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Customers.aspx.cs" Inherits="WineMan.Customers.Customers" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
       Customers
    </h2>

    <table id="jQGridDemo">
    </table>
    <div id="jQGridDemoPager">
    </div>
    <script type="text/javascript">
        jQuery("#jQGridDemo").jqGrid({
            url: '<%=ResolveUrl("~/Customers/CustomersHandler.ashx") %>',
            datatype: "json",
            colNames: ['Id', 'First Name', 'Last Name', 'Address', 'Tel', 'Email'],
            colModel: [
                        { name: 'id', index: 'id', width: 20, stype: 'text' },
   		                { name: 'first_name', index: 'first_name', width: 150, stype: 'text', sortable: true, editable: true },
   		                { name: 'last_name', index: 'last_name', width: 150, editable: true },
                        { name: 'address', index: 'address', width: 300, editable: true },
                        { name: 'telephone', index: 'telephone', width: 100, editable: true },
                        { name: 'email', index: 'email', width: 100, editable: true }
        ],
            rowNum: 10,
            height:250,
            mtype: 'GET',
            loadonce: true,
            rowList: [10, 20, 30],
            pager: '#jQGridDemoPager',
            sortname: 'id',
            viewrecords: true,
            sortorder: 'desc',
            caption: "Customers Details",
            editurl: '<%=ResolveUrl("~/Customers/CustomersHandler.ashx") %>'
        });

        $('#jQGridDemo').jqGrid('navGrid', '#jQGridDemoPager',
                   {
                       edit: true,
                       add: true,
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
                               var value = $('#jQGridDemo').jqGrid('getCell', sel_id, 'id');
                               return value;
                           }
                       }
                   },
                   {//ADD portion
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


</asp:Content>
