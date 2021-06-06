<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="UncreditedTrans.aspx.vb" Inherits="EcollReconFacility.UncreditedTrans" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container sb-content">
        <div class="row mt-2">
            <div class="col mx-auto m-2">
                <asp:GridView ID="gvUncreditedTrans" CssClass="table table-primary tblcreditlines beta"
                    ShowHeaderWhenEmpty ="true" runat="server" AutoGenerateColumns ="false"
                        AllowPaging ="true" PageSize ="10">
                    <Columns>
                        <asp:BoundField DataField="rownumber" HeaderText="#" />
                        <asp:BoundField DataField="bankinsti" HeaderText="Collecting Partner" />
                        <asp:BoundField DataField="transdate" HeaderText="Transaction Date" />
                        <asp:BoundField DataField="amount" HeaderText="Amount" ItemStyle-HorizontalAlign="Right"/>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lnkPopup"  OnClientClick="">
                                    Add Credit Date Line</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerSettings Mode="Numeric" PageButtonCount="10" />
                </asp:GridView>
            </div>
        </div>
    </div>

    <div Class="modal hide fade" tabindex="-1" role="dialog" id="creditLineModal" aria-labelledby="creditLineModalLabel" aria-hidden="true">
        <div Class="modal-dialog modal-lg">
            <div Class="modal-content">
                <div Class="modal-header bg-primary text-light">
                    <h4 Class="modal-title">Add New Credit Date Line</h4>
                </div>
                <div Class="modal-body" style="background:#E0FFFF;">
                    <div class="container-fluid">
                        <div class="row">
                            <div class="col-10">
                                <div class="row mb-2">
                                    <label class="col-3 col-form-label font-weight-bold">Collecting Partner</label>
                                    <div class="col-9">
                                        <input type="text" runat="server" id="txtBankInsti" 
                                            class="form-control border border-primary b-radius" /> 
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <label class="col-3 col-form-label font-weight-bold">Credit Date</label>
                                    <div class="col-9">
                                        <input type="text" id="dtpmodalCreditDate" runat="server" 
                                            class="form-control datepicker border border-primary b-radius" />
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <label class="col-3 col-form-label font-weight-bold">Transaction Date</label>
                                    <div class="col-9">
                                        <div class="row">
                                            <div class="col-6">
                                                <div class="form-group">
                                                    <label class="col-form-label">From</label>
                                                    <input type="text" id="txtTransDateFrom" runat="server"
                                                        class="form-control border border-primary b-radius" readonly="readonly" />
                                                </div>
                                            </div>
                                            <div class="col-6">
                                                <div class="form-group">
                                                    <label class="col-form-label">To</label>
                                                    <input type="text" id="txtTransDateTo" runat="server"
                                                        class="form-control border border-primary b-radius" readonly="readonly" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <label class="col-3 col-form-label font-weight-bold">Amount Credited</label>
                                    <div class="col-9">
                                        <input type="text" id="txtAmountCredited" 
                                            class="form-control border border-primary b-radius" runat="server"/>
                                        <input type="hidden" id="txtAmountOnFile" runat="server" />
                                     
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer" style="background:#E0FFFF;">         
                    <asp:Button ID="btnModalSave" CssClass="btn btn-primary" Text="Save" runat="server"/>
                    <button type="button" id="btnHideCreditLineModal" class="btn btn-danger" onclick="$('#creditLineModal').modal('hide');">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
