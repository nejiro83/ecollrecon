<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site1.Master" CodeBehind="Login.aspx.vb" Inherits="EcollReconFacility.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4">
        <div class="row">
            <div class="m-auto text-center shadow" id="myform">
                <div class="form-group mt-5 m-4">
                    <div class="input-group">
                        <div class="input-group-prepend">
                            <span class="input-group-text fa fa-user fa-lg fa-2x"></span>
                        </div>
                        <asp:TextBox ID="txtUserName" ClientIDMode="Static"
                            CssClass="form-control border border-primary b-radius" runat="server" placeholder="Username" />
                    </div>
                </div>
                <div class="form-group m-4">
                    <div class="input-group">
                        <div class="input-group-prepend">
                            <span class="input-group-text fa fa-lock fa-lg fa-2x"></span>
                        </div>
                        <asp:TextBox TextMode="Password" ID="txtPassword"
                            CssClass="form-control border border-primary b-radius" runat="server" placeholder="Password" />
                    </div>
                </div>
                <div class="form-group m-4">
                    <asp:Button ID="btnLogin" Text="Login" CssClass="btn btn-primary btn-block" runat="server" />
                </div>
                <div class="form-group mb-5">
                    <asp:Label ID="lblMessage" Text="" ForeColor="Red" runat="server"/>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
