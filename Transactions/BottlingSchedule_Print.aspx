﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BottlingSchedule_Print.aspx.cs" Inherits="WineMan.Transactions.BottlingSchedule_Print" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script type="text/javascript">
        function setInputEnableState(reportViewer) {

            // It is ok to export if the viewer is not loading and is displaying a report.
            var disableInputs = reportViewer.get_isLoading() ||
                                reportViewer.get_reportAreaContentType() !== Microsoft.Reporting.WebFormsClient.ReportAreaContent.ReportPage;

            $get("PrintButton").disabled = disableInputs;
        }

        function onReportViewerLoadingChanged(sender, e) {

            var propertyName = e.get_propertyName();

            if (propertyName === "isLoading" || propertyName === "reportAreaContentType") {
                setInputEnableState(sender);
            }
        }

        function onPrintButtonClicked() {

            var reportViewer = $find("ReportViewer1");
            reportViewer.invokePrintDialog();
        }

        var hookedPropertyChangedEvent = false;

        //function pageLoad() {

        //    if (!hookedPropertyChangedEvent) {

        //        var reportViewer = $find("ReportViewer1");
        //        reportViewer.add_propertyChanged(onReportViewerLoadingChanged);

        //        // Make sure the input controls are in the correct state initially
        //        setInputEnableState(reportViewer);

        //        // pageLoad is called after each asynchronous postback.  Only
        //        // hook the property changed event once.
        //        hookedPropertyChangedEvent = true;
        //    }
        //}
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:Button ID="Button_Back" runat="server" OnClick="Button_Back_Click" Text="Back" />
        &nbsp;<asp:Button ID="Button_Show" runat="server" OnClick="Button_Show_Click" Text="Show" Visible="False" />
        <asp:Button ID="PrintButton" runat="server" disabled="disabled" Text="IE Print..." OnClientClick="onPrintButtonClicked();return false;" Visible="False" />
        <asp:Button ID="Button_Print" runat="server" Text="Print" OnClick="Button_Print_Click" />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Label ID="Label_Printer" runat="server" Text="Printer:"></asp:Label>
        <asp:DropDownList ID="DropDownList_Printer" runat="server">
        </asp:DropDownList>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="10.5in" Height="8.4in">
            <LocalReport ReportEmbeddedResource="WineMan.Reports.Report_WorkToComplete.rdlc">
            </LocalReport>
        </rsweb:ReportViewer>
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetData" TypeName="winemanDataSet2TableAdapters.customersTableAdapter"></asp:ObjectDataSource>
    
    </div>
    </form>
</body>
</html>
