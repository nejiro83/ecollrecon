<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="ReverseTrans.aspx.vb" Inherits="EcollReconFacility.ReverseTrans" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container">
        <asp:UpdatePanel ID="upReverse" runat="server">
            <ContentTemplate>
                <div class="row m-2">
                    <div class="col-sm-4">
                        <label class="col-form-label font-weight-bold">Transaction Reference No.</label>
                    </div>
                    <div class="col-sm-8">
                        <div class="row">
                            <div class="col-sm-8">
                                <asp:TextBox ID="txtTransrefno" CssClass ="form-control border border-primary b-radius" runat="server" ></asp:TextBox>
                            </div>
                            <div class="col-sm-2">
                                <asp:Button ID="btnSearchTrans" runat="server" CssClass="btn btn-primary d-block" Text="Search" />
                            </div>
                            <div class="col-sm-2">
                                <asp:Button ID="btnClearTrans" CssClass="btn btn-danger d-block mx-auto" runat="server" Text="Clear" />
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row mt-4">
                    <div class="w-100">
                        <asp:GridView CssClass="table table-primary tblcreditlines" 
                            ShowHeaderWhenEmpty="true" ID="gvTrans" runat="server" AutoGenerateColumns="false"
                            OnRowDeleting="gvTrans_RowDeleting">
                            <Columns>
                                <asp:BoundField DataField="transrefno" HeaderText="Transaction Ref No" />
                                <asp:BoundField DataField="bankinsti" HeaderText="Collecting Partner" />
                                <asp:BoundField DataField="transdate" HeaderText="Transaction Date" />
                                <asp:BoundField DataField="amount" HeaderText="Amount" />
                                <asp:BoundField DataField="ticketno" HeaderText="Ticket Number" />
                                <asp:CommandField ShowDeleteButton="true" DeleteText="Remove" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
                <asp:Label id="lblMessage" CssClass="small" ForeColor="Red" runat="server"></asp:Label>
                <hr />
                <div class="row mt-3">
                    <div class=" col-sm-6 mx-auto">
                        <div class="row">
                            <div class="col-sm-6">
                                <asp:Button ID="btnReverse" runat="server" CssClass="btn btn-primary mx-auto w-100" Text="Submit" />
                            </div>
                            <div class="col-sm-6">
                                <asp:Button ID="btnClear" runat="server" CssClass="btn btn-danger mx-auto w-100" Text="Clear" />
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

</asp:Content>
