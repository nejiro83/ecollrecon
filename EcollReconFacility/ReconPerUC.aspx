<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site1.Master" CodeBehind="ReconPerUC.aspx.vb" Inherits="EcollReconFacility.ReconPerUC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container body-content">
        <div class="row mt-3">
            <div class="col-sm-10 mx-auto">
                <div class="row m-3">
                    <div class="text-center">
                        <span class="h4">Reconcile UC</span>
                    </div>
                </div>
                <hr />
                <div class="row m-2">
                    <label class="col-sm-3 col-form-label font-weight-bold">Collecting Partner</label>
                    <div class="col-sm-9">
                        <asp:Label CssClass="form-control b-radius" runat="server" ID="txtCollPartner"/>

                    </div>
                </div>
                <div class="row m-2">
                    <label class="col-form-label col-sm-3 font-weight-bold">Credit Date</label>
                    <div class="col-sm-9">
                        <asp:Label CssClass="form-control b-radius" runat="server" ID="txtCreditDate"/>
                    </div>
                </div>
                <div class="row m-2">
                    <label class="col-form-label col-sm-3 font-weight-bold">Transaction Date/s </label>
                    <div class="col-sm-9">
                        <asp:Label CssClass="form-control b-radius" runat="server" ID="txtTransDates"></asp:Label>
                    </div>
                </div>
            </div>
        </div>

        <div class="row mt-2">
            <div class="col-sm-10 mx-auto">
                <div class="row m-2">
                    <div class="w-100 overflow-scroll" style="height: 250px;">
                        <asp:GridView CssClass="table table-primary beta" ShowHeaderWhenEmpty="true" ID="gvTransPerCreditLine" runat="server" AutoGenerateColumns="false">
                            <Columns>                  
                                <asp:BoundField DataField="transdate" HeaderText="Transaction Date" />
                                <asp:BoundField DataField="amount" HeaderText="Amount on File" ItemStyle-HorizontalAlign="Right" />
                                <asp:BoundField DataField="paytype" HeaderText="Payment Type" />
                                <asp:BoundField DataField="brcode" HeaderText="Branch Code"/>
                                <asp:BoundField DataField="transcount" HeaderText="No. of Transactions" ItemStyle-HorizontalAlign="Right"/>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>

                <asp:UpdatePanel ID="upTransList" runat="server">
                    <ContentTemplate>
                        <div class="row m-2">
                            <div class="col">
                                <div class="row m-2">
                                    <span class="h5">SUMMARY</span>
                                </div>
                                <hr />
                                <div class="row m-2">
                                    <label class="col-form-label col-sm-5">Amount Credited to Account:</label>
                                    <div class="col-sm-7">
                                        <asp:UpdatePanel ID="upAmountCredited" runat="server">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="col-sm-9">
                                                        <asp:Label CssClass="form-control b-radius" runat="server" ID="txtAmCredited"/>
                                                    </div>
                                                    <div class="col-sm-2">
                                                        <button type="button" class="btn btn-primary" data-toggle="modal" data-target="#modalUpdate">
                                                            Update
                                                        </button>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>
                                <div class="row m-2">
                                    <label class="col-form-label col-sm-5">Total Amount Reported on File:</label>
                                    <div class="col-sm-7">
                                        <asp:Label CssClass="form-control b-radius" ID="lblPanelTotalTransAmount" runat="server" />
                                    </div>
                                </div>
                                <div class="row m-2">
                                    <label class="col-form-label col-sm-5">Amount to Reconcile:</label>
                                    <div class="col-sm-7">
                                        <asp:Label CssClass="form-control b-radius" ID="lblAmtToReconcile" runat="server" />
                                    </div>
                                </div>
                                <div class="row m-2">
                                    <label class="col-form-label col-sm-5">Amount Excluded:</label>
                                    <div class="col-sm-7">
                                        <asp:Label CssClass="form-control b-radius" ID="lblAmountExcluded" runat="server" ForeColor="Red" />
                                    </div>
                                </div>
                                <hr />
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <div class="row m-2">
                    <div class="col">
                        <div class="row m-2">
                            <div class="col-sm-8">
                            </div>
                            <div class="col-sm-2">
                                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary d-block mx-auto w-100" />
                            </div>
                            <div class="col-sm-2">
                                <asp:Button CssClass="btn btn-danger d-block mx-auto w-100" ID="btnBack" runat="server" text="Back" PostBackUrl="~/PendingRecon.aspx"/>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>
</asp:Content>
