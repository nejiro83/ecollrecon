<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site1.Master" CodeBehind="ReconPerCreditDate.aspx.vb" Inherits="EcollReconFacility.WebForm3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container body-content shadow">
        <div class="row mt-3">
            <div class="col-sm-10 mx-auto">
                <div class="row m-3">
                    <div class="text-center">
                        <span class="h4">Bank Credit Reconciliation with Collection File</span>
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
                    <label class="col-sm-3 col-form-label font-weight-bold">Depository Bank</label>
                    <div class="col-sm-9">
                        <asp:Label CssClass="form-control b-radius" runat="server" ID="lblDepositoryBank"/>
                    </div>
                </div>
                <div class="row m-2">
                    <label class="col-sm-3 col-form-label font-weight-bold">Contact Person</label>
                    <div class="col-sm-9">
                        <asp:Label CssClass="form-control b-radius" runat="server" ID="lblContactPerson"/>
                    </div>
                </div>
                <div class="row m-2">
                    <label class="col-sm-3 col-form-label font-weight-bold">Bank Account No.</label>
                    <div class="col-sm-9">
                        <asp:Label CssClass="form-control b-radius" runat="server" ID="lblBankAccountNo"/>
                    </div>
                </div>
                <div class="row m-2">
                    <label class="col-sm-3 col-form-label font-weight-bold">Contact Number</label>
                    <div class="col-sm-9">
                        <asp:Label CssClass="form-control b-radius" runat="server" ID="lblContactNumber"/>
                    </div>
                </div>
                <div class="row m-2">
                    <label class="col-sm-3 col-form-label font-weight-bold">Email Address</label>
                    <div class="col-sm-9">
                        <asp:Label CssClass="form-control b-radius" runat="server" ID="lblEmailAddress"/>
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
                                <asp:BoundField DataField="transdate" HeaderText="Transaction Date" ItemStyle-BackColor="#fefefa" />
                                <asp:BoundField DataField="amount" HeaderText="Amount on File" 
                                    ItemStyle-HorizontalAlign="Right" ItemStyle-BackColor="#fefefa"/>
                                <asp:BoundField DataField="paytype" HeaderText="Payment Type" 
                                    ItemStyle-BackColor="#fefefa"/>
                                <asp:BoundField DataField="brcode" HeaderText="Branch Code" ItemStyle-BackColor="#fefefa"/>
                                <asp:BoundField DataField="transcount" HeaderText="No. of Transactions" 
                                    ItemStyle-HorizontalAlign="Right" ItemStyle-BackColor="#fefefa"/>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>

                <asp:UpdatePanel ID="upTransList" runat="server">
                    <ContentTemplate>
                        <div id="lstExcluded" runat="server" class="row m-2" style="background:#f0f7de">
                            <div class="col">
                                <div class="row m-2">
                                    <span class="h5">Add Transactions to AR</span>
                                </div>
                                <hr />
                                <div id="lst">
                                    <div class="row m-2">
                                        <div class="col-sm-8">
                                            <div class="row m-2">
                                                <label class="col-form-label col-sm-5 font-weight-bold">Transaction Ref No.:</label>
                                                <asp:TextBox ID="txtTransRefNo" 
                                                    CssClass="form-control col-sm-7 b-radius border border-danger" 
                                                    runat="server" ClientIDMode="Static"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-sm-4">
                                            <div class="row">
                                                <div class="col-sm-6">
                                                    <asp:Button ID="btnAddTrans" CssClass="btn btn-primary d-block mx-auto btn-sm w-100" 
                                                        runat="server" Text="Add"/>
                                                </div>
                                                <div class="col-sm-6">
                                                    <asp:Button ID="btnClearTrans" CssClass="btn btn-danger d-block mx-auto btn-sm w-100" runat="server" Text="Clear"/>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row m-2">
                                        <div class="col-sm-8">
                                            <div class="row m-2">
                                                <label class="col-form-label col-sm-5 font-weight-bold">Remarks:</label>
                                                <asp:DropDownList ID="ddlRemarks" runat="server" ClientIDMode="Static"
                                                    CssClass="form-control col-sm-7 b-radius border border-danger" >
                                                    <asp:ListItem Text="OVERPAYMENT" Value="OVERPAYMENT" />
                                                </asp:DropDownList>

                                            </div>
                                        </div>
                                        <div class="col-sm-4">

                                        </div>
                                    </div>
                                    <div class="row m-2">
                                        <div class="w-100">
                                                <asp:GridView CssClass="table table-bordered table-sm" ClientIDMode="Static" 
                                                    ShowHeaderWhenEmpty="true" ID="gvTrans" runat="server" AutoGenerateColumns="false"
                                                    OnRowDeleting="gvTrans_RowDeleting">
                                                <Columns>                  
                                                    <asp:BoundField DataField="transrefno" HeaderText="Transaction Ref No"/>
                                                    <asp:BoundField DataField="transdate" HeaderText="Transaction Date"/>
                                                    <asp:BoundField DataField="paytype" HeaderText="Payment Type" />
                                                    <asp:BoundField DataField="amount" HeaderText="Amount"/>
                                                    <asp:BoundField DataField="remarks" HeaderText="Remarks" />
                                                    <asp:CommandField ShowDeleteButton="true" DeleteText="Remove" />

                                                </Columns>
                                            </asp:GridView>  
                                        </div>
                                    </div>
                                    <div class="row m-2">
                                        <div class="col-sm-8">
                                            <asp:Label ID="lblMessage" runat="server" ForeColor="Red" ClientIDMode="Static"/>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

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
                                                        <asp:Label CssClass="form-control b-radius" 
                                                            runat="server" ID="txtAmCredited" ClientIDMode="Static"/>
                                                    </div>
                                                    <div class="col-sm-2">
                                                        <button type="button" class="btn btn-primary" 
                                                            data-toggle="modal" data-target="#modalUpdate">
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
                                        <asp:Label CssClass="form-control b-radius" 
                                            ID="lblPanelTotalTransAmount" runat="server" 
                                            ClientIDMode="Static" />
                                    </div>
                                </div>
                                <div class="row m-2">
                                    <label class="col-form-label col-sm-5">Amount to Reconcile:</label>
                                    <div class="col-sm-7">
                                        <asp:Label CssClass="form-control b-radius" ID="lblAmtToReconcile" runat="server" />
                                    </div>
                                </div>
                                <div class="row m-2" id="divExcluded" runat="server">
                                    <label class="col-form-label col-sm-5">Amount Excluded:</label>
                                    <div class="col-sm-7">
                                        <asp:Label CssClass="form-control b-radius" ID="lblAmountExcluded" 
                                             ClientIDMode="Static"
                                            runat="server" ForeColor="Red" />
                                    </div>
                                </div>
                                <hr />
                                <asp:HiddenField ID="txtProcessType" runat="server" />
                                <asp:HiddenField ID="txtBankInstiCode" runat="server" />
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
                                <asp:Button ID="btnSave" runat="server" Text="Save" 
                                    CssClass="btn btn-primary d-block mx-auto w-100" 
                                    OnClientClick="return checkAR();" OnClick="btnSave_Click" />
                            </div>
                            <div class="col-sm-2">
                                <asp:Button CssClass="btn btn-danger d-block mx-auto w-100" ID="btnBack" 
                                    runat="server" text="Back" PostBackUrl="~/PendingRecon.aspx"/>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>

    <div class="modal hide fade" tabindex="1" role="dialog" id="modalUpdate" aria-labelledby="modalUpdateLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content" style="background:#fefefa;">
                <div Class="modal-header modal-bg-header text-light">
                    <h4 Class="modal-title">Update Amount Credited</h4>
                </div>
                <div Class="modal-body">
                    <div class="container-fluid">
                        <div class="row">
                            <div class="col-10">
                                <div class="row mt-2">
                                    <label class="col-sm-5 col-form-label font-weight-bold">Updated Amount</label>
                                    <div class="col-sm-7">
                                        <asp:TextBox ID="txtUpdatedAmtCredited" ClientIDMode="Static"
                                            CssClass="form-control border border-primary b-radius" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <label class="col-sm-5 col-form-label font-weight-bold">User ID</label>
                                    <div class="col-sm-7">
                                        <asp:TextBox 
                                            CssClass="form-control border border-primary b-radius" 
                                            ID="txtSupervisorUserID" ClientIDMode="Static"
                                            runat="server"/>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <label class="col-sm-5 col-form-label font-weight-bold">Password</label>
                                    <div class="col-sm-7">
                                        <asp:TextBox 
                                            CssClass="form-control border border-primary b-radius" 
                                            ID="txtSupervisorPassword" ClientIDMode="Static"
                                             TextMode="Password" runat="server"/>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <label class="col-sm-5 col-form-label font-weight-bold">Remarks</label>
                                    <div class="col-sm-7">
                                        <asp:TextBox 
                                            CssClass="form-control border border-primary b-radius" 
                                            ID="txtRemarks" ClientIDMode="Static" runat="server" />
                                    </div>
                                </div>                                
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Label ID="lblUpdateAmountMsg" ForeColor="#f00000" runat="server" ClientIDMode="Static" />  
                    <asp:Button ID="btnModalAdd" CssClass="btn btn-primary" 
                         OnClick="btnModalAdd_Click" OnClientClick="return checkUpdatedAmount();"
                        Text="Update" runat="server"/>
                    <button type="button" id="btnHideCreditLineModal" class="btn btn-danger" data-dismiss="modal">Cancel</button>
                </div>
            </div>
        </div>

    </div>
</asp:Content>
