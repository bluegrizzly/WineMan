<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WorkToComplete.aspx.cs" Inherits="WineMan.Work.WorkToComplete" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 269px;
        }
    </style>

  <script src="//code.jquery.com/ui/1.11.1/jquery-ui.js"></script>

  <script>
      $(function () {
          $("#datepicker").datepicker({ dateFormat: "mm-dd-yy" });
      });

  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

        <h2>Work To Complete</h2>
        <p>
            <table class="auto-style1">
                <tr>
                    <td class="auto-style2">
                        <asp:Calendar ID="Calendar_Date" runat="server" OnSelectionChanged="Calendar_Date_SelectionChanged" >
                                        <SelectedDayStyle BackColor="#0099FF" />
                                        <TodayDayStyle BackColor="#999999" />
                                        <WeekendDayStyle BackColor="#EEEEEE" />
                        </asp:Calendar>
                        <asp:TextBox ID="txtDate" runat="server"></asp:TextBox>
                        <p>Date: <input type="text" id="datepicker" value="<%= this.DateValue %>" ></p>
                        <fieldset>
                            <input id="setToDone" type="button" value="Set to Done"/><br />
                            <input id="selectAll" type="button" value="Select All"/><br />
                            <input id="clear" type="button" value="Clear Selection"/>
                        </fieldset>
                    </td>
                    <td>
                        <table id="jQGridDemo">
                        </table>
                        <div id="jQGridDemoPager">
                        </div>
                    </td>
                </tr>
            </table>
        </p>
    
    <script type="text/javascript">
        var grid = $("#jQGridDemo");

        grid.jqGrid({
            url: '<%=ResolveUrl("~/Work/WorkToCompleteHandler.ashx?date=") %>' + document.getElementById('<%= txtDate.ClientID %>').value,
            //url: '<%=ResolveUrl("~/Work/WorkToCompleteHandler.ashx?date=") %>' + <%= DateValue %>,
            datatype: "json",
            colNames: ['id', 'TxId', 'Step', 'Done'],
            colModel: [
                        { name: 'id', index: 'id', width: 10, stype: 'text', sortable: true, sorttype: 'int', hidden: true},
                        { name: 'txid', index: 'txid', width: 50, stype: 'text', sortable: true, sorttype: 'int' },
   		                { name: 'step', index: 'step', width: 150, sortable: true },
   		                { name: 'done', index: 'done', width: 150, sortable: true, align: 'center' }
            ],
            rowNum: 10,
            multiselect: true,
            height: 250,
            mtype: 'GET',
            loadonce: true,
            rowList: [10, 20, 30],
            pager: '#jQGridDemoPager',
            sortname: 'id',
            viewrecords: true,
//            rownumbers: true,
            gridview: true,
            sortorder: 'desc',
            caption: "Transactions Steps to complete",
            editurl: '<%=ResolveUrl("~/Work/WorkToCompleteHandler.ashx") %>'
        });

        $("#selectAll").click(function () {
            grid.jqGrid('resetSelection');
            var ids = grid.jqGrid('getDataIDs');
            for (var i = 0, il = ids.length; i < il; i++) {
                grid.jqGrid('setSelection', ids[i], true);
            }
        });        $("#clear").click(function () {
            grid.jqGrid('resetSelection');
        });
        
        $("#datepicker").change(function() {
            $("#jQGridDemo").jqGrid('setGridParam',{datatype:'json'}).trigger('reloadGrid');
            grid.trigger('reloadGrid');
        })

        $("#setToDone").click(function () {
            var ids = grid.jqGrid('getGridParam', 'selarrrow');
            if (ids.length > 0)
            {
                var names = [];
                for (var i = 0, il = ids.length; i < il; i++)
                {
                    var name = grid.jqGrid('getCell', ids[i], 'id');
                    names.push(name);
                }
                //alert ("Names: " + names.join(", ") + "; ids: " + ids.join(", "));
                $("#names").html(names.join(", "));
                var jsonData = JSON.stringify(names);
                $.ajax({
                    type: "POST",
                    url: '<%=ResolveUrl("~/Work/WorkToCompleteHandler.ashx?operation=settodone") %>',
                    data: jsonData,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function(msg)
                    {
//                        alert(msg);
                    },
                    error: function (res, status, exeption)
                    {
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


        $('#jQGridDemo').jqGrid('navGrid', '#jQGridDemoPager',
                   {
                       edit: false,
                       add: false,
                       del: false,
                       search: true,
                       searchtext: "Search",
                       addtext: "Add",
                       edittext: "Edit",
                       deltext: "Set to Done"
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
                               var sel_id = $('#jQGridDemo').jqGrid('getGridParam', 'selarrrow');
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
