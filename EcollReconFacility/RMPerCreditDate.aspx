<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="RMPerCreditDate.aspx.vb" Inherits="EcollReconFacility.WebForm6" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="row mt-3">
            <div class="col-sm-10 mx-auto">
                <div class="row m-3">
                    <div class="text-center">
                        <span class="h4">Request for Reversal</span>
                    </div>
                </div>
                <hr />
                <div class="row m-2">
                    <label class="col-sm-3 col-form-label">Collecting Partner</label>
                    <div class="col-sm-9">
                        <asp:Label ID="lblRMCollPartner" runat="server" CssClass="form-control"/>
                    </div>
                </div>
                <div class="row m-2">
                    <label class="col-form-label col-sm-3">Transaction Date</label>
                    <div class="col-sm-9">
                        <asp:Label ID="lblRMSelectedTransDate" runat="server" CssClass="form-control"/>
                    </div>
                </div>
                <div class="row m-2">
                    <label class="col-form-label col-sm-3">Total Amount for Reversal</label>
                    <div class="col-sm-9">
                        <asp:Label CssClass="form-control" ID="lblTotalAmountChecked" runat="server" />
                    </div>
                </div>
                <div class="row m-2">
                    <label class="col-form-label col-sm-3">Amount excluded</label>
                    <div class="col-sm-9">
                        <asp:Label CssClass="form-control" ID="lblAmountExcluded" runat="server" ForeColor="Red" />
                    </div>
                </div>
                <div class="row m-2">
                    <label class="col-form-label col-sm-3">Reason for Reversal</label>
                    <div class="col-sm-9">
                        <asp:TextBox ID="txtReversalReasons" CssClass="form-control" TextMode="MultiLine" runat="server" />
                    </div>
                </div>
            </div>
        </div>

        <div class="row mt-2">
            <div class="col-sm-10 mx-auto">
                <asp:UpdatePanel ID="upTransList" runat="server">
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnAddTrans" />
                        <asp:PostBackTrigger ControlID="gvTrans" />
                    </Triggers>
                    <ContentTemplate>
                        <div class="row m-2" style="background:#f0f7de">
                            <div class="col">
                                <div class="row m-2">
                                    <a href="#" data-toggle="collapse" data-target="#lst"  style="text-decoration:underline">
                                        <span class="h6 font-weight-bold">List of Transactions to Exclude from Reversal ↓</span>
                                    </a>
                                </div>
                                <hr />
                                <div id="lst" class="collapse">
                                    <div class="row m-2">
                                        <div class="w-100">
                                                <asp:GridView CssClass="table table-primary table-sm" 
                                                    ShowHeaderWhenEmpty="true" ID="gvTrans" runat="server" AutoGenerateColumns="false"
                                                    OnRowDeleting="gvTrans_RowDeleting">
                                                <Columns>                  
									                <asp:BoundField DataField="transrefno" HeaderText="Transaction Ref No" />
									                <asp:BoundField DataField="paytype" HeaderText="Payment Type" />
									                <asp:BoundField DataField="pagibigid" HeaderText="Pag-IBIG ID / HAN" />
									                <asp:BoundField DataField="lname" HeaderText="Last Name" />
									                <asp:BoundField DataField="fname" HeaderText="First Name" /> 
									                <asp:BoundField DataField="mname" HeaderText="Middle Name" />
									                <asp:BoundField DataField="amount" HeaderText="Amount" />
                                                    <asp:CommandField ShowDeleteButton="true" DeleteText="Remove" />

                                                </Columns>
                                            </asp:GridView>  
                                        </div>
                                    </div>
                                    <div class="row m-2">
                                        <div class="col-sm-8">
                                            <asp:Label ID="lblMessage" runat="server" ForeColor="Red"/>
                                        </div>
                                        <div class="col-sm-4">
                                            <div class="row">
                                                <div class="col-sm-6">
                                                    <asp:Button ID="btnAddTrans" CssClass="btn btn-primary d-block mx-auto btn-sm w-100" runat="server" Text="Add" />
                                                </div>
                                                <div class="col-sm-6">
                                                    <asp:Button ID="btnClearTrans" CssClass="btn btn-primary d-block mx-auto btn-sm w-100" runat="server" Text="Clear" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                <div class="row m-2">
                    <div class="col">
                        <hr />
                        <div class="row m-2">
                            <div class="col-sm-7">
                            </div>
                            <div class="col-sm-3">
                                <asp:Button ID="btnExecute" Text="Execute Reversal" CssClass="btn btn-primary d-block mx-auto w-100" runat="server" />
                            </div>
                            <div class="col-sm-2">
                                <%--<button class="btn btn-danger d-block mx-auto w-100" onclick="history.back(1); return false;">Back</button> --%>
                                <asp:Button CssClass="btn btn-danger d-block mx-auto w-100" ID="btnBack" PostBackUrl="~/TransToRecon.aspx" runat="server" text="Back"/>
                            </div>
                        </div>
                    </div>
                </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
    </div>

    <div Class="modal hide fade" tabindex="-1" role="dialog" id="creditTransModal" aria-labelledby="creditLineModalLabel" aria-hidden="true">
        <div Class="modal-dialog modal-lg">
            <div Class="modal-content">
                <div Class="modal-header bg-primary text-light">
                    <h4 Class="modal-title">Add Transaction</h4>
                </div>
                <div Class="modal-body">
                    <div class="container-fluid">
                        <div class="row">
                            <div class="col-10">
                                <div class="row mt-2">
                                    <label class="col-sm-5 col-form-label">Transaction Date</label>
                                    <div class="col-sm-7">
                                        <input type="date" id="dtpTransDate" class="form-control required" runat="server" />
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <label class="col-sm-5 col-form-label">Transaction Reference No.</label>
                                    <div class="col-sm-7">
                                        <asp:TextBox CssClass="form-control" ID="txtTransRefno" runat="server" />
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <label class="col-sm-5 col-form-label">Last Name</label>
                                    <div class="col-sm-7">
                                        <asp:TextBox CssClass="form-control" ID="txtPayorLName" runat="server" />
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <label class="col-sm-5 col-form-label">First Name</label>
                                    <div class="col-sm-7">
                                        <asp:TextBox CssClass="form-control" ID="txtPayorFName" runat="server" />
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <label class="col-sm-5 col-form-label">Middle Name</label>
                                    <div class="col-sm-7">
                                        <asp:TextBox CssClass="form-control" ID="txtPayorMName" runat="server" />
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col">
                                        
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">         
                    <asp:Button ID="btnModalAdd" CssClass="btn btn-primary" Text="Add to List" runat="server"/>
                    <button type="button" id="btnHideCreditLineModal" class="btn btn-danger" onclick="$('#creditTransModal').modal('hide');">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
