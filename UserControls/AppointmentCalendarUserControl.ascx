﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AppointmentCalendarUserControl.ascx.cs" Inherits="AppointmentCalendarDemo.UserControls.AppointmentCalendarUserControl" %>

<%--<link rel="stylesheet" href="../Content/calendar.css" />--%>
<%--<link rel="stylesheet" href="../Content/calendar2.css" />--%>
<style>
    .Holiday {
        background-color: lightgray
    }
</style>

<style>
    * {
        box-sizing: border-box;
    }

    ul {
        list-style-type: none;
    }

    body {
        font-family: Verdana, sans-serif;
    }

    .month {
        padding: 20px 25px;
        width: 100%;
        background: #1abc9c;
        text-align: center;
    }

        .month ul {
            margin: 0;
            padding: 0;
        }

            .month ul li {
                color: white;
                font-size: 20px;
                text-transform: uppercase;
                letter-spacing: 3px;
            }

        .month .prev {
            float: left;
            padding-top: 5px;
        }

        .month .next {
            float: right;
            padding-top: 5px;
        }

    .weekdays {
        margin: 0;
        padding: 10px 0;
        background-color: #ddd;
    }

        .weekdays li {
            display: inline-block;
            width: 13.9%;
            color: #666;
            text-align: center;
        }

    .days {
        padding: 10px 0;
        /*  background: #eee;*/
        margin: 0;
        height: 400px
    }

        .days .li {
            list-style-type: none;
            display: inline-block;
            width: 13.9%;
            height: 18%;
            text-align: left;
            vertical-align: top;
            margin-bottom: 5px;
            font-size: 12px;
            color: #777;
            border: 1px solid #cccccc;
            background-color: #fdfdfd;
        }

            .days .li .active {
                padding: 5px;
                background: #cccccc;
                color: white !important
            }

            .days .li .empty {
                color: orange;
                background: #5555;
                color: white !important
            }

    .slots {
        padding: 10px 0;
        margin: 0;
    }

        .slots .li {
            list-style-type: none;
            display: inline-block;
            width: 40px%;
            height: 18%;
            text-align: left;
            vertical-align: top;
            margin-bottom: 5px;
            margin-right: 10px;
            font-size: 12px;
            color: #777;
        }

            .slots .li .active {
                padding: 5px;
                background: #1abc9c;
                color: white !important
            }

    /* Add media queries for smaller screens */
    @media screen and (max-width:720px) {
        .weekdays .li, .days .li {
            width: 13.1%;
        }
    }

    @media screen and (max-width: 420px) {
        .weekdays .li, .days .li {
            width: 12.5%;
        }

            .days .li .active {
                padding: 2px;
            }
    }

    @media screen and (max-width: 290px) {
        .weekdays .li, .days .li {
            width: 12.2%;
        }
    }
</style>

<h1>Appointment Calendar</h1>
<div style="direction: rtl">
    <div class="month">
        <ul>
            <li class="prev">&#10095;</li>
            <li class="next">&#10094;</li>
            <li>August 2017</li>
        </ul>
    </div>

    <ul class="weekdays">
        <li>الأحد</li>
        <li>الاثنين</li>
        <li>الثلاثاء</li>
        <li>الاربعاء</li>
        <li>الخميس</li>
        <li>الجمعة</li>
        <li>السبت</li>
    </ul>

    <asp:Repeater ID="rpMonthDays" runat="server">
        <HeaderTemplate>
            <div class="days">
        </HeaderTemplate>
        <ItemTemplate>
            <asp:Button ID="btnDay" runat="server" CssClass='li <%= DataBinder.Eval(Container.DataItem, "Status")%>' CommandName="DaySelected" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Day")%>' Text='<%# DataBinder.Eval(Container.DataItem, "Day") !=null?  ((DateTime)DataBinder.Eval(Container.DataItem, "Day")).ToString("dd"):"" %>'></asp:Button>
        </ItemTemplate>

        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>

    <asp:Repeater ID="rpSlots" runat="server">
        <HeaderTemplate>
            <div class="slots">
        </HeaderTemplate>
        <ItemTemplate>
            <asp:Button ID="btnSlot" runat="server" CssClass="li" CommandName="SlotSelected" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "StartTime")%>' Text='<%# ((DateTime)DataBinder.Eval(Container.DataItem, "StartTime")).ToString("hh") +" "+((DateTime)DataBinder.Eval(Container.DataItem, "EndTime")).ToString("hh ") %>'></asp:Button>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>
</div>