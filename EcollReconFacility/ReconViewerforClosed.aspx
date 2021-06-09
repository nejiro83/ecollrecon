<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site1.Master" CodeBehind="ReconViewerforClosed.aspx.vb" Inherits="EcollReconFacility.WebForm8" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container body-content">
        <div class="row mt-2">
            <div class="col-sm-11 mx-auto">
                <div class="row m-3">
                    <label class="col-sm-2 col-form-label">Collecting Partner</label>
                    <asp:Label ID="lblRMCollPartner" runat="server" CssClass="form-control col-sm-4"/>
                    <label class="col-sm-2 col-form-label">Transaction Date</label>
                    <asp:Label ID="lblRMSelectedTransDate" runat="server" CssClass="form-control col-sm-4"/>
                </div>
            </div>
        </div>
        <div class="row m-2">
            <div class="col-sm-12 mx-auto">
                <div class="row">
                    <div class="w-100">
                        <asp:GridView CssClass="table table-sm table-primary beta" AllowPaging="true" ShowHeaderWhenEmpty="true" 
                            ID="gvRMTransPerDate" runat="server" AutoGenerateColumns="false" DataKeyNames="transrefno, amount">
                            <Columns>
                                <asp:BoundField DataField="transdate" HeaderText="Transaction Date" />
                                <asp:BoundField DataField="transrefno" HeaderText="Transaction Ref No" />
                                <asp:BoundField DataField="paytype" HeaderText="Payment Type" />
                                <asp:BoundField DataField="amount" HeaderText="Amount" />
                            </Columns>
                            <PagerSettings Mode="Numeric" PageButtonCount="10" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>

        <div class="row m-2">
            <div class="col-sm-11 mx-auto">
                <div class="row m-2">
                    <div class="col-sm-8">
                    </div>
                    <div class="col-sm-4">
                        <%--<button class="btn btn-danger d-block mx-auto w-100">Back</button> --%>
                        <asp:Button runat="server" ID="btnBack" 
                            CssClass="btn btn-danger d-block mx-auto w-100" Text="Back" PostBackUrl="~/SearchRecon.aspx" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
