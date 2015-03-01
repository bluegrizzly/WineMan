﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WorkToCompleteTx.aspx.cs" Inherits="WineMan.Work.WorkToCompleteTx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 149px;
        }
        #setToDone {
            width: 135px;
        }
        #selectAll {
            width: 135px;
        }
        #clear {
            width: 135px;
        }
        .auto-style3 {
            width: 149px;
            height: 129px;
        }
        #print {
            width: 135px;
        }
        .auto-style4 {
            width: 149px;
            height: 43px;
        }
        #editRow {
            width: 135px;
        }
    </style>

  <script src="//code.jquery.com/ui/1.11.1/jquery-ui.js"></script>

  <script>
      $(function () {
          $("#<%= txtDateStart.ClientID %>").datepicker({
              autoclose: true,
              dateFormat: "yy-mm-dd"
          });
      });
      $(function () {
          $("#<%= txtDateEnd.ClientID %>").datepicker({
              autoclose: true,
              dateFormat: "yy-mm-dd"
          });
      });
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
            <table class="auto-style1" cellspacing="3">
                <tr>
                    <td class="auto-style4" valign="top">
                        <h2>Transactions to complete </h2>
                    </td>
                    <td rowspan="3" valign="top" align="center">

                        <asp:Panel id="pnlContents" runat = "server">
                        <fieldset>
                            &nbsp;
                            <asp:CheckBox ID="CheckBox_ShowReady" runat="server" Text="Show Ready Only (all steps done)" AutoPostBack="True" />
                            <asp:CheckBox ID="CheckBox_ShowDone" runat="server" Text="Show Transactions Done" AutoPostBack="True" />

                        </fieldset>
                            <table id="jQGridDemo"></table>
                        </asp:Panel>
                        <div id="jQGridDemoPager">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3" valign="top">
                        <fieldset>
                            Date Start:
                            <asp:TextBox ID="txtDateStart" runat="server" AutoPostBack="True" Width="97px"></asp:TextBox>
                            <asp:Button ID="Button_ClearStart" runat="server" Height="19px" OnClick="Button_ClearStart_Click" Text="X" Width="16px" Font-Bold="False" Font-Size="X-Small" />
                            <br />
                            <br />Date End :
                            <asp:TextBox ID="txtDateEnd" runat="server" AutoPostBack="True" Width="97px"></asp:TextBox>
                            <asp:Button ID="Button_ClearEnd" runat="server" Height="19px" Text="X" Width="16px" Font-Size="X-Small" OnClick="Button_ClearEnd_Click" />
                        </fieldset></td>
                </tr>
                <tr>
                    <td class="auto-style2" align="center" valign="top">
                        &nbsp;<fieldset>

                            <input id="setToDone" type="button" value="Set to Done"/><br />
                            <input id="selectAll" type="button" value="Select All"/><br />
                            <input id="clear" type="button" value="Clear Selection"/><br />
                            <br />&nbsp;&nbsp;<br />
                            <input id="editRow" type="button" value="Edit tx"/> <br />
                        </fieldset></td>
                </tr>
            </table>
    
    <script type="text/javascript">
        var grid = $("#jQGridDemo");

        grid.jqGrid({
            url: '<%=ResolveUrl("~/Work/WorkToCompleteHandler.ashx?date=") %>' +
                document.getElementById('<%= txtDateStart.ClientID %>').value +
                "&dateend=" + document.getElementById('<%= txtDateEnd.ClientID %>').value +
                "&showdone=" + document.getElementById('<%= CheckBox_ShowDone.ClientID %>').checked +
                "&showreadyonly=" + document.getElementById('<%= CheckBox_ShowReady.ClientID %>').checked +
                "&showtx=true",
            datatype: "json",
            colNames: ['ID', 'Customer', 'Brand', 'Type', 'Category', 'Creation Date', 'Steps Status', 'Done'],
            colModel: [
                        { name: 'id', index: 'id', width: 50, stype: 'text', sortable: true, sorttype: 'int' },
                        { name: 'client_id', index: 'client_id', width: 140, sortable: true },
                        { name: 'wine_brand_id', index: 'wine_brand_id', width: 100, stype: 'text', sortable: true },
                        { name: 'wine_type_id', index: 'wine_type_id', width: 100, stype: 'text', sortable: true },
                        { name: 'wine_category_id', index: 'wine_category_id', width: 70, stype: 'text', sortable: true },
                        {
                            name: 'date_creation', index: 'date_creation', width: 80, stype: 'text', sortable: true,
                            formatter: 'date',
                            formatoptions: {
                                srcformat: 'm/d/Y h:i:s A',
                                newformat: 'Y-M-d',
                                defaultValue: null
                            },
                        },
                        { name: 'steps_done', index: 'steps_done', width: 100, stype: 'text', sortable: true, editable:false },
                        {
                            name: 'done', width: 30, index: 'done',
                            align: 'center',
                            editable: true,
                            edittype: 'checkbox', editoptions: { value: "1:0", defaultValue: "1" },
                            formatter: "checkbox", formatoptions: { disabled: true }
                        }
            ],
            rowNum: 50,
            multiselect: true,
            height: 250,
            mtype: 'GET',
            loadonce: true,
            ignoreCase: true,
            rowList: [50, 200, 500],
            pager: '#jQGridDemoPager',
            sortname: 'id',
            viewrecords: true,
            sortorder: 'desc',
            caption: "Transactions",
            editurl: '<%=ResolveUrl("~/Transactions/TransactionHandler.ashx") %>',

            onSelectRow: function (id) {
                // Check if we are selecting a row that is already done 
                // in such case we allow reverting to not done instead.
                var ids = grid.jqGrid('getGridParam', 'selarrrow');
                var reset = true;
                if (ids.length == 1) {
                    var isDone = grid.getCell(id, "done");
                    if (isDone == "1") {
                        document.getElementById("setToDone").value = "Undo";
                        reset = false;
                    }
                }
                if (reset) {
                    document.getElementById("setToDone").value = "Set to Done";
                }
            },
            gridComplete: function () {
                var rows = grid.getDataIDs();
                for (var i = 0; i < rows.length; i++) {
                    var status = grid.getCell(rows[i], "steps_done");
                    var isDone = grid.getCell(rows[i], "done");
                    if (isDone == "1") { grid.jqGrid('setRowData', rows[i], false, { color: 'black', weightfont: 'bold', background: 'gray' }); }
                    else if (status.charAt(0) == "0") { grid.jqGrid('setRowData', rows[i], false, { color: 'white', weightfont: 'bold', background: '#FF0000' }); }
                    else if (status.charAt(0) == "1") { grid.jqGrid('setRowData', rows[i], false, { color: 'white', weightfont: 'bold', background: '#FF3300' }); }
                    else if (status.charAt(0) == "2") { grid.jqGrid('setRowData', rows[i], false, { color: 'white', weightfont: 'bold', background: '#ff6600' }); }
                    else if (status.charAt(0) == "3") { grid.jqGrid('setRowData', rows[i], false, { color: 'black', weightfont: 'bold', background: '#ff9900' }); }
                    else if (status.charAt(0) == "4") { grid.jqGrid('setRowData', rows[i], false, { color: 'black', weightfont: 'bold', background: '#FFCC00' }); }
                    else if (status.charAt(0) == "5") { grid.jqGrid('setRowData', rows[i], false, { color: 'black', weightfont: 'bold', background: '#ccff00' }); }
                    else if (status.charAt(0) == "6") { grid.jqGrid('setRowData', rows[i], false, { color: 'black', weightfont: 'bold', background: '#66ff00' }); }
                    else if (status.charAt(0) == "7") { grid.jqGrid('setRowData', rows[i], false, { color: 'black', weightfont: 'bold', background: '#00FF00' }); }
                }
            }
        });


        $("#selectAll").click(function () {
            grid.jqGrid('resetSelection');
            var ids = grid.jqGrid('getDataIDs');
            for (var i = 0, il = ids.length; i < il; i++) {
                grid.jqGrid('setSelection', ids[i], true);
            }
        });
        $("#clear").click(function () {
            grid.jqGrid('resetSelection');
        });

        $("#setToDone").click(function () {
            var ids = grid.jqGrid('getGridParam', 'selarrrow');
            if (ids.length > 0) {
                var names = [];
                for (var i = 0, il = ids.length; i < il; i++) {
                    var name = grid.jqGrid('getCell', ids[i], 'id');
                    names.push(name);
                }
                //alert ("Names: " + names.join(", ") + "; ids: " + ids.join(", "));
                $("#names").html(names.join(", "));
                var jsonData = JSON.stringify(names);
                $.ajax({
                    type: "POST",
                    url: '<%=ResolveUrl("~/Work/WorkToCompleteHandler.ashx?operation=settodone&showtx=true") %>',
                    data: jsonData,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        //                        alert(msg);
                        window.location.reload();
                    },
                    error: function (res, status, exeption) {
                        alert(res);
                    }
                });

                $("#dialog-confirm").dialog({
                    height: 280,
                    modal: true,
                    buttons: {
                        'Cancel': function () {
                            $(this).dialog('close');
                        },
                        'Confirm': function () {
                            alert("Confirm");
                        }
                    }
                });
            }
        });

        $("#editRow").click(function () {
            var sel_id = $('#jQGridDemo').jqGrid('getGridParam', 'selrow');
            var value = $('#jQGridDemo').jqGrid('getCell', sel_id, 'id');
            $.ajax({
                type: "POST",
                url: '<%=ResolveUrl("~/Transactions/TransactionHandler.ashx?operation=editrow") %>',
                data: value,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    window.location = data;
                },
                error: function (res, status, exeption) {
                    if (value == "")
                        alert("please select a row");
                    else
                        alert(exeption);
                }
            });
        });


        $('#jQGridDemo').jqGrid('navGrid', '#jQGridDemoPager',
                    {
                        edit: false,
                        add: false,
                        del: false,
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

</asp:Content>
