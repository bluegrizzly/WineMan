<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LeftMenu.aspx.cs" Inherits="WineMan.Transactions.LeftMenu" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="Button_AddTransaction" runat="server" Text="Add" Width="110px" PostBackUrl="~/Transactions/AddTransaction.aspx" />
        <br />
        <asp:Button ID="Button_EditTransaction" runat="server" Text="Edit Selection" Width="110px" OnClick="Button_EditTransaction_Click" />
    </div>
    </form>
</body>
</html>
