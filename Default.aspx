<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AppointmentCalendarDemo._Default" %>

<%@ Register Src="~/UserControls/AppointmentCalendarUserControl.ascx" TagPrefix="uc1" TagName="AppointmentCalendarUserControl" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <uc1:AppointmentCalendarUserControl runat="server" id="AppointmentCalendarUserControl" />

</asp:Content>
